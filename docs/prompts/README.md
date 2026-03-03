# Custom Prompts 清單與功能說明

本文件列出 `.github/prompts/` 目錄下所有 Custom Prompts 的功能說明，以及每個 Prompt 所使用的 Agent Skills。

---

## 總覽

共 **16** 個 Custom Prompts，涵蓋 .NET 單元測試到進階整合測試的完整技能。

### 基礎測試

#### 1. `dotnet-testing-fundamentals`

> 單元測試基礎完整指南（AAA 模式、命名規範、xUnit 專案設定）

| 項目         | 說明                                                                   |
| ------------ | ---------------------------------------------------------------------- |
| **功能**     | 引導建立 xUnit 測試專案、撰寫遵循 AAA 模式的單元測試、提供測試命名建議 |
| **互動參數** | 任務類型（setup/write/naming）、目標類別或方法名稱、測試情境描述       |

**使用的 Agent Skills：**

| #   | Skill                                    | 說明           |
| --- | ---------------------------------------- | -------------- |
| 1   | `dotnet-testing-unit-test-fundamentals`  | 單元測試基礎   |
| 2   | `dotnet-testing-test-naming-conventions` | 測試命名規範   |
| 3   | `dotnet-testing-xunit-project-setup`     | xUnit 專案設定 |

---

### 斷言與驗證

#### 2. `dotnet-testing-assertions`

> AwesomeAssertions 斷言與複雜物件比較完整指南

| 項目         | 說明                                                                        |
| ------------ | --------------------------------------------------------------------------- |
| **功能**     | 使用 AwesomeAssertions 進行基本斷言、集合斷言、例外斷言、物件深度比較       |
| **互動參數** | 斷言類型（basic/collection/exception/object）、驗證內容、預期結果、排除屬性 |

**使用的 Agent Skills：**

| #   | Skill                                      | 說明                       |
| --- | ------------------------------------------ | -------------------------- |
| 1   | `dotnet-testing-awesome-assertions-guide`  | AwesomeAssertions 斷言指南 |
| 2   | `dotnet-testing-complex-object-comparison` | 複雜物件比較               |

---

### Mock 與依賴隔離

#### 3. `dotnet-testing-nsubstitute-mocking`

> NSubstitute 模擬與 AutoFixture 整合，涵蓋 Mock、Stub、Spy 的完整使用指南

| 項目         | 說明                                                                               |
| ------------ | ---------------------------------------------------------------------------------- |
| **功能**     | 建立 NSubstitute Mock 物件、設定回傳值與行為、參數匹配、呼叫驗證、AutoFixture 整合 |
| **互動參數** | 要測試的類別、需要 Mock 的依賴、Mock 行為設定、驗證需求、特殊需求                  |

**使用的 Agent Skills：**

| #   | Skill                                                | 說明                           |
| --- | ---------------------------------------------------- | ------------------------------ |
| 1   | `dotnet-testing-nsubstitute-mocking`                 | NSubstitute 核心用法           |
| 2   | `dotnet-testing-autofixture-nsubstitute-integration` | AutoFixture + NSubstitute 整合 |
| 3   | `dotnet-testing-autodata-xunit-integration`          | AutoData 與 xUnit 整合         |

---

### 測試資料產生

#### 4. `dotnet-testing-autofixture-bogus`

> AutoFixture 與 Bogus 測試資料產生完整指南（基礎、自訂、整合、AutoData）

| 項目         | 說明                                                                                             |
| ------------ | ------------------------------------------------------------------------------------------------ |
| **功能**     | 使用 AutoFixture 產生匿名測試資料、Bogus 產生具業務意義的假資料、兩者整合策略、AutoData 驅動測試 |
| **互動參數** | 目標類別名稱、資料產生策略（autofixture/bogus/integration/autodata）、自訂屬性、測試框架         |

**使用的 Agent Skills：**

| #   | Skill                                          | 說明                     |
| --- | ---------------------------------------------- | ------------------------ |
| 1   | `dotnet-testing-autofixture-basics`            | AutoFixture 基礎         |
| 2   | `dotnet-testing-autofixture-customization`     | AutoFixture 自訂化       |
| 3   | `dotnet-testing-autofixture-bogus-integration` | AutoFixture + Bogus 整合 |
| 4   | `dotnet-testing-bogus-fake-data`               | Bogus 假資料產生         |
| 5   | `dotnet-testing-autodata-xunit-integration`    | AutoData 與 xUnit 整合   |

