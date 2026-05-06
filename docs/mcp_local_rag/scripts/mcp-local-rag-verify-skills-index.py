#!/usr/bin/env python3
"""
驗證 mcp-local-rag 索引庫狀態（不重建索引）

確認現有的 dotnet-testing-skills 索引是否正確：
  - DB 存在性
  - documentCount / chunkCount
  - 索引範圍（ingested=true 的項目全為 dotnet-test*，無雜訊）
  - 查詢功能（smoke test）

注意：此腳本不修改索引。若需重建索引，請依環境擇一執行：
        python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-online.py --skills-path <skills 來源路徑> --mode rebuild
        python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-offline.py --skills-path <skills 來源路徑> --mode rebuild
    若只更新有變動的文件（預設模式），請依環境擇一執行：
        python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-online.py --skills-path <skills 來源路徑>
        python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-offline.py --skills-path <skills 來源路徑>

用法：
    python docs/mcp_local_rag/scripts/mcp-local-rag-verify-skills-index.py
"""

import json
import os
import re
import subprocess
import sys
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")
sys.stderr.reconfigure(encoding="utf-8")

MCP_CMD = "mcp-local-rag.cmd" if sys.platform == "win32" else "mcp-local-rag"


GREEN = "\033[92m"
RED = "\033[91m"
CYAN = "\033[96m"
GRAY = "\033[90m"
YELLOW = "\033[93m"
RESET = "\033[0m"


def write_result(step: str, ok: bool, detail: str):
    status = " OK " if ok else "FAIL"
    color = GREEN if ok else RED
    print(f"{color}[{status}] {step:<20} {detail}{RESET}")


