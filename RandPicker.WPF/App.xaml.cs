using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RandPicker.WPF.Services;
using RandPicker.WPF.ViewModels;
using System;
using System.Windows;

namespace RandPicker.WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        public IServiceProvider Services => _host!.Services;

        /// <summary>
        /// 获取当前应用程序实例
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// 应用程序启动事件
        /// </summary>
        protected override async void OnStartup(StartupEventArgs e)
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // 注册服务
                    services.AddSingleton<IConfigurationService, ConfigurationService>();
                    services.AddSingleton<IStudentService, StudentService>();
                    services.AddSingleton<IGroupService, GroupService>();
                    services.AddSingleton<IHistoryService, HistoryService>();
                    services.AddSingleton<IExportService, ExportService>();

                    // 注册视图模型
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<SettingsViewModel>();
                    services.AddTransient<StudentManagementViewModel>();
                    services.AddTransient<GroupManagementViewModel>();
                    services.AddTransient<HistoryViewModel>();

                    // 注册视图
                    services.AddSingleton<MainWindow>();
                })
                .Build();

            await _host.StartAsync();

            // 初始化配置服务
            var configService = _host.Services.GetRequiredService<IConfigurationService>();
            await configService.InitializeAsync();

            // 应用主题
            configService.ApplyTheme(configService.CurrentSettings.Theme);
            configService.ApplyThemeColor(configService.CurrentSettings.ThemeColor);

            // 显示主窗口
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        /// <summary>
        /// 应用程序退出事件
        /// </summary>
        protected override async void OnExit(ExitEventArgs e)
        {
            // 保存配置
            var configService = _host?.Services.GetRequiredService<IConfigurationService>();
            if (configService != null)
            {
                await configService.SaveSettingsAsync(configService.CurrentSettings);
            }

            // 停止主机
            if (_host is not null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            
            base.OnExit(e);
        }
    }
}
