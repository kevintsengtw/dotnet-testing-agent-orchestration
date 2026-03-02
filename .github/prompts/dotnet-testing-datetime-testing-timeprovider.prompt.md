---
agent: 'agent'
description: '使用 TimeProvider 進行日期時間測試，涵蓋時間相依邏輯的可測試性設計。'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：`.github/skills/dotnet-testing-datetime-testing-timeprovider/SKILL.md`

此檔案包含：
- TimeProvider 抽象類別（.NET 8+）
- FakeTimeProvider 測試替身
- 從 DateTime.Now 遷移到 TimeProvider
- 時區與 UTC 處理
- 計時器與延遲測試
- 排程與週期性任務測試
- ISystemClock 相容性（舊版 .NET）
- 測試最佳實踐與陷阱

**你的任務流程：**
1. **必須首先**讀取 SKILL.md 檔案的完整內容
2. 理解檔案中提供的所有時間測試模式和最佳實踐
3. 根據下方使用者輸入和 SKILL.md 中的範例，生成相應的程式碼
4. 確保生成的程式碼遵循 SKILL.md 中列出的所有模式和最佳實踐

---

## 使用者輸入

請提供以下資訊，我將根據 SKILL.md 的完整指南為您生成時間測試程式碼：

**任務類型**：${input:taskType:選擇：「重構現有程式碼」、「撰寫新測試」、「測試排程邏輯」、「測試計時器」}

**要測試的時間相依邏輯**：${input:timeLogic:描述時間相依的邏輯，例如「訂單超過 30 天自動取消」、「每日報表產生」}

**目標 .NET 版本**：${input:dotnetVersion:例如 .NET 8、.NET 9、.NET 6}

**現有程式碼**（可選）：${input:existingCode:貼上現有使用 DateTime.Now 的程式碼}

**特殊需求**（可選）：${input:specialRequirements:例如時區處理、高精度計時等}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. TimeProvider 注入配置
2. FakeTimeProvider 測試設定
3. 時間控制的測試方法
4. 程式碼重構建議（如適用）
5. 時區處理策略
6. 計時器/延遲測試技巧（如需要）
7. 必要的 using 指令和套件引用
8. 遵循 SKILL.md 提到的時間測試最佳實踐
