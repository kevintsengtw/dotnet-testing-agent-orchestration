# .NET Testing Agent Skills

本工作區的 `.github/skills/` 目錄包含 **dotnet-testing-agent-skills v2.2.0** 的 Agent Skills 集合。

---

## 簡介

dotnet-testing-agent-skills 是專為 .NET 開發者打造的 AI Agent Skills 集合，涵蓋從單元測試到整合測試的完整最佳實踐，讓 GitHub Copilot、Claude 等 AI 助理自動提供專業的測試指導。

此 Skills 集合包含 **29 個技能**（2 個總覽技能 + 27 個專業技能），分為五大階段：

| 階段           | 內容                                                   | 技能數量 |
| -------------- | ------------------------------------------------------ | -------- |
| 基礎測試與斷言 | 單元測試基礎、命名規範、xUnit 設定、斷言、Mock、覆蓋率 | 10       |
| 可測試性抽象化 | TimeProvider、System.IO.Abstractions                   | 2        |
| 測試資料生成   | AutoFixture、Bogus、Test Data Builder、AutoData        | 7        |
| 整合測試       | ASP.NET Core、Testcontainers、Web API、.NET Aspire     | 5        |
| 框架遷移       | xUnit v2 to v3 升級、TUnit 新世代框架                  | 3        |

另外還包含 1 個第三方 Skill `dotnet-test`，專注於測試執行與除錯。

---

## Agent Skills 列表

### 總覽技能（2 個）

| Skill                     | 說明                                                 |
| ------------------------- | ---------------------------------------------------- |
| `dotnet-testing`          | 基礎測試技能總覽與引導中心，自動推薦適合的子技能組合 |
| `dotnet-testing-advanced` | 進階測試技能總覽與引導中心，涵蓋整合測試與框架遷移   |

### 基礎測試與斷言（10 個）

| Skill                                      | 說明                                |
| ------------------------------------------ | ----------------------------------- |
| `dotnet-testing-unit-test-fundamentals`    | FIRST 原則、AAA Pattern、測試金字塔 |
| `dotnet-testing-test-naming-conventions`   | 三段式命名法、中文命名建議          |
| `dotnet-testing-xunit-project-setup`       | xUnit 專案結構、配置、套件管理      |
| `dotnet-testing-awesome-assertions-guide`  | AwesomeAssertions 流暢斷言          |
| `dotnet-testing-complex-object-comparison` | 深層物件比對技巧                    |
| `dotnet-testing-code-coverage-analysis`    | Coverlet 覆蓋率分析與報告           |
| `dotnet-testing-nsubstitute-mocking`       | Mock/Stub/Spy 測試替身              |
| `dotnet-testing-test-output-logging`       | ITestOutputHelper 與 ILogger 整合   |
| `dotnet-testing-private-internal-testing`  | Private/Internal 成員測試策略       |
| `dotnet-testing-fluentvalidation-testing`  | FluentValidation 驗證器測試         |

### 可測試性抽象化（2 個）

| Skill                                            | 說明                                |
| ------------------------------------------------ | ----------------------------------- |
| `dotnet-testing-datetime-testing-timeprovider`   | TimeProvider 時間抽象化             |
| `dotnet-testing-filesystem-testing-abstractions` | System.IO.Abstractions 檔案系統測試 |

### 測試資料生成（7 個）

| Skill                                                | 說明                               |
| ---------------------------------------------------- | ---------------------------------- |
| `dotnet-testing-test-data-builder-pattern`           | 手動 Builder Pattern               |
| `dotnet-testing-autofixture-basics`                  | AutoFixture 基礎與匿名測試資料     |
| `dotnet-testing-autofixture-customization`           | AutoFixture 自訂化策略             |
| `dotnet-testing-autodata-xunit-integration`          | AutoData 與 xUnit Theory 整合      |
| `dotnet-testing-autofixture-nsubstitute-integration` | AutoFixture + NSubstitute 自動模擬 |
| `dotnet-testing-bogus-fake-data`                     | Bogus 擬真資料產生                 |
| `dotnet-testing-autofixture-bogus-integration`       | AutoFixture 與 Bogus 整合          |

### 整合測試（5 個）

| Skill                                                | 說明                           |
| ---------------------------------------------------- | ------------------------------ |
| `dotnet-testing-advanced-aspnet-integration-testing` | WebApplicationFactory 整合測試 |
| `dotnet-testing-advanced-testcontainers-database`    | PostgreSQL/MSSQL 容器化測試    |
| `dotnet-testing-advanced-testcontainers-nosql`       | MongoDB/Redis 容器化測試       |
| `dotnet-testing-advanced-webapi-integration-testing` | WebAPI 完整整合測試流程        |
| `dotnet-testing-advanced-aspire-testing`             | .NET Aspire Testing 框架       |

### 框架遷移（3 個）

| Skill                                         | 說明                        |
| --------------------------------------------- | --------------------------- |
| `dotnet-testing-advanced-xunit-upgrade-guide` | xUnit 2.9.x to 3.x 升級指南 |
| `dotnet-testing-advanced-tunit-fundamentals`  | TUnit 新世代測試框架入門    |
| `dotnet-testing-advanced-tunit-advanced`      | TUnit 進階應用              |

### 第三方 Skill（1 個）

| Skill         | 說明                                                        |
| ------------- | ----------------------------------------------------------- |
| `dotnet-test` | .NET Test Runner — Build-First 測試執行、篩選、除錯工作流程 |

> `dotnet-test` 由 [NikiforovAll](https://github.com/NikiforovAll) 開發，來源為 [claude-code-rules/plugins/handbook-dotnet/skills/dotnet-test](https://github.com/NikiforovAll/claude-code-rules/tree/main/plugins/handbook-dotnet/skills/dotnet-test)。此 Skill 專注於「如何執行測試」，與 dotnet-testing-agent-skills 專注於「如何撰寫測試」形成互補。

---

## 詳細文件

Agent Skills 的完整說明（安裝方式、使用範例、技能清單、學習資源等）請參閱原始 GitHub Repository：

**https://github.com/kevintsengtw/dotnet-testing-agent-skills**

---

## 版本資訊

| 項目     | 說明                                                                                                    |
| -------- | ------------------------------------------------------------------------------------------------------- |
| **版本** | v2.2.0                                                                                                  |
| **來源** | [kevintsengtw/dotnet-testing-agent-skills](https://github.com/kevintsengtw/dotnet-testing-agent-skills) |
| **授權** | MIT License                                                                                             |
