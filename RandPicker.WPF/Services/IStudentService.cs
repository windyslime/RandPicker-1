using RandPicker.WPF.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RandPicker.WPF.Services
{
    /// <summary>
    /// 学生服务接口
    /// </summary>
    public interface IStudentService
    {
        /// <summary>
        /// 加载学生数据
        /// </summary>
        /// <returns>学生集合</returns>
        Task<ObservableCollection<Student>> LoadStudentsAsync();
        
        /// <summary>
        /// 保存学生数据
        /// </summary>
        /// <param name="students">学生集合</param>
        Task SaveStudentsAsync(ObservableCollection<Student> students);
        
        /// <summary>
        /// 获取随机学生
        /// </summary>
        /// <param name="students">学生集合</param>
        /// <returns>随机学生</returns>
        Student? GetRandomStudent(ObservableCollection<Student> students);
        
        /// <summary>
        /// 获取随机学生（考虑权重）
        /// </summary>
        /// <param name="students">学生集合</param>
        /// <returns>随机学生</returns>
        Student? GetWeightedRandomStudent(ObservableCollection<Student> students);
        
        /// <summary>
        /// 更新学生权重
        /// </summary>
        /// <param name="student">学生</param>
        /// <param name="newWeight">新权重</param>
        void UpdateStudentWeight(Student student, int newWeight);
        
        /// <summary>
        /// 导入学生数据（从Excel）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>导入的学生集合</returns>
        Task<ObservableCollection<Student>> ImportFromExcelAsync(string filePath);
        
        /// <summary>
        /// 导入学生数据（从CSV）
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>导入的学生集合</returns>
        Task<ObservableCollection<Student>> ImportFromCsvAsync(string filePath);
        
        /// <summary>
        /// 导出学生数据（到Excel）
        /// </summary>
        /// <param name="students">学生集合</param>
        /// <param name="filePath">文件路径</param>
        Task ExportToExcelAsync(IEnumerable<Student> students, string filePath);
        
        /// <summary>
        /// 导出学生数据（到CSV）
        /// </summary>
        /// <param name="students">学生集合</param>
        /// <param name="filePath">文件路径</param>
        Task ExportToCsvAsync(IEnumerable<Student> students, string filePath);
        
        /// <summary>
        /// 获取活跃学生
        /// </summary>
        /// <param name="students">学生集合</param>
        /// <returns>活跃学生集合</returns>
        ObservableCollection<Student> GetActiveStudents(ObservableCollection<Student> students);
        
        /// <summary>
        /// 重置所有学生权重
        /// </summary>
        /// <param name="students">学生集合</param>
        /// <param name="weight">权重值</param>
        void ResetAllWeights(ObservableCollection<Student> students, int weight = 1);
        
        /// <summary>
        /// 重置所有学生状态
        /// </summary>
        /// <param name="students">学生集合</param>
        /// <param name="active">是否启用</param>
        void ResetAllActive(ObservableCollection<Student> students, bool active = true);
    }
}
