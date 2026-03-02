---
agent: 'agent'
description: 'AutoFixture 與 Bogus 測試資料產生完整指南（基礎、自訂、整合、AutoData）'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：

1. `.github/skills/dotnet-testing-autofixture-basics/SKILL.md` - AutoFixture 基礎
2. `.github/skills/dotnet-testing-autofixture-customization/SKILL.md` - AutoFixture 自訂化
3. `.github/skills/dotnet-testing-autofixture-bogus-integration/SKILL.md` - AutoFixture + Bogus 整合
4. `.github/skills/dotnet-testing-bogus-fake-data/SKILL.md` - Bogus 假資料產生
5. `.github/skills/dotnet-testing-autodata-xunit-integration/SKILL.md` - AutoData 與 xUnit 整合

這些檔案包含：

- AutoFixture 的 Fixture 類別與自動產生測試物件
- 使用 Build<T>() 自訂物件屬性
- 自訂 ISpecimenBuilder 與 ICustomization
- Bogus Faker<T> 產生真實假資料（姓名、地址、Email）
- Bogus RuleSet 與條件式資料產生
- AutoFixture 與 Bogus 整合策略
- AutoData 與 InlineAutoData 屬性用法
- xUnit Theory 資料驅動測試模式

**你的任務流程：**

1. **必須首先**讀取所有 SKILL.md 檔案的完整內容
2. 理解 AutoFixture 與 Bogus 的使用場景差異
3. 根據使用者需求選擇適當的資料產生策略
4. 確保生成的程式碼遵循最佳實踐

---

## 使用者輸入

請提供以下資訊：

**目標類別名稱**：${input:className:要產生測試資料的類別名稱，例如 Customer, Order, Product}

**資料產生策略**：${input:strategy:選擇策略 - autofixture（匿名資料）/ bogus（真實假資料）/ integration（整合使用）/ autodata（xUnit 整合）}

**需要自訂的屬性**（可選）：${input:customProperties:需要特殊處理的屬性，例如 Email 格式、日期範圍、字串長度限制}

**測試框架**（可選）：${input:testFramework:使用的測試框架，預設為 xUnit}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. **基於策略的測試資料產生程式碼**
   - AutoFixture：匿名測試資料產生
   - Bogus：具業務意義的假資料
   - Integration：結合兩者優點
   - AutoData：xUnit Theory 資料驅動測試

2. **自訂化實作**（如需要）
   - ISpecimenBuilder 實作
   - ICustomization 實作
   - Bogus RuleSet 定義

3. **完整的測試範例**
   - 使用 [Theory] 與 [AutoData] 或 [InlineAutoData]
   - 展示資料產生與斷言

4. **最佳實踐說明**
   - 何時使用 AutoFixture vs Bogus
   - 效能考量
   - 可維護性建議
