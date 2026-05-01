# v2.0.0 常見問題排查

此文件整理 `dotnet-testing-agent-orchestration` v2.0.0 workflow 層級的常見問題。`mcp-local-rag` 安裝、索引建立、查詢品質與資料庫路徑等專屬問題，相關排查請參見 [../mcp_local_rag/TROUBLESHOOTING.md](../mcp_local_rag/TROUBLESHOOTING.md)。

---

## 問題索引

| 問題                                | 位置                                                                       |
| ----------------------------------- | -------------------------------------------------------------------------- |
| `.orchestrator` JSON handoff 未產生 | 第 1 節                                                                    |
| timing log 不完整或內容異常         | 第 2 節                                                                    |
| Integration / Aspire 環境無法啟動   | 第 3 節                                                                    |
| 設定或文件仍沿用 v1.0.0 的原本設定  | 第 4 節                                                                    |
| `mcp-local-rag` 專屬問題            | [../mcp_local_rag/TROUBLESHOOTING.md](../mcp_local_rag/TROUBLESHOOTING.md) |

---

## 1. `.orchestrator` JSON handoff 未產生

### 問題描述：`.orchestrator` JSON handoff 未產生

- workflow 已執行，但 `.orchestrator/{TargetName}/` 下未產生 JSON 檔案
- Reviewer 或 Executor 無法取得上游結果

### 可能原因：`.orchestrator` JSON handoff 未產生

- workflow 未實際執行至目標 orchestrator
- 階段交接路徑未正確傳遞
- 某一階段於輸出 JSON 前即已中止

### 處理方式：`.orchestrator` JSON handoff 未產生

1. 以單一目標重新執行一次 workflow。
2. 確認目標專案下是否已建立 `.orchestrator/{TargetName}/` 目錄。
3. 依序檢查 `analyzer-result.json`、`writer-result.json`、`executor-result.json` 是否存在。
4. 若僅部分檔案存在，回溯對應階段的輸入與執行結果。

### 相關文件：`.orchestrator` JSON handoff 未產生

- 升級調整與輸出格式： [V1_TO_V2_MIGRATION_GUIDE.md](V1_TO_V2_MIGRATION_GUIDE.md)

---

## 2. timing log 不完整或內容異常

### 問題描述：timing log 不完整或內容異常

- 存在 `WORKFLOW_START`，但缺少 `WORKFLOW_END`
- 同一階段事件重複出現
- phase 結束順序不合理

### 可能原因：timing log 不完整或內容異常

- 同一目標在短時間內重複執行
- 多目標執行結果混入單一目標判讀
- workflow 於中途失敗，導致記錄未完整寫入

### 處理方式：timing log 不完整或內容異常

1. 以單一目標重新執行最小驗證。
2. 分離重跑前後的 log 內容，避免混合判讀。
3. 若異常可重現，回溯對應 orchestrator 的事件寫入邏輯。

### 相關文件：timing log 不完整或內容異常

- 主要變化與 timing log 說明： [RELEASE_OVERVIEW.md](RELEASE_OVERVIEW.md)

---

## 3. Integration / Aspire 環境無法啟動

### 問題描述：Integration / Aspire 環境無法啟動

- Integration 測試無法啟動容器
- Aspire 測試於初始化階段失敗

### 可能原因：Integration / Aspire 環境無法啟動

- Docker Desktop 未啟動
- 容器 runtime 無法使用
- Aspire workload 尚未安裝

### 處理方式：Integration / Aspire 環境無法啟動

1. 確認 Docker Desktop 已啟動。
2. 確認容器 runtime 可正常使用。
3. 以 `dotnet workload list` 檢查 Aspire workload。

```powershell
dotnet workload list
```

若尚未安裝 Aspire workload：

```powershell
dotnet workload install aspire
```

### 相關文件：Integration / Aspire 環境無法啟動

- 升級前檢查項目： [V1_TO_V2_MIGRATION_GUIDE.md](V1_TO_V2_MIGRATION_GUIDE.md)

---

## 4. 設定或文件仍沿用 v1.0.0 的原設定

### 問題描述：設定或文件仍沿用 v1.0.0 的原設定

- 既有操作文件仍引用舊模型設定
- 既有設定未將 `mcp-local-rag` 視為正式前置
- 排查仍只依賴 chat 摘要，而未檢查 `.orchestrator` JSON 與 timing log

### 可能原因：設定或文件仍沿用 v1.0.0 的原設定

- 團隊文件尚未完成更新
- 本機設定沿用 v1.0.0 時期的假設
- 升級作業未完成最小驗證

### 處理方式：設定或文件仍沿用 v1.0.0 的原設定

1. 以 [RELEASE_OVERVIEW.md](RELEASE_OVERVIEW.md) 為準，重新確認 v2.0.0 的主要變化。
2. 以 [V1_TO_V2_MIGRATION_GUIDE.md](V1_TO_V2_MIGRATION_GUIDE.md) 為準，完成必要設定調整。
3. 重新執行單一目標最小驗證，確認 JSON handoff 與 timing log 已正常產生。

### 相關文件：設定或文件仍沿用 v1.0.0 的原設定

- 主要變化與版本差異摘要： [RELEASE_OVERVIEW.md](RELEASE_OVERVIEW.md)
- 升級調整與驗證步驟： [V1_TO_V2_MIGRATION_GUIDE.md](V1_TO_V2_MIGRATION_GUIDE.md)

---

## 5. 最小驗證檢查項目

- [ ] `mcp-local-rag` 已安裝並完成索引驗證
- [ ] 已在 VS Code 選取目標 orchestrator
- [ ] 已完成一次單一目標 workflow 執行
- [ ] `.orchestrator/{TargetName}/` JSON handoff 已正常產生
- [ ] 對應 orchestrator 的 timing log 已正常產生
