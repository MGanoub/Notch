using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using Notch.Shared.Dto;
using Notch.Widget.Services;
using System.Linq;

namespace Notch.Widget.Windows;

public partial class MainWidgetWindow : Window
{
    private List<TaskItemDto> _allTasks = new();
    private List<TaskItemDto> _currenSubTasks = new();
    public MainWidgetWindow()
    {
        InitializeComponent();
    }
    
    private async void LoadTaskList()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "api/tasks");
        var response = await ApiClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            _allTasks = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>() ?? new List<TaskItemDto>();
            TaskComboBox.ItemsSource = _allTasks.Where(t => t.ParentTaskId is null).ToList();
           
        }
    }
    
    private void LoadSubTasksList(TaskItemDto selectedTask)
    {
        _currenSubTasks.Clear();
        if (_allTasks.Count == 0 || selectedTask is null)
        {
            return;
        }

        _currenSubTasks = _allTasks.Where(t => t.ParentTaskId == selectedTask.Id).ToList();
        SubtaskListBox.ItemsSource = _currenSubTasks;
    }

    private void MainWidgetWindow_Loaded(object sender, RoutedEventArgs e)
    {
        LoadTaskList();
    }

    private void TaskComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedTask = TaskComboBox.SelectedItem as TaskItemDto;
        if (selectedTask is null)
        {
            return;
        }
        LoadSubTasksList(selectedTask);
    }

    private void StartStopButton_Click(object sender, RoutedEventArgs e)
    {
        
    }
    
    private void SubtaskListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // TODO
    }
}