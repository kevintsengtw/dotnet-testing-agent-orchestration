# 單元測試 Orchestrator 操作指南

> **Orchestrator**：`dotnet-testing-orchestrator`
> **驗證專案**：`samples/practice/`（.NET 9.0 基線版本，使用 `Practice.Samples.slnx`）

---

## 可驗證版本

本指南以 **.NET 9.0** 為基線版本。三個版本的驗證專案結構完全相同，可自由選擇：

| 版本         | .slnx                         | 來源專案路徑                        |
| ------------ | ----------------------------- | ----------------------------------- |
| **.NET 9.0** | `Practice.Samples.slnx`       | `practice/src/Practice.Core/`       |
| .NET 8.0     | `Practice.Samples.Net8.slnx`  | `practice/src/Practice.Core.Net8/`  |
| .NET 10.0    | `Practice.Samples.Net10.slnx` | `practice/src/Practice.Core.Net10/` |

> 驗證其他版本時，將情境中的檔案路徑替換為對應版本的專案路徑即可。例如：
> `#file:practice/src/Practice.Core.Net8/Services/WeatherAlertService.cs`

### 還原驗證結果

```powershell
git restore samples/practice/tests/
git clean -fd samples/practice/tests/
```

---

## 前置準備

| 項目             | 說明                                       |
| ---------------- | ------------------------------------------ |
| **VS Code**      | 1.109 以上，已安裝 GitHub Copilot Chat     |
| **VS Code 設定** | `chat.customAgentInSubagent.enabled: true` |
| **.NET SDK**     | .NET 9.0 SDK                               |

> Docker Desktop **不需要**。

## 操作步驟

1. 在 Copilot Chat 切換到 **Agent** 模式
2. 從 Agent 下拉選單選擇 `dotnet-testing-orchestrator`
3. 在聊天輸入框中，使用 `#file:` 引用目標檔案，或直接描述測試目標

## 預期結果

測試程式碼產生在 `samples/practice/tests/Practice.Core.Tests/` 目錄中。

---

## 驗證情境

### 情境 1：基礎純函式測試

```plaintext
#file:practice/src/Practice.Core/TemperatureConverter.cs

為 TemperatureConverter 類別寫單元測試
```

> 驗證 Orchestrator 對無依賴純函式的基本完整流程：Analyzer 分析 → Writer 撰寫 → Executor 執行 → Reviewer 審查。

**觀察重點**：

- **Analyzer**：是否識別為無依賴的純函式類別
- **Writer**：是否使用 `[Theory]` + `[InlineData]` 覆蓋多種輸入場景
- **Executor**：`dotnet test` 是否通過
- **Reviewer**：邊界值測試覆蓋是否完整

---

### 情境 2：Mock 依賴的服務測試

```plaintext
#file:practice/src/Practice.Core/Services/WeatherAlertService.cs

為 WeatherAlertService 寫單元測試
```

> 驗證 Analyzer 是否識別 `IWeatherService` 和 `INotificationService` 兩個外部依賴，Writer 是否載入 `nsubstitute-mocking` Skill。

**觀察重點**：

- **Analyzer**：是否識別出 `IWeatherService`、`INotificationService` 依賴
- **Writer**：是否使用 `Substitute.For<T>()` 建立 Mock，搭配 `Received()` 驗證呼叫
- **Reviewer**：正向 / 反向 / 例外場景是否完整

---

### 情境 3：AutoFixture 搭配 Bogus 測試資料

```plaintext
#file:practice/src/Practice.Core/Services/EmployeeService.cs

為 EmployeeService 寫單元測試
```

> 驗證 Writer 是否載入 `autofixture-basics` 和 `bogus-fake-data` Skills，使用自動產生的測試資料。

**觀察重點**：

- **Analyzer**：是否識別 `Employee` 模型的複雜屬性（循環參考 `Department`）
- **Writer**：是否使用 AutoFixture 搭配 Bogus 產生 `Employee` 物件，避免手動建立
- **Reviewer**：測試資料策略是否合理

---

### 情境 4：TimeProvider 時間依賴測試

```plaintext
#file:practice/src/Practice.Core/Services/SubscriptionService.cs

為 SubscriptionService 寫單元測試
```

> 驗證 Analyzer 是否偵測到 `TimeProvider` 依賴，Writer 是否載入 `datetime-testing-timeprovider` Skill 使用 `FakeTimeProvider`。

**觀察重點**：

- **Analyzer**：是否識別 `TimeProvider` 依賴
- **Writer**：是否使用 `FakeTimeProvider` 控制時間，驗證過期、剩餘天數等計算
- **Reviewer**：時間邊界測試（剛好到期、過期一天）是否涵蓋

---

### 情境 5：FileSystem 抽象化測試

```plaintext
#file:practice/src/Practice.Core/Services/ConfigurationLoader.cs

為 ConfigurationLoader 寫單元測試
```

