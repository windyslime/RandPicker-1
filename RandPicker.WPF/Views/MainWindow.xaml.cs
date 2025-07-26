using Microsoft.Extensions.DependencyInjection;
using RandPicker.WPF.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace RandPicker.WPF.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;

            // 注册窗口事件
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        /// <summary>
        /// 窗口加载事件
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 应用主题
            if (_viewModel.AppSettings != null)
            {
                // 应用主题设置
            }
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 保存设置
            if (_viewModel.AppSettings != null)
            {
                // 保存设置
            }
        }

        /// <summary>
        /// 打开设置窗口
        /// </summary>
        private void OpenSettingsWindow()
        {
            var settingsViewModel = App.Current.Services.GetService<SettingsViewModel>();
            var settingsWindow = new SettingsWindow(settingsViewModel);
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        /// <summary>
        /// 打开学生管理窗口
        /// </summary>
        private void OpenStudentManagementWindow()
        {
            var studentManagementViewModel = App.Current.Services.GetService<StudentManagementViewModel>();
            var studentManagementWindow = new StudentManagementWindow(studentManagementViewModel);
            studentManagementWindow.Owner = this;
            studentManagementWindow.ShowDialog();
        }

        /// <summary>
        /// 打开分组管理窗口
        /// </summary>
        private void OpenGroupManagementWindow()
        {
            var groupManagementViewModel = App.Current.Services.GetService<GroupManagementViewModel>();
            var groupManagementWindow = new GroupManagementWindow(groupManagementViewModel);
            groupManagementWindow.Owner = this;
            groupManagementWindow.ShowDialog();
        }

        /// <summary>
        /// 打开历史记录窗口
        /// </summary>
        private void OpenHistoryWindow()
        {
            var historyViewModel = App.Current.Services.GetService<HistoryViewModel>();
            var historyWindow = new HistoryWindow(historyViewModel);
            historyWindow.Owner = this;
            historyWindow.ShowDialog();
        }
    }
}
