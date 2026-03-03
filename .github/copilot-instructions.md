# Repository Custom Instructions

## 專案概述

本 Repository 為 **.NET Testing Agent Orchestration**，包含 Agent 定義檔、Skills、Custom Prompts 以及驗證專案，用於驗證 `dotnet-testing` 及 `dotnet-testing-advanced` Agent Skills 搭配 Orchestrator 的泛化能力。目標是確保 AI Agent 能夠將技能知識應用到「未見過」的程式碼。

---

## 專案結構

```
dotnet-testing-agent-orchestration/
├── .github/
│   ├── agents/                  # Agent 定義檔（4 Orchestrators + 16 Subagents = 20 個）
│   ├── prompts/                 # GitHub Copilot Custom Prompts（16 個）
│   ├── skills/                  # GitHub Copilot Agent Skills（29 共用 + 1 專案專用 + 4 OpenSpec）
│   └── copilot-instructions.md  # 本檔案
├── docs/
│   ├── agent_orchestration/     # Agent Orchestration 相關文件
│   ├── skills/                  # Agent Skills 相關文件
│   └── prompts/                 # Prompt 相關文件
├── samples/                     # 驗證專案（各含 3 種 .NET 版本）
│   ├── practice/                # 單元測試驗證專案
│   ├── practice_integration/    # 整合測試驗證專案
│   ├── practice_aspire/         # Aspire 測試驗證專案
│   └── practice_tunit/          # TUnit 測試驗證專案
├── README.md
└── .gitignore
```

---

## 建置與測試

### 測試執行原則

1. **先建置再測試**：永遠使用 `dotnet build` 先建置，再用 `dotnet test --no-build` 執行測試
2. **建置使用低警告等級**：建置時使用 `-p:WarningLevel=0 /clp:ErrorsOnly` 以減少雜訊
3. **針對性測試**：優先測試特定專案或特定測試類別，避免一次跑全部
4. **覆蓋率收集**：需要覆蓋率時使用 `dotnet test --collect:"XPlat Code Coverage"`
5. **TUnit 使用 dotnet run**：TUnit 測試專案使用 `dotnet run` 執行（非 `dotnet test`）
6. **驗證專案路徑**：所有驗證專案位於 `samples/` 目錄下，每個專案提供 3 種 .NET 版本的 `.slnx`

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

## GitHub Copilot 指導原則

### C# 寫作風格

#### 語言規範
- 使用台灣繁體中文，避免簡體中文以及中國的技術用語
- 類別、建構式、屬性、方法以及程式註解一律使用繁體中文，變數名稱、函數名稱、類別名稱一律使用英文
- 屬性的 Summary 只需要簡潔的描述，不需要有 get, set 的說明
- 變數 CancellationToken 的名稱不要使用 `取消令牌`，直接使用 `CancellationToken`
- 如果 if 判斷式只有一層，也不能省略巢狀結構，一定要有大括號 `{`, `}`
- 一個類別就是一個檔案，不要所有類別都放在同一個 cs 檔案裡
