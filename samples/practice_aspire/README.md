# Practice Aspire — Aspire 整合測試驗證專案

本目錄為 `dotnet-testing-advanced-aspire-orchestrator` 的驗證專案，用於驗證 Aspire Testing Orchestrator 及其 4 個 Subagent 的泛化能力。

包含三種 .NET 版本，透過獨立的 `.slnx` 檔案管理：

| .slnx                        | .NET    | Aspire 版本 | EF Core | 說明                       |
| ---------------------------- | ------- | ----------- | ------- | -------------------------- |
| `Practice.Aspire.slnx`       | net9.0  | 9.0.0       | 9.0.4   | 基線版本                   |
| `Practice.Aspire.Net8.slnx`  | net8.0  | 8.2.2       | 8.0.11  | net8.0 跨版本驗證          |
| `Practice.Aspire.Net10.slnx` | net10.0 | 13.1.2      | 10.0.3  | net10.0 + Aspire 13.x 驗證 |

## 專案結構

```plaintext
practice_aspire/
├── Practice.Aspire.slnx                    # net9.0
├── Practice.Aspire.Net8.slnx               # net8.0
├── Practice.Aspire.Net10.slnx              # net10.0
├── README.md
├── src/
│   ├── Practice.Aspire.AppHost/            # Aspire AppHost（net9.0, Aspire 9.0.0）
│   ├── Practice.Aspire.WebApi/             # WebAPI（net9.0）
│   ├── Practice.Aspire.Net8.AppHost/       # Aspire AppHost（net8.0, Aspire 8.2.2）
│   ├── Practice.Aspire.Net8.WebApi/        # WebAPI（net8.0）
│   ├── Practice.Aspire.Net10.AppHost/      # Aspire AppHost（net10.0, Aspire 13.1.2）
│   └── Practice.Aspire.Net10.WebApi/       # WebAPI（net10.0）
└── tests/
    ├── Practice.Aspire.AppHost.Tests/      # 測試專案（由 Orchestrator 填充）
    ├── Practice.Aspire.Net8.AppHost.Tests/
    └── Practice.Aspire.Net10.AppHost.Tests/
```

## AppHost Resource 拓撲

三個版本共用相同的 Resource 拓撲：

| Resource     | 類型       | 方法                                  | 說明                  |
| ------------ | ---------- | ------------------------------------- | --------------------- |
| `sql`        | SQL Server | `AddSqlServer("sql")`                 | 資料庫容器            |
| `BookingsDb` | Database   | `sqlServer.AddDatabase("BookingsDb")` | SQL Server 上的資料庫 |
| `cache`      | Redis      | `AddRedis("cache")`                   | 快取容器              |
| `bookingapi` | Project    | `AddProject<T>("bookingapi")`         | 被編排的 WebAPI 專案  |

## API 端點

| HTTP Method | Route                             | 說明             |
| ----------- | --------------------------------- | ---------------- |
| GET         | `api/bookings`                    | 取得所有預約     |
| GET         | `api/bookings/{id}`               | 根據 ID 取得預約 |
| GET         | `api/bookings/by-status/{status}` | 根據狀態查詢預約 |
| POST        | `api/bookings`                    | 建立新預約       |
| PUT         | `api/bookings/{id}`               | 更新預約         |
| PATCH       | `api/bookings/{id}/confirm`       | 確認預約         |
| PATCH       | `api/bookings/{id}/checkin`       | 辦理入住         |
| PATCH       | `api/bookings/{id}/cancel`        | 取消預約         |
| DELETE      | `api/bookings/{id}`               | 刪除預約         |

## 版本差異

### net8.0（Aspire 8.2.2）

- `Aspire.AppHost.Sdk` NuGet 套件從 9.0.0 起才存在，因此 AppHost 使用 SDK 9.0.0 作為建置工具，但 runtime 套件（`Aspire.Hosting.*`）使用 8.2.2
- Aspire 8.x 不支援 `WaitFor()` API（從 9.0 起才引入），AppHost 僅使用 `WithReference()` 表達服務依賴
- csproj 格式：分離 SDK（`Microsoft.NET.Sdk` + `<Sdk Name="Aspire.AppHost.Sdk" .../>`)

### net10.0（Aspire 13.1.2）

- Aspire 版本號從 9.x 直接跳到 13.x（跳過 10–12），對應 .NET 10 的發布
- 新 csproj 格式：`<Project Sdk="Aspire.AppHost.Sdk/13.1.2">` 取代舊版格式
- 無獨立 `Aspire.Hosting.AppHost` PackageReference，由 SDK 自動包含

## 建置與測試

```powershell
# 建置（以 net9.0 為例）
dotnet build samples/practice_aspire/Practice.Aspire.slnx -p:WarningLevel=0 /clp:ErrorsOnly --verbosity minimal

# 測試（需要 Docker + Aspire Workload）
dotnet test samples/practice_aspire/Practice.Aspire.slnx --no-build --verbosity minimal
```
