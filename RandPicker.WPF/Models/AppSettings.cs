using System;

namespace RandPicker.WPF.Models
{
    /// <summary>
    /// 应用程序设置模型类
    /// </summary>
    public class AppSettings
    {
        // 常规设置
        public string Theme { get; set; } = "Auto"; // Light, Dark, Auto
        public double Scale { get; set; } = 1.0;
        
        // UI设置
        public bool EdgeHide { get; set; } = true;
        public int EdgeDistance { get; set; } = 20;
        public int HiddenWidth { get; set; } = 10;
        public bool AvatarEnabled { get; set; } = true;
        public int AvatarSize { get; set; } = 64;
        public bool ElasticAnimation { get; set; } = true;
        
        // 颜色设置
        public string LightThemeColor { get; set; } = "#1E88E5"; // 浅色主题的主题色
        public string DarkThemeColor { get; set; } = "#90CAF9"; // 深色主题的主题色
        
        // 分组设置
        public bool UseGlobalGroup { get; set; } = true;
        public string EnabledGroups { get; set; } = string.Empty; // 逗号分隔的启用分组索引
        
        // 历史记录设置
        public bool RecordHistory { get; set; } = true;
        
        // 更新设置
        public int AppUpdateSource { get; set; } = 0; // 0: GitHub, 1: OSS
        public int UpdaterUpdateSource { get; set; } = 0; // 0: GitHub, 1: OSS
        
        // 上次位置
        public int LastPositionX { get; set; } = 0;
        public int LastPositionY { get; set; } = 0;
    }
}