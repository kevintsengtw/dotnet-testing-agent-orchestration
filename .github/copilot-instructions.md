# Repository Custom Instructions

## 專案概述

本 Repository 為 **.NET Testing Agent Skills Sample**，用於建立範例專案與驗證 `dotnet-testing` 及 `dotnet-testing-advanced` Agent Skills 的泛化能力。目標是確保 AI Agent 能夠將技能知識應用到「未見過」的程式碼。

- **主要語言**：C#（.NET 9.0）
- **測試框架**：xUnit 2.9.2
- **斷言庫**：AwesomeAssertions 9.1.0
- **Mock 框架**：NSubstitute 5.3.0
- **測試資料**：AutoFixture 4.18.1 + Bogus 35.6.1
- **特殊場景**：Microsoft.Extensions.TimeProvider.Testing 9.0.0、System.IO.Abstractions.TestingHelpers 21.1.3
- **覆蓋率**：coverlet.collector 6.0.2

---

## 專案結構

```
dotnet-testing-agent-skills-samples/
├── .github/
│   ├── agents/                  # Agent 定義檔（3 Orchestrators + 12 Subagents = 15 個）
│   ├── prompts/                 # GitHub Copilot Custom Prompts（16 個）
│   ├── skills/                  # GitHub Copilot Agent Skills（29 共用 + 1 專案專用）
│   └── copilot-instructions.md  # 本檔案
├── .agent/skills/               # Cursor Agent Skills（Junction → .github/skills/）
├── .agents/skills/              # Cursor AI Agent Skills（Junction → .github/skills/）
├── .claude/skills/              # Claude Code Agent Skills（Junction → .github/skills/）
├── .codex/skills/               # OpenAI Codex Agent Skills（Junction → .github/skills/）
├── .gemini/skills/              # Google Gemini Agent Skills（Junction → .github/skills/）
├── docs/
│   ├── orchestration/           # Agent Orchestration 相關文件
│   │   ├── AGENT_ORCHESTRATION_PLAN.md          # 架構規劃文件
│   │   ├── AGENT_ORCHESTRATION_FINAL_REPORT.md  # 驗證最終報告
│   │   ├── AGENT_TECHNICAL_FOUNDATIONS.md        # 技術基礎指南
│   │   ├── DOTNET_TESTING_ORCHESTRATION_OVERVIEW.md  # 架構總覽
│   │   ├── ORCHESTRATOR_USAGE_GUIDE.md          # 操作指南
│   │   ├── UNIT_TESTING_ORCHESTRATOR_PLAN.md     # Unit Testing Orchestrator 架構設計與實作計畫
│   │   ├── UNIT_TESTING_ORCHESTRATOR_VERIFICATION.md  # Unit Testing Orchestrator 驗證紀錄
│   │   ├── INTEGRATION_ORCHESTRATOR_PLAN.md      # Integration Orchestrator 架構設計與變更日誌
│   │   ├── INTEGRATION_ORCHESTRATOR_VERIFICATION.md   # Integration Orchestrator 驗證紀錄
│   │   ├── ASPIRE_TESTING_ORCHESTRATOR_PLAN.md    # Aspire Testing Orchestrator 架構設計與實作計畫
│   │   ├── ASPIRE_TESTING_ORCHESTRATOR_VERIFICATION.md  # Aspire Testing Orchestrator 驗證紀錄
│   │   ├── TUNIT_ORCHESTRATOR_PLAN.md            # TUnit Orchestrator 架構設計與實作計畫
│   │   └── TUNIT_ORCHESTRATOR_VERIFICATION.md    # TUnit Orchestrator 驗證紀錄
│   ├── skills/                  # Agent Skills 相關文件
│   │   ├── DOTNET_TESTING_SKILLS_GUIDE.md       # 完整學習指引
│   │   ├── SAMPLES_COVERAGE_REPORT.md           # 範例專案涵蓋分析
│   │   └── SKILL_VERIFICATION_PLAN.md           # 驗證計劃
│   └── prompts/                 # Prompt 相關文件
│       ├── github_copilot_prompt_dotnet_testing.md  # Custom Prompts 說明
│       └── PROMPT_PLANNING.md                   # Prompt 規劃文件
├── practice/                    # 練習專案（使用 Practice.Samples.slnx）
│   ├── src/Practice.Core/       # 待測試的程式碼（6 個 Service + 1 個轉換器）
│   └── tests/Practice.Core.Tests/  # 測試專案（Orchestrator 產生測試檔）
├── practice_integration/        # 整合測試驗證專案（使用 Practice.Integration.slnx）
│   ├── src/Practice.Integration.WebApi/   # 待測試的 WebAPI（OrdersController）
│   └── tests/Practice.Integration.WebApi.Tests/  # 測試專案（Integration Orchestrator 產生測試檔）
├── practice_aspire/             # Aspire 測試驗證專案（使用 Practice.Aspire.slnx）
│   ├── src/Practice.Aspire.AppHost/     # Aspire AppHost（SQL Server + Redis + WebAPI 編排）
│   ├── src/Practice.Aspire.WebApi/      # 被編排的 WebAPI（BookingsController）
│   └── tests/Practice.Aspire.AppHost.Tests/  # 測試專案（Aspire Orchestrator 產生測試檔）
├── practice_aspire_net8/        # Aspire net8.0 驗證專案（使用 Practice.Aspire.Net8.slnx）
│   ├── src/Practice.Aspire.Net8.AppHost/    # Aspire AppHost（net8.0, Aspire Hosting 8.2.2, SDK 9.0.0）
│   ├── src/Practice.Aspire.Net8.WebApi/     # 被編排的 WebAPI（net8.0, EF Core 8.0.11）
│   └── tests/Practice.Aspire.Net8.AppHost.Tests/  # 測試專案（Aspire Orchestrator 產生測試檔）
├── practice_aspire_net10/       # Aspire net10.0 驗證專案（使用 Practice.Aspire.Net10.slnx）
│   ├── src/Practice.Aspire.Net10.AppHost/   # Aspire AppHost（net10.0, Aspire 13.1.2）
│   ├── src/Practice.Aspire.Net10.WebApi/    # 被編排的 WebAPI（net10.0, EF Core 10.0.3）
│   └── tests/Practice.Aspire.Net10.AppHost.Tests/  # 測試專案（Aspire Orchestrator 產生測試檔）
├── practice_tunit/              # TUnit 測試驗證專案（使用 Practice.TUnit.slnx）
│   ├── src/Practice.TUnit.Core/             # 待測試的程式碼（5 個 Service + BookCatalog）
│   ├── tests/Practice.TUnit.Core.Tests/     # 測試專案（TUnit Orchestrator 產生測試檔）
│   └── migration_source/                    # xUnit→TUnit 遷移來源（BookCatalogXunitTests.cs）
├── practice_tunit_net8/         # TUnit net8.0 驗證專案（使用 Practice.TUnit.Net8.slnx）
│   ├── src/Practice.TUnit.Net8.Core/        # 待測試的程式碼（net8.0, TUnit 0.6.123）
│   ├── tests/Practice.TUnit.Net8.Core.Tests/  # 測試專案（TUnit Orchestrator 產生測試檔）
│   └── migration_source/                    # xUnit→TUnit 遷移來源
├── practice_tunit_net10/        # TUnit net10.0 驗證專案（使用 Practice.TUnit.Net10.slnx）
│   ├── src/Practice.TUnit.Net10.Core/       # 待測試的程式碼（net10.0, TUnit 0.6.123）
│   ├── tests/Practice.TUnit.Net10.Core.Tests/  # 測試專案（TUnit Orchestrator 產生測試檔）
│   └── migration_source/                    # xUnit→TUnit 遷移來源
├── samples/
│   ├── verification/            # 驗證專案（Verification.Samples.slnx）
│   ├── integration/             # 整合測試範例（Integration.Samples.slnx）
│   ├── aspire/                  # .NET Aspire 測試範例（Aspire.Samples.slnx）
│   ├── tunit/                   # TUnit 測試範例（TUnit.Samples.slnx）
│   └── xunit-upgrade/           # xUnit 升級範例（XunitUpgrade.Samples.slnx）
```

