using System.Collections.ObjectModel;
using System.Windows.Input;
using RandPicker.Config;
using Serilog;

namespace RandPicker.ViewModels;

/// <summary>
/// 关联管理视图模型
/// </summary>
public class AssociationManagementViewModel : ViewModelBase
{
    private ObservableCollection<Student> _unassignedStudents = new();
    private ObservableCollection<Group> _groups = new();
    private Student? _selectedUnassignedStudent;
    private Group? _selectedGroup;
    private ObservableCollection<Student> _selectedGroupMembers = new();

    public ObservableCollection<Student> UnassignedStudents
    {
        get => _unassignedStudents;
        set
        {
            _unassignedStudents = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Group> Groups
    {
        get => _groups;
        set
        {
            _groups = value;
            OnPropertyChanged();
        }
    }

    public Student? SelectedUnassignedStudent
    {
        get => _selectedUnassignedStudent;
        set
        {
            _selectedUnassignedStudent = value;
            OnPropertyChanged();
            AddToGroupCommand.RaiseCanExecuteChanged();
        }
    }

    public Group? SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            _selectedGroup = value;
            OnPropertyChanged();
            LoadGroupMembers();
            AddToGroupCommand.RaiseCanExecuteChanged();
        }
    }

    public ObservableCollection<Student> SelectedGroupMembers
    {
        get => _selectedGroupMembers;
        set
        {
            _selectedGroupMembers = value;
            OnPropertyChanged();
        }
    }

    // 命令
    public RelayCommand AddToGroupCommand { get; }
    public RelayCommand<Student> RemoveFromGroupCommand { get; }
    public RelayCommand RefreshCommand { get; }

    private List<Student> _allStudents = new();
    private List<Group> _allGroups = new();

    public AssociationManagementViewModel()
    {
        // 初始化命令
        AddToGroupCommand = new RelayCommand(AddStudentToGroup, 
            () => SelectedUnassignedStudent != null && SelectedGroup != null);
        RemoveFromGroupCommand = new RelayCommand<Student>(RemoveStudentFromGroup);
        RefreshCommand = new RelayCommand(LoadData);

        // 加载数据
        LoadData();
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    private void LoadData()
    {
        try
        {
            var (students, groups) = DataService.LoadAllData();
            _allStudents = students;
            _allGroups = groups;

            // 更新小组列表
            Groups.Clear();
            foreach (var group in _allGroups)
            {
                Groups.Add(group);
            }

            // 更新未分组学生列表
            LoadUnassignedStudents();

            Log.Information($"加载了 {students.Count} 名学生和 {groups.Count} 个小组");
        }
        catch (Exception ex)
        {
            Log.Error($"加载数据失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 加载未分组学生
    /// </summary>
    private void LoadUnassignedStudents()
    {
        var assignedStudentIds = _allGroups.SelectMany(g => g.StudentIds).ToHashSet();
        var unassigned = _allStudents.Where(s => !assignedStudentIds.Contains(s.Id)).ToList();

        UnassignedStudents.Clear();
        foreach (var student in unassigned)
        {
            UnassignedStudents.Add(student);
        }
    }

    /// <summary>
    /// 加载选中小组的成员
    /// </summary>
    private void LoadGroupMembers()
    {
        SelectedGroupMembers.Clear();
        if (SelectedGroup == null) return;

        var members = _allStudents.Where(s => SelectedGroup.StudentIds.Contains(s.Id)).ToList();
        foreach (var member in members)
        {
            SelectedGroupMembers.Add(member);
        }
    }

    /// <summary>
    /// 将学生添加到小组
    /// </summary>
    private async void AddStudentToGroup()
    {
        if (SelectedUnassignedStudent == null || SelectedGroup == null) return;

        try
        {
            // 添加学生到小组
            SelectedGroup.StudentIds.Add(SelectedUnassignedStudent.Id);

            // 保存数据
            await DataService.SaveAllDataAsync(_allStudents, _allGroups);

            // 刷新界面
            LoadUnassignedStudents();
            LoadGroupMembers();

            Log.Information($"将学生 {SelectedUnassignedStudent.Name} 添加到小组 {SelectedGroup.Name}");
        }
        catch (Exception ex)
        {
            Log.Error($"添加学生到小组失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 从小组中移除学生
    /// </summary>
    private async void RemoveStudentFromGroup(Student? student)
    {
        if (student == null || SelectedGroup == null) return;

        try
        {
            // 从小组中移除学生
            SelectedGroup.StudentIds.Remove(student.Id);

            // 保存数据
            await DataService.SaveAllDataAsync(_allStudents, _allGroups);

            // 刷新界面
            LoadUnassignedStudents();
            LoadGroupMembers();

            Log.Information($"将学生 {student.Name} 从小组 {SelectedGroup.Name} 中移除");
        }
        catch (Exception ex)
        {
            Log.Error($"从小组移除学生失败: {ex.Message}");
        }
    }
}