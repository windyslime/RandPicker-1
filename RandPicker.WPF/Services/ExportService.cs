using Newtonsoft.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using RandPicker.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandPicker.WPF.Services
{
    /// <summary>
    /// 导出服务实现类
    /// </summary>
    public class ExportService : IExportService
    {
        private readonly IConfigurationService _configService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExportService(IConfigurationService configService)
        {
            _configService = configService;
        }
        /// <summary>
        /// 导出学生数据到PDF
        /// </summary>
        public async Task ExportStudentsToPdfAsync(IEnumerable<Student> students, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"学生列表_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            
            await Task.Run(() =>
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 12);
                var titleFont = new XFont("Arial", 16, XFontStyle.Bold);

                gfx.DrawString("学生列表", titleFont, XBrushes.Black, new XRect(0, 20, page.Width, 40), XStringFormats.Center);

                var y = 80;
                foreach (var student in students)
                {
                    gfx.DrawString($"{student.Id} - {student.Name} (权重: {student.Weight}, 状态: {(student.Active ? "启用" : "禁用")})", 
                        font, XBrushes.Black, new XRect(40, y, page.Width - 80, 20));
                    y += 20;
                    
                    // 如果页面剩余空间不足，添加新页面
                    if (y > page.Height - 40)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 40;
                    }
                }

                // 确保目录存在
                _configService.EnsureDirectoryExists(filePath);
                document.Save(filePath);
            });
        }

        /// <summary>
        /// 导出分组数据到PDF
        /// </summary>
        public async Task ExportGroupsToPdfAsync(IEnumerable<Group> groups, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"分组列表_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            
            await Task.Run(() =>
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 12);
                var titleFont = new XFont("Arial", 16, XFontStyle.Bold);
                var groupFont = new XFont("Arial", 14, XFontStyle.Bold);

                gfx.DrawString("分组列表", titleFont, XBrushes.Black, new XRect(0, 20, page.Width, 40), XStringFormats.Center);

                var y = 80;
                foreach (var group in groups)
                {
                    gfx.DrawString($"{group.Name} (学生数量: {group.Students.Count})", 
                        groupFont, XBrushes.Black, new XRect(40, y, page.Width - 80, 20));
                    y += 30;
                    
                    foreach (var student in group.Students)
                    {
                        gfx.DrawString($"  - {student.Name} (学号: {student.Id}, 权重: {student.Weight})", 
                            font, XBrushes.Black, new XRect(60, y, page.Width - 80, 20));
                        y += 20;
                        
                        // 如果页面剩余空间不足，添加新页面
                        if (y > page.Height - 40)
                        {
                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            y = 40;
                        }
                    }
                    y += 20;
                    
                    // 如果页面剩余空间不足，添加新页面
                    if (y > page.Height - 60)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 40;
                    }
                }

                // 确保目录存在
                _configService.EnsureDirectoryExists(filePath);
                document.Save(filePath);
            });
        }

        /// <summary>
        /// 导出历史记录到PDF
        /// </summary>
        public async Task ExportHistoryToPdfAsync(IEnumerable<HistoryItem> history, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"历史记录_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            
            await Task.Run(() =>
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 12);
                var titleFont = new XFont("Arial", 16, XFontStyle.Bold);

                gfx.DrawString("选择历史", titleFont, XBrushes.Black, new XRect(0, 20, page.Width, 40), XStringFormats.Center);

                var y = 80;
                foreach (var item in history)
                {
                    var mode = item.Mode == SelectionMode.Individual ? "学生" : "分组";
                    gfx.DrawString($"[{item.SelectionTime:yyyy-MM-dd HH:mm:ss}] {mode}: {item.SelectedName}", 
                        font, XBrushes.Black, new XRect(40, y, page.Width - 80, 20));
                    y += 20;
                    
                    if (!string.IsNullOrEmpty(item.Details))
                    {
                        gfx.DrawString($"  详情: {item.Details}", 
                            font, XBrushes.Black, new XRect(60, y, page.Width - 80, 20));
                        y += 20;
                    }
                    
                    // 如果页面剩余空间不足，添加新页面
                    if (y > page.Height - 40)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        y = 40;
                    }
                }

                // 确保目录存在
                _configService.EnsureDirectoryExists(filePath);
                document.Save(filePath);
            });
        }

        /// <summary>
        /// 导出学生数据到Excel
        /// </summary>
        public async Task ExportStudentsToExcelAsync(IEnumerable<Student> students, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"学生列表_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            
            await Task.Run(() =>
            {
                var lines = new List<string> { "学号,姓名,权重,状态,头像路径" };
                foreach (var student in students)
                {
                    // 处理CSV中的特殊字符
                    var name = student.Name.Contains(",") ? $"\"{student.Name}\"" : student.Name;
                    var avatarPath = string.IsNullOrEmpty(student.AvatarPath) ? "" : 
                                    (student.AvatarPath.Contains(",") ? $"\"{student.AvatarPath}\"" : student.AvatarPath);
                    
                    lines.Add($"{student.Id},{name},{student.Weight},{(student.Active ? "启用" : "禁用")},{avatarPath}");
                }
                
                // 确保目录存在
                _configService.EnsureDirectoryExists(filePath);
                File.WriteAllLines(filePath, lines, Encoding.UTF8);
            });
        }

        /// <summary>
        /// 导出分组数据到Excel
        /// </summary>
        public async Task ExportGroupsToExcelAsync(IEnumerable<Group> groups, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"分组列表_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            
            await Task.Run(() =>
            {
                var lines = new List<string> { "分组名称,学生数量,成员列表" };
                foreach (var group in groups)
                {
                    var members = string.Join(";", group.Students.Select(s => s.Name));
                    // 处理CSV中的特殊字符
                    var name = group.Name.Contains(",") ? $"\"{group.Name}\"" : group.Name;
                    var membersList = members.Contains(",") ? $"\"{members}\"" : members;
                    
                    lines.Add($"{name},{group.Students.Count},{membersList}");
                }
                
                // 确保目录存在
                _configService.EnsureDirectoryExists(filePath);
                File.WriteAllLines(filePath, lines, Encoding.UTF8);
            });
        }

        /// <summary>
        /// 导出历史记录到Excel
        /// </summary>
        public async Task ExportHistoryToExcelAsync(IEnumerable<HistoryItem> history, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"历史记录_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            
            await Task.Run(() =>
            {
                var lines = new List<string> { "时间,类型,选择结果,详情" };
                foreach (var item in history)
                {
                    var mode = item.Mode == SelectionMode.Individual ? "学生" : "分组";
                    // 处理CSV中的特殊字符
                    var name = item.SelectedName.Contains(",") ? $"\"{item.SelectedName}\"" : item.SelectedName;
                    var details = string.IsNullOrEmpty(item.Details) ? "" : 
                                 (item.Details.Contains(",") ? $"\"{item.Details}\"" : item.Details);
                    
                    lines.Add($"{item.SelectionTime:yyyy-MM-dd HH:mm:ss},{mode},{name},{details}");
                }
                
                // 确保目录存在
                _configService.EnsureDirectoryExists(filePath);
                File.WriteAllLines(filePath, lines, Encoding.UTF8);
            });
        }

        /// <summary>
        /// 导出学生数据到CSV
        /// </summary>
        public async Task ExportStudentsToCsvAsync(IEnumerable<Student> students, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"学生列表_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            
            await ExportStudentsToExcelAsync(students, filePath);
        }

        /// <summary>
        /// 导出分组数据到CSV
        /// </summary>
        public async Task ExportGroupsToCsvAsync(IEnumerable<Group> groups, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"分组列表_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            
            await ExportGroupsToExcelAsync(groups, filePath);
        }

        /// <summary>
        /// 导出历史记录到CSV
        /// </summary>
        public async Task ExportHistoryToCsvAsync(IEnumerable<HistoryItem> history, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"历史记录_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            }
            
            await ExportHistoryToExcelAsync(history, filePath);
        }

        /// <summary>
        /// 导出学生数据到JSON
        /// </summary>
        public async Task ExportStudentsToJsonAsync(IEnumerable<Student> students, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"学生列表_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            }
            
            await Task.Run(() =>
            {
                var data = new Dictionary<string, object>
                {
                    ["students"] = students,
                    ["exportTime"] = DateTime.Now,
                    ["count"] = students.Count()
                };
                
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                
                // 确保目录存在
                _configService.EnsureDirectoryExists(filePath);
                File.WriteAllText(filePath, json, Encoding.UTF8);
            });
        }

        /// <summary>
        /// 导出分组数据到JSON
        /// </summary>
        public async Task ExportGroupsToJsonAsync(IEnumerable<Group> groups, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"分组列表_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            }
            
            await Task.Run(() =>
            {
                var data = new Dictionary<string, object>
                {
                    ["groups"] = groups,
                    ["exportTime"] = DateTime.Now,
                    ["count"] = groups.Count()
                };
                
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                
                // 确保目录存在
                _configService.EnsureDirectoryExists(filePath);
                File.WriteAllText(filePath, json, Encoding.UTF8);
            });
        }

        /// <summary>
        /// 导出历史记录到JSON
        /// </summary>
        public async Task ExportHistoryToJsonAsync(IEnumerable<HistoryItem> history, string filePath)
        {
            // 如果未指定路径，使用配置中的默认导出路径
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = Path.Combine(_configService.ExportPath, $"历史记录_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            }
            
            await Task.Run(() =>
            {
                var data = new Dictionary<string, object>
                {
                    ["history"] = history,
                    ["exportTime"] = DateTime.Now,
                    ["count"] = history.Count()
                };
                
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                
                // 确保目录存在
                _configService.EnsureDirectoryExists(filePath);
                File.WriteAllText(filePath, json, Encoding.UTF8);
            });
        }
        
        /// <summary>
        /// 获取默认导出路径
        /// </summary>
        public string GetDefaultExportPath()
        {
            return _configService.ExportPath;
        }
        
        /// <summary>
        /// 设置默认导出路径
        /// </summary>
        public void SetDefaultExportPath(string path)
        {
            _configService.ExportPath = path;
            _configService.Save();
        }
    }
}
