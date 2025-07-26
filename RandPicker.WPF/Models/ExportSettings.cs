using System;

namespace RandPicker.WPF.Models
{
    /// <summary>
    /// 导出设置模型类
    /// </summary>
    public class ExportSettings
    {
        // 导出路径
        public string ExportPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        
        // 导出格式
        public ExportFormat Format { get; set; } = ExportFormat.Excel;
        
        // 是否包含标题行
        public bool IncludeHeader { get; set; } = true;
        
        // 是否包含时间戳
        public bool IncludeTimestamp { get; set; } = true;
        
        // 导出文件名前缀
        public string FileNamePrefix { get; set; } = "抽选结果";
    }
    
    /// <summary>
    /// 导出格式枚举
    /// </summary>
    public enum ExportFormat
    {
        Excel,
        CSV,
        PDF,
        JSON,
        All
    }
}