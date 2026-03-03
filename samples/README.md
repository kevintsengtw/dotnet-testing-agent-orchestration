# 驗證專案（Practice Samples）

本目錄包含四組驗證專案，分別對應四個 Agent Orchestrator，用於驗證 Orchestrator 及其 Subagent 的泛化能力。

每組專案提供三種 .NET 版本（net8.0 / net9.0 / net10.0），透過獨立的 `.slnx` 檔案管理，方便在不同版本環境下進行驗證。

## 專案總覽

| 目錄                                           | 對應 Orchestrator                                  | 領域                                   | 測試框架                                       |
| ---------------------------------------------- | -------------------------------------------------- | -------------------------------------- | ---------------------------------------------- |
| [practice/](practice/)                         | `dotnet-testing-orchestrator`                      | 多領域（溫度轉換、天氣、訂單、員工等） | xUnit                                          |
| [practice_integration/](practice_integration/) | `dotnet-testing-advanced-integration-orchestrator` | 訂單管理（Orders）                     | xUnit + WebApplicationFactory + Testcontainers |
| [practice_aspire/](practice_aspire/)           | `dotnet-testing-advanced-aspire-orchestrator`      | 預約管理（Bookings）                   | xUnit + DistributedApplicationTestingBuilder   |
| [practice_tunit/](practice_tunit/)             | `dotnet-testing-advanced-tunit-orchestrator`       | 圖書館管理（Library）                  | TUnit                                          |

## 目錄結構

```plaintext
samples/
├── README.md                    # 本檔案
├── practice/                    # 單元測試驗證
│   ├── Practice.Samples.slnx       # net9.0
│   ├── Practice.Samples.Net8.slnx  # net8.0
│   └── Practice.Samples.Net10.slnx # net10.0
├── practice_integration/        # 整合測試驗證
│   ├── Practice.Integration.slnx       # net9.0
│   ├── Practice.Integration.Net8.slnx  # net8.0
│   └── Practice.Integration.Net10.slnx # net10.0
├── practice_aspire/             # Aspire 測試驗證
│   ├── Practice.Aspire.slnx       # net9.0 + Aspire 9.0.0
│   ├── Practice.Aspire.Net8.slnx  # net8.0 + Aspire 8.2.2
│   └── Practice.Aspire.Net10.slnx # net10.0 + Aspire 13.1.2
└── practice_tunit/              # TUnit 測試驗證
    ├── Practice.TUnit.slnx       # net9.0
    ├── Practice.TUnit.Net8.slnx  # net8.0
    └── Practice.TUnit.Net10.slnx # net10.0
```

## 各版本的 .slnx 檔案

每組專案以一個目錄、多個 `.slnx` 的結構管理三種 .NET 版本。每個 `.slnx` 只引用對應版本的專案，互不干擾。

| .slnx 後綴 | .NET 版本 | 說明               |
| ---------- | --------- | ------------------ |
| （無後綴） | net9.0    | 基線版本           |
| `.Net8`    | net8.0    | net8.0 跨版本驗證  |
| `.Net10`   | net10.0   | net10.0 跨版本驗證 |

## 使用方式

### 1. 選擇驗證目標

根據要驗證的 Orchestrator 選擇對應目錄，再選擇目標 .NET 版本的 `.slnx`。

### 2. 建置

```powershell
# 範例：建置 practice 的 net9.0 版本
dotnet build samples/practice/Practice.Samples.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal
```

### 3. 使用 Orchestrator 產生測試

在 VS Code Copilot Chat 的 Agent 下拉選單中選擇對應的 Orchestrator，描述測試目標。測試專案（`tests/` 目錄下）的內容將由 Orchestrator 自動產生。

### 4. 執行測試

```powershell
# xUnit 測試
dotnet test samples/practice/Practice.Samples.slnx --no-build --verbosity minimal

# TUnit 測試（使用 dotnet run）
dotnet run --project samples/practice_tunit/tests/Practice.TUnit.Core.Tests/Practice.TUnit.Core.Tests.csproj
```

## 設計原則

- **獨立領域**：每組專案使用不同的業務領域，避免與既有範例重疊
- **空測試專案**：測試專案僅包含 `.csproj`，測試程式碼由 Orchestrator 從零產生
- **版本隔離**：不同 .NET 版本使用獨立的 Namespace 前綴，不共用程式碼
- **環境需求**：整合測試與 Aspire 測試需要 Docker Desktop；Aspire 測試額外需要 Aspire Workload
