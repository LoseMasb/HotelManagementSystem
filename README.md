# ASP.NET Core MVC 酒店管理系统（.NET 8 + SQL Server）

一个完整可运行的酒店管理系统示例，包含三端角色：管理员（Admin）、酒店管理者（HotelManager）、客户端（Customer）。

## 技术栈

- ASP.NET Core MVC (.NET 8)
- ASP.NET Core Identity + 角色授权
- Entity Framework Core + SQL Server
- Bootstrap 5

## 主要功能

### 1) 客户端（Customer）
- 浏览酒店列表（支持搜索、分页）
- 查看酒店详情
- 查看房型库存与价格
- 按入住/离店日期查询可用房间余量
- 下单预订
- 查看我的订单

### 2) 酒店端（HotelManager）
- 维护自己的酒店信息
- 房型管理（名称、价格、库存、上/下架、增删改）
- 查看本酒店订单
- 更新订单状态（Pending/Confirmed/Cancelled/Completed）

### 3) 管理员端（Admin）
- 酒店管理（增删改查）
- 分配酒店管理员账号
- 启用/停用酒店

## 认证授权

- 使用 Identity + Roles
- 启动时自动种子初始化角色：
  - Admin
  - HotelManager
  - Customer
- 启动时自动创建默认账号：
  - `admin@hotel.local` / `Admin@123`
  - `manager@hotel.local` / `Manager@123`
  - `customer@hotel.local` / `Customer@123`

## 配置说明

### 连接字符串

`appsettings.Development.json`（开发环境）当前默认使用本机 SQL Server Express：

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=HotelManagementSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

`appsettings.json` 可作为通用配置，按需替换为你的 SQL Server 连接串。

## 本地运行

> 当前项目目标框架：`net8.0`

1. 恢复依赖
```bash
dotnet restore
```

2. 编译
```bash
dotnet build
```

3. 运行
```bash
dotnet run
```

首次启动会自动执行 `Migrate + Seed`（应用 EF Migration 并初始化默认角色/账号）。

## 数据迁移（正式）

本项目已包含初始迁移（`Migrations/`）。若你修改了模型，可执行：

```bash
dotnet tool restore
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## 项目结构

- `Controllers/`：客户端相关控制器（酒店浏览、预订、我的订单）
- `Areas/Admin/`：管理员后台
- `Areas/HotelManager/`：酒店管理端
- `Services/`：业务服务（库存/预订）
- `ViewModels/`：视图模型
- `Data/`：DbContext + 种子初始化
- `Models/`：领域模型

## 说明

- UI 为中文界面，Bootstrap 5 布局，包含友好提示、表单校验、导航分区。
- 如运行环境仅安装 .NET 10 Runtime，`dotnet run` 运行 `net8.0` 会失败；请安装 .NET 8 Runtime。
