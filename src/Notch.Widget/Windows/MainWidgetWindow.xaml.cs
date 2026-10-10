using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using Notch.Shared.Dto;
using Notch.Widget.Services;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using System.Windows.Media;
using Notch.Shared.Enums;

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
    
    private async Task LoadTaskList()
    {
        var selectedId = (TaskComboBox.SelectedItem as TaskItemDto)?.Id;
        var response = await ApiClient.GetAsync("api/tasks");
        if (!response.IsSuccessStatusCode) return;

        _allTasks = await response.Content.ReadFromJsonAsync<List<TaskItemDto>>() ?? new List<TaskItemDto>();
        var topLevel = _allTasks.Where(t => t.ParentTaskId is null).ToList();
        TaskComboBox.ItemsSource = topLevel;
        TaskComboBox.SelectedItem = topLevel.FirstOrDefault(t => t.Id == selectedId);
    }

    private async Task LoadCurrentEntry()
    {
        var response = await ApiClient.GetAsync("api/timeentries/current");
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            _currentEntry = string.IsNullOrWhiteSpace(body)
                ? null
                : JsonSerializer.Deserialize<TimeEntryDto>(body, JsonSerializerOptions.Web);
        }
        else
        {
            Debug.WriteLine($"current failed: {(int)response.StatusCode}");
        }

        UpdateStartStopButton();
    }
    
    private void LoadSubTasksList(TaskItemDto selectedTask)
    {
        _currenSubTasks.Clear();
        if (_allTasks.Count == 0 || selectedTask is null)
        {
            return;
        }
        _currenSubTasks = _allTasks.Where(t => t.ParentTaskId == selectedTask.Id).ToList();
        SubtaskItems.ItemsSource = _currenSubTasks;
        SubtasksPanel.Visibility = _currenSubTasks.Count > 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private async void MainWidgetWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadTaskList();
        await LoadCurrentEntry();
        if (_currentEntry is not null)
        {
            TaskComboBox.SelectedItem = (TaskComboBox.ItemsSource as IEnumerable<TaskItemDto>)
                ?.FirstOrDefault(t => t.Id == _currentEntry.TaskItemId);
        }
    }
    
    private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private void UpdateStatusDisplay(TaskItemDto? task)
    {
        var (label, fg, bg) = task?.Status switch
        {
            NotchStatus.InProgress => ("In progress", "AccentBrush", "AccentBgBrush"),
            NotchStatus.Done       => ("Done",        "DoneBrush",   "DoneBgBrush"),
            NotchStatus.Todo       => ("Todo",        "MutedTextBrush", "InsetBrush"),
            _                      => ("—",           "MutedTextBrush", "InsetBrush"),
        };

        StatusText.Text = label;
        StatusText.Foreground = (Brush)FindResource(fg);
        StatusPill.Background = (Brush)FindResource(bg);
    }

    private void TaskComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedTask = TaskComboBox.SelectedItem as TaskItemDto;
        UpdateStatusDisplay(selectedTask);
        DoneButton.IsEnabled = selectedTask is not null;
        UpdateStartStopButton();
        if (selectedTask is null)
        {
            return;
        }
        LoadSubTasksList(selectedTask);
    }
    
    private async void SubtaskRow_Click(object sender, MouseButtonEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not TaskItemDto sub) return;
        var target = sub.Status == NotchStatus.InProgress ? NotchStatus.Todo : NotchStatus.InProgress;
        Debug.WriteLine($"row click: '{sub.Title}' {sub.Status} -> {target}");
        await SetStatus(sub.Id, target);
        var selectedTask = TaskComboBox.SelectedItem as TaskItemDto;
        if (selectedTask is null)
        {
            return;
        }
        LoadSubTasksList(selectedTask);
    }

    private async void SubtaskDone_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not TaskItemDto sub) return;

        var target = sub.Status == NotchStatus.Done ? NotchStatus.Todo : NotchStatus.Done;
        await SetStatus(sub.Id, target);
        var selectedTask = TaskComboBox.SelectedItem as TaskItemDto;
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
            await LoadTaskList();
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
    

    private void OpenWebApp_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("http://localhost:5173") {UseShellExecute = true});
    }

    private async Task SetStatus(Guid taskId, NotchStatus status)
    {
        try
        {
            var response =
                await ApiClient.PatchAsJsonAsync($"api/tasks/{taskId}/status", new UpdateTaskStatusRequest(status));
            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"Status update not right: {(int)response.StatusCode}");
                return;
            }

            await LoadTaskList();
            await LoadCurrentEntry();
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    private async void DoneButton_Click(object sender, RoutedEventArgs e)
    {
        if (TaskComboBox.SelectedItem is not TaskItemDto task) return;
        var target = task.Status == NotchStatus.Done ? NotchStatus.Todo : NotchStatus.Done;
        await SetStatus(task.Id, target);
    }
}