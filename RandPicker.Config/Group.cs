using System.Text.Json.Serialization;

namespace RandPicker.Config;

/// <summary>
/// 小组信息类
/// </summary>
public class Group
{
    [JsonPropertyName("id")] public int Id { get; set; }
    
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
    
    [JsonPropertyName("studentIds")] public List<int> StudentIds { get; set; } = new();
    
    [JsonPropertyName("createdTime")] public DateTime CreatedTime { get; set; } = DateTime.Now;
    
    [JsonPropertyName("isActive")] public bool IsActive { get; set; } = true;

    public override string ToString() => $"{Name} ({StudentIds.Count}人)";
}

/// <summary>
/// 小组列表类
/// </summary>
public class GroupData
{
    [JsonPropertyName("groups")] public List<Group> Groups { get; set; } = new();
}
