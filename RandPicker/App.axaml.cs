using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using RandPicker.ViewModels;
using RandPicker.Views;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace RandPicker;

public partial class App : Application
{
    public static ICommand ExitCommand { get; } = new RelayCommand(Exit);
    public static ICommand SettingsCommand { get; } = new RelayCommand(Settings);
    public static ICommand ManagementCommand { get; } = new RelayCommand(OpenManagement);

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private static void Exit()
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    private static void Settings()
    {
        Settings settings = new Settings();
        settings.Show();
    }

    private static void OpenManagement()
    {
        ManagementMainView managementWindow = new ManagementMainView();
        managementWindow.Show();
    }
}