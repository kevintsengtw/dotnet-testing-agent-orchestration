# mcp-local-rag 安裝與維護指南

這份文件是 `dotnet-testing-agent-orchestration` 內 `mcp-local-rag` 的正式安裝與維護指南。對這套 GitHub Copilot workflow 來說，`mcp-local-rag` 不是可選最佳化，而是讓 Writer / Reviewer 在多 skill 場景下維持可接受延遲與品質的必要前置。

> 範圍限定：這份文件只處理 GitHub Copilot 路徑。Claude Code 路徑的 Skill 載入機制不同，不應直接套用這套安裝要求。

---

## 1. 為什麼它被獨立成專屬文件區

`mcp-local-rag` 現在被視為一個獨立的基礎設施主題，原因有三個：

- 它不是 Agent Skills 本身，而是 GitHub Copilot 取用 Skills 的加速與穩定化機制。
- 它不只影響 Writer，也影響 Reviewer、CLI smoke test 與 repo 根目錄設定。
- 它的安裝、索引、驗證與 troubleshooting 具有獨立的生命週期，不適合只當作 v2 release 文件的一個章節。

---

## 2. 前置需求

| 項目                | 要求                           |
| ------------------- | ------------------------------ |
| Node.js             | 18 以上                        |
| npm / npx           | 可正常執行                     |
| Git                 | 可使用 repo 內腳本與更新工作流 |
| VS Code             | 1.118 以上                     |
| GitHub Copilot Chat | 已安裝                         |

檢查指令：

```powershell
node --version
npm --version
```

---

## 3. 安裝 mcp-local-rag

### 一般環境

```powershell
npm install -g mcp-local-rag
```

### 企業 SSL 受管控環境

```powershell
npm config set strict-ssl false
npm install -g mcp-local-rag
npm config set strict-ssl true
```

以下為臨時性的做法，應該要正確地使用設定憑證的方式進行。

### 驗證安裝

```powershell
npm list -g mcp-local-rag
npx --prefer-offline mcp-local-rag --help
```

---

## 4. MCP 設定

建議在 repo 根目錄的 `.vscode/mcp.json` 採用以下設定：

```json
{
  "servers": {
    "dotnet-testing-skills": {
      "command": "npx",
      "args": ["-y", "mcp-local-rag"],
      "env": {
        "BASE_DIR": "${workspaceFolder}/.github/skills",
        "DB_PATH": "${workspaceFolder}/.mcp/dotnet-testing-skills",
        "CACHE_DIR": "${workspaceFolder}/.mcp/cache",
        "RAG_HYBRID_WEIGHT": "0.7",
        "RAG_GROUPING": "similar"
      }
    }
  }
}
```

這代表 repo 內應準備的相關資產為：

- `.github/skills/` 作為索引來源
- `.mcp/dotnet-testing-skills/` 作為索引資料庫
- `.mcp/cache/` 作為 embedding model 快取

---

## 5. 建立 Skills 索引庫

### Windows 首選：PowerShell 腳本

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1
```

完整重建：

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -Mode rebuild
```

### 跨平台：Python 腳本

```bash
python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py
python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py --mode rebuild
```

### 何時用 `update`，何時用 `rebuild`

| 情境                    | 建議模式             |
| ----------------------- | -------------------- |
| 單一或少量 skill 有修改 | `update`             |
| 新增新的 skill          | `update`，之後做驗證 |
| 大量同步上游 skills     | `rebuild`            |
| 索引狀態異常或結果漂移  | `rebuild`            |

---

## 6. 驗證索引庫

### PowerShell

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-verify-skills-index.ps1
```

### Python

```bash
python docs/mcp_local_rag/scripts/mcp-local-rag-verify-skills-index.py
```

### 你應該看到的成功訊號

- `.mcp/dotnet-testing-skills/` 目錄存在
- status 顯示 `documentCount` / `chunkCount`
- list / query smoke test 有回傳
- cache 目錄已建立

也可手動驗證：

```bash
mcp-local-rag --db-path .mcp/dotnet-testing-skills status
mcp-local-rag --db-path .mcp/dotnet-testing-skills query "NSubstitute mock interface Returns Received" --limit 3
```

---

## 7. 在 workflow 中的實際角色

### Writer

- 先讀 `loadedSkills` 對應的 SKILL.md
- 再使用 MCP `query_documents` 批次查詢技術知識
- 以 `skillMap.writer` 為優先來源，只有缺失時才 fallback 到 `requiredTechniques`

### Reviewer

- 先讀 `loadedSkills` 對應的 SKILL.md
- 再以 CLI 呼叫 bounded RAG 查詢 reviewer rubric
- 優先使用 `skillMap.reviewer`，只有缺失時才 fallback

`mcp-local-rag` 在這裡不是獨立產品功能，而是這套 skill-loading 策略的一部分。
