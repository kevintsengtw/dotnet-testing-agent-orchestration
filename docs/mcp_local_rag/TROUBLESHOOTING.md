# mcp-local-rag 常見問題排查

---

## 1. `mcp-local-rag` 指令不存在

### 症狀：CLI 無法執行

- 終端機執行 `mcp-local-rag` 出現 command not found。
- Writer / Reviewer 看起來沒有走 RAG。

### 排查：確認安裝狀態

```bash
node --version
npm --version
npm list -g mcp-local-rag
npx --prefer-offline mcp-local-rag --help
```

### 處理：重新安裝 CLI

```bash
npm install -g mcp-local-rag
```

---

## 2. dotnet-testing* 技能找不到（前置驗證失敗）

### 症狀：索引腳本中止並顯示錯誤

- 執行索引腳本時出現「找不到 dotnet-testing* 技能目錄」。
- 忘記提供 `-SkillsPath`（PowerShell）或 `--skills-path`（Python）參數。
- 提供的路徑不正確或 `dotnet-testing-agent-skills` 尚未 clone。

### 排查：確認來源路徑

確認 `dotnet-testing-agent-skills` 已 clone 至本機，且路徑指向其 `.github/skills` 目錄：

```bash
# 確認目錄存在
ls /path/to/dotnet-testing-agent-skills/.github/skills

# 應看到多個 dotnet-testing* 目錄
```

### 處理：clone 倉庫並提供正確路徑

```bash
git clone https://github.com/kevintsengtw/dotnet-testing-agent-skills.git
```

PowerShell：

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills
```

跨平台 Python 版本：

```bash
python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py --skills-path /path/to/dotnet-testing-agent-skills/.github/skills
```

---

## 3. 索引庫不存在或 verify script 失敗

### 症狀：索引庫缺失或為空

- `.mcp/dotnet-testing-skills/` 不存在。
- verify script 回報 DB 不存在或 documentCount 為 0。

### 排查與處理：重建或驗證索引

PowerShell：

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills
.\docs\mcp_local_rag\scripts\mcp-local-rag-verify-skills-index.ps1
```

跨平台 Python 版本：

```bash
python ./docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py --skills-path /path/to/dotnet-testing-agent-skills/.github/skills
python ./docs/mcp_local_rag/scripts/mcp-local-rag-verify-skills-index.py
```

若懷疑索引漂移，直接完整重建：

PowerShell：

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills -Mode rebuild
```

跨平台 Python 版本：

```bash
python ./docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py --skills-path /path/to/dotnet-testing-agent-skills/.github/skills --mode rebuild
```

若環境只有 `python3`，請將上述命令中的 `python` 替換為 `python3`。

---

## 4. Embedding model 下載失敗

### 症狀：model 下載被阻擋

- 初次建立索引時卡在 model 下載。
- 公司網路阻擋 HuggingFace。

---

## 5. query 有結果但品質很差或命中為空

### 症狀：query 命中品質差或回傳為空

- `query_documents` / CLI query 可執行，但結果與目標技術不相干。
- reviewer bounded RAG 命中為空。

### 排查：確認索引狀態與查詢字串

```bash
mcp-local-rag --db-path .mcp/dotnet-testing-skills status
mcp-local-rag --db-path .mcp/dotnet-testing-skills query "NSubstitute mock interface Returns Received" --limit 3
```

### 可能原因：索引或設定漂移

- 索引庫太舊，尚未反映最新 skills。
- `.vscode/mcp.json` 的 `DB_PATH` 或 `BASE_DIR` 指錯地方。
- 查詢字串與現有 query surface 不一致。

### 處理：重建索引並校正設定

- 先重建索引（需提供 `-SkillsPath` / `--skills-path`）。
- 再檢查 `.vscode/mcp.json` 的 `BASE_DIR` 是否指向正確的 `dotnet-testing-agent-skills` 路徑。
- 最後回頭確認 Writer / Reviewer 使用的查詢字串是否被改壞。

---

## 6. `.vscode/mcp.json` 存在，但實際查到錯的 DB

### 症狀：VS Code 與 CLI 指到不同 DB

- CLI status 有資料，但 VS Code 裡的 query 行為像是讀到舊資料。
- 改過 `DB_PATH` 後結果沒有跟著變。

### 處理：校正 `.vscode/mcp.json` 與腳本路徑

1. 確認 `.vscode/mcp.json` 的 `DB_PATH` 指向 repo 根目錄內的 `.mcp/dotnet-testing-skills`。
2. 確認 `BASE_DIR` 指向本機 `dotnet-testing-agent-skills` 的 `.github/skills` 目錄。
3. 確認腳本與手動 CLI 使用的是同一個 DB 路徑。
4. 若近期調整過 `CACHE_DIR` 或 `DB_PATH`，重新跑一次 index 與 verify。

---

## 7. 不確定應該執行哪個腳本

### PowerShell

- 初次建立或更新索引：`docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.ps1`（需加 `-SkillsPath`）
- 驗證現有索引狀態：`docs/mcp_local_rag/scripts/mcp-local-rag-verify-skills-index.ps1`

### Python

- 初次建立或更新索引：`docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py`（需加 `--skills-path`）
- 驗證現有索引狀態：`docs/mcp_local_rag/scripts/mcp-local-rag-verify-skills-index.py`

若你是在 macOS / Linux，優先使用 Python 版本；若環境只有 `python3`，請把命令中的 `python` 改成 `python3`。
