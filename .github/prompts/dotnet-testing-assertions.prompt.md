---
agent: 'agent'
description: 'AwesomeAssertions 斷言與複雜物件比較完整指南'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：

1. `.github/skills/dotnet-testing-awesome-assertions-guide/SKILL.md` - AwesomeAssertions 斷言指南
2. `.github/skills/dotnet-testing-complex-object-comparison/SKILL.md` - 複雜物件比較

這些檔案包含：

- AwesomeAssertions 基本語法與鏈式斷言
- 集合斷言（Should().Contain(), Should().BeEquivalentTo()）
- 例外斷言（Should().Throw<T>()）
- 物件深度比較與等價性驗證
- 排除特定屬性的比較策略
- 自訂比較規則

**你的任務流程：**

1. **必須首先**讀取所有 SKILL.md 檔案的完整內容
2. 理解各種斷言類型的使用場景
3. 根據使用者需求選擇適當的斷言方法
4. 確保生成的程式碼具有清晰的失敗訊息

---

## 使用者輸入

請提供以下資訊：

**斷言類型**：${input:assertionType:選擇類型 - basic（基本斷言）/ collection（集合斷言）/ exception（例外斷言）/ object（物件比較）}

**要驗證的內容描述**：${input:verifyContent:描述要驗證的值或物件}

**預期結果**：${input:expected:描述預期的結果或行為}

**需要排除的屬性**（物件比較時可選）：${input:excludeProperties:需要排除比較的屬性名稱，以逗號分隔}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. **基本斷言**（basic）
   - 使用 Should().Be(), Should().NotBe() 等
   - 字串、數值、布林值的驗證

2. **集合斷言**（collection）
   - Should().Contain(), Should().HaveCount()
   - Should().BeEquivalentTo() 用於順序無關比較
   - Should().OnlyContain() 用於條件驗證

3. **例外斷言**（exception）
   - Should().Throw<TException>()
   - 驗證例外訊息與內部例外

4. **物件比較**（object）
   - Should().BeEquivalentTo() 深度比較
   - Excluding() 排除特定屬性
   - 自訂比較選項設定

5. **最佳實踐說明**
   - 斷言訊息的重要性
   - 避免過度斷言
   - 選擇正確的比較方法