> 驗證 Analyzer 是否偵測到 `IFileSystem` 依賴，Writer 是否載入 `filesystem-testing-abstractions` Skill 使用 `MockFileSystem`。

**觀察重點**：

- **Analyzer**：是否識別 `IFileSystem` 依賴
- **Writer**：是否使用 `MockFileSystem` 模擬檔案系統操作
- **Reviewer**：檔案存在 / 不存在 / 格式錯誤等場景是否涵蓋

---

### 情境 6：FluentValidation 驗證器測試

```plaintext
#file:practice/src/Practice.Core/Validators/OrderValidator.cs

為 OrderValidator 寫單元測試
```

> 驗證 Analyzer 是否偵測到 `AbstractValidator<T>` 基底類別，Writer 是否載入 `fluentvalidation-testing` Skill。

**觀察重點**：

- **Analyzer**：是否識別為 FluentValidation Validator
- **Writer**：是否使用 `TestValidate()` + `ShouldHaveValidationErrorFor()` 語法
- **Reviewer**：正向 / 反向驗證覆蓋是否完整

---

### 情境 7：FluentValidation 驗證器測試（第二個目標）

```plaintext
#file:practice/src/Practice.Core/Validators/EmployeeValidator.cs

為 EmployeeValidator 寫單元測試
```

> 驗證對不同 Validator 目標的泛化能力，確認 Writer 能正確處理含 `TimeProvider` 依賴的 Validator。

**觀察重點**：

- **Analyzer**：是否識別 `EmployeeValidator` 繼承 `AbstractValidator<Employee>` 並注入 `TimeProvider`
- **Writer**：是否正確建構含 `FakeTimeProvider` 的 Validator 實例
- **Reviewer**：各規則（基本欄位、Email、數值範圍）驗證是否齊全

---

### 情境 8：跨技能整合（多依賴 + 時間 + 資料）

```plaintext
#file:practice/src/Practice.Core/Services/OrderProcessingService.cs

為 OrderProcessingService 寫單元測試
```

> 驗證 Analyzer 能否同時識別四個依賴（`IOrderRepository`、`IPaymentGateway`、`IEmailService`、`TimeProvider`），Writer 是否整合多個 Skills。

**觀察重點**：

- **Analyzer**：是否識別出所有四個依賴項
- **Writer**：是否同時使用 NSubstitute（Mock 依賴）、AutoFixture（測試資料）、FakeTimeProvider（時間控制）
- **Reviewer**：營業時間內外、付款成功 / 失敗等場景是否涵蓋

---

### 情境 9：遺留程式碼識別

```plaintext
#file:practice/src/Practice.Core/Legacy/LegacyReportGenerator.cs

為 LegacyReportGenerator 寫單元測試
```

> 驗證 Analyzer 是否識別遺留程式碼的測試性問題（靜態依賴、`DateTime.Now`、直接檔案操作），Writer 是否載入 `private-internal-testing` Skill。

**觀察重點**：

- **Analyzer**：是否識別靜態方法依賴（`Database.GetUser`）、`DateTime.Now`、`File.WriteAllText`
- **Writer**：是否建議重構方向，或使用可用的測試策略
- **Reviewer**：是否指出無法完全測試的原因與重構建議

---

### 情境 10：重構後的可測試服務

```plaintext
#file:practice/src/Practice.Core/Services/ReportGenerator.cs

為 ReportGenerator 寫單元測試
```

> 驗證 Analyzer 是否識別到重構後的版本使用介面注入（`IUserRepository`、`ITransactionRepository`、`TimeProvider`、`IReportWriter`），Writer 是否正確 Mock 所有依賴。

**觀察重點**：

- **Analyzer**：是否識別出四個可注入依賴
- **Writer**：是否 Mock 所有介面依賴，使用 `FakeTimeProvider` 控制時間
- **Reviewer**：與 `LegacyReportGenerator` 相比，測試覆蓋率是否顯著提升

---

### 情境 11：多目標平行處理

以下為 **單次對話** 的完整輸入內容：

```plaintext
#file:practice/src/Practice.Core/Services/EmployeeService.cs
#file:practice/src/Practice.Core/Validators/EmployeeValidator.cs
#file:practice/src/Practice.Core/Validators/OrderValidator.cs
#file:practice/src/Practice.Core/Legacy/LegacyReportGenerator.cs

幫 TemperatureConverter 和 SubscriptionService 寫測試
```

> 一次涵蓋六個目標（四個 `#file:` + 兩個名稱描述），驗證 Analyzer 與 Writer 是否能同時平行處理多個不同類型的目標（Service / Validator / Legacy）。

**觀察重點**：

- **Analyzer**：是否正確識別六個目標各自的類型（純函式 / Mock 依賴 / AutoFixture / TimeProvider / FluentValidation / Legacy）
- **Writer**：是否針對每個目標動態載入對應的 Skills 組合
- **Executor**：所有測試是否一次通過
- **Reviewer**：是否對每個目標分別給出品質評分
