# dotnet-testing-agent-orchestration v2.0.0 文件

`docs/v2_0_0/` 提供 `dotnet-testing-agent-orchestration` v2.0.0 的 release、背景分析、migration 與 troubleshooting 文件。內容聚焦於 `dotnet-testing` 測試工作流程在 GitHub Copilot 環境中的主要變化、與 v1.0.0 的差異、升級調整事項，以及流程層級的常見問題排查。

`mcp-local-rag` 的安裝、索引建立、維護腳本與專屬排查，另見 [../mcp_local_rag/README.md](../mcp_local_rag/README.md)。

---

## 文件範圍

- v2.0.0 的主要變化與版本差異摘要
- v2.0.0 設計背景與 skill-loading 問題分析
- v1.0.0 升級至 v2.0.0 的前置條件與驗證步驟
- workflow 層級的常見問題排查

`docs/v2_0_0/` 不重複承載 `mcp-local-rag` 的安裝與索引細節。

---

## 建議閱讀順序

1. [RELEASE_OVERVIEW.md](RELEASE_OVERVIEW.md)

   說明 v2.0.0 的更新背景、主要變化，以及與 v1.0.0 的差異摘要。

2. [SKILL_LOADING_CLAUDE_CODE_VS_GITHUB_COPILOT.md](SKILL_LOADING_CLAUDE_CODE_VS_GITHUB_COPILOT.md)

   說明 GitHub Copilot 與 Claude Code 在 agent skill 載入機制上的差異，作為理解 v2.0.0 升版背景的技術分析文件。

3. [SKILL_LOADING_GITHUB_COPILOT_MCP_LOCAL_RAG.md](SKILL_LOADING_GITHUB_COPILOT_MCP_LOCAL_RAG.md)

   說明在 GitHub Copilot 環境中採用 `mcp-local-rag` 與 `dotnet-testing-skills` MCP 的設計理由與方案分析。

4. [V1_TO_V2_MIGRATION_GUIDE.md](V1_TO_V2_MIGRATION_GUIDE.md)

   說明從 v1.0.0 升級至 v2.0.0 所需的設定調整與最小驗證步驟。

5. [../mcp_local_rag/README.md](../mcp_local_rag/README.md)

   說明 `mcp-local-rag` 的安裝、索引建立、驗證與維護方式。

6. [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

   說明 v2.0.0 workflow 層級的常見問題與處理方式。

---

## 文件清單

| 文件                                                                                             | 用途                                                                       |
| ------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------- |
| [RELEASE_OVERVIEW.md](RELEASE_OVERVIEW.md)                                                       | 說明 v2.0.0 的升版背景、主要變化與版本差異摘要                             |
| [SKILL_LOADING_CLAUDE_CODE_VS_GITHUB_COPILOT.md](SKILL_LOADING_CLAUDE_CODE_VS_GITHUB_COPILOT.md) | 說明 GitHub Copilot 與 Claude Code 的 skill-loading 差異，作為背景分析文件 |
| [SKILL_LOADING_GITHUB_COPILOT_MCP_LOCAL_RAG.md](SKILL_LOADING_GITHUB_COPILOT_MCP_LOCAL_RAG.md)   | 說明 `mcp-local-rag` 與 `dotnet-testing-skills` MCP 的設計分析與解法選擇   |
| [V1_TO_V2_MIGRATION_GUIDE.md](V1_TO_V2_MIGRATION_GUIDE.md)                                       | 說明從 v1.0.0 升級至 v2.0.0 的調整事項與驗證步驟                           |
| [TROUBLESHOOTING.md](TROUBLESHOOTING.md)                                                         | 說明 v2.0.0 workflow 層級的常見問題排查                                    |
| [../mcp_local_rag/README.md](../mcp_local_rag/README.md)                                         | 說明 `mcp-local-rag` 的安裝、索引與維護                                    |
