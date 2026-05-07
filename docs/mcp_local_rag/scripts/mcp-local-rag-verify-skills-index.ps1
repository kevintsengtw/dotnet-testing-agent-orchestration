<#
.SYNOPSIS
    驗證 mcp-local-rag 索引庫狀態（不重建索引）

.DESCRIPTION
    確認現有的 dotnet-testing-skills 索引是否正確：
    - DB 存在性
    - documentCount / chunkCount
    - 索引範圍（ingested=true 的項目全為 dotnet-test*，無雜訊）
    - 查詢功能（smoke test）

    注意：此腳本不修改索引。若需重建索引，請依環境擇一執行：
        .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills-online.ps1 -SkillsPath <skills 來源路徑> -Mode rebuild
        .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills-offline.ps1 -SkillsPath <skills 來源路徑> -Mode rebuild
    若只更新有變動的文件（預設模式），請依環境擇一執行：
        .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills-online.ps1 -SkillsPath <skills 來源路徑>
        .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills-offline.ps1 -SkillsPath <skills 來源路徑>
#>

$repoRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$dbPath = Join-Path $repoRoot ".mcp\dotnet-testing-skills"

$pass = 0
$fail = 0

function Write-Result {
    param([string]$Step, [bool]$Ok, [string]$Detail)
    $color = if ($Ok) { "Green" } else { "Red" }
    Write-Host ("[{0}] {1,-20} {2}" -f $(if ($Ok) { " OK " } else { "FAIL" }), $Step, $Detail) -ForegroundColor $color
}

Write-Host ""
Write-Host "mcp-local-rag 索引驗證" -ForegroundColor Cyan
Write-Host "DB 路徑：$dbPath"
Write-Host ("-" * 60)

$dbExists = Test-Path $dbPath
if ($dbExists) {
    $chunksExist = Test-Path (Join-Path $dbPath "chunks.lance")
    $detail = if ($chunksExist) { "chunks.lance 存在" } else { "目錄存在但 chunks.lance 缺失" }
    $ok = $chunksExist
} else {
    $detail = "目錄不存在，請先執行線上版或離線版索引腳本重建 DB"
    $ok = $false
}
Write-Result "DB 存在性" $ok $detail
if (-not $ok) {
    $fail++
    Write-Host ""
    Write-Host "DB 不存在，中止驗證。" -ForegroundColor Red
    exit 1
}
$pass++

$env:RAG_HYBRID_WEIGHT = "0.7"
$env:RAG_GROUPING = "similar"
$env:CACHE_DIR = Join-Path $repoRoot ".mcp\cache"
Write-Host ""
$statusOutput = mcp-local-rag --db-path $dbPath status 2>&1
$jsonLine = ($statusOutput | ForEach-Object { $_.ToString() } | Where-Object { $_ -match '^\s*\{' }) | Select-Object -Last 1
$docCount = $null
$chunkCount = $null
if ($jsonLine) {
    try {
        $statusJson = $jsonLine | ConvertFrom-Json
        $docCount = $statusJson.documentCount
        $chunkCount = $statusJson.chunkCount
        $searchMode = $statusJson.searchMode
    } catch {
        $docCount = $null
    }
}

if ($docCount -and [int]$docCount -ge 30) {
    Write-Result "documentCount" $true "documentCount: $docCount, chunkCount: $chunkCount"
    $pass++
} elseif ($docCount) {
    Write-Result "documentCount" $false "documentCount: $docCount（預期 >= 30），可能索引不完整"
    $fail++
} else {
    Write-Result "documentCount" $false "無法解析 status 輸出"
    $fail++
}

if ($searchMode -eq "hybrid") {
    Write-Result "searchMode" $true "searchMode: hybrid（FTS 索引已啟用，混合搜尋可用）"
    $pass++
} elseif ($searchMode) {
    Write-Result "searchMode" $false "searchMode: $searchMode（預期 hybrid，可能 FTS 索引未建立）"
    $fail++
} else {
    Write-Result "searchMode" $false "searchMode 欄位缺失（status 輸出可能不完整）"
    $fail++
}

