# mcp-local-rag 安裝指南（離線模型版）

本文件適用於無法連線 HuggingFace 或網路受管制環境，改用 repo 內離線模型 zip 建立索引。

---

## 1. 共同前置作業

以下三項是線上版與離線版共用的必要條件：

1. 環境必須已完成安裝 Node.js 與 Python
2. 環境必須已完成安裝 mcp-local-rag
3. 完成 `.vscode/mcp.json` 設定

建議檢查：

```bash
node --version
python --version
npm list -g mcp-local-rag
npx --prefer-offline mcp-local-rag --help
```

---

## 2. 取得 skills 來源

```bash
git clone https://github.com/kevintsengtw/dotnet-testing-agent-skills.git
```

後續請使用其 `.github/skills` 路徑：

- Windows：`C:\projects\dotnet-testing-agent-skills\.github\skills`
- macOS / Linux：`/path/to/dotnet-testing-agent-skills/.github/skills`

---

## 3. 設定 .vscode/mcp.json

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

---

## 4. 離線模型檔案

預設模型壓縮檔位置：

`docs/mcp_local_rag/model/Xenova-all-MiniLM-L6-v2.zip`

離線腳本會自動：

1. 建立 `.mcp/cache` 與 `models`
2. 將 zip 解壓縮到 `.mcp/cache`
3. 建立或更新 `.mcp/dotnet-testing-skills` 索引

---

## 5. 建立索引（離線模式）

Windows（PowerShell）：

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills-offline.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills
```

完整重建：

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills-offline.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills -Mode rebuild
```

若模型 zip 不在預設位置，可指定：

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills-offline.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills -ModelZipPath C:\path\to\Xenova-all-MiniLM-L6-v2.zip
```

跨平台（Python）：

```bash
python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-offline.py --skills-path /path/to/dotnet-testing-agent-skills/.github/skills
python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-offline.py --skills-path /path/to/dotnet-testing-agent-skills/.github/skills --mode rebuild
python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-offline.py --skills-path /path/to/dotnet-testing-agent-skills/.github/skills --model-zip-path /path/to/Xenova-all-MiniLM-L6-v2.zip
```

> 若環境只有 `python3`，請將 `python` 改為 `python3`。

---

## 6. 驗證索引

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-verify-skills-index.ps1
```

或

```bash
python docs/mcp_local_rag/scripts/mcp-local-rag-verify-skills-index.py
```
