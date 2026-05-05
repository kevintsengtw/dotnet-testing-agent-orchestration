# Claude Code vs GitHub Copilot：Agent Skill 載入機制差異分析

> **建立日期**：2026-05-01
> **分析範圍**：純就兩個平台對 Agent Skill 的讀取與使用機制做比較，不涉及特定 Orchestrator 設計

---

## 目錄

- [Claude Code vs GitHub Copilot：Agent Skill 載入機制差異分析](#claude-code-vs-github-copilotagent-skill-載入機制差異分析)
  - [目錄](#目錄)
  - [1. 結論摘要](#1-結論摘要)
  - [2. 載入機制的根本差異：直接注入 vs 工具中介](#2-載入機制的根本差異直接注入-vs-工具中介)
  - [3. Tool Call 累積成本](#3-tool-call-累積成本)
  - [4. Subagent 的 Context 傳遞差異](#4-subagent-的-context-傳遞差異)
    - [Claude Code](#claude-code)
    - [GitHub Copilot](#github-copilot)
  - [5. 推理開銷：決策本身就耗時](#5-推理開銷決策本身就耗時)
  - [6. Token 預算的結構性差異](#6-token-預算的結構性差異)
  - [7. 總結比較表](#7-總結比較表)
    - [關鍵結論](#關鍵結論)

---

## 1. 結論摘要

Claude Code 讀取 Skill 快、GitHub Copilot 讀取 Skill 慢，根本原因是**載入管道的架構層級差異**：

- **Claude Code**：本地 runtime + 直接 context 注入，Skill 內容不經過 tool call
- **GitHub Copilot**：雲端模型 + 工具中介抽象層，每個 Skill 檔案都需要獨立的 tool call round-trip

這不是 Skill 內容品質或寫法的問題，而是平台架構決定的結構性差異。

---

## 2. 載入機制的根本差異：直接注入 vs 工具中介

| 面向     | Claude Code                                              | GitHub Copilot                                                        |
| -------- | -------------------------------------------------------- | --------------------------------------------------------------------- |
| 載入方式 | Skill 內容**直接注入** prompt context                    | 必須透過 `read_file` **工具呼叫**才能讀取                             |
| 觸發機制 | `/skill-name` → runtime 立即將 SKILL.md 注入對話 context | 系統提示僅含 metadata（名稱、描述、路徑），內容需額外 tool call       |
| I/O 路徑 | 本地 process → 本地檔案系統（毫秒級）                    | Model → Extension Host → 檔案讀取 → 回傳 Model（多次網路 round-trip） |

Claude Code 是**本地 CLI process**，讀檔就是一次 `fs.readFile()`，幾乎零延遲。

GitHub Copilot 的每次 `read_file` 都是一個完整的 **tool call round-trip**：

```text
模型產生工具呼叫 → 傳送到 VS Code Extension Host → 讀檔 → 回傳結果 → 模型處理回應
```

---

## 3. Tool Call 累積成本

一個複雜 Skill（如 `dotnet-testing-advanced-aspnet-integration-testing`）可能包含：

- `SKILL.md` 主檔
- `templates/*.md` 範本
- `references/*.md` 參考資料

**Claude Code**：這些檔案可以一次性注入。

**GitHub Copilot**：**每個檔案都是一次獨立的 tool call**，3–5 個檔案就是 3–5 次 round-trip。

每次 round-trip 的完整流程：

1. 模型推理「我需要讀這個檔案」→ 產生 tool call JSON
2. 網路傳輸到 Extension Host
3. Extension Host 執行讀取
4. 結果回傳（含 tool result 包裝）
5. 模型消化結果後才能繼續下一步

---

## 4. Subagent 的 Context 傳遞差異

這是**差距最大**的地方。

### Claude Code

- 父 agent 用 `Task` tool 啟動 subagent 時，可在 prompt 中**直接傳入已讀取的 Skill 內容摘要或關鍵指令**
- Subagent 也有完整的本地檔案系統存取權，讀 Skill 同樣是毫秒級

### GitHub Copilot

- `runSubagent` 是 **stateless** 的 — 每個 subagent 從零開始，**不繼承父 agent 的已讀 context**
- 如果 subagent 也需要 Skill，它必須**重新走一遍完整的 tool call 流程**

實際影響（以 Orchestrator + 4 Subagents 為例）：

```text
Claude Code：
  Orchestrator 讀 Skills → 直接注入（1 次）
  Subagent 讀 Skills → 本地讀檔（毫秒級 × 4）

GitHub Copilot：
  Orchestrator 讀 Skills → N 次 tool call
  Analyzer 重新讀 Skills → N 次 tool call
  Writer 重新讀 Skills → N 次 tool call
  Executor 重新讀 Skills → N 次 tool call
  Reviewer 重新讀 Skills → N 次 tool call
  總計：重複讀取 5 次，每次都是完整 round-trip
```

---

## 5. 推理開銷：決策本身就耗時

GitHub Copilot 的系統提示包含這樣的指令：

> *BLOCKING REQUIREMENT: When a skill applies to the user's request, you MUST load and read the SKILL.md file IMMEDIATELY as your first action, BEFORE generating any other response or taking action on the task.*

但模型在執行 tool call **之前**，必須先完成一輪推理：

1. 比對 keyword 判斷哪些 Skills 適用
2. 決定讀取順序
3. 產生 `read_file` 的參數（包含完整路徑）

這個「**決策推理 → 工具呼叫 → 等待結果 → 再決策**」的循環，在 Claude Code 中被直接注入機制跳過了。

Claude Code 的 `/skill-name` 觸發是 **deterministic** 的（由 runtime 判斷路徑並注入），不需要模型花推理 token 去決定「要不要讀」和「怎麼讀」。

---

## 6. Token 預算的結構性差異

| 項目           | Claude Code                  | GitHub Copilot                                                                                      |
| -------------- | ---------------------------- | --------------------------------------------------------------------------------------------------- |
| Skill metadata | 不佔 context（runtime 管理） | 每個 Skill 的 name + description + keywords 都在系統提示中，38 個 Skills 的 metadata 已佔顯著 token |
| Tool call 格式 | 無額外包裝                   | 每次 tool call 有 JSON function call 格式包裝，tool result 也有包裝                                 |
| 可用 context   | 更多空間給實際 Skill 內容    | 系統提示已很龐大（Skills metadata + agents list + 各種 instructions），壓縮了有效工作 context       |

當 Skills 數量增加時，GitHub Copilot 的系統提示膨脹問題會更嚴重 — 即使還沒讀取任何 Skill 內容，光是 metadata 就已經消耗大量 token。

---

## 7. 總結比較表

| 比較維度          | Claude Code                      | GitHub Copilot                       |
| ----------------- | -------------------------------- | ------------------------------------ |
| **本質**          | 本地 runtime + 直接 context 注入 | 雲端模型 + 工具中介抽象層            |
| **Skill 讀取**    | 0 次 tool call（runtime 注入）   | N 次 tool call（每檔一次）           |
| **Subagent 繼承** | 可傳遞 context                   | Stateless，完全重來                  |
| **決策開銷**      | Deterministic 觸發               | 模型推理後才觸發 tool call           |
| **Token 效率**    | Skill metadata 不佔 context      | Metadata 常駐系統提示                |
| **瓶頸所在**      | 幾乎沒有                         | 網路 round-trip × tool call 次數     |
| **多 Skill 場景** | 線性增加（毫秒級）               | 指數惡化（round-trip × subagent 數） |

### 關鍵結論

Claude Code 把 Skill 當作 **context 的一部分直接灌入**，GitHub Copilot 把 Skill 當作**需要透過工具才能存取的外部資源**。這個設計選擇決定了兩者在 Skill 密集場景下的效能差距。
