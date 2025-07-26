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
    /// 学生服务实现类
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly string _dataPath;
        private readonly Random _random = new Random();
        private readonly IConfigurationService _configService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public StudentService(IConfigurationService configService)
        {
            _configService = configService;
            _dataPath = _configService.GetFilePath("students.json");
        }

        /// <summary>
        /// 加载学生数据
        /// </summary>
        public async Task<ObservableCollection<Student>> LoadStudentsAsync()
        {
            if (!File.Exists(_dataPath))
            {
                // 如果文件不存在，尝试从项目目录加载默认学生数据
                var defaultJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "default_students.json");
                if (File.Exists(defaultJsonPath))
                {
                    try
                    {
                        var defaultJson = await File.ReadAllTextAsync(defaultJsonPath);
                        var defaultData = JsonConvert.DeserializeObject<Dictionary<string, object>>(defaultJson);
                        if (defaultData != null && defaultData.ContainsKey("students"))
                        {
                            var studentsArray = defaultData["students"];
                            var students = JsonConvert.DeserializeObject<ObservableCollection<Student>>(
                                JsonConvert.SerializeObject(studentsArray));
                            if (students != null && students.Count > 0)
                            {
                                await SaveStudentsAsync(students);
                                return students;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"加载默认学生数据失败: {ex.Message}");
                    }
                }

                // 如果无法从默认文件加载，创建默认学生
                var defaultStudents = new ObservableCollection<Student>
                {
                    new Student { Name = "张三", Id = 1001, Weight = 1, Active = true },
                    new Student { Name = "李四", Id = 1002, Weight = 1, Active = true },
                    new Student { Name = "王五", Id = 1003, Weight = 1, Active = true },
                    new Student { Name = "赵六", Id = 1004, Weight = 1, Active = true },
                    new Student { Name = "钱七", Id = 1005, Weight = 1, Active = true },
                    new Student { Name = "孙八", Id = 1006, Weight = 1, Active = true },
                    new Student { Name = "周九", Id = 1007, Weight = 1, Active = true },
                    new Student { Name = "吴十", Id = 1008, Weight = 1, Active = true }
                };
                await SaveStudentsAsync(defaultStudents);
                return defaultStudents;
            }

            try
            {
                var json = await File.ReadAllTextAsync(_dataPath);
                var students = JsonConvert.DeserializeObject<ObservableCollection<Student>>(json);
                return students ?? new ObservableCollection<Student>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载学生数据失败: {ex.Message}");
                return new ObservableCollection<Student>();
            }
        }

        /// <summary>
        /// 保存学生数据
        /// </summary>
        public async Task SaveStudentsAsync(ObservableCollection<Student> students)
        {
            _configService.EnsureDirectoryExists(_dataPath);
            var json = JsonConvert.SerializeObject(students, Formatting.Indented);
            await File.WriteAllTextAsync(_dataPath, json);
        }

        /// <summary>
        /// 获取随机学生
        /// </summary>
        public Student? GetRandomStudent(ObservableCollection<Student> students)
        {
            var activeStudents = GetActiveStudents(students);
            if (activeStudents.Count == 0)
                return null;

            var index = _random.Next(activeStudents.Count);
            return activeStudents[index];
        }

        /// <summary>
        /// 获取随机学生（考虑权重）
        /// </summary>
        public Student? GetWeightedRandomStudent(ObservableCollection<Student> students)
        {
            var activeStudents = GetActiveStudents(students);
            if (activeStudents.Count == 0)
                return null;

            var totalWeight = activeStudents.Sum(s => s.Weight);
            var randomValue = _random.Next(totalWeight);

            var currentWeight = 0;
            foreach (var student in activeStudents)
            {
                currentWeight += student.Weight;
                if (randomValue < currentWeight)
                    return student;
            }

            return activeStudents.Last();
        }

        /// <summary>
        /// 更新学生权重
        /// </summary>
        public void UpdateStudentWeight(Student student, int newWeight)
        {
            if (newWeight < 1)
                newWeight = 1;
            if (newWeight > 100)
                newWeight = 100;
                
            student.Weight = newWeight;
        }

        /// <summary>
        /// 导入学生数据（从Excel）
        /// </summary>
        public async Task<ObservableCollection<Student>> ImportFromExcelAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("找不到指定的Excel文件", filePath);

            try
            {
                // 使用第三方库处理Excel文件
                // 这里使用简化的CSV格式解析（Excel可以保存为CSV）
                var extension = Path.GetExtension(filePath).ToLower();
                if (extension == ".csv")
                {
                    return await ImportFromCsvAsync(filePath);
                }
                
                // 如果是真正的Excel文件，需要使用专门的库处理
                // 这里简化处理，提示用户转换为CSV
                throw new NotSupportedException("请将Excel文件保存为CSV格式后再导入");
            }
            catch (Exception ex)
            {
                throw new Exception($"导入Excel文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 导入学生数据（从CSV）
        /// </summary>
        public async Task<ObservableCollection<Student>> ImportFromCsvAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("找不到指定的CSV文件", filePath);

            try
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                var students = new ObservableCollection<Student>();
                
                // 尝试识别标题行
                var firstLine = lines.FirstOrDefault();
                bool hasHeader = firstLine != null && 
                                (firstLine.Contains("姓名") || 
                                 firstLine.Contains("学号") || 
                                 firstLine.Contains("权重") || 
                                 firstLine.Contains("Name") || 
                                 firstLine.Contains("ID") || 
                                 firstLine.Contains("Weight"));
                
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
                        // 尝试解析不同格式的CSV
                        try
                        {
                            var student = new Student();
                            
                            // 尝试识别各列的含义
                            if (parts.Length >= 4)
                            {
                                // 假设格式为: 权重,姓名,学号,启用状态
                                if (int.TryParse(parts[0], out var weight))
                                    student.Weight = weight;
                                    
                                student.Name = parts[1];
                                
                                if (int.TryParse(parts[2], out var id))
                                    student.Id = id;
                                    
                                if (bool.TryParse(parts[3], out var active))
                                    student.Active = active;
                            }
                            else if (parts.Length == 3)
                            {
                                // 假设格式为: 姓名,学号,权重
                                student.Name = parts[0];
                                
                                if (int.TryParse(parts[1], out var id))
                                    student.Id = id;
                                    
                                if (int.TryParse(parts[2], out var weight))
                                    student.Weight = weight;
                                    
                                student.Active = true;
                            }
                            else if (parts.Length == 2)
                            {
                                // 假设格式为: 姓名,学号
                                student.Name = parts[0];
                                
                                if (int.TryParse(parts[1], out var id))
                                    student.Id = id;
                                    
                                student.Weight = 1;
                                student.Active = true;
                            }
                            
                            // 确保学生数据有效
                            if (!string.IsNullOrEmpty(student.Name) && student.Id > 0)
                            {
                                students.Add(student);
                            }
                        }
                        catch
                        {
                            // 忽略无法解析的行
                            continue;
                        }
                    }
                }
                
                return students;
            }
            catch (Exception ex)
            {
                throw new Exception($"导入CSV文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 导出学生数据（到Excel）
        /// </summary>
        public async Task ExportToExcelAsync(IEnumerable<Student> students, string filePath)
        {
            try
            {
                // 使用CSV格式导出，可以被Excel打开
                await ExportToCsvAsync(students, filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"导出Excel文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 导出学生数据（到CSV）
        /// </summary>
        public async Task ExportToCsvAsync(IEnumerable<Student> students, string filePath)
        {
            try
            {
                var lines = new List<string>
                {
                    "权重,姓名,学号,启用状态,头像路径"
                };
                
                foreach (var student in students)
                {
                    // 处理CSV中的特殊字符
                    var name = student.Name.Contains(",") ? $"\"{student.Name}\"" : student.Name;
                    var avatarPath = string.IsNullOrEmpty(student.AvatarPath) ? "" : 
                                    (student.AvatarPath.Contains(",") ? $"\"{student.AvatarPath}\"" : student.AvatarPath);
                    
                    lines.Add($"{student.Weight},{name},{student.Id},{student.Active},{avatarPath}");
                }
                
                await File.WriteAllLinesAsync(filePath, lines);
            }
            catch (Exception ex)
            {
                throw new Exception($"导出CSV文件失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 获取活跃学生
        /// </summary>
        public ObservableCollection<Student> GetActiveStudents(ObservableCollection<Student> students)
        {
            return new ObservableCollection<Student>(students.Where(s => s.Active));
        }

        /// <summary>
        /// 重置所有学生权重
        /// </summary>
        public void ResetAllWeights(ObservableCollection<Student> students, int weight = 1)
        {
            foreach (var student in students)
            {
                student.Weight = weight;
            }
        }

        /// <summary>
        /// 重置所有学生状态
        /// </summary>
        public void ResetAllActive(ObservableCollection<Student> students, bool active = true)
        {
            foreach (var student in students)
            {
                student.Active = active;
            }
        }
        
        /// <summary>
        /// 添加学生
        /// </summary>
        public async Task<bool> AddStudentAsync(Student student, ObservableCollection<Student> students)
        {
            // 检查是否存在相同学号的学生
            if (students.Any(s => s.Id == student.Id))
                return false;
                
            students.Add(student);
            await SaveStudentsAsync(students);
            return true;
        }
        
        /// <summary>
        /// 更新学生
        /// </summary>
        public async Task<bool> UpdateStudentAsync(Student student, ObservableCollection<Student> students)
        {
            var existingStudent = students.FirstOrDefault(s => s.Id == student.Id);
            if (existingStudent == null)
                return false;
                
            existingStudent.Name = student.Name;
            existingStudent.Weight = student.Weight;
            existingStudent.Active = student.Active;
            existingStudent.AvatarPath = student.AvatarPath;
            
            await SaveStudentsAsync(students);
            return true;
        }
        
        /// <summary>
        /// 删除学生
        /// </summary>
        public async Task<bool> DeleteStudentAsync(Student student, ObservableCollection<Student> students)
        {
            var result = students.Remove(student);
            if (result)
                await SaveStudentsAsync(students);
            return result;
        }
    }
}
