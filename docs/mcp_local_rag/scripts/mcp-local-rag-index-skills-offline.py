#!/usr/bin/env python3
"""
離線模式：使用本地模型 zip 建立 dotnet-testing-agent-skills 的 SKILL 索引至 mcp-local-rag 向量資料庫

此腳本會先將本地模型 zip 解壓縮到 .mcp/cache，
再建立或更新 .mcp/dotnet-testing-skills 索引。
"""

import argparse
import os
import re
import shutil
import subprocess
import sys
import zipfile
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


def extract_offline_model(model_zip_path: Path, cache_dir: Path):
    with zipfile.ZipFile(model_zip_path, "r") as zf:
        zf.extractall(cache_dir)


def main():
    parser = argparse.ArgumentParser(description="[Offline] 建立 dotnet-testing-skills RAG 索引")
    parser.add_argument("--skills-path", required=True, help="dotnet-testing-agent-skills 的 .github/skills 路徑")
    parser.add_argument("--mode", choices=["update", "rebuild"], default="update")
    parser.add_argument(
        "--model-zip-path",
        default="",
        help="離線模型 zip 路徑。未指定時使用 docs/mcp_local_rag/model/Xenova-all-MiniLM-L6-v2.zip",
    )
    args = parser.parse_args()

    script_dir = Path(__file__).resolve().parent
    repo_root = script_dir.parent.parent.parent
    skills_dir = Path(args.skills_path)
    db_path = repo_root / ".mcp" / "dotnet-testing-skills"
    mcp_cache_path = repo_root / ".mcp" / "cache"
    models_path = repo_root / "models"
    model_zip_path = Path(args.model_zip_path) if args.model_zip_path else repo_root / "docs" / "mcp_local_rag" / "model" / "Xenova-all-MiniLM-L6-v2.zip"

    print(f"[Offline] Skills 來源：{skills_dir}")
    print(f"[Offline] DB 路徑：{db_path}")
    print(f"[Offline] Model zip：{model_zip_path}")
    print(f"[Offline] 模式：{args.mode}")
    print()

    ensure_command_exists(MCP_CMD)

    if not skills_dir.exists():
        print(f"{RED}找不到指定的 Skills 來源目錄：{skills_dir}{RESET}")
        sys.exit(1)

    if not model_zip_path.exists():
        print(f"{RED}找不到模型壓縮檔：{model_zip_path}{RESET}")
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

    print("解壓縮離線模型至 .mcp/cache ...")
    extract_offline_model(model_zip_path, mcp_cache_path)

    offline_model_dir = mcp_cache_path / "Xenova" / "all-MiniLM-L6-v2"
    if not offline_model_dir.exists():
        print(f"{RED}離線模型解壓失敗，找不到目錄：{offline_model_dir}{RESET}")
        sys.exit(1)
    print(f"{GREEN}離線模型已就緒：{offline_model_dir}{RESET}")

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
    print("開始建立索引（離線模型模式）...")
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
