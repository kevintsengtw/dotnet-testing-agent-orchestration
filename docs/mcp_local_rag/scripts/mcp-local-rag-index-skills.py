#!/usr/bin/env python3
"""
將 dotnet-testing-agent-skills 的 SKILL 內容建立索引至 mcp-local-rag 向量資料庫

初次設定或 skills 有更新時執行。
索引來源為 dotnet-testing-agent-skills 倉庫（需另行 clone），以 --skills-path 參數指定其
.github/skills 目錄路徑。執行前會先驗證來源路徑是否包含 dotnet-testing* 技能目錄。
索引位置：.mcp/dotnet-testing-skills（由 .vscode/mcp.json 的 DB_PATH 指定）

用法：
    python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py --skills-path /path/to/dotnet-testing-agent-skills/.github/skills
    python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py --skills-path /path/to/dotnet-testing-agent-skills/.github/skills --mode rebuild
    python docs/mcp_local_rag/scripts/mcp-local-rag-index-skills.py --skills-path /path/to/dotnet-testing-agent-skills/.github/skills --mode update
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

GREEN = "\033[92m"
RED = "\033[91m"
YELLOW = "\033[93m"
RESET = "\033[0m"


def main():
    parser = argparse.ArgumentParser(description="建立 dotnet-testing-skills RAG 索引")
    parser.add_argument(
        "--skills-path",
        required=True,
        help="dotnet-testing-agent-skills 倉庫的 .github/skills 目錄路徑（必填）。"
             "範例：/path/to/dotnet-testing-agent-skills/.github/skills",
    )
    parser.add_argument(
        "--mode",
        choices=["update", "rebuild"],
        default="update",
        help="update: 保留現有索引，只更新有變更的文件（預設）；rebuild: 清除舊索引，完整重建",
    )
    args = parser.parse_args()

    script_dir = Path(__file__).resolve().parent
    repo_root = script_dir.parent.parent.parent
    skills_dir = Path(args.skills_path)
    db_path = repo_root / ".mcp" / "dotnet-testing-skills"

    print(f"Skills 來源：{skills_dir}")
    print(f"DB 路徑：{db_path}")
    print(f"模式：{args.mode}")
    print()

    # 前置驗證：確認來源目錄存在且包含 dotnet-testing* 技能目錄
    if not skills_dir.exists():
        print(f"{RED}❌ 找不到指定的 Skills 來源目錄：{skills_dir}{RESET}")
        print(f"{YELLOW}   請確認 dotnet-testing-agent-skills 已正確 clone，且 --skills-path 路徑正確。{RESET}")
        print(f"{YELLOW}   倉庫來源：https://github.com/kevintsengtw/dotnet-testing-agent-skills{RESET}")
        sys.exit(1)

    testing_skills = [
        d for d in skills_dir.iterdir()
        if d.is_dir() and d.name.startswith("dotnet-testing")
    ]

    if not testing_skills:
        print(f"{RED}❌ 在指定路徑找不到 dotnet-testing* 技能目錄：{skills_dir}{RESET}")
        print(f"{YELLOW}   請確認 --skills-path 指向 dotnet-testing-agent-skills 的 .github/skills 目錄。{RESET}")
        print(f"{YELLOW}   倉庫來源：https://github.com/kevintsengtw/dotnet-testing-agent-skills{RESET}")
        sys.exit(1)

    print(f"{GREEN}✓ 前置驗證通過：找到 {len(testing_skills)} 個 dotnet-testing* 技能目錄{RESET}")
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

    print()
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
