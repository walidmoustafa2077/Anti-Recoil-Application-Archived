using Anti_Recoil_Application.ViewModels.DialogViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Anti_Recoil_Application.Services
{
    public class HostProviderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl = "http://localhost:5226/api"; // Base API URL
        private readonly DialogService _dialogService;

        public HostProviderService(DialogService dialogService)
        {
            _dialogService = dialogService;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseApiUrl)
            };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/Home");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                ShowDialog($"API returned status code: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                ShowDialog($"Error checking connection: {ex.Message}");
                return false;
            }
        }


        public async Task<(bool success, bool isConnectionIssue)> AuthenticateAsync(string username, string password)
        {
            try
            {
                // Check connection first
                var isConnected = await IsConnectedAsync();
                if (!isConnected)
                {
                    return (false, true); // Return false for success and true for connection issue
                }

                // Prepare the credentials and make the API call
                var credentials = new { Username = username, Password = password };
                var content = new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/Auth/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    return (true, false); // Login success, no connection issue
                }

                // If login fails, show an error message
                var errorMessage = await response.Content.ReadAsStringAsync();
                ShowDialog($"Login failed: {errorMessage}");
                return (false, false); // Login failed, no connection issue
            }
            catch (Exception ex)
            {
                // Handle any exception and show the error
                ShowDialog($"Error during login: {ex.Message}");
                return (false, true); // Return false for success and true for connection issue
            }
        }


        /// <summary>
        /// Fetches weapon data from the API.
        /// </summary>
        /// <returns>The weapon data as a string, or null if an error occurs.</returns>
        public async Task<string> GetWeaponDataAsync()
        {
            try
            {
                var isConnected = await IsConnectedAsync();
                if (!isConnected) return null;

                var response = await _httpClient.GetAsync("/Weapons");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                ShowDialog($"Failed to fetch weapon data. Status code: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                ShowDialog($"Error fetching weapon data: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Displays a dialog with the given message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        private void ShowDialog(string message)
        {
            var dialogViewModel = new MainDialogViewModel(() => _dialogService.CloseDialog())
            {
                HeaderText = message,
                ButtonText = "Close"
            };

            _dialogService.ShowDialog(dialogViewModel);
        }

        internal void UpdatePassword(string username, string password)
        {
        }
    }
}
