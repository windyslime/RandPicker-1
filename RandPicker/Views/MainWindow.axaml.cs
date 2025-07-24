using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace RandPicker.Views;

public partial class MainWindow : Window
{
    private bool _isDragging;
    private Point _lastPointerPosition;

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);

        // 订阅鼠标事件
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            _lastPointerPosition = e.GetCurrentPoint(this).Position;
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var currentPosition = e.GetCurrentPoint(this).Position;
            var deltaX = currentPosition.X - _lastPointerPosition.X;
            var deltaY = currentPosition.Y - _lastPointerPosition.Y;

            // 动
            var newPosition = Position;
            newPosition = new PixelPoint(
                (int)(newPosition.X + deltaX),
                (int)(newPosition.Y + deltaY)
            );
            Position = newPosition;
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // 松手就停
        // 嘿刹不住车了嘿，现在人追着车跑嘿
        if (_isDragging) _isDragging = false;
    }

}