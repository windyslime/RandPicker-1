using System.Collections.ObjectModel;
using System.Windows.Input;
using RandPicker.Config;
using Serilog;

namespace RandPicker.ViewModels;

/// <summary>
/// 学生管理视图模型
/// </summary>
public class StudentManagementViewModel : ViewModelBase
{
    private ObservableCollection<Student> _students = new();
    private Student? _selectedStudent;
    private Student _editingStudent = new();
    private string _searchText = string.Empty;
    private bool _isEditPanelVisible;
    private bool _isAddMode;

    public ObservableCollection<Student> Students
    {
        get => _students;
        set
        {
            _students = value;
            OnPropertyChanged();
        }
    }

    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            _selectedStudent = value;
            OnPropertyChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }
    }

    public Student EditingStudent
    {
        get => _editingStudent;
        set
        {
            _editingStudent = value;
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
            FilterStudents();
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

    public string EditPanelTitle => IsAddMode ? "添加学生" : "编辑学生";

    // 命令
    public RelayCommand AddCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }
    public RelayCommand RefreshCommand { get; }

    private List<Student> _allStudents = new();

    public StudentManagementViewModel()
    {
        // 初始化命令
        AddCommand = new RelayCommand(AddStudent);
        EditCommand = new RelayCommand(EditStudent, () => SelectedStudent != null);
        DeleteCommand = new RelayCommand(DeleteStudent, () => SelectedStudent != null);
        SaveCommand = new RelayCommand(SaveStudent);
        CancelCommand = new RelayCommand(CancelEdit);
        RefreshCommand = new RelayCommand(LoadStudents);

        // 加载数据
        LoadStudents();
    }

    /// <summary>
    /// 加载学生数据
    /// </summary>
    private void LoadStudents()
    {
        try
        {
            var (students, _) = DataService.LoadAllData();
            _allStudents = students;
            FilterStudents();
            Log.Information($"加载了 {students.Count} 名学生");
        }
        catch (Exception ex)
        {
            Log.Error($"加载学生数据失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 筛选学生
    /// </summary>
    private void FilterStudents()
    {
        var filteredStudents = string.IsNullOrWhiteSpace(SearchText)
            ? _allStudents
            : _allStudents.Where(s => 
                s.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                s.Id.ToString().Contains(SearchText)).ToList();

        Students.Clear();
        foreach (var student in filteredStudents)
        {
            Students.Add(student);
        }
    }

    /// <summary>
    /// 添加学生
    /// </summary>
    private void AddStudent()
    {
        IsAddMode = true;
        EditingStudent = new Student
        {
            Id = DataService.GetNextStudentId(_allStudents),
            Weight = 1,
            IsActive = true
        };
        IsEditPanelVisible = true;
    }

    /// <summary>
    /// 编辑学生
    /// </summary>
    private void EditStudent()
    {
        if (SelectedStudent == null) return;

        IsAddMode = false;
        EditingStudent = new Student
        {
            Id = SelectedStudent.Id,
            Name = SelectedStudent.Name,
            Weight = SelectedStudent.Weight,
            IsActive = SelectedStudent.IsActive
        };
        IsEditPanelVisible = true;
    }

    /// <summary>
    /// 删除学生
    /// </summary>
    private async void DeleteStudent()
    {
        if (SelectedStudent == null) return;

        try
        {
            _allStudents.Remove(SelectedStudent);
            var (_, groups) = DataService.LoadAllData();
            
            // 从所有小组中移除该学生
            foreach (var group in groups)
            {
                group.StudentIds.Remove(SelectedStudent.Id);
            }

            await DataService.SaveAllDataAsync(_allStudents, groups);
            FilterStudents();
            SelectedStudent = null;
            
            Log.Information($"删除学生: {SelectedStudent?.Name}");
        }
        catch (Exception ex)
        {
            Log.Error($"删除学生失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 保存学生
    /// </summary>
    private async void SaveStudent()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(EditingStudent.Name))
            {
                Log.Warning("学生姓名不能为空");
                return;
            }

            if (IsAddMode)
            {
                // 检查ID是否已存在
                if (_allStudents.Any(s => s.Id == EditingStudent.Id))
                {
                    EditingStudent.Id = DataService.GetNextStudentId(_allStudents);
                }
                _allStudents.Add(EditingStudent);
            }
            else
            {
                var existingStudent = _allStudents.FirstOrDefault(s => s.Id == EditingStudent.Id);
                if (existingStudent != null)
                {
                    existingStudent.Name = EditingStudent.Name;
                    existingStudent.Weight = EditingStudent.Weight;
                    existingStudent.IsActive = EditingStudent.IsActive;
                }
            }

            var (_, groups) = DataService.LoadAllData();
            await DataService.SaveAllDataAsync(_allStudents, groups);
            
            FilterStudents();
            IsEditPanelVisible = false;
            
            Log.Information($"{(IsAddMode ? "添加" : "编辑")}学生成功: {EditingStudent.Name}");
        }
        catch (Exception ex)
        {
            Log.Error($"保存学生失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 取消编辑
    /// </summary>
    private void CancelEdit()
    {
        IsEditPanelVisible = false;
        EditingStudent = new Student();
    }
}