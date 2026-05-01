<#
.SYNOPSIS
    將 dotnet-test* SKILL.md 建立索引至 mcp-local-rag 向量資料庫

.DESCRIPTION
    初次設定或 skills 有更新時執行。
    只索引 dotnet-test 開頭的 skill 目錄，排除 autoresearch、skill-creator-advanced 等無關目錄。
    索引位置：.mcp/dotnet-testing-skills（由 .vscode/mcp.json 的 DB_PATH 指定）

.EXAMPLE
    .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1
    .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -Mode rebuild
    .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -Mode update
#>
param(
    [ValidateSet("update", "rebuild")]
    [string]$Mode = "update"
)

$repoRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$skillsDir = Join-Path $repoRoot ".github\skills"
$dbPath = Join-Path $repoRoot ".mcp\dotnet-testing-skills"

Write-Host "Skills 目錄：$skillsDir"
Write-Host "DB 路徑：$dbPath"
Write-Host "模式：$Mode"
Write-Host ""

if ($Mode -eq "rebuild") {
    if (Test-Path $dbPath) {
        Remove-Item -Path $dbPath -Recurse -Force
        Write-Host "已清除舊索引：$dbPath"
    }
    New-Item -ItemType Directory -Path $dbPath | Out-Null
    Write-Host "建立 DB 目錄：$dbPath"
} else {
    if (-not (Test-Path $dbPath)) {
        New-Item -ItemType Directory -Path $dbPath | Out-Null
        Write-Host "DB 目錄不存在，已建立：$dbPath（自動轉為完整建立）"
    } else {
        Write-Host "保留現有索引，僅更新有變更的文件"
    }
}

$targetSkills = Get-ChildItem -Path $skillsDir -Directory |
    Where-Object { $_.Name -like "dotnet-test*" }

Write-Host "目標 skill 目錄（$($targetSkills.Count) 個）："
$targetSkills | ForEach-Object { Write-Host "  $($_.Name)" }
Write-Host ""
Write-Host "開始建立索引（可能需要 2-5 分鐘，首次執行需下載 embedding model ~90MB）..."
Write-Host ""

foreach ($skill in $targetSkills) {
    Write-Host "  ingesting: $($skill.Name)"
    mcp-local-rag --db-path $dbPath ingest $skill.FullName --base-dir $skillsDir
}

Write-Host ""
Write-Host "驗證索引狀態："
$status = mcp-local-rag --db-path $dbPath status 2>&1
$status | Write-Host

Write-Host ""
Write-Host "冒煙測試（查詢 NSubstitute）："
$env:DB_PATH = $dbPath
mcp-local-rag query "Substitute.For Returns Received mock" --limit 3 2>$null | Select-String "filePath|score" | Select-Object -First 9
