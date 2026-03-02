---
agent: 'agent'
description: 'ASP.NET Core 與 Web API 整合測試完整指南（WebApplicationFactory、HttpClient 測試）'
---

## 重要指示

**在執行以下任務之前，你必須先讀取並完整理解以下技能文件的全部內容：**

**必讀資源**：

1. `.github/skills/dotnet-testing-advanced-aspnet-integration-testing/SKILL.md` - ASP.NET 整合測試
2. `.github/skills/dotnet-testing-advanced-webapi-integration-testing/SKILL.md` - Web API 整合測試

這些檔案包含：

- WebApplicationFactory<T> 使用方式
- 自訂 WebApplicationFactory 設定
- HttpClient 測試模式
- 服務替換與 Mock 注入
- 資料庫測試策略（In-Memory、實際資料庫）
- 認證與授權測試
- API 端點測試最佳實踐

**你的任務流程：**

1. **必須首先**讀取所有 SKILL.md 檔案的完整內容
2. 理解整合測試與單元測試的差異
3. 根據使用者需求選擇適當的測試策略
4. 確保生成的程式碼遵循最佳實踐

---

## 使用者輸入

請提供以下資訊：

**應用程式類型**：${input:appType:選擇類型 - webapi（Web API）/ mvc（MVC）/ minimal（Minimal API）}

**要測試的端點或功能**：${input:endpoint:要測試的 API 端點或功能描述，例如 POST /api/products}

**需要 Mock 的服務**（可選）：${input:mockServices:需要替換的服務名稱，以逗號分隔}

**資料庫策略**（可選）：${input:dbStrategy:選擇 inmemory / testcontainers / none}

---

## 預期輸出

根據以上資訊和 SKILL.md 中的所有範例，請生成：

1. **自訂 WebApplicationFactory**
   - 繼承 WebApplicationFactory<Program>
   - 設定測試環境
   - 服務替換配置

2. **整合測試類別**
   - 實作 IClassFixture<T>
   - HttpClient 初始化
   - 測試生命週期管理

3. **API 端點測試**
   - HTTP 請求建構
   - 回應驗證
   - JSON 序列化處理

4. **Mock 服務設定**（如需要）
   - 服務替換程式碼
   - Mock 行為設定

5. **測試資料管理**
   - 資料庫初始化
   - 測試資料清理
