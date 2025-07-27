using System.Text.Json;
using Serilog;

namespace RandPicker.Config;

/// <summary>
/// 统一数据服务类，管理学生和小组数据
/// </summary>
public static class DataService
{
    private static readonly string DataFilePath = Path.Combine(
        Path.GetDirectoryName(Environment.ProcessPath) ?? AppDomain.CurrentDomain.BaseDirectory,
        "students.json"
    );

    /// <summary>
    /// 加载所有数据
    /// </summary>
    public static (List<Student> students, List<Group> groups) LoadAllData()
    {
        try
        {
            if (!File.Exists(DataFilePath))
            {
                Log.Warning("数据文件不存在，创建新文件");
                var emptyData = new { students = new List<Student>(), groups = new List<Group>() };
                var emptyJson = JsonSerializer.Serialize(emptyData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(DataFilePath, emptyJson);
                return (new List<Student>(), new List<Group>());
            }

            var jsonString = File.ReadAllText(DataFilePath);
            using var document = JsonDocument.Parse(jsonString);
            var root = document.RootElement;

            var students = new List<Student>();
            var groups = new List<Group>();

            if (root.TryGetProperty("students", out var studentsElement))
            {
                students = JsonSerializer.Deserialize<List<Student>>(studentsElement.GetRawText()) ?? new List<Student>();
            }

            if (root.TryGetProperty("groups", out var groupsElement))
            {
                groups = JsonSerializer.Deserialize<List<Group>>(groupsElement.GetRawText()) ?? new List<Group>();
            }

            Log.Information($"加载了 {students.Count} 名学生和 {groups.Count} 个小组");
            return (students, groups);
        }
        catch (Exception ex)
        {
            Log.Error($"加载数据时出错: {ex.Message}");
            return (new List<Student>(), new List<Group>());
        }
    }

    /// <summary>
    /// 保存所有数据
    /// </summary>
    public static async Task<bool> SaveAllDataAsync(List<Student> students, List<Group> groups)
    {
        try
        {
            var data = new { students, groups };
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonString = JsonSerializer.Serialize(data, options);
            await File.WriteAllTextAsync(DataFilePath, jsonString);

            Log.Information($"成功保存 {students.Count} 名学生和 {groups.Count} 个小组");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"保存数据时出错: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取下一个可用的学生ID
    /// </summary>
    public static int GetNextStudentId(List<Student> students)
    {
        return students.Count > 0 ? students.Max(s => s.Id) + 1 : 1;
    }

    /// <summary>
    /// 获取下一个可用的小组ID
    /// </summary>
    public static int GetNextGroupId(List<Group> groups)
    {
        return groups.Count > 0 ? groups.Max(g => g.Id) + 1 : 1;
    }
}