---
agent: 'agent'
description: 'xUnit v2 到 v3 升級指南與遷移策略'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：

`.github/skills/dotnet-testing-advanced-xunit-upgrade-guide/SKILL.md` - xUnit 升級指南

此檔案包含：

- xUnit v2 到 v3 的重大變更
- API 變更與棄用項目
- 遷移步驟與策略
- 常見問題與解決方案
- 新功能介紹

**你的任務流程：**

1. **必須首先**讀取 SKILL.md 檔案的完整內容
2. 理解 xUnit 版本間的差異
3. 根據使用者的現有程式碼提供遷移建議
4. 確保遷移後的程式碼符合 xUnit v3 最佳實踐

---

## 使用者輸入

請提供以下資訊：

**目前 xUnit 版本**：${input:currentVersion:目前使用的 xUnit 版本，例如 2.4.2、2.5.0}

**目標 xUnit 版本**：${input:targetVersion:要升級到的 xUnit 版本，例如 3.0.0}

**專案規模**（可選）：${input:projectScale:專案中的測試數量規模，例如 small / medium / large}

**使用的特殊功能**（可選）：${input:specialFeatures:目前使用的特殊功能，例如 CollectionFixture、TheoryData、自訂 Attributes}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有內容，請生成：

1. **版本差異分析**
   - 重大變更清單
   - 影響範圍評估
   - 棄用 API 對照表

2. **遷移步驟**
   - NuGet 套件更新指令
   - csproj 設定調整
   - 程式碼修改範例

3. **程式碼調整建議**
   - 棄用 API 的替代方案
   - 新 API 使用範例
   - Before/After 對照

4. **驗證檢查清單**
   - 遷移後的測試驗證步驟
   - 常見問題排查
   - 回滾策略
