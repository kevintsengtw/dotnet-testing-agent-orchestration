---
agent: 'agent'
description: '測試輸出與日誌記錄，涵蓋 ITestOutputHelper 與測試診斷資訊的最佳實踐。'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：`.github/skills/dotnet-testing-test-output-logging/SKILL.md`

此檔案包含：
- xUnit ITestOutputHelper 的使用方式
- 測試輸出的時機與內容
- 與 ILogger 的整合
- 測試診斷資訊的設計
- 除錯與問題排查的輸出策略
- 測試報告與 CI/CD 整合
- 效能考量與輸出限制
- 結構化日誌與測試輸出

**你的任務流程：**
1. **必須首先**讀取 SKILL.md 檔案的完整內容
2. 理解檔案中提供的所有輸出模式和最佳實踐
3. 根據下方使用者輸入和 SKILL.md 中的範例，生成相應的程式碼
4. 確保生成的程式碼遵循 SKILL.md 中列出的所有模式和最佳實踐

---

## 使用者輸入

請提供以下資訊，我將根據 SKILL.md 的完整指南為您生成測試輸出程式碼：

**測試類型**：${input:testType:選擇：「單元測試」、「整合測試」、「端對端測試」}

**輸出需求**：${input:outputNeeds:描述需要輸出的資訊，例如「API 請求/回應」、「執行時間」、「狀態變化」}

**Logger 整合需求**：${input:loggerIntegration:是否需要整合 ILogger，例如「使用 Serilog」、「使用 Microsoft.Extensions.Logging」}

**現有測試類別**（可選）：${input:existingTestClass:貼上現有的測試類別程式碼}

**特殊需求**（可選）：${input:specialRequirements:例如結構化輸出、自訂格式等}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. ITestOutputHelper 注入與使用
2. 適當的輸出方法封裝
3. ILogger 整合配置（如需要）
4. 診斷資訊輸出範例
5. 測試基底類別設計（如適用）
6. 必要的 using 指令和套件引用
7. 遵循 SKILL.md 提到的測試輸出最佳實踐
