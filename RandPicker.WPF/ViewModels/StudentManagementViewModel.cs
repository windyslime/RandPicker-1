using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using RandPicker.WPF.Models;
using RandPicker.WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RandPicker.WPF.ViewModels
{
    /// <summary>
    /// 学生管理视图模型
    /// </summary>
    public partial class StudentManagementViewModel : ObservableObject
    {
        private readonly IStudentService _studentService;
        private readonly IExportService _exportService;

        /// <summary>
        /// 学生集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Student> students = new();

        /// <summary>
        /// 选中的学生
        /// </summary>
        [ObservableProperty]
        private Student? selectedStudent;

        /// <summary>
        /// 新学生
        /// </summary>
        [ObservableProperty]
        private Student newStudent = new Student { Name = "", Id = 0, Weight = 1, Active = true };

        /// <summary>
        /// 是否正在编辑
        /// </summary>
        [ObservableProperty]
        private bool isEditing = false;

        /// <summary>
        /// 是否正在忙碌
        /// </summary>
        [ObservableProperty]
        private bool isBusy = false;

        /// <summary>
        /// 搜索文本
        /// </summary>
        [ObservableProperty]
        private string searchText = string.Empty;

        /// <summary>
        /// 过滤后的学生集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Student> filteredStudents = new();

        /// <summary>
        /// 添加学生命令
        /// </summary>
        public ICommand AddStudentCommand { get; }

        /// <summary>
        /// 编辑学生命令
        /// </summary>
        public ICommand EditStudentCommand { get; }

        /// <summary>
        /// 删除学生命令
        /// </summary>
        public ICommand DeleteStudentCommand { get; }

        /// <summary>
        /// 保存学生命令
        /// </summary>
        public ICommand SaveStudentCommand { get; }

        /// <summary>
        /// 取消编辑命令
        /// </summary>
        public ICommand CancelEditCommand { get; }

        /// <summary>
        /// 导入学生命令
        /// </summary>
        public ICommand ImportStudentsCommand { get; }

        /// <summary>
        /// 导出学生命令
        /// </summary>
        public ICommand ExportStudentsCommand { get; }

        /// <summary>
        /// 重置权重命令
        /// </summary>
        public ICommand ResetWeightsCommand { get; }

        /// <summary>
        /// 重置状态命令
        /// </summary>
        public ICommand ResetActiveCommand { get; }

        /// <summary>
        /// 搜索命令
        /// </summary>
        public ICommand SearchCommand { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StudentManagementViewModel(IStudentService studentService, IExportService exportService)
        {
            _studentService = studentService;
            _exportService = exportService;

            AddStudentCommand = new RelayCommand(AddStudent);
            EditStudentCommand = new RelayCommand(EditStudent, CanEditStudent);
            DeleteStudentCommand = new RelayCommand(DeleteStudent, CanEditStudent);
            SaveStudentCommand = new AsyncRelayCommand(SaveStudentAsync);
            CancelEditCommand = new RelayCommand(CancelEdit);
            ImportStudentsCommand = new AsyncRelayCommand(ImportStudentsAsync);
            ExportStudentsCommand = new AsyncRelayCommand<string>(ExportStudentsAsync);
            ResetWeightsCommand = new AsyncRelayCommand(ResetWeightsAsync);
            ResetActiveCommand = new AsyncRelayCommand<bool>(ResetActiveAsync);
            SearchCommand = new RelayCommand(Search);

            _ = LoadStudentsAsync();
        }

        /// <summary>
        /// 加载学生数据
        /// </summary>
        private async Task LoadStudentsAsync()
        {
            IsBusy = true;
            Students = await _studentService.LoadStudentsAsync();
            FilterStudents();
            IsBusy = false;
        }

        /// <summary>
        /// 添加学生
        /// </summary>
        private void AddStudent()
        {
            NewStudent = new Student { Name = "", Id = 0, Weight = 1, Active = true };
            IsEditing = true;
        }

        /// <summary>
        /// 编辑学生
        /// </summary>
        private void EditStudent()
        {
            if (SelectedStudent == null)
                return;

            NewStudent = new Student
            {
                Name = SelectedStudent.Name,
                Id = SelectedStudent.Id,
                Weight = SelectedStudent.Weight,
                Active = SelectedStudent.Active,
                AvatarPath = SelectedStudent.AvatarPath
            };
            IsEditing = true;
        }

        /// <summary>
        /// 是否可以编辑学生
        /// </summary>
        private bool CanEditStudent()
        {
            return SelectedStudent != null && !IsEditing;
        }

        /// <summary>
        /// 删除学生
        /// </summary>
        private void DeleteStudent()
        {
            if (SelectedStudent == null)
                return;

            var result = MessageBox.Show($"确定要删除学生 {SelectedStudent.Name} 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Students.Remove(SelectedStudent);
                _ = SaveStudentsAsync();
                FilterStudents();
            }
        }

        /// <summary>
        /// 保存学生
        /// </summary>
        private async Task SaveStudentAsync()
        {
            if (string.IsNullOrWhiteSpace(NewStudent.Name))
            {
                MessageBox.Show("学生姓名不能为空", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NewStudent.Id <= 0)
            {
                MessageBox.Show("学生学号必须大于0", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NewStudent.Weight <= 0)
            {
                NewStudent.Weight = 1;
            }

            // 检查是否存在相同学号的学生
            var existingStudent = Students.FirstOrDefault(s => s.Id == NewStudent.Id && s != SelectedStudent);
            if (existingStudent != null)
            {
                MessageBox.Show($"已存在学号为 {NewStudent.Id} 的学生: {existingStudent.Name}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedStudent != null)
            {
                // 更新现有学生
                SelectedStudent.Name = NewStudent.Name;
                SelectedStudent.Id = NewStudent.Id;
                SelectedStudent.Weight = NewStudent.Weight;
                SelectedStudent.Active = NewStudent.Active;
                SelectedStudent.AvatarPath = NewStudent.AvatarPath;
            }
            else
            {
                // 添加新学生
                Students.Add(NewStudent);
            }

            await SaveStudentsAsync();
            IsEditing = false;
            FilterStudents();
        }

        /// <summary>
        /// 取消编辑
        /// </summary>
        private void CancelEdit()
        {
            IsEditing = false;
        }

        /// <summary>
        /// 保存学生数据
        /// </summary>
        private async Task SaveStudentsAsync()
        {
            try
            {
                IsBusy = true;
                await _studentService.SaveStudentsAsync(Students);
                IsBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存学生数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                IsBusy = false;
            }
        }

        /// <summary>
        /// 导入学生数据
        /// </summary>
        private async Task ImportStudentsAsync()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Excel文件|*.xlsx;*.xls|CSV文件|*.csv|所有文件|*.*",
                Title = "选择要导入的文件"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IsBusy = true;
                    ObservableCollection<Student> importedStudents;

                    if (dialog.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                    {
                        importedStudents = await _studentService.ImportFromCsvAsync(dialog.FileName);
                    }
                    else
                    {
                        importedStudents = await _studentService.ImportFromExcelAsync(dialog.FileName);
                    }

                    // 合并导入的学生与现有学生
                    foreach (var student in importedStudents)
                    {
                        var existingStudent = Students.FirstOrDefault(s => s.Id == student.Id);
                        if (existingStudent != null)
                        {
                            // 更新现有学生
                            existingStudent.Name = student.Name;
                            existingStudent.Weight = student.Weight;
                            existingStudent.Active = student.Active;
                        }
                        else
                        {
                            // 添加新学生
                            Students.Add(student);
                        }
                    }

                    await SaveStudentsAsync();
                    FilterStudents();
                    MessageBox.Show($"成功导入 {importedStudents.Count} 名学生", "导入成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导入学生数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        /// <summary>
        /// 导出学生数据
        /// </summary>
        private async Task ExportStudentsAsync(string format)
        {
            if (string.IsNullOrEmpty(format))
                return;

            var dialog = new SaveFileDialog();
            string extension;

            switch (format.ToLower())
            {
                case "excel":
                    dialog.Filter = "Excel文件|*.xlsx";
                    extension = ".xlsx";
                    break;
                case "csv":
                    dialog.Filter = "CSV文件|*.csv";
                    extension = ".csv";
                    break;
                case "pdf":
                    dialog.Filter = "PDF文件|*.pdf";
                    extension = ".pdf";
                    break;
                case "json":
                    dialog.Filter = "JSON文件|*.json";
                    extension = ".json";
                    break;
                default:
                    return;
            }

            dialog.FileName = $"学生数据{extension}";
            dialog.Title = "保存学生数据";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IsBusy = true;
                    switch (format.ToLower())
                    {
                        case "excel":
                            await _exportService.ExportStudentsToExcelAsync(Students, dialog.FileName);
                            break;
                        case "csv":
                            await _exportService.ExportStudentsToCsvAsync(Students, dialog.FileName);
                            break;
                        case "pdf":
                            await _exportService.ExportStudentsToPdfAsync(Students, dialog.FileName);
                            break;
                        case "json":
                            await _exportService.ExportStudentsToJsonAsync(Students, dialog.FileName);
                            break;
                    }
                    MessageBox.Show($"学生数据已成功导出到: {dialog.FileName}", "导出成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出学生数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        /// <summary>
        /// 重置权重
        /// </summary>
        private async Task ResetWeightsAsync()
        {
            var result = MessageBox.Show("确定要将所有学生的权重重置为1吗？", "确认重置", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _studentService.ResetAllWeights(Students);
                await SaveStudentsAsync();
                FilterStudents();
            }
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        private async Task ResetActiveAsync(bool active)
        {
            var status = active ? "启用" : "禁用";
            var result = MessageBox.Show($"确定要将所有学生的状态重置为{status}吗？", "确认重置", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _studentService.ResetAllActive(Students, active);
                await SaveStudentsAsync();
                FilterStudents();
            }
        }

        /// <summary>
        /// 搜索
        /// </summary>
        private void Search()
        {
            FilterStudents();
        }

        /// <summary>
        /// 过滤学生
        /// </summary>
        private void FilterStudents()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredStudents = new ObservableCollection<Student>(Students);
            }
            else
            {
                var searchLower = SearchText.ToLower();
                FilteredStudents = new ObservableCollection<Student>(
                    Students.Where(s =>
                        s.Name.ToLower().Contains(searchLower) ||
                        s.Id.ToString().Contains(searchLower)
                    )
                );
            }
        }
    }
}