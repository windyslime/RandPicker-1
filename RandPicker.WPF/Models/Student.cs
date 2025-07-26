using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RandPicker.WPF.Models
{
    /// <summary>
    /// 学生模型类
    /// </summary>
    public class Student : ObservableObject
    {
        private string _name = string.Empty;
        private int _id;
        private int _weight = 1;
        private bool _active = true;
        private string? _avatarPath;

        /// <summary>
        /// 学生姓名
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// 学生学号
        /// </summary>
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// 选择权重
        /// </summary>
        public int Weight
        {
            get => _weight;
            set => SetProperty(ref _weight, value);
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Active
        {
            get => _active;
            set => SetProperty(ref _active, value);
        }

        /// <summary>
        /// 头像路径
        /// </summary>
        public string? AvatarPath
        {
            get => _avatarPath;
            set => SetProperty(ref _avatarPath, value);
        }

        /// <summary>
        /// 获取学生显示名称
        /// </summary>
        public string DisplayName => $"{Id.ToString().Substring(Math.Max(0, Id.ToString().Length - 2))} {Name}";

        public override string ToString() => Name;
    }
}
