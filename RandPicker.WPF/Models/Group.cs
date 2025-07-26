using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RandPicker.WPF.Models
{
    /// <summary>
    /// 分组模型类
    /// </summary>
    public class Group : ObservableObject
    {
        private string _name = string.Empty;
        private ObservableCollection<Student> _students = new();

        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// 分组中的学生集合
        /// </summary>
        public ObservableCollection<Student> Students
        {
            get => _students;
            set => SetProperty(ref _students, value);
        }

        /// <summary>
        /// 获取分组中的学生数量
        /// </summary>
        public int StudentCount => Students.Count;

        /// <summary>
        /// 获取分组中的学生姓名列表（逗号分隔）
        /// </summary>
        public string StudentNames => string.Join(", ", Students.Select(s => s.Name));

        /// <summary>
        /// 获取分组中的活跃学生数量
        /// </summary>
        public int ActiveStudentCount => Students.Count(s => s.Active);

        public override string ToString() => Name;
    }
}
