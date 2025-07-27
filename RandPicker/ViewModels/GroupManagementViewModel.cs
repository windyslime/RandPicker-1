using System.Collections.ObjectModel;
using System.Windows.Input;
using RandPicker.Config;
using Serilog;

namespace RandPicker.ViewModels;

/// <summary>
/// 小组管理视图模型
/// </summary>
public class GroupManagementViewModel : ViewModelBase
{
    private ObservableCollection<Group> _groups = new();
    private Group? _selectedGroup;
    private Group _editingGroup = new();
    private string _searchText = string.Empty;
    private bool _isEditPanelVisible;
    private bool _isAddMode;
    private ObservableCollection<Student> _groupMembers = new();

    public ObservableCollection<Group> Groups
    {
        get => _groups;
        set
        {
            _groups = value;
            OnPropertyChanged();
        }
    }

    public Group? SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            _selectedGroup = value;
            OnPropertyChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            LoadGroupMembers();
        }
    }

    public Group EditingGroup
    {
        get => _editingGroup;
        set
        {
            _editingGroup = value;
            OnPropertyChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterGroups();
        }
    }

    public bool IsEditPanelVisible
    {
        get => _isEditPanelVisible;
        set
        {
            _isEditPanelVisible = value;
            OnPropertyChanged();
        }
    }

    public bool IsAddMode
    {
        get => _isAddMode;
        set
        {
            _isAddMode = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Student> GroupMembers
    {
        get => _groupMembers;
        set
        {
            _groupMembers = value;
            OnPropertyChanged();
        }
    }

    public string EditPanelTitle => IsAddMode ? "添加小组" : "编辑小组";

    // 命令
    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand RefreshCommand { get; }

    private List<Group> _allGroups = new();
    private List<Student> _allStudents = new();

    public GroupManagementViewModel()
    {
        // 初始化命令
        AddCommand = new RelayCommand(AddGroup);
        EditCommand = new RelayCommand(EditGroup, () => SelectedGroup != null);
        DeleteCommand = new RelayCommand(DeleteGroup, () => SelectedGroup != null);
        SaveCommand = new RelayCommand(SaveGroup);
        CancelCommand = new RelayCommand(CancelEdit);
        RefreshCommand = new RelayCommand(LoadGroups);

        // 加载数据
        LoadGroups();
    }

    /// <summary>
    /// 加载小组数据
    /// </summary>
    private void LoadGroups()
    {
        try
        {
            var (students, groups) = DataService.LoadAllData();
            _allStudents = students;
            _allGroups = groups;
            FilterGroups();
            Log.Information($"加载了 {groups.Count} 个小组");
        }
        catch (Exception ex)
        {
            Log.Error($"加载小组数据失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 筛选小组
    /// </summary>
    private void FilterGroups()
    {
        var filteredGroups = string.IsNullOrWhiteSpace(SearchText)
            ? _allGroups
            : _allGroups.Where(g => 
                g.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                g.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

        Groups.Clear();
        foreach (var group in filteredGroups)
        {
            Groups.Add(group);
        }
    }

    /// <summary>
    /// 加载小组成员
    /// </summary>
    private void LoadGroupMembers()
    {
        GroupMembers.Clear();
        if (SelectedGroup == null) return;

        var members = _allStudents.Where(s => SelectedGroup.StudentIds.Contains(s.Id)).ToList();
        foreach (var member in members)
        {
            GroupMembers.Add(member);
        }
    }

    /// <summary>
    /// 添加小组
    /// </summary>
    private void AddGroup()
    {
        IsAddMode = true;
        EditingGroup = new Group
        {
            Id = DataService.GetNextGroupId(_allGroups),
            CreatedTime = DateTime.Now,
            IsActive = true
        };
        IsEditPanelVisible = true;
    }

    /// <summary>
    /// 编辑小组
    /// </summary>
    private void EditGroup()
    {
        if (SelectedGroup == null) return;

        IsAddMode = false;
        EditingGroup = new Group
        {
            Id = SelectedGroup.Id,
            Name = SelectedGroup.Name,
            Description = SelectedGroup.Description,
            StudentIds = new List<int>(SelectedGroup.StudentIds),
            CreatedTime = SelectedGroup.CreatedTime,
            IsActive = SelectedGroup.IsActive
        };
        IsEditPanelVisible = true;
    }

    /// <summary>
    /// 删除小组
    /// </summary>
    private async void DeleteGroup()
    {
        if (SelectedGroup == null) return;

        try
        {
            _allGroups.Remove(SelectedGroup);
            await DataService.SaveAllDataAsync(_allStudents, _allGroups);
            FilterGroups();
            SelectedGroup = null;
            
            Log.Information($"删除小组: {SelectedGroup?.Name}");
        }
        catch (Exception ex)
        {
            Log.Error($"删除小组失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 保存小组
    /// </summary>
    private async void SaveGroup()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EditingGroup.Name))
            {
                Log.Warning("小组名称不能为空");
                return;
            }

            if (IsAddMode)
            {
                // 检查ID是否已存在
                if (_allGroups.Any(g => g.Id == EditingGroup.Id))
                {
                    EditingGroup.Id = DataService.GetNextGroupId(_allGroups);
                }
                _allGroups.Add(EditingGroup);
            }
            else
            {
                var existingGroup = _allGroups.FirstOrDefault(g => g.Id == EditingGroup.Id);
                if (existingGroup != null)
                {
                    existingGroup.Name = EditingGroup.Name;
                    existingGroup.Description = EditingGroup.Description;
                    existingGroup.IsActive = EditingGroup.IsActive;
                    // 注意：这里不更新StudentIds，因为成员管理在关联管理页面处理
                }
            }

            await DataService.SaveAllDataAsync(_allStudents, _allGroups);
            
            FilterGroups();
            IsEditPanelVisible = false;
            
            Log.Information($"{(IsAddMode ? "添加" : "编辑")}小组成功: {EditingGroup.Name}");
        }
        catch (Exception ex)
        {
            Log.Error($"保存小组失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 取消编辑
    /// </summary>
    private void CancelEdit()
    {
        IsEditPanelVisible = false;
        EditingGroup = new Group();
    }
}