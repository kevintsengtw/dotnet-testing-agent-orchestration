---
agent: 'agent'
description: '.NET Aspire 應用測試指南，涵蓋分散式應用的整合測試與端對端測試。'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：`.github/skills/dotnet-testing-advanced-aspire-testing/SKILL.md`

此檔案包含：
- .NET Aspire 測試架構概述
- DistributedApplicationTestingBuilder 的使用方式
- 服務間通訊的測試策略
- 資源依賴（Redis、SQL Server、RabbitMQ 等）的測試配置
- 健康檢查與就緒檢查的驗證
- 環境變數與配置的測試
- 容器化服務的整合測試
- 效能與負載測試考量

**你的任務流程：**
1. **必須首先**讀取 SKILL.md 檔案的完整內容
2. 理解檔案中提供的所有測試模式和最佳實踐
3. 根據下方使用者輸入和 SKILL.md 中的範例，生成相應的測試程式碼
4. 確保生成的測試程式碼遵循 SKILL.md 中列出的所有模式和最佳實踐

---

## 使用者輸入

請提供以下資訊，我將根據 SKILL.md 的完整指南為您生成 Aspire 測試程式碼：

**Aspire 專案結構**：${input:projectStructure:描述您的 Aspire 專案結構，包含哪些服務和資源}

**要測試的服務**：${input:targetService:輸入要測試的服務名稱，例如 ApiService、WorkerService}

**依賴的資源**：${input:dependencies:列出服務依賴的資源，例如 Redis、SQL Server、RabbitMQ}

**測試情境**：${input:testScenario:描述要測試的情境，例如「API 端點」、「服務間通訊」、「健康檢查」}

**特殊需求**（可選）：${input:specialRequirements:例如自訂環境變數、特定的容器配置等}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. 完整的 Aspire 整合測試類別
2. DistributedApplicationTestingBuilder 的正確配置
3. 資源依賴的測試設定
4. 服務健康檢查的驗證邏輯
5. 適當的測試清理與資源釋放
6. 必要的 using 指令和套件引用
7. 測試專案的 .csproj 配置
8. 遵循 SKILL.md 提到的 Aspire 測試最佳實踐
