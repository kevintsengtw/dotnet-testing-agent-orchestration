---
agent: 'agent'
description: '檔案系統抽象與測試，涵蓋 System.IO.Abstractions 的使用與檔案操作測試。'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：`.github/skills/dotnet-testing-filesystem-testing-abstractions/SKILL.md`

此檔案包含：
- System.IO.Abstractions 框架概述
- IFileSystem 介面與依賴注入
- MockFileSystem 測試替身
- 常見檔案操作的測試模式
- 目錄操作測試
- 檔案監視（FileSystemWatcher）測試
- 從 System.IO 遷移的策略
- 整合測試 vs 單元測試的考量

**你的任務流程：**
1. **必須首先**讀取 SKILL.md 檔案的完整內容
2. 理解檔案中提供的所有檔案系統測試模式和最佳實踐
3. 根據下方使用者輸入和 SKILL.md 中的範例，生成相應的程式碼
4. 確保生成的程式碼遵循 SKILL.md 中列出的所有模式和最佳實踐

---

## 使用者輸入

請提供以下資訊，我將根據 SKILL.md 的完整指南為您生成檔案系統測試程式碼：

**任務類型**：${input:taskType:選擇：「重構現有程式碼」、「撰寫新測試」、「設定 MockFileSystem」}

**要測試的檔案操作**：${input:fileOperations:描述檔案操作，例如「讀取設定檔」、「寫入日誌」、「監視目錄變化」}

**現有程式碼**（可選）：${input:existingCode:貼上現有使用 System.IO 的程式碼}

**測試情境**：${input:testScenarios:描述測試情境，例如「檔案存在」、「檔案不存在」、「權限不足」}

**特殊需求**（可選）：${input:specialRequirements:例如大檔案處理、串流操作等}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. IFileSystem 注入配置
2. MockFileSystem 測試設定
3. 完整的測試類別與方法
4. 程式碼重構建議（如適用）
5. 各種檔案狀態的測試案例
6. 例外處理測試
7. 必要的 using 指令和套件引用
8. 遵循 SKILL.md 提到的檔案系統測試最佳實踐
