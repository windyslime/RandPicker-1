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
    /// 分组服务实现类
    /// </summary>
    public class GroupService : IGroupService
    {
        private readonly string _dataPath;
        private readonly Random _random = new Random();
        private readonly IConfigurationService _configService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public GroupService(IConfigurationService configService)
        {
            _configService = configService;
            _dataPath = _configService.GetFilePath("groups.json");
        }

        /// <summary>
        /// 加载分组数据
        /// </summary>
        public async Task<ObservableCollection<Group>> LoadGroupsAsync()
        {
            if (!File.Exists(_dataPath))
            {
                // 如果文件不存在，尝试从项目目录加载默认分组数据
                var defaultJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "default_groups.json");
                if (File.Exists(defaultJsonPath))
                {
                    try
                    {
                        var defaultJson = await File.ReadAllTextAsync(defaultJsonPath);
                        var groups = JsonConvert.DeserializeObject<ObservableCollection<Group>>(defaultJson);
                        if (groups != null && groups.Count > 0)
                        {
                            await SaveGroupsAsync(groups);
                            return groups;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"加载默认分组数据失败: {ex.Message}");
                    }
                }

                // 如果无法从默认文件加载，创建空分组集合
                var defaultGroups = new ObservableCollection<Group>();
                await SaveGroupsAsync(defaultGroups);
                return defaultGroups;
            }

            try
            {
                var json = await File.ReadAllTextAsync(_dataPath);
                var groups = JsonConvert.DeserializeObject<ObservableCollection<Group>>(json);
                return groups ?? new ObservableCollection<Group>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载分组数据失败: {ex.Message}");
                return new ObservableCollection<Group>();
            }
        }

        /// <summary>
        /// 保存分组数据
        /// </summary>
        public async Task SaveGroupsAsync(ObservableCollection<Group> groups)
        {
            _configService.EnsureDirectoryExists(_dataPath);
            var json = JsonConvert.SerializeObject(groups, Formatting.Indented);
            await File.WriteAllTextAsync(_dataPath, json);
        }

        /// <summary>
        /// 获取随机分组
        /// </summary>
        public Group? GetRandomGroup(ObservableCollection<Group> groups)
        {
            if (groups.Count == 0)
                return null;

            var index = _random.Next(groups.Count);
            return groups[index];
        }

        /// <summary>
        /// 添加分组
        /// </summary>
        public async Task AddGroupAsync(string name, ObservableCollection<Student> students)
        {
            var groups = await LoadGroupsAsync();
            var group = new Group { Name = name };
            foreach (var student in students)
            {
                group.Students.Add(student);
            }
            groups.Add(group);
            await SaveGroupsAsync(groups);
        }

        /// <summary>
        /// 更新分组
        /// </summary>
        public async Task UpdateGroupAsync(Group group, string name, ObservableCollection<Student> students)
        {
            var groups = await LoadGroupsAsync();
            var existingGroup = groups.FirstOrDefault(g => g.Name == group.Name);
            if (existingGroup != null)
            {
                existingGroup.Name = name;
                existingGroup.Students.Clear();
                foreach (var student in students)
                {
                    existingGroup.Students.Add(student);
                }
                await SaveGroupsAsync(groups);
            }
        }

        /// <summary>
        /// 删除分组
        /// </summary>
        public async Task RemoveGroupAsync(Group group)
        {
            var groups = await LoadGroupsAsync();
            var existingGroup = groups.FirstOrDefault(g => g.Name == group.Name);
            if (existingGroup != null)
            {
                groups.Remove(existingGroup);
                await SaveGroupsAsync(groups);
            }
        }

        /// <summary>
        /// 获取启用的分组
        /// </summary>
        public ObservableCollection<Group> GetEnabledGroups(ObservableCollection<Group> groups, IEnumerable<int> enabledGroupIndices)
        {
            var result = new ObservableCollection<Group>();
            var indices = enabledGroupIndices.ToList();
            
            foreach (var index in indices)
            {
                if (index >= 0 && index < groups.Count)
                {
                    result.Add(groups[index]);
                }
            }
            
            return result;
        }

        /// <summary>
        /// 获取分组中的学生
        /// </summary>
        public ObservableCollection<Student> GetStudentsInGroup(Group group)
        {
            return group.Students;
        }

        /// <summary>
        /// 获取分组中的活跃学生
        /// </summary>
        public ObservableCollection<Student> GetActiveStudentsInGroup(Group group)
        {
            return new ObservableCollection<Student>(group.Students.Where(s => s.Active));
        }

        /// <summary>
        /// 导出分组数据
        /// </summary>
        public async Task ExportGroupsAsync(IEnumerable<Group> groups, string filePath)
        {
            try
            {
                var lines = new List<string>
                {
                    "分组名称,学生数量,学生列表"
                };
                
                foreach (var group in groups)
                {
                    var studentNames = string.Join(";", group.Students.Select(s => s.Name));
                    // 处理CSV中的特殊字符
                    var name = group.Name.Contains(",") ? $"\"{group.Name}\"" : group.Name;
                    var studentList = studentNames.Contains(",") ? $"\"{studentNames}\"" : studentNames;
                    
                    lines.Add($"{name},{group.Students.Count},{studentList}");
                }
                
                await File.WriteAllLinesAsync(filePath, lines);
            }
            catch (Exception ex)
            {
                throw new Exception($"导出分组数据失败: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 导入分组数据
        /// </summary>
        public async Task<ObservableCollection<Group>> ImportGroupsAsync(string filePath, ObservableCollection<Student> allStudents)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("找不到指定的文件", filePath);
                
            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                var groups = new ObservableCollection<Group>();
                
                // 尝试识别标题行
                var firstLine = lines.FirstOrDefault();
                bool hasHeader = firstLine != null && 
                                (firstLine.Contains("分组名称") || 
                                 firstLine.Contains("学生数量") || 
                                 firstLine.Contains("学生列表") || 
                                 firstLine.Contains("Group") || 
                                 firstLine.Contains("Count") || 
                                 firstLine.Contains("Students"));
                
                // 从适当的行开始处理
                int startIndex = hasHeader ? 1 : 0;
                
                for (int i = startIndex; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (string.IsNullOrEmpty(line))
                        continue;
                        
                    var parts = line.Split(',');
                    if (parts.Length >= 2)
                    {
                        var group = new Group { Name = parts[0] };
                        
                        // 如果有学生列表
                        if (parts.Length >= 3)
                        {
                            var studentNames = parts[2].Split(';');
                            foreach (var name in studentNames)
                            {
                                var student = allStudents.FirstOrDefault(s => s.Name == name);
                                if (student != null)
                                {
                                    group.Students.Add(student);
                                }
                            }
                        }
                        
                        groups.Add(group);
                    }
                }
                
                return groups;
            }
            catch (Exception ex)
            {
                throw new Exception($"导入分组数据失败: {ex.Message}", ex);
            }
        }
    }
}