---

## 建置與測試

### 基本指令

```powershell
# 建置整個練習方案
dotnet build practice/Practice.Samples.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal

# 執行練習專案測試
dotnet test practice/Practice.Samples.slnx --no-build --verbosity minimal

# 建置並執行整合測試驗證專案
dotnet build practice_integration/Practice.Integration.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal
dotnet test practice_integration/Practice.Integration.slnx --no-build --verbosity minimal

# 建置並執行 Aspire 測試驗證專案（需要 Docker + Aspire workload）
dotnet build practice_aspire/Practice.Aspire.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal
dotnet test practice_aspire/Practice.Aspire.slnx --no-build --verbosity minimal

# 建置並執行 Aspire net8.0 跨版本驗證專案（需要 Docker）
dotnet build practice_aspire_net8/Practice.Aspire.Net8.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal
dotnet test practice_aspire_net8/Practice.Aspire.Net8.slnx --no-build --verbosity minimal

# 建置並執行 Aspire net10.0 跨版本驗證專案（需要 Docker）
dotnet build practice_aspire_net10/Practice.Aspire.Net10.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal
dotnet test practice_aspire_net10/Practice.Aspire.Net10.slnx --no-build --verbosity minimal

# 建置 TUnit 測試驗證專案（TUnit 使用 dotnet run 執行測試）
dotnet build practice_tunit/Practice.TUnit.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal
dotnet run --project practice_tunit/tests/Practice.TUnit.Core.Tests/Practice.TUnit.Core.Tests.csproj --no-build

# 建置 TUnit net8.0 跨版本驗證專案
dotnet build practice_tunit_net8/Practice.TUnit.Net8.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal
dotnet run --project practice_tunit_net8/tests/Practice.TUnit.Net8.Core.Tests/Practice.TUnit.Net8.Core.Tests.csproj --no-build

# 建置 TUnit net10.0 跨版本驗證專案
dotnet build practice_tunit_net10/Practice.TUnit.Net10.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal
dotnet run --project practice_tunit_net10/tests/Practice.TUnit.Net10.Core.Tests/Practice.TUnit.Net10.Core.Tests.csproj --no-build

# 建置並執行驗證專案
dotnet build samples/verification/Verification.Samples.slnx
dotnet test samples/verification/Verification.Samples.slnx --no-build --verbosity minimal

# 建置並執行整合測試範例（需要 Docker）
dotnet build samples/integration/Integration.Samples.slnx
dotnet test samples/integration/Integration.Samples.slnx --no-build --verbosity minimal
```

