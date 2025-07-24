using System;
using Avalonia.Interactivity;
using RandPicker.Config;
namespace RandPicker.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string ResultName { get; set; } = "无结果";
    public string ResultId { get; set; } = "000000";
    
    
    public void PickPerson()
    {
        Console.WriteLine("触发抽人");
    }
}