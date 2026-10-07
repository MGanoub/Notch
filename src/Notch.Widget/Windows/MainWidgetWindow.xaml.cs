using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using Notch.Shared.Dto;
using Notch.Widget.Services;
using System.Linq;
using System.Windows.Media;

namespace Notch.Widget.Windows;

public partial class MainWidgetWindow : Window
{
    private List<TaskItemDto> _allTasks = new();
    private List<TaskItemDto> _currenSubTasks = new();
    private TimeEntryDto? _currentEntry = null;
    public MainWidgetWindow()
    {
        InitializeComponent();
    }
    
    private async void LoadTaskList()
    {
        var response = await ApiClient.GetAsync("api/tasks");
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
        UpdateStartStopButton();
    }

    private void TaskComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedTask = TaskComboBox.SelectedItem as TaskItemDto;
        UpdateStartStopButton();
        if (selectedTask is null)
        {
            return;
        }
        LoadSubTasksList(selectedTask);
    }

    private async void StartStopButton_Click(object sender, RoutedEventArgs e)
    {
        var selected = TaskComboBox.SelectedItem as TaskItemDto;
        if (selected is null)
        {
            return;
        }

        var isRunning = _currentEntry?.TaskItemId == selected.Id;
        try
        {
            if (isRunning)
            {
                var response = await ApiClient.PostAsync("api/timeentries/stop");
                if (response.IsSuccessStatusCode)
                {
                    _currentEntry = null;
                }
            }
            else
            {
                var response = await ApiClient.PostAsJsonAsync("api/timeentries/start", new StartTimerRequest(selected.Id));
                if (response.IsSuccessStatusCode)
                {
                    _currentEntry = await response.Content.ReadFromJsonAsync<TimeEntryDto>();
                }
            }
            UpdateStartStopButton();
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private void UpdateStartStopButton()
    {
        var selected = TaskComboBox.SelectedItem as TaskItemDto;
        var isRunning = selected is not null
                        && _currentEntry is not null
                        && _currentEntry.TaskItemId == selected.Id;
        StartStopButton.Content = isRunning ? "Stop" : "Start";
        StartStopButton.Background = (Brush)FindResource(isRunning ? "DangerBrush" : "SuccessBrush");
        StartStopButton.IsEnabled = selected is not null;
    }
    
    private void SubtaskListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // TODO
    }

    private void OpenWebApp_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("http://localhost:5173") {UseShellExecute = true});
    }
}