---
agent: 'agent'
description: '測試 private 與 internal 成員，涵蓋存取限制成員的測試策略與技巧。'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：`.github/skills/dotnet-testing-private-internal-testing/SKILL.md`

此檔案包含：
- Internal 成員的測試策略（InternalsVisibleTo）
- Private 成員測試的權衡考量
- 反射存取 private 成員
- 設計重構以提高可測試性
- 測試替身與間接測試
- 何時應該測試 private 方法
- 封裝 vs 可測試性的平衡
- 最佳實踐與反模式

**你的任務流程：**
1. **必須首先**讀取 SKILL.md 檔案的完整內容
2. 理解檔案中提供的所有測試策略和最佳實踐
3. 根據下方使用者輸入和 SKILL.md 中的範例，提供相應的指導
4. 確保提供的方案遵循 SKILL.md 中列出的所有模式和最佳實踐

---

## 使用者輸入

請提供以下資訊，我將根據 SKILL.md 的完整指南為您提供測試策略：

**成員存取層級**：${input:accessLevel:選擇：「internal 類別」、「internal 方法」、「private 方法」、「protected 方法」}

**要測試的成員**：${input:targetMember:描述要測試的成員，例如「OrderService 的 private CalculateDiscount 方法」}

**測試需求**：${input:testingNeeds:描述為何需要測試這個成員，例如「複雜的商業邏輯」、「重要的輔助方法」}

**程式碼片段**（可選）：${input:codeSnippet:貼上相關的程式碼片段}

**限制條件**（可選）：${input:constraints:例如「無法修改原始程式碼」、「需要維持向後相容」}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請提供：

1. 推薦的測試策略分析
2. InternalsVisibleTo 配置（如適用）
3. 反射存取的實作（如需要）
4. 設計重構建議（如適用）
5. 間接測試策略說明
6. 完整的測試程式碼範例
7. 必要的 using 指令和套件引用
8. 遵循 SKILL.md 提到的最佳實踐
