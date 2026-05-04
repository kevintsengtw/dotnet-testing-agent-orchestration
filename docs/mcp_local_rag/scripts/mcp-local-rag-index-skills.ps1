<#
.SYNOPSIS
    將 dotnet-testing-agent-skills 的 SKILL 內容建立索引至 mcp-local-rag 向量資料庫

.DESCRIPTION
    初次設定或 skills 有更新時執行。
    索引來源為 dotnet-testing-agent-skills 倉庫（需另行 clone），以 -SkillsPath 參數指定其
    .github/skills 目錄路徑。執行前會先驗證來源路徑是否包含 dotnet-testing* 技能目錄。
    索引位置：.mcp/dotnet-testing-skills（由 .vscode/mcp.json 的 DB_PATH 指定）

.PARAMETER SkillsPath
    dotnet-testing-agent-skills 倉庫的 .github/skills 目錄路徑（必填）。
    範例：C:\projects\dotnet-testing-agent-skills\.github\skills

.PARAMETER Mode
    update（預設）：保留現有索引，只更新有變更的文件。
    rebuild：清除舊索引，完整重建。

.EXAMPLE
    .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills
    .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills -Mode rebuild
    .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills -Mode update
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

Write-Host "Skills 來源：$skillsDir"
Write-Host "DB 路徑：$dbPath"
Write-Host "模式：$Mode"
Write-Host ""

# 前置驗證：確認來源目錄存在且包含 dotnet-testing* 技能目錄
if (-not (Test-Path $skillsDir)) {
    Write-Host "❌ 找不到指定的 Skills 來源目錄：$skillsDir" -ForegroundColor Red
    Write-Host "   請確認 dotnet-testing-agent-skills 已正確 clone，且 -SkillsPath 路徑正確。" -ForegroundColor Yellow
    Write-Host "   倉庫來源：https://github.com/kevintsengtw/dotnet-testing-agent-skills" -ForegroundColor Yellow
    exit 1
}

$testingSkills = Get-ChildItem -Path $skillsDir -Directory -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -like "dotnet-testing*" }

if ($null -eq $testingSkills -or $testingSkills.Count -eq 0) {
    Write-Host "❌ 在指定路徑找不到 dotnet-testing* 技能目錄：$skillsDir" -ForegroundColor Red
    Write-Host "   請確認 -SkillsPath 指向 dotnet-testing-agent-skills 的 .github/skills 目錄。" -ForegroundColor Yellow
    Write-Host "   倉庫來源：https://github.com/kevintsengtw/dotnet-testing-agent-skills" -ForegroundColor Yellow
    exit 1
}

Write-Host "✓ 前置驗證通過：找到 $($testingSkills.Count) 個 dotnet-testing* 技能目錄" -ForegroundColor Green
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

Write-Host ""
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
