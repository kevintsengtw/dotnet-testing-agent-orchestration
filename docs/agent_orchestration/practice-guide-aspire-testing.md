# Aspire 整合測試 Orchestrator 操作指南

> **Orchestrator**：`dotnet-testing-advanced-aspire-orchestrator`
> **驗證專案**：`samples/practice_aspire/`（.NET 9.0 基線版本，使用 `Practice.Aspire.slnx`）

---

## 可驗證版本

本指南以 **.NET 9.0** 為基線版本。三個版本的驗證專案結構完全相同，可自由選擇：

| 版本         | .slnx                        | AppHost 路徑                                         |
| ------------ | ---------------------------- | ---------------------------------------------------- |
| **.NET 9.0** | `Practice.Aspire.slnx`       | `practice_aspire/src/Practice.Aspire.AppHost/`       |
| .NET 8.0     | `Practice.Aspire.Net8.slnx`  | `practice_aspire/src/Practice.Aspire.Net8.AppHost/`  |
| .NET 10.0    | `Practice.Aspire.Net10.slnx` | `practice_aspire/src/Practice.Aspire.Net10.AppHost/` |

> 驗證其他版本時，將情境中的檔案路徑替換為對應版本的專案路徑即可。例如：
> `#file:practice_aspire/src/Practice.Aspire.Net8.AppHost/Program.cs`

### 還原驗證結果

```powershell
git restore samples/practice_aspire/tests/
git clean -fd samples/practice_aspire/tests/
```

---

## 前置準備

| 項目                | 說明                                         |
| ------------------- | -------------------------------------------- |
| **VS Code**         | 1.109 以上，已安裝 GitHub Copilot Chat       |
| **VS Code 設定**    | `chat.customAgentInSubagent.enabled: true`   |
| **.NET SDK**        | .NET 9.0 SDK                                 |
| **Docker Desktop**  | 必須執行中（Aspire Resource 容器需要）       |
| **Aspire Workload** | 必須安裝（`dotnet workload install aspire`） |

### 預先拉取 Docker Images

Aspire AppHost 會自動拉取所需的容器映像，但首次啟動時下載耗時較長。建議在驗證前預先拉取，加速測試執行：

| 容器資源                            | Docker Image                                 | 用途              |
| ----------------------------------- | -------------------------------------------- | ----------------- |
| SQL Server（`AddSqlServer("sql")`） | `mcr.microsoft.com/mssql/server:2022-latest` | BookingsDb 資料庫 |
| Redis（`AddRedis("cache")`）        | `redis:latest`                               | 快取服務          |

```powershell
# 一次拉取所有需要的 images
docker pull mcr.microsoft.com/mssql/server:2022-latest
docker pull redis:latest
```

> Aspire 實際使用的映像版本可能因 Aspire SDK 版本而異，上述為常見預設映像。若 Aspire 啟動時自動拉取了不同版本，屬正常行為。

## 操作步驟

1. 在 Copilot Chat 切換到 **Agent** 模式
2. 從 Agent 下拉選單選擇 `dotnet-testing-advanced-aspire-orchestrator`
3. 使用 `#file:` 引用 AppHost 的 `Program.cs`，或直接描述要測試的服務

## 預期結果

測試程式碼產生在 `samples/practice_aspire/tests/Practice.Aspire.AppHost.Tests/` 目錄中，包含 AspireAppFixture、CollectionDefinition、IntegrationTestBase。

---

## 驗證情境

### 情境 1：Aspire 整合測試（使用 `#file:` 引用）

```plaintext
#file:practice_aspire/src/Practice.Aspire.AppHost/Program.cs

測試 bookingapi 服務的所有 API 端點
```

> 驗證 Orchestrator 對 Aspire AppHost 定義的分散式應用程式拓撲能否正確解析，並使用 `DistributedApplicationTestingBuilder` 建立測試。

**觀察重點**：

- **Analyzer**：是否正確解析 AppHost Resource 拓撲（`sql` → `BookingsDb`、`cache`、`bookingapi`），識別服務間依賴關係
- **Writer**：是否使用 `DistributedApplicationTestingBuilder`（**非** `WebApplicationFactory`），Resource 名稱是否與 AppHost 定義一致（`bookingapi`）
- **Executor**：Docker + Aspire Workload 環境檢查是否通過，超時設定是否足夠（Aspire 啟動較慢，需 10 分鐘以上）
- **Reviewer**：是否正確使用 Collection Fixture 共用 Aspire App 實例，無 `WebApplicationFactory` 殘留

---

### 情境 2：Aspire 整合測試（純文字描述）

```plaintext
為 Practice.Aspire.AppHost 中的 bookingapi 建立 Aspire 整合測試
```

> 驗證 Orchestrator 在不提供 `#file:` 的情況下，能否自行定位到 AppHost 的 `Program.cs` 並正確分析。

**觀察重點**：

- **Analyzer**：是否能搜尋到正確的 AppHost 專案並解析 Resource 定義
- **Writer**：產出品質是否與情境 1 一致，是否正確建立 Fixture 共用 App 實例
- **Executor**：容器啟動順序（先 SQL Server → 再 Redis → 最後 bookingapi）是否正確
- **Reviewer**：BookingsController 的 CRUD 端點測試覆蓋是否完整
