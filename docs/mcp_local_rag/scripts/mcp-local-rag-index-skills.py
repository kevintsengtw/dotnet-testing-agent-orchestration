#!/usr/bin/env python3
"""
將 dotnet-test* SKILL.md 建立索引至 mcp-local-rag 向量資料庫

初次設定或 skills 有更新時執行。
只索引 dotnet-test 開頭的 skill 目錄，排除 autoresearch、skill-creator-advanced 等無關目錄。
索引位置：.mcp/dotnet-testing-skills（由 .vscode/mcp.json 的 DB_PATH 指定）

用法：
    python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py
    python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py --mode rebuild
    python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py --mode update
"""

import argparse
import os
import re
import shutil
import subprocess
import sys
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")
sys.stderr.reconfigure(encoding="utf-8")

MCP_CMD = "mcp-local-rag.cmd" if sys.platform == "win32" else "mcp-local-rag"


def main():
    parser = argparse.ArgumentParser(description="建立 dotnet-testing-skills RAG 索引")
    parser.add_argument(
        "--mode",
        choices=["update", "rebuild"],
        default="update",
        help="update: 保留現有索引，只更新有變更的文件（預設）；rebuild: 清除舊索引，完整重建",
    )
    args = parser.parse_args()

    script_dir = Path(__file__).resolve().parent
    repo_root = script_dir.parent.parent.parent
    skills_dir = repo_root / ".github" / "skills"
    db_path = repo_root / ".mcp" / "dotnet-testing-skills"

    print(f"Skills 目錄：{skills_dir}")
    print(f"DB 路徑：{db_path}")
    print(f"模式：{args.mode}")
    print()

    if args.mode == "rebuild":
        if db_path.exists():
            shutil.rmtree(db_path)
            print(f"已清除舊索引：{db_path}")
        db_path.mkdir(parents=True)
        print(f"建立 DB 目錄：{db_path}")
    else:
        if not db_path.exists():
            db_path.mkdir(parents=True)
            print(f"DB 目錄不存在，已建立：{db_path}（自動轉為完整建立）")
        else:
            print("保留現有索引，僅更新有變更的文件")

    target_skills = sorted(
        d for d in skills_dir.iterdir()
        if d.is_dir() and d.name.startswith("dotnet-test")
    )

    print(f"目標 skill 目錄（{len(target_skills)} 個）：")
    for skill in target_skills:
        print(f"  {skill.name}")
    print()
    print("開始建立索引（可能需要 2-5 分鐘，首次執行需下載 embedding model ~90MB）...")
    print()

    for skill in target_skills:
        print(f"  ingesting: {skill.name}")
        subprocess.run(
            [MCP_CMD, "--db-path", str(db_path), "ingest", str(skill), "--base-dir", str(skills_dir)],
            check=True,
        )

    print()
    print("驗證索引狀態：")
    subprocess.run([MCP_CMD, "--db-path", str(db_path), "status"])

    print()
    print("冒煙測試（查詢 NSubstitute）：")
    env = os.environ.copy()
    env["DB_PATH"] = str(db_path)
    result = subprocess.run(
        [MCP_CMD, "query", "Substitute.For Returns Received mock", "--limit", "3"],
        capture_output=True, text=True, encoding="utf-8", errors="replace", env=env,
    )
    matches = [line for line in result.stdout.splitlines() if re.search(r"filePath|score", line)]
    for line in matches[:9]:
        print(line)


if __name__ == "__main__":
    main()
