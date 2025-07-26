using RandPicker.WPF.ViewModels;
using System.Windows;

namespace RandPicker.WPF.Views
{
    /// <summary>
    /// StudentManagementWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StudentManagementWindow : Window
    {
        private readonly StudentManagementViewModel _viewModel;

        /// <summary>
        /// 构造函数
        /// </summary>
        public StudentManagementWindow(StudentManagementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;

            // 注册窗口事件
            Loaded += StudentManagementWindow_Loaded;
            Closing += StudentManagementWindow_Closing;
        }

        /// <summary>
        /// 窗口加载事件
        /// </summary>
        private void StudentManagementWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 窗口加载时的初始化操作
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        private void StudentManagementWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 窗口关闭时的清理操作
        }
    }
}