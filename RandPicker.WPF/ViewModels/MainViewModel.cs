using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RandPicker.WPF.Models;
using RandPicker.WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace RandPicker.WPF.ViewModels
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        private readonly IHistoryService _historyService;
        private readonly IConfigurationService _configService;
        private readonly IExportService _exportService;
        private readonly Random _random = new Random();

        /// <summary>
        /// 学生集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Student> students = new();

        /// <summary>
        /// 分组集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Group> groups = new();

        /// <summary>
        /// 历史记录集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<HistoryItem> historyItems = new();

        /// <summary>
        /// 选择模式
        /// </summary>
        [ObservableProperty]
        private string selectedMode = "Individual";

        /// <summary>
        /// 选择结果
        /// </summary>
        [ObservableProperty]
        private string selectedResult = string.Empty;

        /// <summary>
        /// 选中的学生
        /// </summary>
        [ObservableProperty]
        private Student? selectedStudent;

        /// <summary>
        /// 选中的分组
        /// </summary>
        [ObservableProperty]
        private Group? selectedGroup;

        /// <summary>
        /// 是否正在忙碌
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// 是否使用权重
        /// </summary>
        [ObservableProperty]
        private bool useWeight = true;

        /// <summary>
        /// 是否显示动画
        /// </summary>
        [ObservableProperty]
        private bool showAnimation = true;

        /// <summary>
        /// 是否显示浮动窗口
        /// </summary>
        [ObservableProperty]
        private bool showFloatingWindow = false;

        /// <summary>
        /// 浮动窗口设置
        /// </summary>
        [ObservableProperty]
        private FloatingWindow floatingWindow = new FloatingWindow();

        /// <summary>
        /// 应用设置
        /// </summary>
        [ObservableProperty]
        private AppSettings appSettings = new AppSettings();

        /// <summary>
        /// 随机选择命令
        /// </summary>
        public ICommand PickRandomCommand { get; }

        /// <summary>
        /// 刷新数据命令
        /// </summary>
        public ICommand RefreshDataCommand { get; }

        /// <summary>
        /// 打开设置窗口命令
        /// </summary>
        public ICommand OpenSettingsCommand { get; }

        /// <summary>
        /// 打开学生管理窗口命令
        /// </summary>
        public ICommand OpenStudentManagementCommand { get; }

        /// <summary>
        /// 打开分组管理窗口命令
        /// </summary>
        public ICommand OpenGroupManagementCommand { get; }

        /// <summary>
        /// 打开历史记录窗口命令
        /// </summary>
        public ICommand OpenHistoryCommand { get; }

        /// <summary>
        /// 切换浮动窗口命令
        /// </summary>
        public ICommand ToggleFloatingWindowCommand { get; }

        /// <summary>
        /// 导出数据命令
        /// </summary>
        public ICommand ExportDataCommand { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainViewModel(
            IStudentService studentService,
            IGroupService groupService,
            IHistoryService historyService,
            IConfigurationService configService,
            IExportService exportService)
        {
            _studentService = studentService;
            _groupService = groupService;
            _historyService = historyService;
            _configService = configService;
            _exportService = exportService;

            PickRandomCommand = new AsyncRelayCommand(PickRandomAsync);
            RefreshDataCommand = new AsyncRelayCommand(RefreshDataAsync);
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            OpenStudentManagementCommand = new RelayCommand(OpenStudentManagement);
            OpenGroupManagementCommand = new RelayCommand(OpenGroupManagement);
            OpenHistoryCommand = new RelayCommand(OpenHistory);
            ToggleFloatingWindowCommand = new RelayCommand(ToggleFloatingWindow);
            ExportDataCommand = new AsyncRelayCommand<string>(ExportDataAsync);

            _ = InitializeAsync();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private async Task InitializeAsync()
        {
            IsBusy = true;
            
            // 加载应用设置
            AppSettings = await _configService.LoadSettingsAsync();
            
            // 应用主题
            _configService.ApplyTheme(AppSettings.Theme);
            
            // 应用主题颜色
            if (!string.IsNullOrEmpty(AppSettings.ThemeColor))
            {
                _configService.ApplyThemeColor(AppSettings.ThemeColor);
            }
            
            // 加载数据
            await RefreshDataAsync();
            
            // 加载历史记录
            await LoadHistoryAsync();
            
            // 初始化浮动窗口设置
            FloatingWindow = new FloatingWindow
            {
                EdgeHide = AppSettings.EdgeHide,
                EdgeDistance = AppSettings.EdgeDistance,
                HiddenWidth = AppSettings.HiddenWidth,
                Scale = AppSettings.Scale
            };
            
            IsBusy = false;
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        private async Task RefreshDataAsync()
        {
            Students = await _studentService.LoadStudentsAsync();
            Groups = await _groupService.LoadGroupsAsync();
        }

        /// <summary>
        /// 加载历史记录
        /// </summary>
        private async Task LoadHistoryAsync()
        {
            HistoryItems = await _historyService.GetRecentHistoryAsync(20);
        }

        /// <summary>
        /// 随机选择
        /// </summary>
        private async Task PickRandomAsync()
        {
            if (IsBusy)
                return;
                
            IsBusy = true;
            
            if (ShowAnimation)
            {
                // 模拟动画效果
                await SimulateAnimationAsync();
            }
            
            if (SelectedMode == "Individual")
            {
                Student? student = null;
                
                if (UseWeight)
                {
                    student = _studentService.GetWeightedRandomStudent(Students);
                }
                else
                {
                    student = _studentService.GetRandomStudent(Students);
                }
                
                if (student != null)
                {
                    SelectedStudent = student;
                    SelectedResult = student.Name;
                    
                    if (AppSettings.RecordHistory)
                    {
                        var historyItem = new HistoryItem
                        {
                            Mode = SelectionMode.Individual,
                            SelectedName = student.Name,
                            Student = student,
                            Details = $"学号: {student.Id}, 权重: {student.Weight}"
                        };
                        
                        await _historyService.AddHistoryItemAsync(historyItem);
                        HistoryItems.Insert(0, historyItem);
                        
                        // 保持历史记录数量
                        if (HistoryItems.Count > 20)
                        {
                            HistoryItems.RemoveAt(HistoryItems.Count - 1);
                        }
                    }
                }
            }
            else
            {
                var group = _groupService.GetRandomGroup(Groups);
                if (group != null)
                {
                    SelectedGroup = group;
                    SelectedResult = group.Name;
                    
                    if (AppSettings.RecordHistory)
                    {
                        var memberNames = string.Join(", ", group.Students.Select(s => s.Name));
                        var historyItem = new HistoryItem
                        {
                            Mode = SelectionMode.Group,
                            SelectedName = group.Name,
                            Group = group,
                            Details = $"成员: {memberNames}"
                        };
                        
                        await _historyService.AddHistoryItemAsync(historyItem);
                        HistoryItems.Insert(0, historyItem);
                        
                        // 保持历史记录数量
                        if (HistoryItems.Count > 20)
                        {
                            HistoryItems.RemoveAt(HistoryItems.Count - 1);
                        }
                    }
                }
            }
            
            IsBusy = false;
        }

        /// <summary>
        /// 模拟动画效果
        /// </summary>
        private async Task SimulateAnimationAsync()
        {
            // 模拟动画效果，随机显示10次结果
            for (int i = 0; i < 10; i++)
            {
                if (SelectedMode == "Individual")
                {
                    var activeStudents = Students.Where(s => s.Active).ToList();
                    if (activeStudents.Count > 0)
                    {
                        var index = _random.Next(activeStudents.Count);
                        SelectedResult = activeStudents[index].Name;
                    }
                }
                else
                {
                    if (Groups.Count > 0)
                    {
                        var index = _random.Next(Groups.Count);
                        SelectedResult = Groups[index].Name;
                    }
                }
                
                // 等待一小段时间
                await Task.Delay(100);
            }
        }

        /// <summary>
        /// 打开设置窗口
        /// </summary>
        private void OpenSettings()
        {
            // 在实际实现中，这里会打开设置窗口
            // 这里只是一个占位符
            MessageBox.Show("打开设置窗口");
        }

        /// <summary>
        /// 打开学生管理窗口
        /// </summary>
        private void OpenStudentManagement()
        {
            // 在实际实现中，这里会打开学生管理窗口
            // 这里只是一个占位符
            MessageBox.Show("打开学生管理窗口");
        }

        /// <summary>
        /// 打开分组管理窗口
        /// </summary>
        private void OpenGroupManagement()
        {
            // 在实际实现中，这里会打开分组管理窗口
            // 这里只是一个占位符
            MessageBox.Show("打开分组管理窗口");
        }

        /// <summary>
        /// 打开历史记录窗口
        /// </summary>
        private void OpenHistory()
        {
            // 在实际实现中，这里会打开历史记录窗口
            // 这里只是一个占位符
            MessageBox.Show("打开历史记录窗口");
        }

        /// <summary>
        /// 切换浮动窗口
        /// </summary>
        private void ToggleFloatingWindow()
        {
            ShowFloatingWindow = !ShowFloatingWindow;
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        private async Task ExportDataAsync(string format)
        {
            if (string.IsNullOrEmpty(format))
                return;
                
            try
            {
                var path = AppSettings.ExportPath;
                
                switch (format.ToLower())
                {
                    case "students_excel":
                        await _exportService.ExportStudentsToExcelAsync(Students, System.IO.Path.Combine(path, "students.xlsx"));
                        break;
                    case "students_csv":
                        await _exportService.ExportStudentsToCsvAsync(Students, System.IO.Path.Combine(path, "students.csv"));
                        break;
                    case "students_pdf":
                        await _exportService.ExportStudentsToPdfAsync(Students, System.IO.Path.Combine(path, "students.pdf"));
                        break;
                    case "students_json":
                        await _exportService.ExportStudentsToJsonAsync(Students, System.IO.Path.Combine(path, "students.json"));
                        break;
                    case "groups_excel":
                        await _exportService.ExportGroupsToExcelAsync(Groups, System.IO.Path.Combine(path, "groups.xlsx"));
                        break;
                    case "groups_csv":
                        await _exportService.ExportGroupsToCsvAsync(Groups, System.IO.Path.Combine(path, "groups.csv"));
                        break;
                    case "groups_pdf":
                        await _exportService.ExportGroupsToPdfAsync(Groups, System.IO.Path.Combine(path, "groups.pdf"));
                        break;
                    case "groups_json":
                        await _exportService.ExportGroupsToJsonAsync(Groups, System.IO.Path.Combine(path, "groups.json"));
                        break;
                    case "history_excel":
                        var history = await _historyService.LoadHistoryAsync();
                        await _exportService.ExportHistoryToExcelAsync(history, System.IO.Path.Combine(path, "history.xlsx"));
                        break;
                    case "history_csv":
                        history = await _historyService.LoadHistoryAsync();
                        await _exportService.ExportHistoryToCsvAsync(history, System.IO.Path.Combine(path, "history.csv"));
                        break;
                    case "history_pdf":
                        history = await _historyService.LoadHistoryAsync();
                        await _exportService.ExportHistoryToPdfAsync(history, System.IO.Path.Combine(path, "history.pdf"));
                        break;
                    case "history_json":
                        history = await _historyService.LoadHistoryAsync();
                        await _exportService.ExportHistoryToJsonAsync(history, System.IO.Path.Combine(path, "history.json"));
                        break;
                }
                
                MessageBox.Show($"导出成功: {format}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出失败: {ex.Message}");
            }
        }
    }
}
