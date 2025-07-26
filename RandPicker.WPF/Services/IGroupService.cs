using RandPicker.WPF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RandPicker.WPF.Services
{
    /// <summary>
    /// 分组服务接口
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// 加载分组数据
        /// </summary>
        /// <returns>分组集合</returns>
        Task<ObservableCollection<Group>> LoadGroupsAsync();
        
        /// <summary>
        /// 保存分组数据
        /// </summary>
        /// <param name="groups">分组集合</param>
        Task SaveGroupsAsync(ObservableCollection<Group> groups);
        
        /// <summary>
        /// 获取随机分组
        /// </summary>
        /// <param name="groups">分组集合</param>
        /// <returns>随机分组</returns>
        Group? GetRandomGroup(ObservableCollection<Group> groups);
        
        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="name">分组名称</param>
        /// <param name="students">学生集合</param>
        Task AddGroupAsync(string name, ObservableCollection<Student> students);
        
        /// <summary>
        /// 更新分组
        /// </summary>
        /// <param name="group">分组</param>
        /// <param name="name">新名称</param>
        /// <param name="students">新学生集合</param>
        Task UpdateGroupAsync(Group group, string name, ObservableCollection<Student> students);
        
        /// <summary>
        /// 删除分组
        /// </summary>
        /// <param name="group">分组</param>
        Task RemoveGroupAsync(Group group);
        
        /// <summary>
        /// 获取启用的分组
        /// </summary>
        /// <param name="groups">分组集合</param>
        /// <param name="enabledGroupIndices">启用的分组索引</param>
        /// <returns>启用的分组集合</returns>
        ObservableCollection<Group> GetEnabledGroups(ObservableCollection<Group> groups, IEnumerable<int> enabledGroupIndices);
        
        /// <summary>
        /// 获取分组中的学生
        /// </summary>
        /// <param name="group">分组</param>
        /// <returns>学生集合</returns>
        ObservableCollection<Student> GetStudentsInGroup(Group group);
        
        /// <summary>
        /// 获取分组中的活跃学生
        /// </summary>
        /// <param name="group">分组</param>
        /// <returns>活跃学生集合</returns>
        ObservableCollection<Student> GetActiveStudentsInGroup(Group group);
        
        /// <summary>
        /// 导出分组数据
        /// </summary>
        /// <param name="groups">分组集合</param>
        /// <param name="filePath">文件路径</param>
        Task ExportGroupsAsync(IEnumerable<Group> groups, string filePath);
    }
}
