using Avalonia.Controls;
using Avalonia.Interactivity;
using RandPicker.ViewModels;

namespace RandPicker.Views;

public partial class ManagementMainView : Window
{
    public ManagementMainView()
    {
        InitializeComponent();
        DataContext = new ManagementMainViewModel();
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}