### 測試執行原則

1. **先建置再測試**：永遠使用 `dotnet build` 先建置，再用 `dotnet test --no-build` 執行測試
2. **建置使用低警告等級**：建置時使用 `-p:WarningLevel=0 /clp:ErrorsOnly` 以減少雜訊
3. **針對性測試**：優先測試特定專案或特定測試類別，避免一次跑全部
4. **覆蓋率收集**：需要覆蓋率時使用 `dotnet test --collect:"XPlat Code Coverage"`

---

## Agent Orchestration 架構

本專案使用 **1 Orchestrator + 4 Subagents** 架構來處理 .NET 單元測試任務：

| Agent | 角色 | 說明 |
|-------|------|------|
| `dotnet-testing-orchestrator` | 指揮中心 | 分析被測試目標、決定技術組合、委派 subagent |
| `dotnet-testing-analyzer` | 分析器 | 分析被測類別結構、依賴項、需要的測試技術 |
| `dotnet-testing-writer` | 撰寫器 | 載入對應 Skills，撰寫符合最佳實踐的測試 |
| `dotnet-testing-executor` | 執行器 | 建置與執行測試，處理編譯錯誤與失敗修正 |
| `dotnet-testing-reviewer` | 審查器 | 審查測試品質，驗證命名、斷言、覆蓋率等 |

### 進階整合測試 Orchestrator（Phase 5 P1）

本專案另有 **Integration Testing Orchestrator**（1 + 4 Subagents）處理 WebAPI 整合測試：

