using Avalonia.Controls;
using Avalonia.Interactivity;
using RandPicker.Config;
using RandPicker.ViewModels;

namespace RandPicker.Views;

public partial class GroupManagementView : UserControl
{
    public GroupManagementView()
    {
        InitializeComponent();
    }

    private void EditGroup_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Group group && DataContext is GroupManagementViewModel vm)
        {
            vm.SelectedGroup = group;
            vm.EditCommand.Execute(null);
        }
    }

    private void DeleteGroup_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Group group && DataContext is GroupManagementViewModel vm)
        {
            vm.SelectedGroup = group;
            vm.DeleteCommand.Execute(null);
        }
    }
}