def main():
    script_dir = Path(__file__).resolve().parent
    repo_root = script_dir.parent.parent.parent
    db_path = repo_root / ".mcp" / "dotnet-testing-skills"

    passed = 0
    failed = 0

    print()
    print(f"{CYAN}mcp-local-rag 索引驗證{RESET}")
    print(f"DB 路徑：{db_path}")
    print("-" * 60)

    chunks_path = db_path / "chunks.lance"
    if db_path.exists():
        if chunks_path.exists():
            write_result("DB 存在性", True, "chunks.lance 存在")
            passed += 1
        else:
            write_result("DB 存在性", False, "目錄存在但 chunks.lance 缺失")
            print()
            print(f"{RED}DB 不完整，中止驗證。{RESET}")
            sys.exit(1)
    else:
        write_result("DB 存在性", False, "目錄不存在，請先執行線上版或離線版索引腳本建立 DB")
        print()
        print(f"{RED}DB 不存在，中止驗證。{RESET}")
        sys.exit(1)

    rag_env = os.environ.copy()
    rag_env["RAG_HYBRID_WEIGHT"] = "0.7"
    rag_env["RAG_GROUPING"] = "similar"
    rag_env["CACHE_DIR"] = str(repo_root / ".mcp" / "cache")
    print()
    status_result = subprocess.run(
        [MCP_CMD, "--db-path", str(db_path), "status"],
        capture_output=True, text=True, encoding="utf-8", errors="replace", env=rag_env,
    )
    status_lines = [line for line in status_result.stdout.splitlines() if re.match(r"^\s*\{", line)]
    try:
        status_json = json.loads(status_lines[-1]) if status_lines else json.loads(status_result.stdout)
        doc_count = status_json.get("documentCount")
        chunk_count = status_json.get("chunkCount", "?")
        search_mode = status_json.get("searchMode")
    except (json.JSONDecodeError, AttributeError, IndexError):
        doc_count = None
        chunk_count = "?"
        search_mode = None

    if doc_count is not None and doc_count >= 30:
        write_result("documentCount", True, f"documentCount: {doc_count}, chunkCount: {chunk_count}")
        passed += 1
    elif doc_count is not None:
        write_result("documentCount", False, f"documentCount: {doc_count}（預期 >= 30），可能索引不完整")
        failed += 1
    else:
        write_result("documentCount", False, "無法解析 status 輸出")
        failed += 1

    if search_mode == "hybrid":
        write_result("searchMode", True, "searchMode: hybrid（FTS 索引已啟用，混合搜尋可用）")
        passed += 1
    elif search_mode:
        write_result("searchMode", False, f"searchMode: {search_mode}（預期 hybrid，可能 FTS 索引未建立）")
        failed += 1
    else:
        write_result("searchMode", False, "searchMode 欄位缺失（status 輸出可能不完整）")
        failed += 1

    print()
    list_result = subprocess.run(
        [MCP_CMD, "--db-path", str(db_path), "list"],
        capture_output=True, text=True, encoding="utf-8", errors="replace",
    )
    try:
        list_json = json.loads(list_result.stdout)
        all_files = list_json.get("files", [])
        indexed = [f for f in all_files if f.get("ingested") is True]
        noise = [f for f in indexed if not re.search(r"dotnet-test", f.get("filePath", ""))]

        if not indexed:
            if doc_count is not None and doc_count >= 29:
                write_result("索引範圍", True, f"list 不追蹤目錄 ingest（ingested=0），但 documentCount={doc_count} 確認索引完整")
                passed += 1
            else:
                write_result("索引範圍", False, "已索引檔案數為 0，且 documentCount 未達預期")
                failed += 1
        elif noise:
            noise_names = ", ".join(Path(f["filePath"]).name for f in noise)
            write_result("索引範圍", False, f"含雜訊（ingested=true 但非 dotnet-test*）：{noise_names}")
            failed += 1
        else:
            write_result("索引範圍", True, f"已索引 {len(indexed)} 個檔案，全部為 dotnet-test*")
            passed += 1
        print(f"{GRAY}  （list 亦含 {len(all_files)} 個磁碟檔案記錄，其中未索引的顯示 ingested=false，屬正常現象）{RESET}")
    except (json.JSONDecodeError, KeyError) as error:
        write_result("索引範圍", False, f"list 命令解析失敗：{error}")
        failed += 1

    print()
    env = os.environ.copy()
    env["DB_PATH"] = str(db_path)
    query_result = subprocess.run(
        [MCP_CMD, "query", "NSubstitute mock interface Returns Received", "--limit", "3"],
        capture_output=True, text=True, encoding="utf-8", errors="replace", env=env,
    )
    query_output = query_result.stdout or ""
    has_result = "dotnet-testing-nsubstitute-mocking" in query_output

    if has_result:
        write_result("Smoke test", True, "回傳 dotnet-testing-nsubstitute-mocking 相關結果")
        passed += 1
    elif query_output.strip():
        write_result("Smoke test", False, "有回傳結果但未命中 nsubstitute-mocking（可能索引不完整）")
        failed += 1
    else:
        write_result("Smoke test", False, "查詢無回傳結果")
        failed += 1

    print()
    cache_dir = repo_root / ".mcp" / "cache"
    if cache_dir.exists():
        cache_files = list(cache_dir.rglob("*"))
        cache_count = sum(1 for file in cache_files if file.is_file())
        write_result("cache 目錄", True, f"{cache_dir} 存在（{cache_count} 個檔案，CACHE_DIR 已生效）")
        passed += 1
    else:
        write_result("cache 目錄", False, f"{cache_dir} 不存在（CACHE_DIR 設定可能未生效，或尚未產生快取）")
        print(f"{GRAY}  提示：執行一次 query 後 cache 目錄才會建立{RESET}")
        failed += 1

    print()
    print("-" * 60)
    if failed == 0:
        print(f"{GREEN}所有驗證通過（{passed} / {passed + failed}）{RESET}")
    else:
        print(f"{RED}驗證未通過（通過 {passed}，失敗 {failed}）{RESET}")
        print(f"{YELLOW}   若需重建索引請擇一執行：{RESET}")
        print(f"{YELLOW}      python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-online.py --skills-path <skills 來源路徑> --mode rebuild{RESET}")
        print(f"{YELLOW}      python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-offline.py --skills-path <skills 來源路徑> --mode rebuild{RESET}")
    print()


if __name__ == "__main__":
    main()
