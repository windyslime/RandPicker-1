using RandPicker.WPF.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RandPicker.WPF.Services
{
    /// <summary>
    /// 历史记录服务接口
    /// </summary>
    public interface IHistoryService
    {
        /// <summary>
        /// 加载历史记录
        /// </summary>
        /// <returns>历史记录集合</returns>
        Task<ObservableCollection<HistoryItem>> LoadHistoryAsync();
        
        /// <summary>
        /// 添加历史记录项
        /// </summary>
        /// <param name="item">历史记录项</param>
        Task AddHistoryItemAsync(HistoryItem item);
        
        /// <summary>
        /// 清除历史记录
        /// </summary>
        Task ClearHistoryAsync();
        
        /// <summary>
        /// 获取最近的历史记录
        /// </summary>
        /// <param name="count">数量</param>
        /// <returns>历史记录集合</returns>
        Task<ObservableCollection<HistoryItem>> GetRecentHistoryAsync(int count = 50);
        
        /// <summary>
        /// 获取指定日期范围的历史记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>历史记录集合</returns>
        Task<ObservableCollection<HistoryItem>> GetHistoryByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// 获取指定模式的历史记录
        /// </summary>
        /// <param name="mode">选择模式</param>
        /// <returns>历史记录集合</returns>
        Task<ObservableCollection<HistoryItem>> GetHistoryByModeAsync(SelectionMode mode);
        
        /// <summary>
        /// 导出历史记录
        /// </summary>
        /// <param name="history">历史记录集合</param>
        /// <param name="filePath">文件路径</param>
        Task ExportHistoryAsync(IEnumerable<HistoryItem> history, string filePath);
        
        /// <summary>
        /// 获取历史记录统计信息
        /// </summary>
        /// <returns>统计信息字典</returns>
        Task<Dictionary<string, int>> GetHistoryStatisticsAsync();
    }
}
