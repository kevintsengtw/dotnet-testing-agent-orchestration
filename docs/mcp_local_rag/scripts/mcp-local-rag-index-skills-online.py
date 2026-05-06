#!/usr/bin/env python3
"""
線上模式：建立 dotnet-testing-agent-skills 的 SKILL 索引至 mcp-local-rag 向量資料庫

此腳本使用線上模式建立索引，首次執行時若本機尚無 embedding model，
mcp-local-rag 會自動下載並快取至 .mcp/cache。
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
RESET = "\033[0m"


def ensure_command_exists(command_name: str):
    if shutil.which(command_name) is None:
        print(f"{RED}找不到 {command_name} 指令。請先安裝：npm install -g mcp-local-rag{RESET}")
        sys.exit(1)


def main():
    parser = argparse.ArgumentParser(description="[Online] 建立 dotnet-testing-skills RAG 索引")
    parser.add_argument("--skills-path", required=True, help="dotnet-testing-agent-skills 的 .github/skills 路徑")
    parser.add_argument("--mode", choices=["update", "rebuild"], default="update")
    args = parser.parse_args()

    script_dir = Path(__file__).resolve().parent
    repo_root = script_dir.parent.parent.parent
    skills_dir = Path(args.skills_path)
    db_path = repo_root / ".mcp" / "dotnet-testing-skills"
    mcp_cache_path = repo_root / ".mcp" / "cache"
    models_path = repo_root / "models"

    print(f"[Online] Skills 來源：{skills_dir}")
    print(f"[Online] DB 路徑：{db_path}")
    print(f"[Online] 模式：{args.mode}")
    print()

    ensure_command_exists(MCP_CMD)

    if not skills_dir.exists():
        print(f"{RED}找不到指定的 Skills 來源目錄：{skills_dir}{RESET}")
        sys.exit(1)

    testing_skills = [d for d in skills_dir.iterdir() if d.is_dir() and d.name.startswith("dotnet-testing")]
    if not testing_skills:
        print(f"{RED}在指定路徑找不到 dotnet-testing* 技能目錄：{skills_dir}{RESET}")
        sys.exit(1)

    print(f"{GREEN}前置驗證通過：找到 {len(testing_skills)} 個 dotnet-testing* 技能目錄{RESET}")
    print()

    if not mcp_cache_path.exists():
        mcp_cache_path.mkdir(parents=True)
        print(f"建立目錄：{mcp_cache_path}")

    if not models_path.exists():
        models_path.mkdir(parents=True)
        print(f"建立目錄：{models_path}")

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

    target_skills = sorted(d for d in skills_dir.iterdir() if d.is_dir() and d.name.startswith("dotnet-test"))

    print()
    print(f"目標 skill 目錄（{len(target_skills)} 個）：")
    for skill in target_skills:
        print(f"  {skill.name}")
    print()
    print("開始建立索引（線上模式，首次執行可能需下載 embedding model）...")
    print()

    for skill in target_skills:
        print(f"  ingesting: {skill.name}")
        subprocess.run([MCP_CMD, "--db-path", str(db_path), "ingest", str(skill), "--base-dir", str(skills_dir)], check=True)

    print()
    print("驗證索引狀態：")
    subprocess.run([MCP_CMD, "--db-path", str(db_path), "status"], check=True)

    print()
    print("冒煙測試（查詢 NSubstitute）：")
    env = os.environ.copy()
    env["DB_PATH"] = str(db_path)
    result = subprocess.run(
        [MCP_CMD, "query", "Substitute.For Returns Received mock", "--limit", "3"],
        capture_output=True,
        text=True,
        encoding="utf-8",
        errors="replace",
        env=env,
        check=True,
    )
    matches = [line for line in result.stdout.splitlines() if re.search(r"filePath|score", line)]
    for line in matches[:9]:
        print(line)


if __name__ == "__main__":
    main()
