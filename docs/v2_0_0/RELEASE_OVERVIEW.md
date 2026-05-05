# dotnet-testing-agent-orchestration v2.0.0 總覽

此文件說明 `dotnet-testing-agent-orchestration` v2.0.0 的變更焦點、主要變化，以及相對於 v1.0.0 的差異摘要。

---

## 1. v2.0.0 的變更焦點

`dotnet-testing-agent-orchestration` v2.0.0 的主要調整對象，是 `dotnet-testing` 測試工作流程在 GitHub Copilot 環境中的執行成本，而非測試場景種類的擴充。此版本的核心目標不是增加新的測試類型，而是處理 v1.0.0 在 GitHub Copilot 架構下暴露出的三類結構性問題：skill 載入延遲、subagent handoff 的 context 成本，以及 workflow 缺乏階段耗時可觀測性。

因此，v2.0.0 的重點在於工作流程本身的運作調整，而不是應用程式專案程式碼的相容性變更，也不是 sample 業務邏輯的重新設計。

### 1.1 v1.0.0 在 GitHub Copilot 下的主要瓶頸

v1.0.0 的 orchestrator 與四類 subagents，原則上會直接讀取 `dotnet-testing` 與 `dotnet-testing-advanced` 相關 skills。這種設計在 Claude Code 類型的執行環境中問題較小，但在 GitHub Copilot 中，skill 讀取是經由 tool call 與檔案 round-trip 完成；當 Writer、Reviewer 或其他 subagents 需要同時使用多個 skills 時，讀取成本會隨 tool call 次數快速累積。

此外，v1.0.0 的階段交接主要透過 prompt 內嵌摘要與自由文字 handoff 傳遞。當 Analyzer、Writer、Executor 與 Reviewer 的交接資訊持續膨脹時，subagent handoff 內容會直接擠壓 context window。v1.0.0 也缺乏正式的 phase timing 與總耗時記錄，使使用者難以判讀工作流程究竟耗時於 skill 載入、handoff、建置執行，或其他階段。

---

## 2. v2.0.0 的主要變化

### 2.1 以 `mcp-local-rag` 與 `dotnet-testing-skills` MCP 降低 skill 載入延遲

v2.0.0 明確要求先完成 `mcp-local-rag` 的安裝、技能索引建立與 MCP 設定，並以 `dotnet-testing-skills` 作為技能查詢的 MCP server。這項調整的目的，是將多 skill 場景從「每次都完整讀檔」改為「依查詢回傳相關片段」，降低 GitHub Copilot 在 tool-call 式 skill 讀取上的延遲成本。

repo 內以 `.vscode/mcp.json` 作為 MCP 的標準設定檔；相關安裝、索引建立與維護作業，另由 `docs/mcp_local_rag/` 文件區集中說明。關於 GitHub Copilot 與 Claude Code 的 skill-loading 差異，以及 `mcp-local-rag` 為何被選為解法，另見 [SKILL_LOADING_CLAUDE_CODE_VS_GITHUB_COPILOT.md](SKILL_LOADING_CLAUDE_CODE_VS_GITHUB_COPILOT.md) 與 [SKILL_LOADING_GITHUB_COPILOT_MCP_LOCAL_RAG.md](SKILL_LOADING_GITHUB_COPILOT_MCP_LOCAL_RAG.md)。

### 2.2 階段交接改為結構化 JSON handoff

Analyzer、Writer 與 Executor 的中間結果，改以 `.orchestrator/{TargetName}/` 下的 JSON 檔案交接，包含：

- `analyzer-result.json`
- `writer-result.json`
- `executor-result.json`

此變更使下游階段可直接取用上游結果，並將原本容易在 prompt 中持續膨脹的 handoff 資訊外移到結構化檔案，避免 subagent handoff 內容持續擠壓 context window。

### 2.3 workflow 增加 phase timing 與總耗時記錄

各 orchestrator 在 v2.0.0 均輸出對應的 timing log，記錄 phase 開始、phase 結束與 workflow 完成時間。這使使用者可以直接判讀各階段耗時與整體總耗時，作為 workflow 執行追蹤與問題診斷的基礎資料。

### 2.4 其他配套調整

除前三項核心調整外，v2.0.0 亦同步整理下列配套設定與版本資訊：

repo 內 agent 定義目前統一採用的模型為：

- 主要模型：`GPT-5.3-Codex (copilot)`
- 備用模型：`GPT-5.4 (copilot)`

v2.0.0 將 release / migration 文件與 `mcp-local-rag` 基礎設施文件分開管理：

- `docs/v2_0_0/`：release、migration、troubleshooting
- `docs/mcp_local_rag/`：安裝、索引、腳本、專屬排查
- `docs/v2_0_0/` 另補入 skill-loading 背景分析與設計分析文件

---

## 3. v1.0.0 與 v2.0.0 差異摘要

相較於 v1.0.0，v2.0.0 的差異主要集中在 GitHub Copilot 工作流程的執行方式，而不是測試類型或 sample 內容的擴充。

### 3.1 三項核心差異

1. Skill 取用方式改變

   v1.0.0 主要由 orchestrator 與 subagents 直接讀取 SKILL.md；v2.0.0 則因應 GitHub Copilot 的多 skill 讀取延遲，引入 `mcp-local-rag` 與 `dotnet-testing-skills` MCP，將技能取用改成查詢式流程。

2. 階段交接方式改變

   v1.0.0 以 prompt/context 傳遞階段摘要；v2.0.0 改為以 `.orchestrator/{TargetName}/` JSON 檔案交接 Analyzer、Writer 與 Executor 的中間結果，降低 handoff 對 context window 的壓力。

3. workflow 可觀測性提高

   v1.0.0 沒有正式的 phase timing 與總耗時記錄；v2.0.0 則要求各 orchestrator 輸出 timing log，使使用者可以區分 skill 載入、handoff 與建置執行等階段的耗時。

### 3.2 配套差異

除上述三項核心差異外，v2.0.0 也同步整理了工作流程周邊的配套資訊：

- repo 內 agent 定義目前統一採用 `GPT-5.3-Codex (copilot)` 與 `GPT-5.4 (copilot)`
- release、migration、`mcp-local-rag` 文件與 skill-loading 分析文件的分工更明確

這些項目屬於配套整理，而不是 v2.0.0 最核心的版本差異。

---

## 4. 維持不變的範圍

下列範圍在 v2.0.0 仍為正式提供或驗證的內容：

- 四類 orchestrators：Unit、Integration、Aspire、TUnit
- 四階段結構：Analyzer、Writer、Executor、Reviewer
- sample 驗證範圍：`.NET 8 / 9 / 10`

---

## 5. 相關文件

- skill-loading 背景分析： [Claude Code vs GitHub Copilot：Agent Skill 載入機制差異分析](SKILL_LOADING_CLAUDE_CODE_VS_GITHUB_COPILOT.md)
- `mcp-local-rag` 解法分析： [GitHub Copilot × mcp-local-rag：dotnet-testing Agent Skills 讀取效能解決方案](SKILL_LOADING_GITHUB_COPILOT_MCP_LOCAL_RAG.md)
- 升級調整與驗證步驟： [從 v1.0.0 升級至 v2.0.0](V1_TO_V2_MIGRATION_GUIDE.md)
- `mcp-local-rag` 安裝與索引： [mcp-local-rag 文件中心](../mcp_local_rag/README.md)
- workflow 層級排查： [v2.0.0 常見問題排查](TROUBLESHOOTING.md)
