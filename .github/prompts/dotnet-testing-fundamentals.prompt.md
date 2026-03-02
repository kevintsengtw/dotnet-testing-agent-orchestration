---
agent: 'agent'
description: '單元測試基礎完整指南（AAA 模式、命名規範、xUnit 專案設定）'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：

1. `.github/skills/dotnet-testing-unit-test-fundamentals/SKILL.md` - 單元測試基礎
2. `.github/skills/dotnet-testing-test-naming-conventions/SKILL.md` - 測試命名規範
3. `.github/skills/dotnet-testing-xunit-project-setup/SKILL.md` - xUnit 專案設定

這些檔案包含：

- AAA (Arrange-Act-Assert) 測試模式
- 測試命名規範與最佳實踐
- xUnit 專案結構與設定
- NuGet 套件相關設定
- Fact 與 Theory 屬性使用

**你的任務流程：**

1. **必須首先**讀取所有 SKILL.md 檔案的完整內容
2. 理解單元測試的基本概念與最佳實踐
3. 根據使用者需求選擇適當的內容
4. 確保生成的程式碼遵循所有最佳實踐

---

## 使用者輸入

請提供以下資訊：

**任務類型**：${input:taskType:選擇任務 - setup（專案設定）/ write（撰寫測試）/ naming（命名建議）}

**目標類別或方法名稱**：${input:targetName:要測試的類別或方法名稱}

**測試情境描述**（可選）：${input:scenario:描述要測試的情境或行為}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. **專案設定**（setup）
   - xUnit 專案結構建議
   - 必要的 NuGet 套件
   - csproj 設定範例

2. **測試撰寫**（write）
   - 遵循 AAA 模式的完整測試
   - 符合命名規範的測試方法名稱
   - 適當使用 [Fact] 或 [Theory]

3. **命名建議**（naming）
   - 根據情境建議測試方法名稱
   - 說明命名規範原則
