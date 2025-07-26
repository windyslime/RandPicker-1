using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RandPicker.WPF.Models;
using RandPicker.WPF.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RandPicker.WPF.ViewModels
{
    /// <summary>
    /// 设置视图模型
    /// </summary>
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IConfigurationService _configService;

        /// <summary>
        /// 应用设置
        /// </summary>
        [ObservableProperty]
        private AppSettings settings;

        /// <summary>
        /// 可用主题列表
        /// </summary>
        public List<string> AvailableThemes { get; } = new List<string> { "Light", "Dark", "Auto" };

        /// <summary>
        /// 可用主题颜色列表
        /// </summary>
        public List<string> AvailableColors { get; } = new List<string>
        {
            "#FF2196F3", // 蓝色
            "#FF4CAF50", // 绿色
            "#FFF44336", // 红色
            "#FFFF9800", // 橙色
            "#FF9C27B0", // 紫色
            "#FF795548", // 棕色
            "#FF607D8B"  // 蓝灰色
        };

        /// <summary>
        /// 保存设置命令
        /// </summary>
        public ICommand SaveSettingsCommand { get; }

        /// <summary>
        /// 重置设置命令
        /// </summary>
        public ICommand ResetSettingsCommand { get; }

        /// <summary>
        /// 选择导出路径命令
        /// </summary>
        public ICommand SelectExportPathCommand { get; }

        /// <summary>
        /// 应用主题命令
        /// </summary>
        public ICommand ApplyThemeCommand { get; }

        /// <summary>
        /// 应用主题颜色命令
        /// </summary>
        public ICommand ApplyThemeColorCommand { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SettingsViewModel(IConfigurationService configService)
        {
            _configService = configService;
            settings = _configService.CurrentSettings;

            SaveSettingsCommand = new AsyncRelayCommand(SaveSettingsAsync);
            ResetSettingsCommand = new AsyncRelayCommand(ResetSettingsAsync);
            SelectExportPathCommand = new RelayCommand(SelectExportPath);
            ApplyThemeCommand = new RelayCommand<string>(ApplyTheme);
            ApplyThemeColorCommand = new RelayCommand<string>(ApplyThemeColor);
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        private async Task SaveSettingsAsync()
        {
            try
            {
                await _configService.SaveSettingsAsync(Settings);
                _configService.Theme = Settings.Theme;
                _configService.RecordHistory = Settings.RecordHistory;
                _configService.ExportPath = Settings.ExportPath;
                _configService.Save();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"保存设置失败: {ex.Message}", "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 重置设置
        /// </summary>
        private async Task ResetSettingsAsync()
        {
            Settings = new AppSettings();
            await SaveSettingsAsync();
        }

        /// <summary>
        /// 选择导出路径
        /// </summary>
        private void SelectExportPath()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "选择导出路径",
                ShowNewFolderButton = true
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Settings.ExportPath = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 应用主题
        /// </summary>
        private void ApplyTheme(string theme)
        {
            if (string.IsNullOrEmpty(theme))
                return;

            Settings.Theme = theme;
            _configService.ApplyTheme(theme);
        }

        /// <summary>
        /// 应用主题颜色
        /// </summary>
        private void ApplyThemeColor(string color)
        {
            if (string.IsNullOrEmpty(color))
                return;

            Settings.ThemeColor = color;
            _configService.ApplyThemeColor(color);
        }
    }
}