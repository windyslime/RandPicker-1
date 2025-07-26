using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using RandPicker.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace RandPicker.WPF.Services
{
    /// <summary>
    /// 配置服务实现类
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configPath;
        private readonly string _settingsPath;
        private readonly Dictionary<string, object> _config;
        private AppSettings _currentSettings;
        private bool _isInitialized = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConfigurationService()
        {
            // 配置文件路径
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RandPicker");

            _configPath = Path.Combine(appDataPath, "config.json");
            _settingsPath = Path.Combine(appDataPath, "settings.json");

            // 确保目录存在
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            // 初始化配置字典
            _config = new Dictionary<string, object>();
            
            // 创建默认设置
            _currentSettings = new AppSettings();
        }

        /// <summary>
        /// 初始化配置服务
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            // 加载配置
            if (File.Exists(_configPath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(_configPath);
                    var config = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                    if (config != null)
                    {
                        foreach (var item in config)
                        {
                            _config[item.Key] = item.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"加载配置失败: {ex.Message}");
                }
            }

            // 加载设置
            _currentSettings = await LoadSettingsAsync();

            _isInitialized = true;
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        public T GetValue<T>(string key, T defaultValue = default!)
        {
            if (_config.TryGetValue(key, out var value))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value))!;
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// 设置配置值
        /// </summary>
        public void SetValue<T>(string key, T value)
        {
            _config[key] = value!;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public async Task SaveAsync()
        {
            var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
            await File.WriteAllTextAsync(_configPath, json);
        }

        /// <summary>
        /// 同步保存配置
        /// </summary>
        public void Save()
        {
            var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
            File.WriteAllText(_configPath, json);
        }

        /// <summary>
        /// 加载应用设置
        /// </summary>
        public async Task<AppSettings> LoadSettingsAsync()
        {
            if (File.Exists(_settingsPath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(_settingsPath);
                    var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                    if (settings != null)
                    {
                        _currentSettings = settings;
                        return settings;
                    }
                }
                catch (Exception ex)
                {
                    // 如果加载失败，使用默认设置
                    System.Diagnostics.Debug.WriteLine($"加载设置失败: {ex.Message}");
                }
            }

            // 如果文件不存在或加载失败，创建默认设置
            var defaultSettings = new AppSettings
            {
                Theme = "Dark",
                ThemeColor = "#FF2196F3", // 蓝色
                EdgeHide = false,
                EdgeDistance = 20,
                HiddenWidth = 5,
                AvatarEnabled = true,
                AvatarSize = 64,
                ElasticAnimation = true,
                Scale = 1.0,
                RecordHistory = true,
                ExportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            
            await SaveSettingsAsync(defaultSettings);
            _currentSettings = defaultSettings;
            return defaultSettings;
        }

        /// <summary>
        /// 保存应用设置
        /// </summary>
        public async Task SaveSettingsAsync(AppSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            await File.WriteAllTextAsync(_settingsPath, json);
            _currentSettings = settings;

            // 同步更新配置
            Theme = settings.Theme;
            RecordHistory = settings.RecordHistory;
            ExportPath = settings.ExportPath;
            await SaveAsync();
        }

        /// <summary>
        /// 应用主题
        /// </summary>
        public void ApplyTheme(string theme)
        {
            var paletteHelper = new PaletteHelper();
            var currentTheme = paletteHelper.GetTheme();

            switch (theme.ToLower())
            {
                case "light":
                    currentTheme.SetBaseTheme(Theme.Light);
                    break;
                case "dark":
                    currentTheme.SetBaseTheme(Theme.Dark);
                    break;
                case "auto":
                    // 获取系统主题
                    var systemTheme = SystemParameters.HighContrast ? Theme.Light : Theme.Dark;
                    currentTheme.SetBaseTheme(systemTheme);
                    break;
                default:
                    currentTheme.SetBaseTheme(Theme.Dark);
                    break;
            }

            paletteHelper.SetTheme(currentTheme);
            Theme = theme;
            
            // 如果已经设置了主题颜色，重新应用
            if (!string.IsNullOrEmpty(_currentSettings.ThemeColor))
            {
                ApplyThemeColor(_currentSettings.ThemeColor);
            }
        }

        /// <summary>
        /// 应用主题颜色
        /// </summary>
        public void ApplyThemeColor(string color)
        {
            if (string.IsNullOrEmpty(color) || !color.StartsWith("#"))
                return;

            try
            {
                var paletteHelper = new PaletteHelper();
                var currentTheme = paletteHelper.GetTheme();
                currentTheme.SetPrimaryColor(System.Windows.Media.ColorConverter.ConvertFromString(color));
                paletteHelper.SetTheme(currentTheme);
                
                // 更新设置
                if (_currentSettings != null)
                {
                    _currentSettings.ThemeColor = color;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"应用主题颜色失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取当前是否为深色主题
        /// </summary>
        public bool IsDarkTheme
        {
            get
            {
                var paletteHelper = new PaletteHelper();
                var currentTheme = paletteHelper.GetTheme();
                return currentTheme.GetBaseTheme() == BaseTheme.Dark;
            }
        }

        /// <summary>
        /// 主题
        /// </summary>
        public string Theme
        {
            get => GetValue("Theme", "Dark");
            set => SetValue("Theme", value);
        }

        /// <summary>
        /// 是否记录历史
        /// </summary>
        public bool RecordHistory
        {
            get => GetValue("RecordHistory", true);
            set => SetValue("RecordHistory", value);
        }

        /// <summary>
        /// 导出路径
        /// </summary>
        public string ExportPath
        {
            get => GetValue("ExportPath", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            set => SetValue("ExportPath", value);
        }

        /// <summary>
        /// 当前应用设置
        /// </summary>
        public AppSettings CurrentSettings => _currentSettings;

        /// <summary>
        /// 获取应用数据路径
        /// </summary>
        public string GetAppDataPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RandPicker");
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        public string GetFilePath(string fileName)
        {
            return Path.Combine(GetAppDataPath(), fileName);
        }

        /// <summary>
        /// 确保目录存在
        /// </summary>
        public void EnsureDirectoryExists(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}
