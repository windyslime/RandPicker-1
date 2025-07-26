using Newtonsoft.Json;
using RandPicker.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RandPicker.WPF.Services
{
    /// <summary>
    /// 历史记录服务实现类
    /// </summary>
    public class HistoryService : IHistoryService
    {
        private readonly string _dataPath;
        private readonly IConfigurationService _configService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HistoryService(IConfigurationService configService)
        {
            _configService = configService;
            _dataPath = _configService.GetFilePath("history.json");
        }

        /// <summary>
        /// 加载历史记录
        /// </summary>
        public async Task<ObservableCollection<HistoryItem>> LoadHistoryAsync()
        {
            if (!File.Exists(_dataPath))
            {
                var history = new ObservableCollection<HistoryItem>();
                await SaveHistoryAsync(history);
                return history;
            }

            try
            {
                var json = await File.ReadAllTextAsync(_dataPath);
                var historyItems = JsonConvert.DeserializeObject<ObservableCollection<HistoryItem>>(json);
                return historyItems ?? new ObservableCollection<HistoryItem>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载历史记录失败: {ex.Message}");
                return new ObservableCollection<HistoryItem>();
            }
        }

        /// <summary>
        /// 添加历史记录项
        /// </summary>
        public async Task AddHistoryItemAsync(HistoryItem item)
        {
            // 检查是否需要记录历史
            if (!_configService.RecordHistory)
                return;
                
            var history = await LoadHistoryAsync();
            history.Insert(0, item);
            
            // 限制历史记录数量
            if (history.Count > 1000)
            {
                while (history.Count > 1000)
                {
                    history.RemoveAt(history.Count - 1);
                }
            }

            await SaveHistoryAsync(history);
        }

        /// <summary>
        /// 清除历史记录
        /// </summary>
        public async Task ClearHistoryAsync()
        {
            var history = new ObservableCollection<HistoryItem>();
            await SaveHistoryAsync(history);
        }

        /// <summary>
        /// 获取最近的历史记录
        /// </summary>
        public async Task<ObservableCollection<HistoryItem>> GetRecentHistoryAsync(int count = 50)
        {
            var history = await LoadHistoryAsync();
            var recent = history.Take(count);
            return new ObservableCollection<HistoryItem>(recent);
        }

        /// <summary>
        /// 获取指定日期范围的历史记录
        /// </summary>
        public async Task<ObservableCollection<HistoryItem>> GetHistoryByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var history = await LoadHistoryAsync();
            // 确保结束日期包含整天
            var adjustedEndDate = endDate.Date.AddDays(1).AddSeconds(-1);
            var filtered = history.Where(h => h.SelectionTime >= startDate && h.SelectionTime <= adjustedEndDate);
            return new ObservableCollection<HistoryItem>(filtered);
        }

        /// <summary>
        /// 获取指定模式的历史记录
        /// </summary>
        public async Task<ObservableCollection<HistoryItem>> GetHistoryByModeAsync(SelectionMode mode)
        {
            var history = await LoadHistoryAsync();
            var filtered = history.Where(h => h.Mode == mode);
            return new ObservableCollection<HistoryItem>(filtered);
        }

        /// <summary>
        /// 导出历史记录
        /// </summary>
        public async Task ExportHistoryAsync(IEnumerable<HistoryItem> history, string filePath)
        {
            try
            {
                var lines = new List<string>
                {
                    "选择模式,选中名称,选择时间,详细信息"
                };
                
                foreach (var item in history)
                {
                    var mode = item.Mode == SelectionMode.Individual ? "个人" : "分组";
                    // 处理CSV中的特殊字符
                    var name = item.SelectedName.Contains(",") ? $"\"{item.SelectedName}\"" : item.SelectedName;
                    var details = string.IsNullOrEmpty(item.Details) ? "" : 
                                 (item.Details.Contains(",") ? $"\"{item.Details}\"" : item.Details);
                    
                    lines.Add($"{mode},{name},{item.SelectionTime:yyyy-MM-dd HH:mm:ss},{details}");
                }
                
                await File.WriteAllLinesAsync(filePath, lines);
            }
            catch (Exception ex)
            {
                throw new Exception($"导出历史记录失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取历史记录统计信息
        /// </summary>
        public async Task<Dictionary<string, int>> GetHistoryStatisticsAsync()
        {
            var history = await LoadHistoryAsync();
            var stats = new Dictionary<string, int>();
            
            // 统计个人选择次数
            var individualItems = history.Where(h => h.Mode == SelectionMode.Individual);
            foreach (var item in individualItems)
            {
                if (stats.ContainsKey(item.SelectedName))
                {
                    stats[item.SelectedName]++;
                }
                else
                {
                    stats[item.SelectedName] = 1;
                }
            }
            
            return stats;
        }

        /// <summary>
        /// 保存历史记录
        /// </summary>
        private async Task SaveHistoryAsync(ObservableCollection<HistoryItem> history)
        {
            _configService.EnsureDirectoryExists(_dataPath);
            var json = JsonConvert.SerializeObject(history, Formatting.Indented);
            await File.WriteAllTextAsync(_dataPath, json);
        }
        
        /// <summary>
        /// 搜索历史记录
        /// </summary>
        public async Task<ObservableCollection<HistoryItem>> SearchHistoryAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await LoadHistoryAsync();
                
            var history = await LoadHistoryAsync();
            var filtered = history.Where(h => 
                h.SelectedName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                (h.Details != null && h.Details.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
                
            return new ObservableCollection<HistoryItem>(filtered);
        }
        
        /// <summary>
        /// 获取复合筛选的历史记录
        /// </summary>
        public async Task<ObservableCollection<HistoryItem>> GetFilteredHistoryAsync(
            string searchText = null, 
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            SelectionMode? mode = null)
        {
            var history = await LoadHistoryAsync();
            var query = history.AsEnumerable();
            
            // 应用搜索文本筛选
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(h => 
                    h.SelectedName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    (h.Details != null && h.Details.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
            }
            
            // 应用开始日期筛选
            if (startDate.HasValue)
            {
                query = query.Where(h => h.SelectionTime >= startDate.Value);
            }
            
            // 应用结束日期筛选
            if (endDate.HasValue)
            {
                // 确保结束日期包含整天
                var adjustedEndDate = endDate.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(h => h.SelectionTime <= adjustedEndDate);
            }
            
            // 应用模式筛选
            if (mode.HasValue)
            {
                query = query.Where(h => h.Mode == mode.Value);
            }
            
            return new ObservableCollection<HistoryItem>(query);
        }
    }
}
