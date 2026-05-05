# GitHub Copilot × mcp-local-rag 設計背景

本文件說明在 `dotnet-testing-agent-orchestration` 中，為什麼把 `mcp-local-rag` 視為必要前置。

---

## 1. 它解決的不是技能內容問題，而是技能讀取成本問題

`dotnet-testing-agent-skills` 的問題不在於 SKILL.md 不夠完整，而在於 GitHub Copilot 在多 skill 場景下，讀取成本過高：

- Writer 往往需要 7 到 12 個 skills。
- 每個 skill 都要經過工具呼叫與 round-trip。
- subagent 是 stateless，無法直接繼承上游已讀的完整內容。

因此，這套 GitHub Copilot workflow 需要一個能把 skill 取用從「完整讀檔」改成「取回最相關片段」的本地檢索層，`mcp-local-rag` 正是這個角色。

---

## 2. 為什麼 Claude Code 與 GitHub Copilot 的需求不同

兩個平台的 skill 載入機制不同：

- Claude Code 偏向直接把 skill 內容注入 context。
- GitHub Copilot 則需要透過工具層讀取或查詢檔案。

這個差異帶來兩個結果：

1. GitHub Copilot 的 skill 密集場景更容易被 round-trip 成本拖慢。
2. 當 Orchestrator、Writer、Reviewer 各自重新探索同一批 skills 時，延遲會被放大。

因此，在 GitHub Copilot 中，`mcp-local-rag` 必須被視為一個工作流層級的能力，而不是可有可無的個人最佳化技巧。

---

## 3. 它在 workflow 裡扮演的角色

### Writer

- 依 `loadedSkills` 與 `skillMap.writer` 判斷要取用哪些知識。
- 透過 `query_documents` 或 bounded RAG CLI 補回 API 用法、測試慣例與 reviewer rubric。
- 減少大量完整讀取 SKILL.md 帶來的 context 與延遲成本。

### Reviewer

- 以 `skillMap.reviewer` 為主，補充 reviewer rubric。
- 讓審查規則查詢可界定在有限範圍，而不是重新閱讀整份文件集。

### repo 內設定

- `.vscode/mcp.json` 宣告 MCP server。
- `.mcp/dotnet-testing-skills/` 保存索引資料庫。
- `.mcp/cache/` 保存 embedding model 快取。

這代表 `mcp-local-rag` 不只是文件議題，而是 workflow 配置、索引狀態與 agent 行為的一部分。
