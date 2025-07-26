using System.Windows;

namespace RandPicker.WPF.Models
{
    /// <summary>
    /// 浮动窗口模型类，用于存储浮动窗口的设置和状态
    /// </summary>
    public class FloatingWindow
    {
        // 窗口位置
        public Point Position { get; set; } = new Point(100, 100);
        
        // 窗口是否可见
        public bool IsVisible { get; set; } = true;
        
        // 窗口是否被钉住（不会自动隐藏）
        public bool IsPinned { get; set; } = false;
        
        // 当前显示的结果
        public Student? CurrentStudent { get; set; }
        
        // 当前显示的分组
        public Group? CurrentGroup { get; set; }
        
        // 当前显示模式（个人/分组）
        public SelectionMode CurrentMode { get; set; } = SelectionMode.Individual;
        
        // 窗口是否正在拖动
        public bool IsDragging { get; set; } = false;
        
        // 拖动起始点
        public Point DragStartPoint { get; set; }
        
        // 窗口是否处于边缘隐藏状态
        public bool IsEdgeHidden { get; set; } = false;
        
        // 窗口隐藏的边缘（左/右/无）
        public EdgePosition HiddenEdge { get; set; } = EdgePosition.None;
    }
    
    /// <summary>
    /// 边缘位置枚举
    /// </summary>
    public enum EdgePosition
    {
        None,
        Left,
        Right,
        Top,
        Bottom
    }
}