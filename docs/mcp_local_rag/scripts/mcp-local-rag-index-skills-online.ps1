<#
.SYNOPSIS
    線上模式：建立 dotnet-testing-agent-skills 的 SKILL 索引至 mcp-local-rag 向量資料庫

.DESCRIPTION
    此腳本使用線上模式建立索引，首次執行時若本機尚無 embedding model，
    mcp-local-rag 會自動下載並快取至 .mcp\cache。

.PARAMETER SkillsPath
    dotnet-testing-agent-skills 倉庫的 .github/skills 目錄路徑（必填）。

.PARAMETER Mode
    update（預設）：保留現有索引，只更新有變更的文件。
    rebuild：清除舊索引，完整重建。
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$SkillsPath,

    [ValidateSet("update", "rebuild")]
    [string]$Mode = "update"
)

$repoRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
$skillsDir = $SkillsPath
$dbPath = Join-Path $repoRoot ".mcp\dotnet-testing-skills"
$mcpCachePath = Join-Path $repoRoot ".mcp\cache"
$modelsPath = Join-Path $repoRoot "models"

Write-Host "[Online] Skills 來源：$skillsDir"
Write-Host "[Online] DB 路徑：$dbPath"
Write-Host "[Online] 模式：$Mode"
Write-Host ""

if (-not (Get-Command mcp-local-rag -ErrorAction SilentlyContinue)) {
    Write-Host "找不到 mcp-local-rag 指令。請先安裝：npm install -g mcp-local-rag" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $skillsDir)) {
    Write-Host "找不到指定的 Skills 來源目錄：$skillsDir" -ForegroundColor Red
    exit 1
}

$testingSkills = Get-ChildItem -Path $skillsDir -Directory -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -like "dotnet-testing*" }

if ($null -eq $testingSkills -or $testingSkills.Count -eq 0) {
    Write-Host "在指定路徑找不到 dotnet-testing* 技能目錄：$skillsDir" -ForegroundColor Red
    exit 1
}

Write-Host "前置驗證通過：找到 $($testingSkills.Count) 個 dotnet-testing* 技能目錄" -ForegroundColor Green
Write-Host ""

if (-not (Test-Path $mcpCachePath)) {
    New-Item -ItemType Directory -Path $mcpCachePath | Out-Null
    Write-Host "建立目錄：$mcpCachePath"
}

if (-not (Test-Path $modelsPath)) {
    New-Item -ItemType Directory -Path $modelsPath | Out-Null
    Write-Host "建立目錄：$modelsPath"
}

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

Write-Host ""
Write-Host "目標 skill 目錄（$($targetSkills.Count) 個）："
$targetSkills | ForEach-Object { Write-Host "  $($_.Name)" }
Write-Host ""
Write-Host "開始建立索引（線上模式，首次執行可能需下載 embedding model）..."
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
