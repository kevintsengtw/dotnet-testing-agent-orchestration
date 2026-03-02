---
agent: 'agent'
description: 'Testcontainers 資料庫測試，涵蓋 SQL 與 NoSQL 資料庫的容器化整合測試。'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：
- `.github/skills/dotnet-testing-advanced-testcontainers-database/SKILL.md`
- `.github/skills/dotnet-testing-advanced-testcontainers-nosql/SKILL.md`

這些檔案包含：
- Testcontainers.NET 框架概述
- SQL Server、PostgreSQL、MySQL 容器配置
- MongoDB、Redis、Elasticsearch 容器配置
- Entity Framework Core 整合測試
- 資料庫初始化與種子資料
- 容器生命週期管理
- 測試隔離與資料清理策略
- 效能優化與容器重用

**你的任務流程：**
1. **必須首先**讀取所有 SKILL.md 檔案的完整內容
2. 理解檔案中提供的所有測試模式和最佳實踐
3. 根據下方使用者輸入和 SKILL.md 中的範例，生成相應的測試程式碼
4. 確保生成的測試程式碼遵循 SKILL.md 中列出的所有模式和最佳實踐

---

## 使用者輸入

請提供以下資訊，我將根據 SKILL.md 的完整指南為您生成 Testcontainers 測試程式碼：

**資料庫類型**：${input:databaseType:選擇：SQL Server、PostgreSQL、MySQL、MongoDB、Redis、Elasticsearch、其他}

**要測試的功能**：${input:targetFeature:例如 Repository、DbContext、資料存取層、快取服務}

**使用的 ORM/資料存取技術**：${input:dataAccessTech:例如 Entity Framework Core、Dapper、MongoDB Driver}

**測試情境描述**：${input:testScenario:描述測試情境，例如「CRUD 操作」、「複雜查詢」、「交易處理」}

**特殊需求**（可選）：${input:specialRequirements:例如特定的資料庫版本、自訂初始化腳本等}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. Testcontainers 容器配置類別
2. 測試 Fixture 與 Collection 配置
3. 完整的資料庫整合測試類別
4. 資料庫初始化與種子資料設定
5. 測試資料清理邏輯
6. 容器生命週期管理
7. 必要的 using 指令和套件引用
8. 專案配置說明（.csproj）
9. 遵循 SKILL.md 提到的 Testcontainers 最佳實踐
