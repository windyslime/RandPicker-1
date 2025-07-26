# RandPicker.WPF 运行指南

## 系统要求
- Windows 10/11 或 macOS (使用虚拟机或双系统)
- .NET 8.0 SDK 或更高版本
- Visual Studio 2022 或 Visual Studio Code (推荐)

## 安装步骤

### 1. 安装 .NET 8.0 SDK
从 [Microsoft官网](https://dotnet.microsoft.com/download/dotnet/8.0) 下载并安装 .NET 8.0 SDK。

### 2. 安装开发工具 (任选其一)

#### Visual Studio 2022 (推荐)
1. 下载并安装 [Visual Studio 2022](https://visualstudio.microsoft.com/)
2. 安装时选择".NET桌面开发"工作负载
3. 打开 `RandPicker.WPF.sln` 文件

#### Visual Studio Code
1. 安装 [Visual Studio Code](https://code.visualstudio.com/)
2. 安装 C# 扩展
3. 安装 .NET MAUI 扩展 (可选，用于更好的XAML支持)

### 3. 还原NuGet包
在项目根目录运行：
```bash
dotnet restore
```

### 4. 运行项目

#### 命令行方式
```bash
cd RandPicker.WPF
dotnet run
```

#### Visual Studio
1. 设置 `RandPicker.WPF` 为启动项目
2. 按 F5 或点击"开始调试"按钮

## 项目结构

```
RandPicker.WPF/
├── App.xaml              # 应用程序入口
├── Models/               # 数据模型
├── Services/             # 业务逻辑服务
├── ViewModels/           # MVVM视图模型
├── Views/                # 用户界面
└── Styles.xaml           # 样式定义
```

## 功能特性

- **学生管理**: 添加、编辑、删除学生信息
- **分组管理**: 创建和管理学生分组
- **随机选择**: 支持个人和分组两种选择模式
- **历史记录**: 记录所有选择历史
- **数据导出**: 支持PDF和Excel格式导出
- **主题切换**: 支持深色/浅色主题

## 技术栈

- **框架**: .NET 8.0 + WPF
- **UI库**: Material Design In XAML
- **MVVM**: CommunityToolkit.Mvvm
- **依赖注入**: Microsoft.Extensions.DependencyInjection
- **PDF生成**: PdfSharpCore
- **JSON处理**: Newtonsoft.Json

## 故障排除

### 编译错误
1. 确保已安装 .NET 8.0 SDK
2. 运行 `dotnet restore` 还原NuGet包
3. 检查Visual Studio是否安装了WPF工作负载

### 运行时错误
1. 检查是否有文件权限问题
2. 确保应用程序有写入本地数据的权限
3. 查看输出窗口的错误信息

## 开发说明

### 添加新功能
1. 在对应的Service接口中添加方法定义
2. 在Service实现中添加具体逻辑
3. 在ViewModel中添加相应命令和属性
4. 在View中添加UI元素

### 修改样式
编辑 `Styles.xaml` 文件来自定义应用程序外观。

## 发布

### Windows发布
```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

### 创建安装包
使用Visual Studio Installer项目或第三方工具如WiX Toolset创建安装程序。