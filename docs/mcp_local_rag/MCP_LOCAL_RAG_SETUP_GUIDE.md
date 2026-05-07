# mcp-local-rag 安裝與維護指南

本頁為總覽入口，安裝流程已拆分為兩份文件：

- 線上安裝版：[MCP_LOCAL_RAG_SETUP_GUIDE_ONLINE.md](MCP_LOCAL_RAG_SETUP_GUIDE_ONLINE.md)
- 離線模型版：[MCP_LOCAL_RAG_SETUP_GUIDE_OFFLINE.md](MCP_LOCAL_RAG_SETUP_GUIDE_OFFLINE.md)

---

## 共同前置作業（兩種方式都必須完成）

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

## 腳本分流

### 線上模式（可對外下載模型）

- PowerShell：`docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-online.ps1`
- Python：`docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-online.py`

### 離線模型模式（使用本地 zip）

- PowerShell：`docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-offline.ps1`
- Python：`docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-offline.py`

索引驗證腳本維持共用：

- `docs/mcp_local_rag/scripts/mcp-local-rag-verify-skills-index.ps1`
- `docs/mcp_local_rag/scripts/mcp-local-rag-verify-skills-index.py`

