using RandPicker.WPF.Models;
using System.Threading.Tasks;

namespace RandPicker.WPF.Services
{
    /// <summary>
    /// 配置服务接口
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// 初始化配置服务
        /// </summary>
        Task InitializeAsync();
        
        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>配置值</returns>
        T GetValue<T>(string key, T defaultValue = default!);
        
        /// <summary>
        /// 设置配置值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        void SetValue<T>(string key, T value);
        
        /// <summary>
        /// 保存配置
        /// </summary>
        void Save();
        
        /// <summary>
        /// 异步保存配置
        /// </summary>
        Task SaveAsync();
        
        /// <summary>
        /// 加载应用设置
        /// </summary>
        /// <returns>应用设置</returns>
        Task<AppSettings> LoadSettingsAsync();
        
        /// <summary>
        /// 保存应用设置
        /// </summary>
        /// <param name="settings">应用设置</param>
        Task SaveSettingsAsync(AppSettings settings);
        
        /// <summary>
        /// 应用主题
        /// </summary>
        void ApplyTheme(string theme);
        
        /// <summary>
        /// 应用主题颜色
        /// </summary>
        /// <param name="color">颜色</param>
        void ApplyThemeColor(string color);
        
        /// <summary>
        /// 获取当前是否为深色主题
        /// </summary>
        bool IsDarkTheme { get; }
        
        /// <summary>
        /// 主题
        /// </summary>
        string Theme { get; set; }
        
        /// <summary>
        /// 是否记录历史
        /// </summary>
        bool RecordHistory { get; set; }
        
        /// <summary>
        /// 导出路径
        /// </summary>
        string ExportPath { get; set; }
        
        /// <summary>
        /// 当前应用设置
        /// </summary>
        AppSettings CurrentSettings { get; }
        
        /// <summary>
        /// 获取应用数据路径
        /// </summary>
        string GetAppDataPath();
        
        /// <summary>
        /// 获取文件路径
        /// </summary>
        string GetFilePath(string fileName);
        
        /// <summary>
        /// 确保目录存在
        /// </summary>
        void EnsureDirectoryExists(string path);
    }
}
