# Practice — 單元測試驗證專案

本目錄為 `dotnet-testing-orchestrator` 的驗證專案，用於驗證 Unit Testing Orchestrator 及其 4 個 Subagent 的泛化能力。

包含三種 .NET 版本，透過獨立的 `.slnx` 檔案管理：

| .slnx                         | .NET    | 說明               |
| ----------------------------- | ------- | ------------------ |
| `Practice.Samples.slnx`       | net9.0  | 基線版本           |
| `Practice.Samples.Net8.slnx`  | net8.0  | net8.0 跨版本驗證  |
| `Practice.Samples.Net10.slnx` | net10.0 | net10.0 跨版本驗證 |

## 專案結構

```plaintext
practice/
├── Practice.Samples.slnx                # net9.0
├── Practice.Samples.Net8.slnx           # net8.0
├── Practice.Samples.Net10.slnx          # net10.0
├── README.md
├── src/
│   ├── Practice.Core/                   # 被測專案（net9.0）
│   │   ├── Interfaces/                  # 介面定義
│   │   ├── Models/                      # 資料模型
│   │   ├── Services/                    # 服務類別
│   │   ├── Validators/                  # FluentValidation 驗證器
│   │   ├── Legacy/                      # 遺留程式碼（重構練習）
│   │   └── TemperatureConverter.cs      # 純函式類別
│   ├── Practice.Core.Net8/              # 被測專案（net8.0）
│   └── Practice.Core.Net10/             # 被測專案（net10.0）
└── tests/
    ├── Practice.Core.Tests/             # 測試專案（由 Orchestrator 填充）
    ├── Practice.Core.Net8.Tests/
    └── Practice.Core.Net10.Tests/
```

## 領域說明

以多種業務場景覆蓋三種目標類型（Service / Validator / Legacy），驗證 Orchestrator 的目標類型識別與技能組合能力。

### 被測類別與目標類型對應

| 類別                     | 目標類型  | 測試技術需求                                    |
| ------------------------ | --------- | ----------------------------------------------- |
| `TemperatureConverter`   | Service   | 純函式、基礎 3A Pattern                         |
| `WeatherAlertService`    | Service   | NSubstitute Mock、非同步方法                    |
| `EmployeeService`        | Service   | AutoFixture、Bogus、循環參考處理                |
| `SubscriptionService`    | Service   | TimeProvider 時間控制                           |
| `ConfigurationLoader`    | Service   | System.IO.Abstractions 檔案系統模擬             |
| `OrderProcessingService` | Service   | 跨技能整合（Mock + AutoFixture + TimeProvider） |
| `EmployeeValidator`      | Validator | FluentValidation `TestValidate()`               |
| `OrderValidator`         | Validator | FluentValidation 巢狀驗證                       |
| `OrderItemValidator`     | Validator | FluentValidation 子驗證器                       |
| `LegacyReportGenerator`  | Legacy    | Characterization Test、靜態依賴、硬編碼資料     |
| `ReportGenerator`        | Service   | 重構後的可測試版本（與 Legacy 對照）            |

## 版本差異

| 面向                   | net9.0          | net8.0               | net10.0               |
| ---------------------- | --------------- | -------------------- | --------------------- |
| FluentValidation       | 11.11.0         | —                    | —                     |
| System.IO.Abstractions | 21.1.3          | —                    | —                     |
| Namespace 前綴         | `Practice.Core` | `Practice.Core.Net8` | `Practice.Core.Net10` |
| 驗證重點               | 基線迴歸        | net8.0 相容性        | net10.0 相容性        |

> **注意**：Net8 與 Net10 版本的被測專案為簡化版，不包含 FluentValidation 與 System.IO.Abstractions 依賴。

## 建置與測試

```powershell
# 建置（以 net9.0 為例）
dotnet build samples/practice/Practice.Samples.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal

# 測試
dotnet test samples/practice/Practice.Samples.slnx --no-build --verbosity minimal
```