| Agent | 角色 | 說明 |
|-------|------|------|
| `dotnet-testing-advanced-integration-orchestrator` | 整合測試指揮中心 | 分析 WebAPI 端點、決定容器需求、委派 subagent |
| `dotnet-testing-advanced-integration-analyzer` | 整合測試分析器 | 分析 API 端點結構、DbContext、容器需求、FluentValidation |
| `dotnet-testing-advanced-integration-writer` | 整合測試撰寫器 | 載入整合測試 Skills，撰寫 WebApplicationFactory 測試 |
| `dotnet-testing-advanced-integration-executor` | 整合測試執行器 | Docker 環境檢查、建置、執行測試、修正迴圈 |
| `dotnet-testing-advanced-integration-reviewer` | 整合測試審查器 | 審查整合測試品質、容器管理、端點覆蓋率 |

### Aspire Testing Orchestrator（Phase 5 P2）

本專案另有 **Aspire Testing Orchestrator**（1 + 4 Subagents）處理 .NET Aspire 分散式應用程式整合測試：

| Agent | 角色 | 說明 |
|-------|------|------|
| `dotnet-testing-advanced-aspire-orchestrator` | Aspire 測試指揮中心 | 分析 AppHost Resource 定義、委派 subagent |
| `dotnet-testing-advanced-aspire-analyzer` | Aspire 測試分析器 | 分析 AppHost Resource 拓撲、被編排 API 端點結構 |
| `dotnet-testing-advanced-aspire-writer` | Aspire 測試撰寫器 | 載入 aspire-testing Skill，撰寫 DistributedApplicationTestingBuilder 測試 |
| `dotnet-testing-advanced-aspire-executor` | Aspire 測試執行器 | Docker + Aspire workload 檢查、建置、執行測試 |
| `dotnet-testing-advanced-aspire-reviewer` | Aspire 測試審查器 | 審查 Aspire 特定項目、容器管理、Resource 名稱一致性 |

Agent 定義檔位於 `.github/agents/` 目錄。詳細架構請參考 `docs/orchestration/AGENT_ORCHESTRATION_PLAN.md`。

---

## Agent Skills 說明

### Skills 目錄結構

