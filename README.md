<p align="center">
  <img width="16%" align="center" src="img/Logo.png" alt="logo">
</p>
  <h1 align="center">
  RandPicker - 随机点名器
</h1>
<p align="center">
 基于WPF和Material Design的现代化随机点名器应用。
</p>
<p align="center">
  <small>当前版本：WPF版（C#） | <a href="#pyqt-legacy">PyQt版已废弃</a></small>
</p>
<div align="center">

[![星标](https://img.shields.io/github/stars/xuanxuan1231/RandPicker?style=for-the-badge&color=orange&label=星标)](https://github.com/xuanxuan1231/RandPicker)
[![开源许可](https://img.shields.io/badge/license-GPLv3-blue.svg?label=开源许可证&style=for-the-badge)](https://github.com/xuanxuan1231/RandPicker?tab=GPL-3.0-1-ov-file)


![Alt](https://repobeats.axiom.co/api/embed/ff60ad27c90fd6c3b3cd25ec6b25816277fcd45e.svg "Repobeats analytics image")

</div>

## 功能
- 基础抽选
- 抽选权重
- 分组抽选
- 大头照
- 从 Excel 和 CSV 文件导入
- 自定义缩放
- 历史记录
- 抽选结果导出（支持Excel、CSV、JSON、PDF格式）

## 下一步
- 增加更多的导入文件格式支持，如 JSON 和 TXT
- 优化 UI 设计，提升用户体验
- 用户账户系统

## 导出功能使用说明
在设置界面的"历史记录"页面中，您可以通过以下方式导出抽选结果：
- **导出Excel**: 生成.xlsx格式的Excel文件，包含格式化的表格和样式
- **导出CSV**: 生成.csv格式的逗号分隔文件，便于数据分析
- **导出JSON**: 生成.json格式的结构化数据文件，包含完整的元数据
- **导出PDF**: 生成.pdf格式的报告文件，适合打印和分享
- **全部导出**: 一次性导出所有格式的文件

所有导出的文件将保存在项目根目录下的`exports`文件夹中。

## 协议
此项目 (RandPicker) 基于 GPL-3.0 许可证授权发布，详情请参阅 [LICENSE](LICENSE) 文件。

Copyright © 2025 Wenxuan Shen & Jerry Wu.

## 致谢
### 技术栈（WPF版）
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Material Design in XAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/windows/communitytoolkit/mvvm/)
- [EPPlus](https://github.com/JanKallman/EPPlus) (Excel处理)

### 技术栈（PyQt版 - 已废弃）
- [PyQt6](https://pypi.org/project/PyQt6)  
- [PyQt6-Fluent-Widgets](https://pypi.org/project/PyQt6-Fluent-Widgets)  
- [loguru](https://pypi.org/project/loguru)  
- [pandas](https://pypi.org/project/pandas)  

### 本项目受到以下项目的启发而开发
- [Class-Widgets](https://github.com/Class-Widgets/Class-Widgets)
