---
agent: 'agent'
description: 'TUnit 完整教學，涵蓋 Source Generator 驅動的新世代測試框架基礎到進階用法。'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：
- `.github/skills/dotnet-testing-advanced-tunit-fundamentals/SKILL.md`
- `.github/skills/dotnet-testing-advanced-tunit-advanced/SKILL.md`

這些檔案包含：
- TUnit 框架的核心概念與優勢
- Source Generator 驅動的測試發現機制
- 測試生命週期與 Hook 系統
- 參數化測試與資料驅動測試
- 平行執行與效能優化
- 進階功能：依賴注入、自訂 Attribute、擴展點
- 從 xUnit/NUnit 遷移指南

**你的任務流程：**
1. **必須首先**讀取所有 SKILL.md 檔案的完整內容
2. 理解檔案中提供的所有測試模式和最佳實踐
3. 根據下方使用者輸入和 SKILL.md 中的範例，生成相應的測試程式碼
4. 確保生成的測試程式碼遵循 SKILL.md 中列出的所有模式和最佳實踐

---

## 使用者輸入

請提供以下資訊，我將根據 SKILL.md 的完整指南為您生成 TUnit 測試程式碼：

**任務類型**：${input:taskType:選擇：「新測試專案」、「撰寫測試」、「從 xUnit 遷移」、「從 NUnit 遷移」}

**要測試的類別或功能**：${input:targetClass:輸入要測試的類別或功能名稱}

**測試情境描述**：${input:testScenario:描述測試情境，例如「需要參數化測試」、「需要依賴注入」、「需要平行執行」}

**現有測試程式碼**（遷移時提供）：${input:existingTests:貼上現有的 xUnit/NUnit 測試程式碼}

**特殊需求**（可選）：${input:specialRequirements:例如自訂生命週期、特定的 Hook 需求等}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. 完整的 TUnit 測試類別
2. 使用適當的 TUnit Attribute（[Test]、[Arguments]、[MethodDataSource] 等）
3. 正確的生命週期 Hook 配置（如需要）
4. 平行執行配置（如適用）
5. 必要的 using 指令和套件引用
6. 專案配置說明（.csproj）
7. 遵循 SKILL.md 提到的 TUnit 最佳實踐
