# mcp-local-rag 文件中心

本目錄集中整理 `mcp-local-rag` 相關的文件、腳本與故障排查資訊，供這套 GitHub Copilot workflow 使用。

`mcp-local-rag` 在這裡被視為一個獨立的基礎設施主題，而不是 Agent Skills 附錄，也不是 v2 release 文件中的單一章節。`docs/v2_0_0/` 只說明「v2 為何需要它」與「升級時要做什麼」；實際的安裝、配置、索引與排查細節都集中在本目錄。

---

## 建議閱讀順序

1. [MCP_LOCAL_RAG_SETUP_GUIDE.md](MCP_LOCAL_RAG_SETUP_GUIDE.md)

   首次安裝或重建環境時先讀這份，內容涵蓋前置需求（含取得 dotnet-testing-agent-skills）、Node.js、CLI 安裝、索引建立、驗證與維護。

2. [MCP_LOCAL_RAG_DESIGN.md](MCP_LOCAL_RAG_DESIGN.md)

   用來理解為什麼這套 GitHub Copilot workflow 需要 mcp-local-rag，以及它如何和 SKILL 載入、RAG 查詢、subagent handoff 一起運作。

3. [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

   處理 CLI、dotnet-testing* 技能找不到、索引、embedding model、query 品質與 `.vscode/mcp.json` 相關問題。

4. [scripts/](scripts/)

   存放索引與驗證腳本，供 Windows PowerShell 或跨平台 Python 環境使用。

---

## 目錄內容

| 文件或目錄                                                   | 用途                           |
| ------------------------------------------------------------ | ------------------------------ |
| [MCP_LOCAL_RAG_SETUP_GUIDE.md](MCP_LOCAL_RAG_SETUP_GUIDE.md) | 安裝與維護指南                 |
| [MCP_LOCAL_RAG_DESIGN.md](MCP_LOCAL_RAG_DESIGN.md)           | 設計背景、平台差異與採用理由   |
| [TROUBLESHOOTING.md](TROUBLESHOOTING.md)                     | mcp-local-rag 專屬常見問題排查 |
| [scripts/](scripts/)                                         | 建立與驗證 skills 索引的腳本   |

---

## 與其他文件區的邊界

- `docs/v2_0_0/`：說明 v2 release/migration 差異，並鏈向本目錄。
- `docs/skills/`：保留 Agent Skills 本身的學習與驗證文件，不再作為 mcp-local-rag 的主要入口。
- `.vscode/mcp.json`：屬於 repo 根目錄設定，本目錄會引用它作為標準設定檔。

---

## 快速開始

### 步驟一：clone dotnet-testing-agent-skills（技能來源）

```bash
git clone https://github.com/kevintsengtw/dotnet-testing-agent-skills.git
```

### 步驟二：安裝 mcp-local-rag

```bash
npm install -g mcp-local-rag
```

### 步驟三：建立索引

Windows（PowerShell）：

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills
```

macOS / Linux：

```bash
python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py --skills-path /path/to/dotnet-testing-agent-skills/.github/skills
```

### 步驟四：驗證索引

Windows（PowerShell）：

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-verify-skills-index.ps1
```

macOS / Linux：

```bash
python docs/mcp_local_rag/scripts/mcp-local-rag-verify-skills-index.py
```

> 若環境只有 `python3`，請將上述命令中的 `python` 替換為 `python3`。

若你是從 v1.0.0 升級，先讀 [../v2_0_0/V1_TO_V2_MIGRATION_GUIDE.md](../v2_0_0/V1_TO_V2_MIGRATION_GUIDE.md)，再回到本目錄完成安裝與驗證。