---

#### 5. `dotnet-testing-test-data-builder`

> 測試資料建立器模式實現，涵蓋 Builder Pattern 在測試資料準備中的應用

| 項目         | 說明                                                                       |
| ------------ | -------------------------------------------------------------------------- |
| **功能**     | 實作 Test Data Builder Pattern、Fluent API 設計、與 AutoFixture/Bogus 整合 |
| **互動參數** | 目標資料模型、必要欄位與預設值、Builder 使用情境、整合需求、特殊需求       |

**使用的 Agent Skills：**

| #   | Skill                                      | 說明                      |
| --- | ------------------------------------------ | ------------------------- |
| 1   | `dotnet-testing-test-data-builder-pattern` | Test Data Builder Pattern |

---

### 特殊依賴測試

#### 6. `dotnet-testing-datetime-testing-timeprovider`

> 使用 TimeProvider 進行日期時間測試，涵蓋時間相依邏輯的可測試性設計

| 項目         | 說明                                                                                              |
| ------------ | ------------------------------------------------------------------------------------------------- |
| **功能**     | 使用 .NET 8+ TimeProvider 抽象、FakeTimeProvider 測試替身、從 DateTime.Now 遷移、時區與計時器測試 |
| **互動參數** | 任務類型（重構/撰寫/排程/計時器）、時間相依邏輯、.NET 版本、現有程式碼、特殊需求                  |

**使用的 Agent Skills：**

| #   | Skill                                          | 說明                      |
| --- | ---------------------------------------------- | ------------------------- |
| 1   | `dotnet-testing-datetime-testing-timeprovider` | TimeProvider 日期時間測試 |

---

#### 7. `dotnet-testing-filesystem-testing-abstractions`

> 檔案系統抽象與測試，涵蓋 System.IO.Abstractions 的使用與檔案操作測試

| 項目         | 說明                                                                                       |
| ------------ | ------------------------------------------------------------------------------------------ |
| **功能**     | 使用 System.IO.Abstractions 的 IFileSystem 注入、MockFileSystem 測試替身、檔案操作測試模式 |
| **互動參數** | 任務類型（重構/撰寫/設定 MockFileSystem）、檔案操作、現有程式碼、測試情境、特殊需求        |

**使用的 Agent Skills：**

| #   | Skill                                            | 說明             |
| --- | ------------------------------------------------ | ---------------- |
| 1   | `dotnet-testing-filesystem-testing-abstractions` | 檔案系統抽象測試 |

---

#### 8. `dotnet-testing-fluentvalidation-testing`

> 為 FluentValidation Validator 類別建立單元測試，涵蓋基本驗證、複雜邏輯、非同步驗證等場景

| 項目         | 說明                                                                                                              |
| ------------ | ----------------------------------------------------------------------------------------------------------------- |
| **功能**     | 使用 FluentValidation.TestHelper 撰寫驗證器測試、ShouldHaveValidationErrorFor/ShouldNotHaveValidationErrorFor API |
| **互動參數** | Validator 類別名稱、驗證屬性及規則、驗證器程式碼片段、特殊需求                                                    |

**使用的 Agent Skills：**

| #   | Skill                                     | 說明                  |
| --- | ----------------------------------------- | --------------------- |
| 1   | `dotnet-testing-fluentvalidation-testing` | FluentValidation 測試 |

---

#### 9. `dotnet-testing-private-internal-testing`

> 測試 private 與 internal 成員，涵蓋存取限制成員的測試策略與技巧

| 項目         | 說明                                                                                         |
| ------------ | -------------------------------------------------------------------------------------------- |
| **功能**     | InternalsVisibleTo 配置、反射存取 private 成員、設計重構提高可測試性、封裝 vs 可測試性的平衡 |
| **互動參數** | 成員存取層級、要測試的成員、測試需求、程式碼片段、限制條件                                   |

**使用的 Agent Skills：**

| #   | Skill                                     | 說明                      |
| --- | ----------------------------------------- | ------------------------- |
| 1   | `dotnet-testing-private-internal-testing` | Private/Internal 成員測試 |

---

### 測試工具

#### 10. `dotnet-testing-test-output-logging`