Write-Host ""
try {
    $listRaw = mcp-local-rag --db-path $dbPath list 2>$null
    $listJson = $listRaw | ConvertFrom-Json
    $indexed = $listJson.files | Where-Object { $_.ingested -eq $true }
    $noise = $indexed | Where-Object { $_.filePath -notmatch 'dotnet-test' }

    if ($null -eq $indexed -or $indexed.Count -eq 0) {
        if ($docCount -and [int]$docCount -ge 29) {
            Write-Result "索引範圍" $true "list 不追蹤目錄 ingest（ingested=0），但 documentCount=$docCount 確認索引完整"
            $pass++
        } else {
            Write-Result "索引範圍" $false "已索引檔案數為 0，且 documentCount 未達預期"
            $fail++
        }
    } elseif ($noise -and $noise.Count -gt 0) {
        $noiseNames = ($noise | ForEach-Object { Split-Path $_.filePath -Leaf }) -join ", "
        Write-Result "索引範圍" $false "含雜訊（ingested=true 但非 dotnet-test*）：$noiseNames"
        $fail++
    } else {
        Write-Result "索引範圍" $true "已索引 $($indexed.Count) 個檔案，全部為 dotnet-test*"
        $pass++
    }
    Write-Host "  （list 亦含 $($listJson.files.Count) 個磁碟檔案記錄，其中未索引的顯示 ingested=false，屬正常現象）" -ForegroundColor DarkGray
} catch {
    Write-Result "索引範圍" $false "list 命令解析失敗：$_"
    $fail++
}

Write-Host ""
$env:DB_PATH = $dbPath
$queryOutput = mcp-local-rag query "NSubstitute mock interface Returns Received" --limit 3 2>$null
$hasResult = $queryOutput -match "dotnet-testing-nsubstitute-mocking"

if ($hasResult) {
    Write-Result "Smoke test" $true "回傳 dotnet-testing-nsubstitute-mocking 相關結果"
    $pass++
} elseif ($queryOutput) {
    Write-Result "Smoke test" $false "有回傳結果但未命中 nsubstitute-mocking（可能索引不完整）"
    $fail++
} else {
    Write-Result "Smoke test" $false "查詢無回傳結果"
    $fail++
}

Write-Host ""
$cacheDir = Join-Path $repoRoot ".mcp\cache"
if (Test-Path $cacheDir) {
    $cacheFiles = Get-ChildItem -Path $cacheDir -Recurse -File -ErrorAction SilentlyContinue
    $cacheCount = if ($cacheFiles) { $cacheFiles.Count } else { 0 }
    Write-Result "cache 目錄" $true "$cacheDir 存在（$cacheCount 個檔案，CACHE_DIR 已生效）"
    $pass++
} else {
    Write-Result "cache 目錄" $false "$cacheDir 不存在（CACHE_DIR 設定可能未生效，或尚未產生快取）"
    $fail++
    Write-Host "  提示：執行一次 query 後 cache 目錄才會建立" -ForegroundColor DarkGray
}

Write-Host ""
Write-Host ("-" * 60)
if ($fail -eq 0) {
    Write-Host "所有驗證通過（$pass / $($pass + $fail)）" -ForegroundColor Green
} else {
    Write-Host "驗證未通過（通過 $pass，失敗 $fail）" -ForegroundColor Red
    Write-Host "   若需重建索引請擇一執行：" -ForegroundColor Yellow
    Write-Host "      .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills-online.ps1 -SkillsPath <skills 來源路徑> -Mode rebuild" -ForegroundColor Yellow
    Write-Host "      .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills-offline.ps1 -SkillsPath <skills 來源路徑> -Mode rebuild" -ForegroundColor Yellow
}
Write-Host ""
