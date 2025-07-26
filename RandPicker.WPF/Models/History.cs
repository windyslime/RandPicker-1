using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RandPicker.WPF.Models
{
    /// <summary>
    /// 选择模式枚举
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// 个人选择模式
        /// </summary>
        Individual,
        
        /// <summary>
        /// 分组选择模式
        /// </summary>
        Group
    }

    /// <summary>
    /// 历史记录项模型类
    /// </summary>
    public class HistoryItem : ObservableObject
    {
        private SelectionMode _mode;
        private string _selectedName = string.Empty;
        private DateTime _selectionTime = DateTime.Now;
        private string? _details;
        private Student? _student;
        private Group? _group;

        /// <summary>
        /// 选择模式
        /// </summary>
        public SelectionMode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        /// <summary>
        /// 选中的名称
        /// </summary>
        public string SelectedName
        {
            get => _selectedName;
            set => SetProperty(ref _selectedName, value);
        }

        /// <summary>
        /// 选择时间
        /// </summary>
        public DateTime SelectionTime
        {
            get => _selectionTime;
            set => SetProperty(ref _selectionTime, value);
        }

        /// <summary>
        /// 详细信息
        /// </summary>
        public string? Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }

        /// <summary>
        /// 选中的学生（仅在个人模式下有效）
        /// </summary>
        public Student? Student
        {
            get => _student;
            set => SetProperty(ref _student, value);
        }

        /// <summary>
        /// 选中的分组（仅在分组模式下有效）
        /// </summary>
        public Group? Group
        {
            get => _group;
            set => SetProperty(ref _group, value);
        }

        /// <summary>
        /// 获取格式化的选择时间
        /// </summary>
        public string FormattedTime => SelectionTime.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 获取模式的显示名称
        /// </summary>
        public string ModeDisplayName => Mode == SelectionMode.Individual ? "个人选择" : "分组选择";
    }
}
