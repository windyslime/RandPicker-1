using RandPicker.WPF.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandPicker.WPF.Services
{
    /// <summary>
    /// 导出服务接口
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        /// 导出学生数据到PDF
        /// </summary>
        /// <param name="students">学生集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportStudentsToPdfAsync(IEnumerable<Student> students, string filePath);
        
        /// <summary>
        /// 导出分组数据到PDF
        /// </summary>
        /// <param name="groups">分组集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportGroupsToPdfAsync(IEnumerable<Group> groups, string filePath);
        
        /// <summary>
        /// 导出历史记录到PDF
        /// </summary>
        /// <param name="history">历史记录集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportHistoryToPdfAsync(IEnumerable<HistoryItem> history, string filePath);
        
        /// <summary>
        /// 导出学生数据到Excel
        /// </summary>
        /// <param name="students">学生集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportStudentsToExcelAsync(IEnumerable<Student> students, string filePath);
        
        /// <summary>
        /// 导出分组数据到Excel
        /// </summary>
        /// <param name="groups">分组集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportGroupsToExcelAsync(IEnumerable<Group> groups, string filePath);
        
        /// <summary>
        /// 导出历史记录到Excel
        /// </summary>
        /// <param name="history">历史记录集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportHistoryToExcelAsync(IEnumerable<HistoryItem> history, string filePath);
        
        /// <summary>
        /// 导出学生数据到CSV
        /// </summary>
        /// <param name="students">学生集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportStudentsToCsvAsync(IEnumerable<Student> students, string filePath);
        
        /// <summary>
        /// 导出分组数据到CSV
        /// </summary>
        /// <param name="groups">分组集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportGroupsToCsvAsync(IEnumerable<Group> groups, string filePath);
        
        /// <summary>
        /// 导出历史记录到CSV
        /// </summary>
        /// <param name="history">历史记录集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportHistoryToCsvAsync(IEnumerable<HistoryItem> history, string filePath);
        
        /// <summary>
        /// 导出学生数据到JSON
        /// </summary>
        /// <param name="students">学生集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportStudentsToJsonAsync(IEnumerable<Student> students, string filePath);
        
        /// <summary>
        /// 导出分组数据到JSON
        /// </summary>
        /// <param name="groups">分组集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportGroupsToJsonAsync(IEnumerable<Group> groups, string filePath);
        
        /// <summary>
        /// 导出历史记录到JSON
        /// </summary>
        /// <param name="history">历史记录集合</param>
        /// <param name="filePath">文件路径，如果为空则使用默认路径</param>
        Task ExportHistoryToJsonAsync(IEnumerable<HistoryItem> history, string filePath);
        
        /// <summary>
        /// 获取默认导出路径
        /// </summary>
        /// <returns>默认导出路径</returns>
        string GetDefaultExportPath();
        
        /// <summary>
        /// 设置默认导出路径
        /// </summary>
        /// <param name="path">路径</param>
        void SetDefaultExportPath(string path);
    }
}
