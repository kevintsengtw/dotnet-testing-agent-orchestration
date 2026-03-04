# 使用驗證專案操作 Agent Orchestrator

本文件說明如何使用 `samples/` 目錄中的驗證專案，搭配各 Agent Orchestrator 進行實際操作。

各 Orchestrator 的詳細操作步驟與驗證情境，請參閱對應的獨立文件。

---

## 前置準備

### 環境需求

| 項目                | 說明                                                   |
| ------------------- | ------------------------------------------------------ |
| **VS Code**         | 1.109 以上，已安裝 GitHub Copilot Chat                 |
| **VS Code 設定**    | `chat.customAgentInSubagent.enabled: true`             |
| **.NET SDK**        | 依據目標版本安裝 .NET 8 / 9 / 10 SDK                   |
| **Docker Desktop**  | 整合測試與 Aspire 測試需要（單元測試與 TUnit 不需要）  |
| **Aspire Workload** | 僅 Aspire 測試需要（`dotnet workload install aspire`） |

### 確認 Agent 可用

1. 開啟 Copilot Chat 面板（`Ctrl+Alt+I`）
2. 在聊天輸入框上方，將模式切換為 **Agent**（預設可能是 Ask 或 Edit）
3. 點擊 Agent 名稱旁的下拉選單，確認可以看到以下四個 Orchestrator：
   - `dotnet-testing-orchestrator`
   - `dotnet-testing-advanced-integration-orchestrator`
   - `dotnet-testing-advanced-aspire-orchestrator`
   - `dotnet-testing-advanced-tunit-orchestrator`

> 如果看不到，請參考 [啟用設定](README.md#啟用設定) 進行配置。

---

## 驗證專案與 Orchestrator 對應

> **主要驗證版本為 .NET 9.0**（基線版本）。每個驗證專案另提供 .NET 8.0 與 .NET 10.0 版本，供跨版本驗證使用，詳見各文件末尾的「跨版本驗證」章節。

| 驗證專案                        | Orchestrator                                       | 測試類型        | 操作指南                                                              |
| ------------------------------- | -------------------------------------------------- | --------------- | --------------------------------------------------------------------- |
| `samples/practice/`             | `dotnet-testing-orchestrator`                      | 單元測試        | [practice-guide-unit-testing.md](practice-guide-unit-testing.md)             |
| `samples/practice_integration/` | `dotnet-testing-advanced-integration-orchestrator` | 整合測試        | [practice-guide-integration-testing.md](practice-guide-integration-testing.md) |
| `samples/practice_aspire/`      | `dotnet-testing-advanced-aspire-orchestrator`      | Aspire 整合測試 | [practice-guide-aspire-testing.md](practice-guide-aspire-testing.md)         |
| `samples/practice_tunit/`       | `dotnet-testing-advanced-tunit-orchestrator`       | TUnit 測試      | [practice-guide-tunit-testing.md](practice-guide-tunit-testing.md)           |

---

## 還原驗證結果

Orchestrator 會在 `tests/` 目錄下產生測試程式碼。驗證完成後，可使用以下方式還原：

```powershell
# 還原單一驗證專案的測試結果
git restore samples/practice/tests/
git restore samples/practice_integration/tests/
git restore samples/practice_aspire/tests/
git restore samples/practice_tunit/tests/

# 還原所有驗證專案
git restore samples/

# 清理 Orchestrator 新增的未追蹤檔案
git clean -fd samples/
```
