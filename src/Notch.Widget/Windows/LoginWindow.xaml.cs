using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using Notch.Shared.Dto;
using Notch.Widget.Services;

namespace Notch.Widget.Windows;

public partial class LoginWindow : Window
{
    private static readonly HttpClient _httpClient = new HttpClient{BaseAddress  = new Uri("http://localhost:5105")};
    public LoginWindow()
    {
        InitializeComponent();
    }
    

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var userName = UsernameBox.Text;
        var password = PasswordBox.Password;
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest(userName, password));
        if (response.IsSuccessStatusCode)
        {
            var tokens = await response.Content.ReadFromJsonAsync<TokenResponse>();
            TokenStorage.Save(tokens!);
            this.Close();
        }
        else
        {
            ErrorText.Text = await response.Content.ReadAsStringAsync();
        }
    }
}