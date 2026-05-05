# GitHub Copilot × mcp-local-rag：dotnet-testing Agent Skills 讀取效能解決方案

> **建立日期**：2026-05-01
> **背景**：延伸自 [SKILL_LOADING_CLAUDE_CODE_VS_GITHUB_COPILOT.md](SKILL_LOADING_CLAUDE_CODE_VS_GITHUB_COPILOT.md) 的分析結論
> **適用範圍**：GitHub Copilot 版本的 dotnet-testing Orchestrator（非 Claude Code 版本）

---

## 目錄

- [GitHub Copilot × mcp-local-rag：dotnet-testing Agent Skills 讀取效能解決方案](#github-copilot--mcp-local-ragdotnet-testing-agent-skills-讀取效能解決方案)
  - [目錄](#目錄)
  - [1. 問題根源](#1-問題根源)
    - [Orchestrator 架構（1+4）](#orchestrator-架構14)
  - [2. 各 Agent 讀取次數分析](#2-各-agent-讀取次數分析)
    - [Writer 的讀取問題](#writer-的讀取問題)
  - [3. 排除的方案](#3-排除的方案)
    - [方案一：縮減 Skill 內容 / 合併 Skill / 移除 references 讀取](#方案一縮減-skill-內容--合併-skill--移除-references-讀取)
    - [方案二：自行用 C# MCP SDK 開發 MCP Server](#方案二自行用-c-mcp-sdk-開發-mcp-server)
  - [4. 選定方案：mcp-local-rag](#4-選定方案mcp-local-rag)
    - [為何選擇 mcp-local-rag](#為何選擇-mcp-local-rag)
    - [資源](#資源)
    - [MCP Tools（6 個）](#mcp-tools6-個)
  - [5. 使用模式改變](#5-使用模式改變)
    - [改前：Writer 逐一讀檔（每次 1 個完整 round-trip）](#改前writer-逐一讀檔每次-1-個完整-round-trip)
    - [改後：Writer 透過 RAG 取得相關片段（精準回傳所需段落）](#改後writer-透過-rag-取得相關片段精準回傳所需段落)
  - [6. 注意事項](#6-注意事項)
    - [Embedding Model 下載問題](#embedding-model-下載問題)
    - [MCP 設定層級](#mcp-設定層級)
  - [7. VS Code mcp.json 設定範本](#7-vs-code-mcpjson-設定範本)
    - [工作區設定（`.vscode\mcp.json`）](#工作區設定vscodemcpjson)
    - [搜尋調校（技術文件建議值）](#搜尋調校技術文件建議值)

---

## 1. 問題根源

`dotnet-testing-agent-skills` 包含 27 個 skill（19 個基礎 + 8 個進階）。在 GitHub Copilot Agent Orchestration 工作流程中，AI Agent 每次執行都需要進行多輪讀取與分析，造成執行時間過長。

根本原因是 GitHub Copilot 的架構特性（詳見 [載入機制差異分析](SKILL_LOADING_CLAUDE_CODE_VS_GITHUB_COPILOT.md)）：

- 每個 SKILL.md 都需要一次完整的 tool call round-trip
- Subagent 是 stateless，每個 subagent 都從零開始讀取 Skills
- 決策推理本身也消耗時間（模型先推理需要哪些 Skills，才觸發讀取）

### Orchestrator 架構（1+4）

```plaintext
dotnet-testing-orchestrator
    ├── dotnet-testing-analyzer    ← 分析被測試目標
    ├── dotnet-testing-writer      ← 載入 Skills 撰寫測試（主要瓶頸）
    ├── dotnet-testing-executor    ← 建置與執行測試
    └── dotnet-testing-reviewer    ← 審查測試品質
```

---

## 2. 各 Agent 讀取次數分析

| Agent        | 讀取次數    | 說明                          |
| ------------ | ----------- | ----------------------------- |
| Orchestrator | 0 ✅        | 硬性禁止讀取 SKILL.md         |
| Analyzer     | 0 ✅        | 純程式碼分析                  |
| Executor     | 1 ✅        | 固定讀 `dotnet-test/SKILL.md` |
| Reviewer     | 3–5 ⚠️    | 固定 3 個 + 條件最多 2 個     |
| **Writer**   | **7–12 ❌** | 主要瓶頸                      |

### Writer 的讀取問題

Writer 根據 `requiredTechniques` 逐一讀取 SKILL.md，一個典型的 service 測試場景：

```text
unit-test-fundamentals     → 1 次讀檔 + round-trip
test-naming-conventions    → 1 次讀檔 + round-trip
xunit-project-setup        → 1 次讀檔 + round-trip
nsubstitute-mocking        → 1 次讀檔 + round-trip
autofixture-basics         → 1 次讀檔 + round-trip
awesome-assertions         → 1 次讀檔 + round-trip
datetime-testing           → 1 次讀檔 + round-trip
autodata-xunit-integration → 1 次讀檔 + round-trip
+ references/ 目錄下的參考文件（每個 SKILL 可能再多 1 次）
+ existingTestPatternFile  → 1 次
+ source files / interfaces → 2–3 次
```

**每次執行 Writer 最少 11 次、可能到 15+ 次讀取，才開始寫第一行程式碼。**

在 4 個 Subagents 的架構下，若 2 個 Writers 並行（split 場景），這些 round-trips 會再乘以 2。

---

## 3. 排除的方案

### 方案一：縮減 Skill 內容 / 合併 Skill / 移除 references 讀取

❌ 否決。為了減少讀取次數而犧牲測試程式碼品質，思路根本上是錯的。Skill 內容的完整性是測試品質的保證。

### 方案二：自行用 C# MCP SDK 開發 MCP Server

❌ 否決。開發時間成本高，能否節省足夠執行時間是未知數，且分發與落地成本未解決。

---

## 4. 選定方案：mcp-local-rag

### 為何選擇 mcp-local-rag

| 條件                            | 說明                                                                                     |
| ------------------------------- | ---------------------------------------------------------------------------------------- |
| **Node.js 實作**                | `npx` 啟動，.NET 開發者環境幾乎都有 Node.js，不需額外安裝                                |
| **Markdown 原生支援**           | SKILL.md 直接 ingest，不需格式轉換                                                       |
| **Semantic + Keyword 混合搜尋** | 技術詞彙（`NSubstitute`、`FakeTimeProvider`、`AutoDataWithCustomization`）不會被語意模糊 |
| **完全本地端**                  | 不需 API key，不需另起 server process                                                    |
| **不需自行開發**                | 直接使用現成套件                                                                         |

### 資源

| 資源         | 連結                                                   |
| ------------ | ------------------------------------------------------ |
| GitHub Repo  | <https://github.com/shinpr/mcp-local-rag>              |
| npm 套件     | <https://www.npmjs.com/package/mcp-local-rag>          |
| 技術深度文章 | <https://www.norsica.jp/blog/local-rag-agentic-coding> |

### MCP Tools（6 個）

| Tool              | 說明                 |
| ----------------- | -------------------- |
| `ingest_file`     | 將檔案建立索引       |
| `ingest_data`     | 將 HTML 內容建立索引 |
| `query_documents` | 語意 + 關鍵字搜尋    |
| `list_files`      | 列出已索引的檔案     |
| `delete_file`     | 刪除索引中的檔案     |
| `status`          | 查看 server 狀態     |

---

## 5. 使用模式改變

### 改前：Writer 逐一讀檔（每次 1 個完整 round-trip）

```text
read(.github/skills/dotnet-testing-nsubstitute-mocking/SKILL.md)  ← 完整讀檔
read(.github/skills/dotnet-testing-autofixture-basics/SKILL.md)   ← 完整讀檔
read(.github/skills/dotnet-testing-datetime-testing.../SKILL.md)  ← 完整讀檔
... × 7-8 次
```

### 改後：Writer 透過 RAG 取得相關片段（精準回傳所需段落）

```text
query_documents("nsubstitute mock interface returns received async")
→ 回傳最相關的 chunk，精準包含需要的 API 用法

query_documents("autofixture build with frozen create")
→ 精準取回 AutoFixture 核心用法片段

query_documents("datetime testing FakeTimeProvider SetLocalNow GetLocalNow GetUtcNow")
→ 回傳 FakeTimeProvider 使用方式
```

**效果：** 原本 7–12 次完整讀檔 → 7–12 次 query（但每次只回傳相關 chunk，延遲遠低於完整讀檔）

---

## 6. 注意事項

### Embedding Model 下載問題

mcp-local-rag 初次啟動會從 HuggingFace 下載 embedding model（`all-MiniLM-L6-v2`，約 90MB）。

### MCP 設定層級

| 層級           | 設定檔位置                     | 適合場景                 |
| -------------- | ------------------------------ | ------------------------ |
| 工作區（專案） | `.vscode/mcp.json`             | commit 進 repo，團隊共用 |

---

## 7. VS Code mcp.json 設定範本

### 工作區設定（`.vscode\mcp.json`）

```json
{
  "servers": {
    "dotnet-testing-skills": {
      "command": "npx",
      "args": ["-y", "mcp-local-rag"],
      "env": {
        "BASE_DIR": "/path/to/dotnet-testing-agent-skills/.github/skills",
        "DB_PATH": "${workspaceFolder}/.mcp/dotnet-testing-skills",
        "CACHE_DIR": "${workspaceFolder}/.mcp/cache",
        "RAG_HYBRID_WEIGHT": "0.7",
        "RAG_GROUPING": "similar"
      }
    }
  }
}
```

> `BASE_DIR` 需替換為本機 `dotnet-testing-agent-skills` 的 `.github/skills` 實際路徑，因為技能索引來源已統一由外部倉庫提供。完整安裝與設定步驟參見 [../mcp_local_rag/MCP_LOCAL_RAG_SETUP_GUIDE.md](../mcp_local_rag/MCP_LOCAL_RAG_SETUP_GUIDE.md)。

### 搜尋調校（技術文件建議值）

```json
"env": {
  "RAG_HYBRID_WEIGHT": "0.7",
  "RAG_GROUPING": "similar"
}
```

| 參數                | 值        | 說明                                                                         |
| ------------------- | --------- | ---------------------------------------------------------------------------- |
| `RAG_HYBRID_WEIGHT` | `0.7`     | 語意 + 關鍵字平衡，確保 `NSubstitute`、`FakeTimeProvider` 等精確詞彙排名靠前 |
| `RAG_GROUPING`      | `similar` | 只回傳最相關的 chunk group，減少雜訊                                         |
