---
agent: 'agent'
description: 'NSubstitute 模擬與 AutoFixture 整合，涵蓋 Mock、Stub、Spy 的完整使用指南。'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：
- `.github/skills/dotnet-testing-nsubstitute-mocking/SKILL.md`
- `.github/skills/dotnet-testing-autofixture-nsubstitute-integration/SKILL.md`
- `.github/skills/dotnet-testing-autodata-xunit-integration/SKILL.md`

這些檔案包含：
- NSubstitute 核心概念（Substitute.For<T>）
- 方法回傳值設定（Returns、ReturnsForAnyArgs）
- 參數匹配（Arg.Is、Arg.Any）
- 呼叫驗證（Received、DidNotReceive）
- 例外拋出與回呼設定
- AutoFixture.AutoNSubstitute 整合
- AutoMock 容器使用
- AutoData 與 InlineAutoData 屬性用法
- xUnit Theory 資料驅動測試模式
- 常見陷阱與解決方案

**你的任務流程：**
1. **必須首先**讀取所有 SKILL.md 檔案的完整內容
2. 理解檔案中提供的所有 Mocking 模式和最佳實踐
3. 根據下方使用者輸入和 SKILL.md 中的範例，生成相應的測試程式碼
4. 確保生成的測試程式碼遵循 SKILL.md 中列出的所有模式和最佳實踐

---

## 使用者輸入

請提供以下資訊，我將根據 SKILL.md 的完整指南為您生成 NSubstitute 測試程式碼：

**要測試的類別**：${input:targetClass:輸入要測試的類別名稱，例如 OrderService、UserManager}

**需要 Mock 的依賴**：${input:dependencies:列出需要 Mock 的介面，例如 IOrderRepository、IEmailService、ILogger}

**Mock 行為設定**：${input:mockBehavior:描述 Mock 需要的行為，例如「GetById 回傳特定 Order」、「SendEmail 拋出例外」}

**驗證需求**：${input:verificationNeeds:描述需要驗證的呼叫，例如「確認 Save 被呼叫一次」、「確認 SendEmail 參數正確」}

**特殊需求**（可選）：${input:specialRequirements:例如 AutoFixture 整合、AutoMock 容器等}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. 完整的測試類別設定
2. Mock 物件建立與配置
3. 回傳值與行為設定
4. 參數匹配配置
5. 呼叫驗證邏輯
6. AutoFixture 整合（如需要）
7. 必要的 using 指令和套件引用
8. 遵循 SKILL.md 提到的 NSubstitute 最佳實踐
