using GetPattern.Models;
using GetPattern.Services;
using GetWeaponsPatterns___CSMacro_LUA_Version.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AntiRecoil_Tester.Services
{
    public class WeaponService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "http://localhost:5226/api"; // Your API base URL
        private readonly string _apiWeaponUrl = "http://localhost:5226/api/Weapons"; // Your API base URL
        private string _token;

        public WeaponService()
        {
            _httpClient = new HttpClient();

            IsConnectedAsync().Wait();

            string username = UserInteractionService.GetUserText("Enter Admin Username");
            string password = UserInteractionService.GetUserText("Enter Admin Password");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string loginToken = LoginAsync(username, password).Result;

            // Check if the token is null or empty
            if (string.IsNullOrEmpty(loginToken))
            {
                Console.WriteLine("Failed to get a token.");
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginToken);

        }


        // Check if the service is available
        private async Task<bool> IsConnectedAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/Home");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    // Log the unsuccessful status code or handle it as needed
                    Console.WriteLine($"API returned status code {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error checking connection: {ex.Message}");
                return false;
            }
        }

        public async Task AddWeaponAsync(Weapon request)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await _httpClient.PostAsync(_apiWeaponUrl, content);
                Console.WriteLine($"Adding weapon");

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response if needed, for example, the added weapon
                    var responseData = await response.Content.ReadAsStringAsync();
                    var addedWeapon = JsonSerializer.Deserialize<Weapon>(responseData);

                    // Example: Update UI or notify user
                    Console.WriteLine($"Weapon {addedWeapon.WeaponName} Added successfully!");
                }
                else
                {
                    // Handle failure (e.g., show error message)
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to add weapon: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., show network error message)
                Console.WriteLine($"Error adding weapon: {ex.Message}");
            }
        }

        public async Task UpdateWeaponAsync(Weapon request)
        {
            var content = new StringContent(
             JsonSerializer.Serialize(request),
             Encoding.UTF8,
             "application/json"
            );

            try
            {
                Console.WriteLine($"Updating weapon: {request.WeaponName}");

                var response = await _httpClient.PutAsync($"{_apiWeaponUrl}/{request.WeaponName}", content);

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response if needed, for example, the updated weapon
                    var responseData = await response.Content.ReadAsStringAsync();
                    var updatedWeapon = JsonSerializer.Deserialize<Weapon>(responseData);

                    // Example: Update UI or notify user
                    Console.WriteLine($"Weapon {updatedWeapon?.WeaponName} updated successfully!");
                }
                else
                {
                    // Log the error details from the response
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to update weapon: {errorMessage}");

                    // Log the status code
                    Console.WriteLine($"Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Log exception (e.g., network issues)
                Console.WriteLine($"Error updating weapon: {ex.Message}");
            }
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var loginRequest = new
            {
                UsernameOrEmail = username,
                Password = password
            };

            var content = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/Authentication/login", content);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();

                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseData);
                // Assuming the token is in the response under the "Token" field
                string token = loginResponse.token;
                return token;
            }

            // Handle failure (e.g., invalid credentials)
            Console.WriteLine("Login failed.");
            return null;
        }

        public async Task<Weapon> GetWeaponByNameAsync(string weaponName)
        {
            // Set the Authorization header with the token before making the request

            try
            {
                var response = await _httpClient.GetAsync($"{_apiWeaponUrl}/{weaponName}");

                if (response.IsSuccessStatusCode)
                {
                    var weaponJson = await response.Content.ReadAsStringAsync();
                    var weapon = JsonSerializer.Deserialize<Weapon>(weaponJson);
                    return weapon;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to authenticate: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting weapon: {ex.Message}");
            }
            return null;
        }
    }
}
