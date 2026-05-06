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
- 出現 `fetch failed`、`ECONNREFUSED`、`certificate` 或 timeout 等下載錯誤。

### 說明：企業環境的限制

`mcp-local-rag` 首次建立索引時會從 HuggingFace 下載 embedding model（`Xenova/all-MiniLM-L6-v2`，約 90MB）並快取至 `CACHE_DIR`（`.mcp/cache`）。企業網路通常封鎖對外連線，導致下載失敗。

本 repo 已在 `docs/mcp_local_rag/model/` 預先打包 zip 檔案，可直接解壓縮後使用，無需存取 HuggingFace。

### 處理：解壓縮預打包模型

#### 步驟 1：確認 zip 檔案存在

```text
docs/mcp_local_rag/model/Xenova-all-MiniLM-L6-v2.zip
```

#### 步驟 2：建立 cache 目錄並解壓縮

PowerShell：

```powershell
New-Item -ItemType Directory -Force -Path .mcp\cache | Out-Null
Expand-Archive -Path docs\mcp_local_rag\model\Xenova-all-MiniLM-L6-v2.zip -DestinationPath .mcp\cache -Force
```

Linux / macOS：

```bash
mkdir -p .mcp/cache
unzip -o docs/mcp_local_rag/model/Xenova-all-MiniLM-L6-v2.zip -d .mcp/cache
```

解壓縮後的目錄結構：

```text
.mcp/cache/
└── Xenova/
    └── all-MiniLM-L6-v2/
        ├── config.json
        ├── tokenizer.json
        ├── tokenizer_config.json
        ├── special_tokens_map.json
        ├── vocab.txt
        └── onnx/
            └── model_quantized.onnx
```

#### 步驟 3：執行索引（不需網路）

PowerShell：

```powershell
.\docs\mcp_local_rag\scripts\mcp-local-rag-index-skills.ps1 -SkillsPath C:\projects\dotnet-testing-agent-skills\.github\skills
```

跨平台 Python 版本：

```bash
python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py --skills-path /path/to/dotnet-testing-agent-skills/.github/skills
```

### 維護：更新模型 zip

當 `mcp-local-rag` 升級 embedding model 版本時，由維護者重新打包。  
zip 只包含 model 本身，與 index 無關，不需要 clone `dotnet-testing-agent-skills`。

#### 步驟 1：在有網路的機器上下載 model

使用 `huggingface_hub` 直接下載 model 檔案，不建立 index：

```bash
pip install huggingface_hub
```

```bash
python -c "
from huggingface_hub import snapshot_download
snapshot_download(
    'Xenova/all-MiniLM-L6-v2',
    local_dir='.mcp/cache/Xenova/all-MiniLM-L6-v2',
    ignore_patterns=['*.msgpack','*.h5','flax_model*','tf_model*','pytorch_model*','rust_model*']
)
"
```

下載完成後確認：

```bash
ls .mcp/cache/Xenova/all-MiniLM-L6-v2/
# 應看到 config.json、tokenizer.json、onnx/ 等檔案
```

#### 步驟 2：打包成 zip（僅包含必要檔案）

zip 只打包 mcp-local-rag 實際需要的 6 個檔案，排除其他 ONNX 變體與 HuggingFace 管理檔。

PowerShell：

```powershell
$src = ".mcp\cache\Xenova\all-MiniLM-L6-v2"
$tmp = "$env:TEMP\model-pack\Xenova\all-MiniLM-L6-v2"
New-Item -ItemType Directory -Force -Path "$tmp\onnx" | Out-Null
Copy-Item "$src\config.json", "$src\tokenizer.json", "$src\tokenizer_config.json", "$src\special_tokens_map.json", "$src\vocab.txt" $tmp
Copy-Item "$src\onnx\model_quantized.onnx" "$tmp\onnx\"
Compress-Archive -Path "$env:TEMP\model-pack\Xenova" -DestinationPath docs\mcp_local_rag\model\Xenova-all-MiniLM-L6-v2.zip -Force
Remove-Item -Recurse -Force "$env:TEMP\model-pack"
```

Linux / macOS：

```bash
src=".mcp/cache/Xenova/all-MiniLM-L6-v2"
tmp="/tmp/model-pack/Xenova/all-MiniLM-L6-v2"
mkdir -p "$tmp/onnx"
cp "$src/config.json" "$src/tokenizer.json" "$src/tokenizer_config.json" "$src/special_tokens_map.json" "$src/vocab.txt" "$tmp/"
cp "$src/onnx/model_quantized.onnx" "$tmp/onnx/"
cd /tmp/model-pack
zip -r - Xenova/ > /path/to/repo/docs/mcp_local_rag/model/Xenova-all-MiniLM-L6-v2.zip
rm -rf /tmp/model-pack
```

#### 步驟 3：Commit 並 push

```bash
git add docs/mcp_local_rag/model/Xenova-all-MiniLM-L6-v2.zip
git commit -m "更新: 更新預打包 embedding model zip"
git push
```

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
