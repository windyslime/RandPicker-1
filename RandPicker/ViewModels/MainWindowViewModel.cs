using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using RandPicker.Config;

namespace RandPicker.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private List<Student> _students = new();
    private readonly Random _random = new();
    
    private string _resultName = "无结果";
    private string _resultId = "000000";
    
    public string ResultName 
    { 
        get => _resultName;
        set
        {
            _resultName = value;
            OnPropertyChanged();
        }
    }
    
    public string ResultId 
    { 
        get => _resultId;
        set
        {
            _resultId = value;
            OnPropertyChanged();
        }
    }

    public MainWindowViewModel()
    {
        LoadStudents();
    }
    
    private void LoadStudents()
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "students.json");
        _students = StudentService.LoadFromJson(jsonPath);
    }
    
    public void PickPerson()
    {
        // 刷新学生数据
        LoadStudents();
        // 获取活跃的学生
        var activeStudents = StudentService.GetActiveList(_students);
        
        if (activeStudents.Count == 0)
        {
            ResultName = "无结果";
            ResultId = "000000";
            return;
        }
        
        // 创建加权列表并随机选择
        var weightedStudents = StudentService.GetWeightedList(activeStudents);
        var selectedStudent = weightedStudents[_random.Next(weightedStudents.Count)];
        
        ResultName = selectedStudent.Name;
        ResultId = selectedStudent.Id.ToString();
        
        Console.WriteLine($"选中: {selectedStudent.Name} (ID: {selectedStudent.Id})");
    }
}