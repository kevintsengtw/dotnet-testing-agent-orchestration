<#
.SYNOPSIS
    相容別名：預設以線上模式建立 dotnet-testing-agent-skills 的 SKILL 索引

.DESCRIPTION
    此檔案為舊版通用腳本名稱的相容入口。
    實際行為等同 mcp-local-rag-index-skills-online.ps1。

.PARAMETER SkillsPath
    dotnet-testing-agent-skills 倉庫的 .github/skills 目錄路徑（必填）。

.PARAMETER Mode
    update（預設）：保留現有索引，只更新有變更的文件。
    rebuild：清除舊索引，完整重建。

.EXAMPLE
    .\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$SkillsPath,

    [ValidateSet("update", "rebuild")]
    [string]$Mode = "update"
)

$onlineScript = Join-Path $PSScriptRoot "mcp-local-rag-index-skills-online.ps1"

if (-not (Test-Path $onlineScript)) {
    Write-Host "找不到線上模式腳本：$onlineScript" -ForegroundColor Red
    exit 1
}

Write-Host "注意：mcp-local-rag-index-skills.ps1 為相容別名，預設等同線上模式。" -ForegroundColor Yellow
& $onlineScript -SkillsPath $SkillsPath -Mode $Mode
exit $LASTEXITCODE
