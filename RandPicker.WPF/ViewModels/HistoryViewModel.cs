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
    /// 历史记录视图模型
    /// </summary>
    public partial class HistoryViewModel : ObservableObject
    {
        private readonly IHistoryService _historyService;
        private readonly IExportService _exportService;

        /// <summary>
        /// 历史记录集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<HistoryItem> historyItems = new();

        /// <summary>
        /// 选中的历史记录
        /// </summary>
        [ObservableProperty]
        private HistoryItem? selectedHistoryItem;

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
        /// 过滤后的历史记录集合
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<HistoryItem> filteredHistoryItems = new();

        /// <summary>
        /// 开始日期
        /// </summary>
        [ObservableProperty]
        private DateTime startDate = DateTime.Today.AddDays(-30);

        /// <summary>
        /// 结束日期
        /// </summary>
        [ObservableProperty]
        private DateTime endDate = DateTime.Today;

        /// <summary>
        /// 选择模式
        /// </summary>
        [ObservableProperty]
        private string selectedMode = "全部";

        /// <summary>
        /// 清除历史记录命令
        /// </summary>
        public ICommand ClearHistoryCommand { get; }

        /// <summary>
        /// 导出历史记录命令
        /// </summary>
        public ICommand ExportHistoryCommand { get; }

        /// <summary>
        /// 搜索命令
        /// </summary>
        public ICommand SearchCommand { get; }

        /// <summary>
        /// 刷新命令
        /// </summary>
        public ICommand RefreshCommand { get; }

        /// <summary>
        /// 过滤日期命令
        /// </summary>
        public ICommand FilterByDateCommand { get; }

        /// <summary>
        /// 过滤模式命令
        /// </summary>
        public ICommand FilterByModeCommand { get; }

        /// <summary>
        /// 可用模式列表
        /// </summary>
        public ObservableCollection<string> AvailableModes { get; } = new ObservableCollection<string>
        {
            "全部",
            "个人选择",
            "分组选择"
        };

        /// <summary>
        /// 构造函数
        /// </summary>
        public HistoryViewModel(IHistoryService historyService, IExportService exportService)
        {
            _historyService = historyService;
            _exportService = exportService;

            ClearHistoryCommand = new AsyncRelayCommand(ClearHistoryAsync);
            ExportHistoryCommand = new AsyncRelayCommand<string>(ExportHistoryAsync);
            SearchCommand = new RelayCommand(Search);
            RefreshCommand = new AsyncRelayCommand(LoadHistoryAsync);
            FilterByDateCommand = new AsyncRelayCommand(FilterByDateAsync);
            FilterByModeCommand = new AsyncRelayCommand(FilterByModeAsync);

            _ = LoadHistoryAsync();
        }

        /// <summary>
        /// 加载历史记录
        /// </summary>
        private async Task LoadHistoryAsync()
        {
            IsBusy = true;
            HistoryItems = await _historyService.LoadHistoryAsync();
            FilterHistory();
            IsBusy = false;
        }

        /// <summary>
        /// 清除历史记录
        /// </summary>
        private async Task ClearHistoryAsync()
        {
            var result = MessageBox.Show("确定要清除所有历史记录吗？此操作不可恢复。", "确认清除", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                IsBusy = true;
                await _historyService.ClearHistoryAsync();
                HistoryItems.Clear();
                FilteredHistoryItems.Clear();
                IsBusy = false;
                MessageBox.Show("历史记录已清除", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 导出历史记录
        /// </summary>
        private async Task ExportHistoryAsync(string format)
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

            dialog.FileName = $"历史记录{extension}";
            dialog.Title = "保存历史记录";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    IsBusy = true;
                    switch (format.ToLower())
                    {
                        case "excel":
                            await _exportService.ExportHistoryToExcelAsync(FilteredHistoryItems, dialog.FileName);
                            break;
                        case "csv":
                            await _exportService.ExportHistoryToCsvAsync(FilteredHistoryItems, dialog.FileName);
                            break;
                        case "pdf":
                            await _exportService.ExportHistoryToPdfAsync(FilteredHistoryItems, dialog.FileName);
                            break;
                        case "json":
                            await _exportService.ExportHistoryToJsonAsync(FilteredHistoryItems, dialog.FileName);
                            break;
                    }
                    MessageBox.Show($"历史记录已成功导出到: {dialog.FileName}", "导出成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出历史记录失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
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
            FilterHistory();
        }

        /// <summary>
        /// 按日期过滤
        /// </summary>
        private async Task FilterByDateAsync()
        {
            if (StartDate > EndDate)
            {
                MessageBox.Show("开始日期不能晚于结束日期", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IsBusy = true;
            var history = await _historyService.GetHistoryByDateRangeAsync(StartDate, EndDate.AddDays(1).AddSeconds(-1));
            HistoryItems = history;
            FilterHistory();
            IsBusy = false;
        }

        /// <summary>
        /// 按模式过滤
        /// </summary>
        private async Task FilterByModeAsync()
        {
            IsBusy = true;
            
            if (SelectedMode == "全部")
            {
                await LoadHistoryAsync();
            }
            else
            {
                var mode = SelectedMode == "个人选择" ? SelectionMode.Individual : SelectionMode.Group;
                var history = await _historyService.GetHistoryByModeAsync(mode);
                HistoryItems = history;
                FilterHistory();
            }
            
            IsBusy = false;
        }

        /// <summary>
        /// 过滤历史记录
        /// </summary>
        private void FilterHistory()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredHistoryItems = new ObservableCollection<HistoryItem>(HistoryItems);
            }
            else
            {
                var searchLower = SearchText.ToLower();
                FilteredHistoryItems = new ObservableCollection<HistoryItem>(
                    HistoryItems.Where(h =>
                        h.SelectedName.ToLower().Contains(searchLower) ||
                        (h.Details != null && h.Details.ToLower().Contains(searchLower))
                    )
                );
            }
        }

        /// <summary>
        /// 获取历史记录统计信息
        /// </summary>
        public async Task<Dictionary<string, int>> GetHistoryStatisticsAsync()
        {
            return await _historyService.GetHistoryStatisticsAsync();
        }
    }
}