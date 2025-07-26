using RandPicker.WPF.ViewModels;
using System.Windows;

namespace RandPicker.WPF.Views
{
    /// <summary>
    /// GroupManagementWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GroupManagementWindow : Window
    {
        private readonly GroupManagementViewModel _viewModel;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GroupManagementWindow(GroupManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;

            // 注册窗口事件
            Loaded += GroupManagementWindow_Loaded;
            Closing += GroupManagementWindow_Closing;
        }

        /// <summary>
        /// 窗口加载事件
        /// </summary>
        private void GroupManagementWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 窗口加载时的初始化操作
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        private void GroupManagementWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 窗口关闭时的清理操作
        }
    }
}