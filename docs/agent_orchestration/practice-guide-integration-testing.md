# 整合測試 Orchestrator 操作指南

> **Orchestrator**：`dotnet-testing-advanced-integration-orchestrator`
> **驗證專案**：`samples/practice_integration/`（.NET 9.0 基線版本，使用 `Practice.Integration.slnx`）

---

## 可驗證版本

本指南以 **.NET 9.0** 為基線版本。三個版本的驗證專案結構完全相同，可自由選擇：

| 版本         | .slnx                             | 來源專案路徑                                                  |
| ------------ | --------------------------------- | ------------------------------------------------------------- |
| **.NET 9.0** | `Practice.Integration.slnx`       | `practice_integration/src/Practice.Integration.WebApi/`       |
| .NET 8.0     | `Practice.Integration.Net8.slnx`  | `practice_integration/src/Practice.Integration.WebApi.Net8/`  |
| .NET 10.0    | `Practice.Integration.Net10.slnx` | `practice_integration/src/Practice.Integration.WebApi.Net10/` |

> 驗證其他版本時，將情境中的檔案路徑替換為對應版本的專案路徑即可。例如：
> `#file:practice_integration/src/Practice.Integration.WebApi.Net8/Controllers/OrdersController.cs`

### 還原驗證結果

```powershell
git restore samples/practice_integration/tests/
git clean -fd samples/practice_integration/tests/
```

---

## 前置準備

| 項目               | 說明                                       |
| ------------------ | ------------------------------------------ |
| **VS Code**        | 1.109 以上，已安裝 GitHub Copilot Chat     |
| **VS Code 設定**   | `chat.customAgentInSubagent.enabled: true` |
| **.NET SDK**       | .NET 9.0 SDK                               |
| **Docker Desktop** | 必須執行中（Testcontainers 需要）          |

## 操作步驟

1. 在 Copilot Chat 切換到 **Agent** 模式
2. 從 Agent 下拉選單選擇 `dotnet-testing-advanced-integration-orchestrator`
3. 使用 `#file:` 引用 Controller 檔案，或直接描述要測試的端點

## 預期結果

測試程式碼產生在 `samples/practice_integration/tests/Practice.Integration.WebApi.Tests/` 目錄中，包含 `Fixtures/`、`TestBase/`、`Controllers/` 目錄結構。

---

## 驗證情境

### 情境 1：SQL Server 整合測試（使用 `#file:` 引用）

```plaintext
#file:practice_integration/src/Practice.Integration.WebApi/Controllers/OrdersController.cs

測試 OrdersController 的所有 CRUD 端點
```

> 驗證 Orchestrator 對使用 SQL Server + Entity Framework Core 的 WebAPI Controller 的完整整合測試流程。

**觀察重點**：

- **Analyzer**：是否正確分析出 `OrderDbContext`（SQL Server）、FluentValidation 依賴、`TimeProvider` 注入模式
- **Writer**：是否建立 `WebApiFactory`（覆寫 `ConfigureWebHost` 替換 Provider）、`IntegrationTestBase`（處理資料隔離）、測試類別
- **Executor**：Docker 環境檢查、SQL Server Testcontainer 啟動、資料庫 Migration 與測試執行
- **Reviewer**：容器配置是否正確、資料隔離策略是否每測試獨立

---

### 情境 2：SQL Server 整合測試（純文字描述）

```plaintext
為 OrdersController 建立使用 SQL Server Testcontainers 的整合測試
```

> 驗證 Orchestrator 在不提供 `#file:` 的情況下，能否透過專案搜尋找到 `OrdersController` 並正確分析。

**觀察重點**：

- **Analyzer**：是否能自行定位到 `OrdersController.cs` 並分析完整的 API 架構
- **Writer**：產出品質是否與情境 1（使用 `#file:`）一致
- **Executor**：是否能找到正確的測試專案路徑進行建置與執行
- **Reviewer**：測試覆蓋率與 CRUD 端點對應是否完整

---

### 情境 3：MongoDB NoSQL 整合測試

```plaintext
#file:practice_integration/src/Practice.Integration.WebApi/Controllers/CustomerActivitiesController.cs

測試 CustomerActivitiesController 的所有 API 端點
```

> 驗證 Orchestrator 對使用 MongoDB 的 Controller 能否正確識別 NoSQL 容器需求，並使用 MongoDB Testcontainer。

**觀察重點**：

- **Analyzer**：是否識別出 `ICustomerActivityRepository`（MongoDB）依賴，而非 `DbContext`（SQL Server）
- **Writer**：是否使用 MongoDB Testcontainer（而非 SQL Server），是否正確替換 MongoDB 連線字串
- **Executor**：MongoDB 容器啟動是否正常，文件型資料的 CRUD 是否通過
- **Reviewer**：NoSQL 資料隔離策略是否合理（例如每測試清理 Collection）


