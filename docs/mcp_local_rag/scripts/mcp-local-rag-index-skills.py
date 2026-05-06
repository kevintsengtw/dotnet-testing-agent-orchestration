#!/usr/bin/env python3
"""
相容別名：預設以線上模式建立 dotnet-testing-agent-skills 的 SKILL 索引

此檔案保留給舊版通用腳本名稱使用，實際行為等同
docs/mcp_local_rag/scripts/mcp-local-rag-index-skills-online.py。
"""

import subprocess
import sys
from pathlib import Path

sys.stdout.reconfigure(encoding="utf-8")
sys.stderr.reconfigure(encoding="utf-8")


def main():
    online_script = Path(__file__).with_name("mcp-local-rag-index-skills-online.py")
    if not online_script.exists():
        print(f"找不到線上模式腳本：{online_script}")
        sys.exit(1)

    print("注意：mcp-local-rag-index-skills.py 為相容別名，預設等同線上模式。")
    result = subprocess.run([sys.executable, str(online_script), *sys.argv[1:]], check=False)
    sys.exit(result.returncode)

