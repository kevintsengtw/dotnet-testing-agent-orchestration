---
agent: 'agent'
description: '程式碼涵蓋率分析與改善策略，涵蓋覆蓋率工具、報告解讀與品質提升。'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：`.github/skills/dotnet-testing-code-coverage-analysis/SKILL.md`

此檔案包含：
- 程式碼覆蓋率的類型（行覆蓋、分支覆蓋、方法覆蓋）
- Coverlet 與 dotnet test 的整合
- 覆蓋率報告工具（ReportGenerator）
- CI/CD 中的覆蓋率整合
- 覆蓋率門檻設定
- 排除特定程式碼的策略
- 覆蓋率 vs 測試品質的權衡
- 改善覆蓋率的實務策略

**你的任務流程：**
1. **必須首先**讀取 SKILL.md 檔案的完整內容
2. 理解檔案中提供的所有覆蓋率分析模式和最佳實踐
3. 根據下方使用者輸入和 SKILL.md 中的範例，提供相應的指導
4. 確保提供的配置遵循 SKILL.md 中列出的所有模式和最佳實踐

---

## 使用者輸入

請提供以下資訊，我將根據 SKILL.md 的完整指南為您提供覆蓋率分析指導：

**任務類型**：${input:taskType:選擇：「設定覆蓋率收集」、「解讀覆蓋率報告」、「改善覆蓋率」、「CI/CD 整合」}

**專案類型**：${input:projectType:例如「ASP.NET Core API」、「類別庫」、「主控台應用程式」}

**目前覆蓋率狀況**（可選）：${input:currentCoverage:描述目前的覆蓋率數據或貼上報告摘要}

**目標覆蓋率**：${input:targetCoverage:例如「80% 行覆蓋率」、「70% 分支覆蓋率」}

**特殊需求**（可選）：${input:specialRequirements:例如排除特定檔案、自訂報告格式等}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請提供：

1. Coverlet 配置與設定
2. dotnet test 覆蓋率收集指令
3. ReportGenerator 報告產生配置
4. CI/CD 整合腳本（如需要）
5. 排除配置（ExcludeByAttribute、ExcludeByFile）
6. 覆蓋率改善建議
7. 必要的套件引用
8. 遵循 SKILL.md 提到的覆蓋率分析最佳實踐