Skills 內容的上游來源為 [dotnet-testing-agent-skills](https://github.com/kevintsengtw/dotnet-testing-agent-skills) Repository。

`.github/skills/` 為唯一的實體目錄，其他 5 個目錄透過 **Directory Junction** 指向它：

| 目錄 | 類型 | 說明 |
|------|------|------|
| `.github/skills/` | 實體目錄 | GitHub Copilot（唯一來源） |
| `.agent/skills/` | Junction | Cursor Agent → `.github/skills/` |
| `.agents/skills/` | Junction | Cursor AI Agent → `.github/skills/` |
| `.claude/skills/` | Junction | Claude Code → `.github/skills/` |
| `.codex/skills/` | Junction | OpenAI Codex → `.github/skills/` |
| `.gemini/skills/` | Junction | Google Gemini → `.github/skills/` |

### 共用 Skills（29 個，來自上游）

涵蓋基礎測試（unit-test-fundamentals、xunit-project-setup 等）、斷言、Mock、測試資料、特殊場景、進階整合測試、容器化測試、TUnit、xUnit 升級等。

### 專案專用 Skill（1 個）

- `dotnet-test`：本專案專用的 .NET 測試執行工作流程 Skill，**不來自上游**，僅存在於本 Repository。

---

## Skills 同步工作流程

當上游 [dotnet-testing-agent-skills](https://github.com/kevintsengtw/dotnet-testing-agent-skills) 有更新時，需同步 skills 內容到本 Repository。

### 同步步驟

由於其他 5 個目錄已透過 Junction 指向 `.github/skills/`，同步時只需更新主要目錄即可：

```powershell
# 1. 淺層 clone 上游最新版本到暫存目錄
git clone --depth 1 https://github.com/kevintsengtw/dotnet-testing-agent-skills.git C:\temp\dotnet-testing-agent-skills-latest

# 2. 同步到 .github/skills/（唯一實體目錄，使用 /MIR 鏡像模式）
#    ⚠️ 注意：/MIR 會刪除目標中來源不存在的檔案
#    ⚠️ 需先排除專案專用的 dotnet-test/ 目錄
robocopy "C:\temp\dotnet-testing-agent-skills-latest\skills" ".github\skills" /E /MIR /XD "dotnet-test"

# 3. 不需要再同步其他目錄（Junction 會自動反映變更）

# 4. 清理暫存目錄
Remove-Item -Recurse -Force "C:\temp\dotnet-testing-agent-skills-latest"
```

### 同步注意事項

- **使用 `/MIR`（鏡像模式）** 而非 `/E`：確保自動清理上游已刪除的檔案，避免殘留舊檔
- **排除專案專用 Skill**：同步 `.github/skills/` 時使用 `/XD "dotnet-test"` 排除不來自上游的目錄
- **只需同步主要目錄**：其他 5 個目錄透過 Directory Junction 自動反映 `.github/skills/` 的變更
- **Junction 與 Symlink 差異**：Windows 上 Directory Junction 不需管理員權限，使用 `mklink /J` 建立
- **勿刪除 Junction 目錄後重建為實體目錄**：若誤刪 Junction，使用 `cmd /c "mklink /J <target> <source_absolute_path>"` 重建

---

## 編碼慣例

- 測試命名遵循 `MethodName_Scenario_ExpectedResult` 格式
- 測試方法使用 3A Pattern（Arrange / Act / Assert）
- 使用 AwesomeAssertions 的流暢語法撰寫斷言（例如 `result.Should().Be(expected)`）
- Mock 使用 NSubstitute（`Substitute.For<T>()`）
- 測試資料優先使用 AutoFixture 或 Bogus，減少手動建立
- 時間相依測試使用 `TimeProvider` + `FakeTimeProvider`
- 檔案系統測試使用 `IFileSystem` + `MockFileSystem`

---

## 相關文件

### Orchestration 文件（`docs/orchestration/`）

- `docs/orchestration/DOTNET_TESTING_ORCHESTRATION_OVERVIEW.md`：Agent Skills 系列 + Orchestrator 架構總覽（效益與驗證成果）
- `docs/orchestration/AGENT_TECHNICAL_FOUNDATIONS.md`：技術基礎指南（Subagent、Custom Agent、Skills 載入原理）
- `docs/orchestration/ORCHESTRATOR_USAGE_GUIDE.md`：dotnet-testing-orchestrator 使用者操作指南
- `docs/orchestration/AGENT_ORCHESTRATION_PLAN.md`：Agent Orchestration 共用架構規劃（背景、技術基礎、Phase 5 規劃、風險評估）
- `docs/orchestration/AGENT_ORCHESTRATION_FINAL_REPORT.md`：Agent Orchestration 驗證最終報告
- `docs/orchestration/UNIT_TESTING_ORCHESTRATOR_PLAN.md`：Unit Testing Orchestrator 架構設計與 Phase 1~4 實作計畫
- `docs/orchestration/UNIT_TESTING_ORCHESTRATOR_VERIFICATION.md`：Unit Testing Orchestrator 驗證過程紀錄（11 次驗證）
- `docs/orchestration/INTEGRATION_ORCHESTRATOR_PLAN.md`：Integration Testing Orchestrator 架構設計與變更日誌
- `docs/orchestration/INTEGRATION_ORCHESTRATOR_VERIFICATION.md`：Integration Testing Orchestrator 驗證過程紀錄（5 個場景）
- `docs/orchestration/ASPIRE_TESTING_ORCHESTRATOR_PLAN.md`：Aspire Testing Orchestrator 架構設計與實作計畫（Phase 5 P2）
- `docs/orchestration/ASPIRE_TESTING_ORCHESTRATOR_VERIFICATION.md`：Aspire Testing Orchestrator 驗證紀錄
- `docs/orchestration/TUNIT_ORCHESTRATOR_PLAN.md`：TUnit Orchestrator 架構設計與實作計畫（Phase 5 P3）

### Skills 文件（`docs/skills/`）

- `docs/skills/DOTNET_TESTING_SKILLS_GUIDE.md`：Skills 完整學習指引與使用說明
- `docs/skills/SAMPLES_COVERAGE_REPORT.md`：範例專案涵蓋分析報告
- `docs/skills/SKILL_VERIFICATION_PLAN.md`：驗證計劃文件

### Prompts 文件（`docs/prompts/`）

- `docs/prompts/github_copilot_prompt_dotnet_testing.md`：GitHub Copilot Custom Prompts 說明
- `docs/prompts/PROMPT_PLANNING.md`：Prompt 規劃文件
