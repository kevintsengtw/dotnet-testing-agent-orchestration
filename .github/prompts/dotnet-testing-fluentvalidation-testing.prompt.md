---
agent: 'agent'
description: '為 FluentValidation Validator 類別建立單元測試，涵蓋基本驗證、複雜邏輯、非同步驗證等場景。'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：`.github/skills/dotnet-testing-fluentvalidation-testing/SKILL.md`

此檔案包含：
- 所有的核心測試模式（基本欄位驗證、複雜驗證、非同步驗證、跨欄位驗證）
- FluentValidation.TestHelper 的 API 用法（ShouldHaveValidationErrorFor、ShouldNotHaveValidationErrorFor、TestValidate、TestValidateAsync）
- 必要的 NuGet 套件清單和 using 指令
- 完整的程式碼範例
- 測試最佳實踐和常見陷阱

**你的任務流程：**
1. **必須首先**讀取 SKILL.md 檔案的完整內容
2. 理解檔案中提供的所有測試模式和最佳實踐
3. 根據下方使用者輸入和 SKILL.md 中的範例，生成相應的測試程式碼
4. 確保生成的測試程式碼遵循 SKILL.md 中列出的所有模式和最佳實踐

---

## 使用者輸入

請提供以下資訊，我將根據 SKILL.md 的完整指南為您生成單元測試程式碼：

**Validator 類別名稱**：${input:validatorName:輸入 Validator 類別名稱，例如 SampleParameterValidator、UserValidator}

**要驗證的屬性及規則**：${input:validationRules:詳細描述要驗證的屬性名稱和驗證規則，例如 Name (必填、長度 1-100)、Email (格式驗證)}

**驗證器程式碼片段**（可選）：${input:validatorCode:貼上現有的 Validator 程式碼片段，或留空}

**特殊需求**（可選）：${input:specialRequirements:非同步驗證、時間相依、跨欄位邏輯、外部依賴等需求}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. 完整的測試類別，使用 xUnit 和 FluentValidation.TestHelper
2. 為每個驗證規則建立對應的測試方法（參考 SKILL.md 的模式）
3. 涵蓋通過案例和失敗案例（如 SKILL.md 範例所示）
4. 使用 ShouldHaveValidationErrorFor 和 ShouldNotHaveValidationErrorFor 進行驗證
5. 包含必要的 using 指令和套件引用（SKILL.md 中列出的）
6. 遵循 SKILL.md 提到的測試最佳實踐
