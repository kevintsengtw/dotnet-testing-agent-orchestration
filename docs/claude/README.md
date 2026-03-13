# Claude Code Agent Orchestration 遷移分析報告

> 將 GitHub Copilot Agent Orchestration 遷移至 Claude Code 的完整分析與架構規劃

## 目錄

- [Claude Code Agent Orchestration 遷移分析報告](#claude-code-agent-orchestration-遷移分析報告)
  - [目錄](#目錄)
  - [一、概念對照表](#一概念對照表)
  - [二、架構方案：推薦使用 Subagents 模式](#二架構方案推薦使用-subagents-模式)
    - [為什麼選 Subagents 而非 Agent Teams](#為什麼選-subagents-而非-agent-teams)
  - [三、目錄結構規劃](#三目錄結構規劃)
  - [四、各元件的 Claude Code 實作對照](#四各元件的-claude-code-實作對照)
    - [4.1 Orchestrator → Skill（user-invocable）](#41-orchestrator--skilluser-invocable)
    - [4.2 Subagent 定義（AGENT.md）](#42-subagent-定義agentmd)
      - [Analyzer](#analyzer)
      - [Writer](#writer)
      - [Executor](#executor)
      - [Reviewer](#reviewer)
    - [4.3 Knowledge Skills（知識型，非 user-invocable）](#43-knowledge-skills知識型非-user-invocable)
  - [五、執行流程示意](#五執行流程示意)
  - [六、關鍵技術差異與對策](#六關鍵技術差異與對策)
  - [七、Hooks 的品質閘道應用（可選）](#七hooks-的品質閘道應用可選)
  - [八、實作優先順序建議](#八實作優先順序建議)
  - [九、與 GitHub Copilot 體驗差異對照](#九與-github-copilot-體驗差異對照)
  - [十、結論](#十結論)

---

## 一、概念對照表

| GitHub Copilot 概念                        | Claude Code 對應機制                                         | 對應程度 |
| ------------------------------------------ | ------------------------------------------------------------ | -------- |
| `.github/agents/*.agent.md` (Orchestrator) | `.claude/agents/name/AGENT.md` + **Skills (user-invocable)** | 完全對應 |
| `.github/agents/*.agent.md` (Subagent)     | `.claude/agents/name/AGENT.md` (subagent 定義)               | 完全對應 |
| `.github/skills/*/SKILL.md`                | `.claude/skills/name/SKILL.md`                               | 完全對應 |
| `.github/prompts/*.prompt.md`              | `.claude/skills/` (user-invocable skill)                     | 完全對應 |
| `tools: createOrEditFiles`                 | `tools: [Edit, Write]`                                       | 完全對應 |
| `tools: readFile`                          | `tools: [Read, Glob, Grep]`                                  | 完全對應 |
| `tools: runInTerminal`                     | `tools: [Bash]`                                              | 完全對應 |
| `tools: agent` (呼叫子 Agent)              | `tools: [Agent]` (spawn subagent)                            | 完全對應 |
| Copilot `description` 觸發                 | Claude `description` 自動委派                                | 完全對應 |

---

## 二、架構方案：推薦使用 Subagents 模式

```plaintext
使用者輸入：「建立 OrderService」
         │
         ▼
┌──────────────────────────────────────────────┐
│  主對話 Context（Orchestrator Skill 在此執行） │
│  /dotnet-testing（user-invocable, 無 fork）   │
│                                              │
│  Agent tool ──► Analyzer Subagent ──► 回傳 JSON
│       │
│       ▼ （將 Analyzer JSON 明確帶入 prompt）
│  Agent tool ──► Writer Subagent   ──► 回傳檔案路徑
│       │
│       ▼ （將檔案路徑帶入 prompt）
│  Agent tool ──► Executor Subagent ──► 回傳測試結果
│       │
│       ▼ （將測試檔案路徑帶入 prompt）
│  Agent tool ──► Reviewer Subagent ──► 回傳品質報告
│                                              │
│  彙整四階段結果，輸出給使用者                   │
└──────────────────────────────────────────────┘
```

### 關鍵架構限制：Subagent 不可巢狀

> **Claude Code 的 Subagent 不支援巢狀呼叫**。若 Skill 使用 `context: fork`，Skill 本身會成為
> Subagent，其內部將**無法再使用 Agent tool 產生子 Subagent**，導致四階段串接失敗。
>
> 因此 Orchestrator Skill **必須不使用 `context: fork`**，讓它在主對話 context 中執行，
> 保留 Agent tool 的使用權限來呼叫 Analyzer、Writer、Executor、Reviewer。

### 為什麼選 Subagents 而非 Agent Teams

- Agent Teams 仍為**實驗性功能**，穩定性不足
- Subagents 可透過 Agent tool **順序或平行呼叫**，完美對應現有的四階段串接流程
- 每個 Subagent 可限制 `tools`、指定 `model`、設定 `maxTurns`
- 成本較低（共用主 session context，不需每人獨立 context window）

### 多目標平行處理能力

對應 GitHub Copilot 情境 11（六目標同時處理），Claude Code 的 Agent tool 支援在單一回應中發出多個平行呼叫：

```plaintext
階段 1：平行分析（6 個 Agent tool 同時呼叫）
  ├── Agent → Analyzer("EmployeeService")      ──┐
  ├── Agent → Analyzer("EmployeeValidator")     ──┤
  ├── Agent → Analyzer("OrderValidator")        ──┼── 全部平行
  ├── Agent → Analyzer("LegacyReportGenerator") ──┤
  ├── Agent → Analyzer("TemperatureConverter")  ──┤
  └── Agent → Analyzer("SubscriptionService")   ──┘

階段 2：平行撰寫（收到所有分析結果後）
  ├── Agent → Writer(分析結果1) ──┐
  ├── Agent → Writer(分析結果2) ──┼── 全部平行
  └── ...                        ──┘

階段 3：循序執行（共用 build，必須循序）
  └── Agent → Executor(所有測試檔案)  ── 單一呼叫

階段 4：平行審查
  ├── Agent → Reviewer(測試檔案1) ──┐
  └── ...                          ──┼── 全部平行
```

---

## 三、目錄結構規劃

```plaintext
.claude/
├── CLAUDE.md                              # 專案層級指令（等同 copilot-instructions.md）
├── settings.local.json                    # 已存在
├── rules/                                 # 路徑條件規則
│   └── dotnet-testing.md                  # 當讀取 *.Tests/ 時載入的規則
│
├── agents/                                # Subagent 定義（Claude Code 原生格式）
│   ├── dotnet-testing-analyzer/
│   │   └── AGENT.md                       # Analyzer subagent
│   ├── dotnet-testing-writer/
│   │   └── AGENT.md                       # Writer subagent
│   ├── dotnet-testing-executor/
│   │   └── AGENT.md                       # Executor subagent
│   ├── dotnet-testing-reviewer/
│   │   └── AGENT.md                       # Reviewer subagent
│   │
│   ├── dotnet-testing-advanced-tunit-analyzer/
│   │   └── AGENT.md
│   ├── dotnet-testing-advanced-tunit-writer/
│   │   └── AGENT.md
│   ├── dotnet-testing-advanced-tunit-executor/
│   │   └── AGENT.md
│   ├── dotnet-testing-advanced-tunit-reviewer/
│   │   └── AGENT.md
│   │
│   ├── dotnet-testing-advanced-integration-analyzer/
│   │   └── AGENT.md
│   ├── dotnet-testing-advanced-integration-writer/
│   │   └── AGENT.md
│   ├── dotnet-testing-advanced-integration-executor/
│   │   └── AGENT.md
│   ├── dotnet-testing-advanced-integration-reviewer/
│   │   └── AGENT.md
│   │
│   ├── dotnet-testing-advanced-aspire-analyzer/
│   │   └── AGENT.md
│   ├── dotnet-testing-advanced-aspire-writer/
│   │   └── AGENT.md
│   ├── dotnet-testing-advanced-aspire-executor/
│   │   └── AGENT.md
│   └── dotnet-testing-advanced-aspire-reviewer/
│       └── AGENT.md
│
├── skills/                                # Skills（知識庫 + 入口指令）
│   │
│   ├── dotnet-testing/                    # ★ 主入口 Orchestrator Skill
│   │   └── SKILL.md                       # user-invocable, 無 context: fork
│   ├── dotnet-testing-advanced-tunit/
│   │   └── SKILL.md                       # TUnit Orchestrator
│   ├── dotnet-testing-advanced-integration/
│   │   └── SKILL.md                       # Integration Orchestrator
│   ├── dotnet-testing-advanced-aspire/
│   │   └── SKILL.md                       # Aspire Orchestrator
│   │
│   ├── dotnet-testing-fundamentals/       # ← 搬移自 .github/skills/
│   │   ├── SKILL.md                       #    user-invocable: false
│   │   ├── references/                    #    參考文件（原封搬移）
│   │   └── templates/                     #    程式碼模板（原封搬移）
│   ├── dotnet-testing-naming-conventions/
│   │   └── SKILL.md
│   ├── dotnet-testing-xunit-setup/
│   │   └── SKILL.md
│   ├── dotnet-testing-awesome-assertions/
│   │   └── SKILL.md
│   │  ... (其餘 30 個知識型 Skills，含各自的 references/ 和 templates/)
│   │
│   │  # ── 以下為 Prompts 遷移（16 個 .github/prompts/ → user-invocable Skills）
│   ├── dotnet-testing-prompt-fundamentals/
│   │   └── SKILL.md                       # user-invocable: true（對應原 prompt）
│   ├── dotnet-testing-prompt-assertions/
│   │   └── SKILL.md
│   │  ... (其餘 14 個 Prompt Skills)
│   │
│   └── dotnet-testing-aspire-testing/
│       └── SKILL.md
```

---

## 四、各元件的 Claude Code 實作對照

### 4.1 Orchestrator → Skill（user-invocable）

GitHub Copilot 的 Orchestrator 是 `.agent.md`，在 Claude Code 中**最適合的入口是 Skill**：

```yaml
# .claude/skills/dotnet-testing/SKILL.md
---
name: dotnet-testing
description: >
  .NET 單元測試 Orchestrator。當使用者要求建立測試、產生測試、
  或提到任何 .NET 類別需要測試時，使用此 skill。
  Use proactively when user mentions creating tests for .NET code.
user-invocable: true
# ⚠️ 不可使用 context: fork — 否則 Skill 成為 Subagent，無法再呼叫 Agent tool
allowed-tools:
  - Agent               # 可呼叫子 Agent（核心能力）
  - Read                # 讀取使用者指定的目標檔案
  - Glob                # 尋找檔案
  - Grep                # 搜尋程式碼
---

## 你是 .NET 單元測試 Orchestrator

### 工作流程（嚴格四階段）

**階段 1：分析（Analyzer）**
使用 Agent tool 呼叫 `dotnet-testing-analyzer` subagent，傳入目標類別資訊。
等待回傳結構化 JSON 分析報告。

**階段 2：撰寫（Writer）**
將 Analyzer 的 JSON 分析結果傳給 `dotnet-testing-writer` subagent。
Writer 會依據 requiredTechniques 自行讀取對應的 SKILL.md。

**階段 3：執行（Executor）**
將 Writer 產生的測試檔案路徑傳給 `dotnet-testing-executor` subagent。
Executor 負責 build + test，最多 3 輪修正。

**階段 4：審查（Reviewer）**
將最終測試檔案路徑傳給 `dotnet-testing-reviewer` subagent。
Reviewer 產出品質報告。

### 硬性禁止
- ❌ 不可自行讀取 SKILL.md（Writer 的職責）
- ❌ 不可自行撰寫測試程式碼
- ❌ 不可修改 .csproj
- ❌ 不可執行 dotnet build/test
```

### 4.2 Subagent 定義（AGENT.md）

#### Analyzer

```yaml
# .claude/agents/dotnet-testing-analyzer/AGENT.md
---
name: dotnet-testing-analyzer
description: >
  分析 .NET 原始碼結構，產出結構化 JSON 分析報告。
  包含類別依賴、方法簽章、測試技術建議、建議測試案例。
tools:
  - Read
  - Glob
  - Grep
  - Bash                # 需要執行 dotnet list package
model: sonnet           # Analyzer 不需要 Opus，Sonnet 足夠
maxTurns: 20
---

## 你是 .NET 測試分析器

（搬移自 dotnet-testing-analyzer.agent.md 的完整內容）
```

#### Writer

```yaml
# .claude/agents/dotnet-testing-writer/AGENT.md
---
name: dotnet-testing-writer
description: >
  依據分析報告撰寫 .NET 測試程式碼。
  會讀取 requiredTechniques 對應的 SKILL.md 檔案作為撰寫依據。
tools:
  - Read
  - Glob
  - Grep
  - Edit
  - Write
  - Bash                # dotnet list package --outdated
model: opus             # Writer 需要最強的程式碼產生能力
maxTurns: 30
skills:
  - dotnet-testing-fundamentals    # 預載基礎知識
  - dotnet-testing-naming-conventions
---

## 你是 .NET 測試撰寫器

（搬移自 dotnet-testing-writer.agent.md 的完整內容）
```

#### Executor

```yaml
# .claude/agents/dotnet-testing-executor/AGENT.md
---
name: dotnet-testing-executor
description: >
  建置並執行 .NET 測試，分析失敗原因並修正，最多 3 輪迭代。
tools:
  - Bash                # dotnet build, dotnet test
  - Read
  - Edit                # 修正測試程式碼
  - Grep
model: sonnet
maxTurns: 25
---

## 你是 .NET 測試執行器

（搬移自 dotnet-testing-executor.agent.md 的完整內容）
```

#### Reviewer

```yaml
# .claude/agents/dotnet-testing-reviewer/AGENT.md
---
name: dotnet-testing-reviewer
description: >
  審查測試程式碼品質，產出結構化品質報告（A+ ~ D 評分）。
tools:
  - Read
  - Glob
  - Grep
  # 注意：沒有 Edit/Write/Bash，Reviewer 不可修改程式碼
model: sonnet
maxTurns: 15
skills:
  - dotnet-testing-naming-conventions
  - dotnet-testing-awesome-assertions
  - dotnet-testing-fundamentals
---

## 你是 .NET 測試審查員

（搬移自 dotnet-testing-reviewer.agent.md 的完整內容）
```

### 4.3 Knowledge Skills（知識型，非 user-invocable）

```yaml
# .claude/skills/dotnet-testing-fundamentals/SKILL.md
---
name: dotnet-testing-fundamentals
description: >
  .NET 單元測試基礎知識：AAA 模式、測試結構、xUnit 設定。
  當撰寫或審查 .NET 測試時自動載入。
user-invocable: false    # 只被 Subagent 使用，不暴露為 slash command
allowed-tools:
  - Read
---

（搬移自 .github/skills/dotnet-testing-fundamentals/SKILL.md 的內容）
```

---

## 五、執行流程示意

使用者輸入：`建立 OrderService 的單元測試`

```plaintext
1. Claude Code 辨識意圖 → 自動觸發 /dotnet-testing skill
   （或使用者手動輸入 /dotnet-testing OrderService）

2. Skill 在主對話 context 中啟動（無 fork）
   ├── 讀取使用者指定的 OrderService.cs
   ├── 確認目標類別位置與專案結構

3. 呼叫 Agent tool → dotnet-testing-analyzer
   │  Input: "分析 OrderService.cs，產出結構化 JSON"
   │  Analyzer:
   │    ├── Read OrderService.cs, IOrderRepository.cs, Order.cs...
   │    ├── Grep 找出所有依賴介面
   │    ├── Bash: dotnet list package
   │    └── 回傳: { className, dependencies, methodsToTest,
   │              requiredTechniques, suggestedTestScenarios... }

4. 呼叫 Agent tool → dotnet-testing-writer
   │  Input: Analyzer 的完整 JSON + "依此撰寫測試"
   │  Writer:
   │    ├── Read 對應的 SKILL.md（依 requiredTechniques）
   │    ├── Bash: dotnet list package --outdated
   │    ├── Write/Edit 測試檔案
   │    └── 回傳: { testFilePath, packagesAdded, summary }

5. 呼叫 Agent tool → dotnet-testing-executor
   │  Input: Writer 的結果 + 測試檔案路徑
   │  Executor:
   │    ├── Bash: dotnet build
   │    ├── Bash: dotnet test
   │    ├── 若失敗 → Edit 修正 → 重新 build/test（最多 3 輪）
   │    └── 回傳: { buildStatus, testResults, modifications }

6. 呼叫 Agent tool → dotnet-testing-reviewer
   │  Input: 最終測試檔案路徑 + Executor 結果
   │  Reviewer:
   │    ├── Read 測試檔案
   │    ├── 對照 naming/assertions/fundamentals Skills 審查
   │    └── 回傳: { overallScore, issues[], positives[] }

7. Orchestrator 彙整四階段結果，輸出給使用者
```

---

## 六、關鍵技術差異與對策

| 議題 | GitHub Copilot 做法 | Claude Code 對策 |
| --- | --- | --- |
| Subagent 呼叫 | `tools: [{ type: agent }]` 直接呼叫指定 agent | Agent tool 的 `subagent_type` 或由 Claude 依 description 自動選擇 |
| Skill 讀取 | Writer 用 `readFile` 讀 `.github/skills/*/SKILL.md` | AGENT.md 的 `skills:` 欄位預載，或 Writer 用 Read tool 讀 `.claude/skills/*/SKILL.md` |
| Context 傳遞 | Agent 之間透過 Copilot 的 context 自動傳遞 | Orchestrator Skill 在呼叫 Agent tool 時透過 `prompt` 參數明確傳遞前一階段的結果 |
| Tool 限制 | `tools:` 白名單（技術強制） | Subagent AGENT.md 的 `tools:` 為技術強制；Orchestrator Skill 的 `allowed-tools` 為軟限制（見下方說明） |
| Orchestrator 限制 | Orchestrator 的 tools 白名單技術強制 | Orchestrator Skill 在主對話 context 執行，`allowed-tools` 依賴 prompt 工程而非技術強制 |
| 多 Orchestrator 共存 | 4 個獨立 .agent.md | 4 個獨立 user-invocable Skills（`/dotnet-testing`、`/dotnet-testing-advanced-tunit` 等） |
| Tier 選擇 UX | 下拉選單選擇 Orchestrator | 使用者輸入對應的 slash command，或由 description 自動路由至正確的 Orchestrator Skill |
| 大量 Skill 檔案 | `.github/skills/` 34 個 | `.claude/skills/` 搬移，知識型設 `user-invocable: false`，含 `references/` 和 `templates/` 子目錄 |
| Prompts 遷移 | `.github/prompts/` 16 個 | 轉為 `.claude/skills/` 中的 user-invocable Skills，每個 prompt 成為獨立的 Skill |

---

## 七、Hooks 的品質閘道應用（可選）

```json
// .claude/settings.json
{
  "hooks": {
    "PostToolUse": [
      {
        "matcher": { "toolName": "^Edit$" },
        "hooks": [
          {
            "type": "command",
            "command": "echo 'File modified: check test naming convention'"
          }
        ]
      }
    ]
  }
}
```

可用於：

- **PreToolUse** — 攔截 Executor 修改 production code（只允許改 `*.Tests/` 下的檔案）
- **PostToolUse** — 在 Write/Edit 後自動驗證命名規範
- **Stop** — 在 Reviewer 完成後觸發通知

---

## 八、實作優先順序建議

| 順序 | 項目                          | 範圍                                  | 原因                       |
| ---- | ----------------------------- | ------------------------------------- | -------------------------- |
| 1    | 搬移 34 個 Knowledge Skills   | `.github/skills/` → `.claude/skills/` | 基礎建設，所有 Agent 依賴  |
| 2    | 建立 4 個 Analyzer AGENT.md   | 最簡單的 subagent，唯讀               | 驗證 Agent 呼叫機制        |
| 3    | 建立 4 個 Writer AGENT.md     | 核心產出能力                          | 搭配 Skills 驗證完整讀取鏈 |
| 4    | 建立 4 個 Executor AGENT.md   | 建置/執行/修正迴圈                    | 驗證 Bash tool 限制與迭代  |
| 5    | 建立 4 個 Reviewer AGENT.md   | 品質審查                              | 驗證唯讀限制               |
| 6    | 建立 4 個 Orchestrator Skills | 入口 Skill，串接四階段                | 整合測試完整流程           |
| 7    | CLAUDE.md + Rules             | 專案指令                              | 補充全域規範               |
| 8    | Hooks（可選）                 | 品質閘道                              | 進階防護                   |

---

## 九、與 GitHub Copilot 體驗差異對照

| 面向 | GitHub Copilot | Claude Code | 差異評估 |
| --- | --- | --- | --- |
| **觸發方式** | 下拉選 Agent + 一句話 | `/dotnet-testing` + 一句話，或 description 自動觸發 | 幾乎相同 |
| **檔案引用** | `#file:path/to/file.cs` | 直接在 prompt 中寫路徑，Subagent 自行 Read | 略不同但功能等價 |
| **Context 傳遞** | Copilot 自動在 Agent 間傳遞 | Orchestrator 必須在 `prompt` 參數中明確帶入前一階段結果 | 需更精確的 prompt 工程 |
| **Tool 限制** | `tools:` 白名單（粗粒度） | `tools:` 白名單，可精確到每個 tool name | Claude Code 更好 |
| **Skill 載入** | Writer 用 readFile 讀 SKILL.md | `skills:` 預載 + Read tool 動態讀取 | Claude Code 更靈活 |
| **Model 選擇** | 統一使用 Copilot 模型 | 每個 Subagent 可指定 sonnet/opus/haiku | Claude Code 更好 |
| **平行處理** | Analyzer/Writer/Reviewer 可平行 | Agent tool 支援同一回應多個平行呼叫 | 相同 |
| **循序限制** | Executor 必須循序（共用 build） | 相同限制 | 相同 |

---

## 十、結論

Claude Code 提供的 **Subagents + Skills** 機制與 GitHub Copilot 的 **Agent + Skills** 幾乎是 1:1 對應。主要差異在於：

1. **Context 傳遞需要明確化** — Claude Code 的 Subagent 回傳結果給 Orchestrator 後，需要 Orchestrator 在呼叫下一個 Subagent 時明確把前一階段結果寫入 prompt
2. **Skill 預載機制更強** — AGENT.md 的 `skills:` 欄位可以讓 Subagent 啟動時就預載知識，不需要像 Copilot 那樣每次 readFile
3. **Tool 限制更細緻** — 可以精確到每個 tool name，比 Copilot 的粗粒度更好

整體而言，**技術上完全可行**，且 Claude Code 的 Subagent 架構在某些方面（skills 預載、model 選擇、maxTurns 限制）比 GitHub Copilot 的 Agent 系統更靈活。
