# 從 v1.0.0 升級至 v2.0.0

此文件適用於已採用 `dotnet-testing-agent-orchestration` v1.0.0 文件、設定或操作流程之使用者。內容說明升級至 v2.0.0 所需的前置條件、設定調整、最小驗證步驟與確認項目。

---

## 1. 適用範圍

適用於下列情境：

- 已依 v1.0.0 文件建立使用流程
- 已依 v1.0.0 設定維護本機或團隊操作指引
- 已使用既有 agent 設定、模型說明或 workflow 輸出假設

v2.0.0 的差異摘要參見 [RELEASE_OVERVIEW.md](RELEASE_OVERVIEW.md)。

---

## 2. 升級前檢查

| 項目                        | 要求                      | 說明                                                                  |
| --------------------------- | ------------------------- | --------------------------------------------------------------------- |
| VS Code                     | 1.118 以上                | 需支援 Custom Agents / Subagents                                      |
| GitHub Copilot Chat         | 已安裝                    | 為 workflow 的執行環境                                                |
| Node.js                     | 18 以上                   | `mcp-local-rag` 必要前置                                              |
| dotnet-testing-agent-skills | 已 clone 至本機           | mcp-local-rag 的技能索引來源，clone 自 dotnet-testing-agent-skills    |
| .NET SDK                    | 8 / 9 / 10                | 依 sample 與驗證情境而定                                              |
| Docker Desktop              | Integration / Aspire 適用 | 容器化測試情境適用                                                    |
| Aspire workload             | Aspire 適用               | 以 `dotnet workload install aspire` 安裝                              |

---

## 3. 必要調整項目

### 3.1 取得 dotnet-testing-agent-skills 並安裝 `mcp-local-rag`

`dotnet-testing-agent-skills` 是 mcp-local-rag 索引庫的技能來源，需先 clone 至本機：

```bash
git clone https://github.com/kevintsengtw/dotnet-testing-agent-skills.git
```

clone 完成後，再安裝 `mcp-local-rag`、建立索引並完成驗證。

相關步驟與腳本參見 [../mcp_local_rag/README.md](../mcp_local_rag/README.md)。

### 3.2 確認 `.vscode/mcp.json` 設定

目前 repo 內採用的 MCP 標準設定如下，請將 `BASE_DIR` 替換為本機 `dotnet-testing-agent-skills` 的實際路徑：

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

> `BASE_DIR` 需指向本機 `dotnet-testing-agent-skills` 的 `.github/skills` 目錄。`DB_PATH` 與 `CACHE_DIR` 使用 `${workspaceFolder}` 指向本 repo。

若既有設定仍沿用 v1.0.0 時期的路徑或假設，應先完成對齊。

### 3.3 調整對 workflow 輸出的預期

v2.0.0 的 workflow 輸出除測試檔案外，亦包含：

- `.orchestrator/{TargetName}/analyzer-result.json`
- `.orchestrator/{TargetName}/writer-result.json`
- `.orchestrator/{TargetName}/executor-result.json`
- 對應 orchestrator 的 timing log

後續檢查與排查應以這些輸出為主要依據。

### 3.4 更新模型設定說明

若既有操作文件、團隊說明或自訂設定仍引用 v1.0.0 的模型設定，應更新為目前 repo 內 agent 定義所採用的：

- `GPT-5.3-Codex (copilot)`
- `GPT-5.4 (copilot)`

---

## 4. 最小驗證步驟

完成升級調整後，建議以單一目標進行最小驗證：

1. 確認 `mcp-local-rag` 已完成安裝與索引驗證。
2. 在 VS Code 中選取目標 orchestrator。
3. 以單一類別或單一方法執行一次 workflow。
4. 確認目標專案下已產生 `.orchestrator/{TargetName}/` JSON 檔案。
5. 確認對應 orchestrator 的 timing log 已正常產生。

---

## 5. 升級後確認清單

- [ ] `dotnet-testing-agent-skills` 已 clone 至本機
- [ ] `mcp-local-rag` 已安裝並完成索引驗證
- [ ] `.vscode/mcp.json` 已建立，`BASE_DIR` 已指向本機 `dotnet-testing-agent-skills` 路徑
- [ ] 已完成一次單一目標最小驗證
- [ ] `.orchestrator/{TargetName}/` JSON 檔案已正常產生
- [ ] 對應 orchestrator 的 timing log 已正常產生
- [ ] 既有文件或團隊說明中的模型設定已完成更新

---

## 6. 相關文件

- 主要變化與版本差異摘要： [RELEASE_OVERVIEW.md](RELEASE_OVERVIEW.md)
- `mcp-local-rag` 安裝與索引： [../mcp_local_rag/README.md](../mcp_local_rag/README.md)
- workflow 層級排查： [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