> 測試輸出與日誌記錄，涵蓋 ITestOutputHelper 與測試診斷資訊的最佳實踐

| 項目         | 說明                                                                            |
| ------------ | ------------------------------------------------------------------------------- |
| **功能**     | 使用 xUnit ITestOutputHelper 輸出測試資訊、整合 ILogger、測試診斷資訊設計       |
| **互動參數** | 測試類型（單元/整合/端對端）、輸出需求、Logger 整合需求、現有測試類別、特殊需求 |

**使用的 Agent Skills：**

| #   | Skill                                | 說明           |
| --- | ------------------------------------ | -------------- |
| 1   | `dotnet-testing-test-output-logging` | 測試輸出與日誌 |

---

#### 11. `dotnet-testing-code-coverage-analysis`

> 程式碼涵蓋率分析與改善策略，涵蓋覆蓋率工具、報告解讀與品質提升

| 項目         | 說明                                                                                 |
| ------------ | ------------------------------------------------------------------------------------ |
| **功能**     | 設定 Coverlet 覆蓋率收集、ReportGenerator 報告產生、CI/CD 整合、覆蓋率門檻與排除策略 |
| **互動參數** | 任務類型（設定/解讀/改善/CI-CD）、專案類型、目前覆蓋率狀況、目標覆蓋率、特殊需求     |

**使用的 Agent Skills：**

| #   | Skill                                   | 說明             |
| --- | --------------------------------------- | ---------------- |
| 1   | `dotnet-testing-code-coverage-analysis` | 程式碼覆蓋率分析 |

---

### 進階測試

#### 12. `dotnet-testing-advanced-integration`

> ASP.NET Core 與 Web API 整合測試完整指南（WebApplicationFactory、HttpClient 測試）

| 項目         | 說明                                                                                                              |
| ------------ | ----------------------------------------------------------------------------------------------------------------- |
| **功能**     | 使用 WebApplicationFactory 建立整合測試、自訂 Factory 設定、HttpClient 測試、服務替換與 Mock 注入、資料庫測試策略 |
| **互動參數** | 應用程式類型（webapi/mvc/minimal）、測試端點、Mock 服務、資料庫策略                                               |

**使用的 Agent Skills：**

| #   | Skill                                                | 說明             |
| --- | ---------------------------------------------------- | ---------------- |
| 1   | `dotnet-testing-advanced-aspnet-integration-testing` | ASP.NET 整合測試 |
| 2   | `dotnet-testing-advanced-webapi-integration-testing` | Web API 整合測試 |

---

#### 13. `dotnet-testing-advanced-testcontainers`

> Testcontainers 資料庫測試，涵蓋 SQL 與 NoSQL 資料庫的容器化整合測試

| 項目         | 說明                                                                                                           |
| ------------ | -------------------------------------------------------------------------------------------------------------- |
| **功能**     | 使用 Testcontainers.NET 建立容器化資料庫測試、SQL Server/PostgreSQL/MySQL/MongoDB/Redis 容器配置、EF Core 整合 |
| **互動參數** | 資料庫類型、測試功能、ORM/資料存取技術、測試情境、特殊需求                                                     |

**使用的 Agent Skills：**

| #   | Skill                                             | 說明                 |
| --- | ------------------------------------------------- | -------------------- |
| 1   | `dotnet-testing-advanced-testcontainers-database` | SQL 資料庫容器測試   |
| 2   | `dotnet-testing-advanced-testcontainers-nosql`    | NoSQL 資料庫容器測試 |

---

#### 14. `dotnet-testing-advanced-aspire-testing`

> .NET Aspire 應用測試指南，涵蓋分散式應用的整合測試與端對端測試

| 項目         | 說明                                                                                                       |
| ------------ | ---------------------------------------------------------------------------------------------------------- |
| **功能**     | 使用 DistributedApplicationTestingBuilder 建立 Aspire 整合測試、服務間通訊測試、資源依賴配置、健康檢查驗證 |
| **互動參數** | Aspire 專案結構、測試服務、依賴資源、測試情境、特殊需求                                                    |

**使用的 Agent Skills：**

| #   | Skill                                    | 說明             |
| --- | ---------------------------------------- | ---------------- |
| 1   | `dotnet-testing-advanced-aspire-testing` | .NET Aspire 測試 |

---

#### 15. `dotnet-testing-advanced-tunit`

