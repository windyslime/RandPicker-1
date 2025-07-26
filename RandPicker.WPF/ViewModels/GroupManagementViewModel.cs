using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using RandPicker.WPF.Models;
using RandPicker.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RandPicker.WPF.ViewModels
{
    /// <summary>
    /// 分组管理视图模型
    /// </summary>
    public partial class GroupManagementViewModel : ObservableObject
    {
        private readonly IGroupService _groupService;
        private readonly IStudentService _studentService;
        private readonly IExportService _exportService;

        /// <summary>
        /// 分组集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Group> groups = new();

        /// <summary>
        /// 学生集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Student> allStudents = new();

        /// <summary>
        /// 选中的分组
        /// </summary>
        [ObservableProperty]
        private Group? selectedGroup;

        /// <summary>
        /// 新分组
        /// </summary>
        [ObservableProperty]
        private Group newGroup = new Group { Name = "" };

        /// <summary>
        /// 选中的学生
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Student> selectedStudents = new();

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
        /// 过滤后的分组集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Group> filteredGroups = new();

        /// <summary>
        /// 添加分组命令
        /// </summary>
        public ICommand AddGroupCommand { get; }

        /// <summary>
        /// 编辑分组命令
        /// </summary>
        public ICommand EditGroupCommand { get; }

        /// <summary>
        /// 删除分组命令
        /// </summary>
        public ICommand DeleteGroupCommand { get; }

        /// <summary>
        /// 保存分组命令
        /// </summary>
        public ICommand SaveGroupCommand { get; }

        /// <summary>
        /// 取消编辑命令
        /// </summary>
        public ICommand CancelEditCommand { get; }

        /// <summary>
        /// 导出分组命令
        /// </summary>
        public ICommand ExportGroupsCommand { get; }

        /// <summary>
        /// 搜索命令
        /// </summary>
        public ICommand SearchCommand { get; }

        /// <summary>
        /// 随机分组命令
        /// </summary>
        public ICommand RandomGroupCommand { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public GroupManagementViewModel(IGroupService groupService, IStudentService studentService, IExportService exportService)
        {
            _groupService = groupService;
            _studentService = studentService;
            _exportService = exportService;

            AddGroupCommand = new RelayCommand(AddGroup);
            EditGroupCommand = new RelayCommand(EditGroup, CanEditGroup);
            DeleteGroupCommand = new RelayCommand(DeleteGroup, CanEditGroup);
            SaveGroupCommand = new AsyncRelayCommand(SaveGroupAsync);
            CancelEditCommand = new RelayCommand(CancelEdit);
            ExportGroupsCommand = new AsyncRelayCommand<string>(ExportGroupsAsync);
            SearchCommand = new RelayCommand(Search);
            RandomGroupCommand = new AsyncRelayCommand<int>(RandomGroupAsync);

            _ = LoadDataAsync();
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        private async Task LoadDataAsync()
        {
            IsBusy = true;
            Groups = await _groupService.LoadGroupsAsync();
            AllStudents = await _studentService.LoadStudentsAsync();
            FilterGroups();
            IsBusy = false;
        }

        /// <summary>
        /// 添加分组
        /// </summary>
        private void AddGroup()
        {
            NewGroup = new Group { Name = "" };
            SelectedStudents.Clear();
            IsEditing = true;
        }

        /// <summary>
        /// 编辑分组
        /// </summary>
        private void EditGroup()
        {
            if (SelectedGroup == null)
                return;

            NewGroup = new Group { Name = SelectedGroup.Name };
            SelectedStudents = new ObservableCollection<Student>(SelectedGroup.Students);
            IsEditing = true;
        }

        /// <summary>
        /// 是否可以编辑分组
        /// </summary>
        private bool CanEditGroup()
        {
            return SelectedGroup != null && !IsEditing;
        }

        /// <summary>
        /// 删除分组
        /// </summary>
        private void DeleteGroup()
        {
            if (SelectedGroup == null)
                return;

            var result = MessageBox.Show($"确定要删除分组 {SelectedGroup.Name} 吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _ = _groupService.RemoveGroupAsync(SelectedGroup);
                Groups.Remove(SelectedGroup);
                FilterGroups();
            }
        }

        /// <summary>
        /// 保存分组
        /// </summary>
        private async Task SaveGroupAsync()
        {
            if (string.IsNullOrWhiteSpace(NewGroup.Name))
            {
                MessageBox.Show("分组名称不能为空", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedStudents.Count == 0)
            {
                MessageBox.Show("分组必须包含至少一名学生", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 检查是否存在相同名称的分组
            var existingGroup = Groups.FirstOrDefault(g => g.Name == NewGroup.Name && g != SelectedGroup);
            if (existingGroup != null)
            {
                MessageBox.Show($"已存在名称为 {NewGroup.Name} 的分组", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedGroup != null)
            {
                // 更新现有分组
                await _groupService.UpdateGroupAsync(SelectedGroup, NewGroup.Name, SelectedStudents);
                SelectedGroup.Name = NewGroup.Name;
                SelectedGroup.Students.Clear();
                foreach (var student in SelectedStudents)
                {
                    SelectedGroup.Students.Add(student);
                }
            }
            else
            {
                // 添加新分组
                await _groupService.AddGroupAsync(NewGroup.Name, SelectedStudents);
                var newGroup = new Group { Name = NewGroup.Name };
                foreach (var student in SelectedStudents)
                {
                    newGroup.Students.Add(student);
                }
                Groups.Add(newGroup);
            }

            IsEditing = false;
            FilterGroups();
        }

        /// <summary>
        /// 取消编辑
        /// </summary>
        private void CancelEdit()
        {
            IsEditing = false;
        }

        /// <summary>
        /// 导出分组数据
        /// </summary>
        private async Task ExportGroupsAsync(string format)
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

            dialog.FileName = $"分组数据{extension}";
            dialog.Title = "保存分组数据";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IsBusy = true;
                    switch (format.ToLower())
                    {
                        case "excel":
                            await _exportService.ExportGroupsToExcelAsync(Groups, dialog.FileName);
                            break;
                        case "csv":
                            await _exportService.ExportGroupsToCsvAsync(Groups, dialog.FileName);
                            break;
                        case "pdf":
                            await _exportService.ExportGroupsToPdfAsync(Groups, dialog.FileName);
                            break;
                        case "json":
                            await _exportService.ExportGroupsToJsonAsync(Groups, dialog.FileName);
                            break;
                    }
                    MessageBox.Show($"分组数据已成功导出到: {dialog.FileName}", "导出成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出分组数据失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        /// <summary>
        /// 搜索
        /// </summary>
        private void Search()
        {
            FilterGroups();
        }

        /// <summary>
        /// 过滤分组
        /// </summary>
        private void FilterGroups()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredGroups = new ObservableCollection<Group>(Groups);
            }
            else
            {
                var searchLower = SearchText.ToLower();
                FilteredGroups = new ObservableCollection<Group>(
                    Groups.Where(g =>
                        g.Name.ToLower().Contains(searchLower) ||
                        g.Students.Any(s => s.Name.ToLower().Contains(searchLower))
                    )
                );
            }
        }

        /// <summary>
        /// 随机分组
        /// </summary>
        private async Task RandomGroupAsync(int groupCount)
        {
            if (groupCount <= 0)
            {
                MessageBox.Show("分组数量必须大于0", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var activeStudents = AllStudents.Where(s => s.Active).ToList();
            if (activeStudents.Count == 0)
            {
                MessageBox.Show("没有可用的学生进行分组", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (activeStudents.Count < groupCount)
            {
                MessageBox.Show("学生数量少于分组数量", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("确定要创建随机分组吗？这将清除现有的所有分组。", "确认", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                IsBusy = true;

                // 随机打乱学生列表
                var random = new Random();
                var shuffledStudents = activeStudents.OrderBy(x => random.Next()).ToList();

                // 创建新分组
                var newGroups = new ObservableCollection<Group>();
                var studentsPerGroup = shuffledStudents.Count / groupCount;
                var remainingStudents = shuffledStudents.Count % groupCount;

                var studentIndex = 0;
                for (int i = 0; i < groupCount; i++)
                {
                    var group = new Group { Name = $"分组 {i + 1}" };
                    var studentsInThisGroup = studentsPerGroup + (i < remainingStudents ? 1 : 0);

                    for (int j = 0; j < studentsInThisGroup; j++)
                    {
                        group.Students.Add(shuffledStudents[studentIndex++]);
                    }

                    newGroups.Add(group);
                }

                // 保存新分组
                Groups = newGroups;
                await _groupService.SaveGroupsAsync(Groups);
                FilterGroups();

                MessageBox.Show($"已成功创建 {groupCount} 个随机分组", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建随机分组失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// 添加学生到选中列表
        /// </summary>
        public void AddStudentToSelection(Student student)
        {
            if (!SelectedStudents.Contains(student))
            {
                SelectedStudents.Add(student);
            }
        }

        /// <summary>
        /// 从选中列表移除学生
        /// </summary>
        public void RemoveStudentFromSelection(Student student)
        {
            SelectedStudents.Remove(student);
        }

        /// <summary>
        /// 获取未选中的学生
        /// </summary>
        public ObservableCollection<Student> GetUnselectedStudents()
        {
            return new ObservableCollection<Student>(
                AllStudents.Where(s => !SelectedStudents.Contains(s))
            );
        }
    }
}