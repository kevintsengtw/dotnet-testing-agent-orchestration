---
agent: 'agent'
description: '測試資料建立器模式實現，涵蓋 Builder Pattern 在測試資料準備中的應用。'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：`.github/skills/dotnet-testing-test-data-builder-pattern/SKILL.md`

此檔案包含：
- Test Data Builder Pattern 核心概念
- Fluent Builder API 設計
- 預設值與客製化策略
- 與 AutoFixture/Bogus 的整合
- Builder 繼承與組合
- 複雜物件圖的建立
- 測試可讀性與維護性考量
- 常見反模式與解決方案

**你的任務流程：**
1. **必須首先**讀取 SKILL.md 檔案的完整內容
2. 理解檔案中提供的所有 Builder 模式和最佳實踐
3. 根據下方使用者輸入和 SKILL.md 中的範例，生成相應的程式碼
4. 確保生成的程式碼遵循 SKILL.md 中列出的所有模式和最佳實踐

---

## 使用者輸入

請提供以下資訊，我將根據 SKILL.md 的完整指南為您生成 Test Data Builder 程式碼：

**目標資料模型**：${input:dataModel:貼上或描述要建立 Builder 的類別，例如 Order、Customer、Product}

**必要欄位與預設值**：${input:fieldsAndDefaults:列出欄位名稱與建議的預設值}

**Builder 使用情境**：${input:usageScenario:描述 Builder 將如何使用，例如「單元測試」、「整合測試」、「特定商業情境」}

**整合需求**：${input:integrationNeeds:是否需要整合 AutoFixture 或 Bogus}

**特殊需求**（可選）：${input:specialRequirements:例如巢狀 Builder、Builder 繼承等}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. 完整的 Builder 類別實作
2. Fluent API 方法（WithXxx 方法）
3. Build() 方法實作
4. 預設值配置
5. 使用 Builder 的測試範例
6. 與 AutoFixture/Bogus 的整合（如需要）
7. 必要的 using 指令和套件引用
8. 遵循 SKILL.md 提到的 Builder Pattern 最佳實踐