> TUnit 完整教學，涵蓋 Source Generator 驅動的新世代測試框架基礎到進階用法

| 項目         | 說明                                                                                       |
| ------------ | ------------------------------------------------------------------------------------------ |
| **功能**     | TUnit 測試框架的核心概念、Source Generator 機制、參數化測試、平行執行、從 xUnit/NUnit 遷移 |
| **互動參數** | 任務類型（新專案/撰寫/遷移）、測試類別、測試情境、現有測試程式碼、特殊需求                 |

**使用的 Agent Skills：**

| #   | Skill                                        | 說明           |
| --- | -------------------------------------------- | -------------- |
| 1   | `dotnet-testing-advanced-tunit-fundamentals` | TUnit 基礎     |
| 2   | `dotnet-testing-advanced-tunit-advanced`     | TUnit 進階用法 |

---

#### 16. `dotnet-testing-advanced-xunit-upgrade`

> xUnit v2 到 v3 升級指南與遷移策略

| 項目         | 說明                                                                    |
| ------------ | ----------------------------------------------------------------------- |
| **功能**     | xUnit v2 到 v3 的重大變更分析、API 變更對照、遷移步驟與策略、新功能介紹 |
| **互動參數** | 目前 xUnit 版本、目標版本、專案規模、使用的特殊功能                     |

**使用的 Agent Skills：**

| #   | Skill                                         | 說明           |
| --- | --------------------------------------------- | -------------- |
| 1   | `dotnet-testing-advanced-xunit-upgrade-guide` | xUnit 升級指南 |

---

## Prompt 與 Skill 對應總表

下表彙整所有 Custom Prompts 與其使用的 Agent Skills 對應關係：

| #   | Prompt                                           | Skills 數量 | Agent Skills                                                                                                                        |
| --- | ------------------------------------------------ | ----------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| 1   | `dotnet-testing-fundamentals`                    | 3           | `unit-test-fundamentals`, `test-naming-conventions`, `xunit-project-setup`                                                          |
| 2   | `dotnet-testing-assertions`                      | 2           | `awesome-assertions-guide`, `complex-object-comparison`                                                                             |
| 3   | `dotnet-testing-nsubstitute-mocking`             | 3           | `nsubstitute-mocking`, `autofixture-nsubstitute-integration`, `autodata-xunit-integration`                                          |
| 4   | `dotnet-testing-autofixture-bogus`               | 5           | `autofixture-basics`, `autofixture-customization`, `autofixture-bogus-integration`, `bogus-fake-data`, `autodata-xunit-integration` |
| 5   | `dotnet-testing-test-data-builder`               | 1           | `test-data-builder-pattern`                                                                                                         |
| 6   | `dotnet-testing-datetime-testing-timeprovider`   | 1           | `datetime-testing-timeprovider`                                                                                                     |
| 7   | `dotnet-testing-filesystem-testing-abstractions` | 1           | `filesystem-testing-abstractions`                                                                                                   |
| 8   | `dotnet-testing-fluentvalidation-testing`        | 1           | `fluentvalidation-testing`                                                                                                          |
| 9   | `dotnet-testing-private-internal-testing`        | 1           | `private-internal-testing`                                                                                                          |
| 10  | `dotnet-testing-test-output-logging`             | 1           | `test-output-logging`                                                                                                               |
| 11  | `dotnet-testing-code-coverage-analysis`          | 1           | `code-coverage-analysis`                                                                                                            |
| 12  | `dotnet-testing-advanced-integration`            | 2           | `advanced-aspnet-integration-testing`, `advanced-webapi-integration-testing`                                                        |
| 13  | `dotnet-testing-advanced-testcontainers`         | 2           | `advanced-testcontainers-database`, `advanced-testcontainers-nosql`                                                                 |
| 14  | `dotnet-testing-advanced-aspire-testing`         | 1           | `advanced-aspire-testing`                                                                                                           |
| 15  | `dotnet-testing-advanced-tunit`                  | 2           | `advanced-tunit-fundamentals`, `advanced-tunit-advanced`                                                                            |
| 16  | `dotnet-testing-advanced-xunit-upgrade`          | 1           | `advanced-xunit-upgrade-guide`                                                                                                      |

> **備註**：上表 Agent Skills 欄位省略了共同前綴 `dotnet-testing-`，完整 Skill 名稱請加上此前綴。
