using RandPicker.WPF.ViewModels;
using System.Windows;

namespace RandPicker.WPF.Views
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly SettingsViewModel _viewModel;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;

            // 注册窗口事件
            Loaded += SettingsWindow_Loaded;
            Closing += SettingsWindow_Closing;
        }

        /// <summary>
        /// 窗口加载事件
        /// </summary>
        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 窗口加载时的初始化操作
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        private void SettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 窗口关闭时的清理操作
        }
    }
}