using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RandPicker.ViewModels;

/// <summary>
/// 管理主界面视图模型
/// </summary>
public class ManagementMainViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    private string _selectedMenuItem = "学生管理";

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
            OnPropertyChanged();
        }
    }

    public string SelectedMenuItem
    {
        get => _selectedMenuItem;
        set
        {
            _selectedMenuItem = value;
            OnPropertyChanged();
            NavigateToView();
        }
    }

    public ObservableCollection<string> MenuItems { get; }

    // 各个子ViewModel
    public StudentManagementViewModel StudentManagementViewModel { get; }
    public GroupManagementViewModel GroupManagementViewModel { get; }
    public AssociationManagementViewModel AssociationManagementViewModel { get; }

    public ManagementMainViewModel()
    {
        // 初始化子ViewModel
        StudentManagementViewModel = new StudentManagementViewModel();
        GroupManagementViewModel = new GroupManagementViewModel();
        AssociationManagementViewModel = new AssociationManagementViewModel();

        // 初始化菜单项
        MenuItems = new ObservableCollection<string>
        {
            "学生管理",
            "小组管理", 
            "关联管理"
        };

        // 设置默认视图
        _currentViewModel = StudentManagementViewModel;
    }

    /// <summary>
    /// 导航到指定视图
    /// </summary>
    private void NavigateToView()
    {
        CurrentViewModel = SelectedMenuItem switch
        {
            "学生管理" => StudentManagementViewModel,
            "小组管理" => GroupManagementViewModel,
            "关联管理" => AssociationManagementViewModel,
            _ => StudentManagementViewModel
        };
    }
}