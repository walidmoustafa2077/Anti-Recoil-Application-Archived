using Anti_Recoil_Application.Helpers;
using Anti_Recoil_Application.Models;
using Anti_Recoil_Application.ViewModels;
using Anti_Recoil_Application.ViewModels.DialogViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Anti_Recoil_Application.Services
{
    public class HostProviderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl = "https://localhost:7236/api"; // Base API URL
        private readonly DialogService _dialogService;
        private string _authToken;

        private readonly MainWindowViewModel _mainWindowViewModel;


        public HostProviderService(DialogService dialogService, MainWindowViewModel mainWindowViewModel)
        {
            _dialogService = dialogService;

            _httpClient = new HttpClient();
            _mainWindowViewModel = mainWindowViewModel;
        }


        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();


                var response = await _httpClient.GetAsync($"{_baseApiUrl}/home");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                _dialogService.ShowDialog($"API returned status code: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                _dialogService.ShowDialog($"Error checking connection: {ex.Message}");
                return false;
            }
        }


        public async Task<(bool success, bool isConnectionIssue)> AuthenticateAsync(string username, string password)
        {
            try
            {
                // Check connection first
                _mainWindowViewModel.IsLoading = true;

                var isConnected = await IsConnectedAsync();
                if (!isConnected)
                {
                    _mainWindowViewModel.IsLoading = false;

                    return (false, true); // Return false for success and true for connection issue
                }

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Prepare the credentials and make the API call
                var request = new
                {
                    UsernameOrEmail = username,
                    Password = password
                };

                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");


                var response = await _httpClient.PostAsync($"{_baseApiUrl}/Authentication/login", content);

                if (response.IsSuccessStatusCode)
                {
                    // Extract the token from the response body
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

                    if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.Token))
                    {
                        // Store the token securely, e.g., in memory or local storage
                        StoreToken(tokenResponse.Token);
                        _mainWindowViewModel.IsLoading = false;

                        return (true, false); // Login success, no connection issue
                    }

                    _mainWindowViewModel.IsLoading = false;

                    _dialogService.ShowDialog("Login failed: No token received.");

                    return (true, false); // Login success, no connection issue
                }

                // If login fails, show an error message
                var errorMessage = await response.Content.ReadAsStringAsync();
                _mainWindowViewModel.IsLoading = false;
                _dialogService.ShowDialog($"Login failed: {errorMessage}");
                return (false, false); // Login failed, no connection issue
            }
            catch (Exception ex)
            {
                // Handle any exception and show the error
                _mainWindowViewModel.IsLoading = false;
                _dialogService.ShowDialog($"Error during login: {ex.Message}");
                return (false, true); // Return false for success and true for connection issue
            }
        }


        public async Task<User?> ValidateUsernameOrEmailAsync(string enteredText)
        {
            try
            {
                _mainWindowViewModel.IsLoading = true;

                var isConnected = await IsConnectedAsync();
                if (!isConnected)
                {
                    _mainWindowViewModel.IsLoading = false;
                    return null; // Return null if not connected
                }

                _httpClient.DefaultRequestHeaders.Clear();

                // Make an HTTP GET request to the API to validate the username or email
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/Authentication/validate-username-or-email?usernameOrEmail={enteredText}");

                // If the response is successful (status code 200 OK), validation passed
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response content into the User object
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userDto = JsonSerializer.Deserialize<User>(responseContent);

                    _mainWindowViewModel.IsLoading = false;

                    // If the userDto is not null, return the User object
                    return userDto;
                }
                else
                {
                    _mainWindowViewModel.IsLoading = false;
                    return null; // Return null in case of failed validation
                }
            }
            catch (Exception ex)
            {
                // Log the exception (if necessary)
                _mainWindowViewModel.IsLoading = false;

                // Optionally, you can log the exception message or show a generic error dialog
                // Example: _dialogService.ShowDialog($"Error: {ex.Message}");

                return null; // Return null in case of any error during the process
            }
        }


        public async Task<bool> SendVerificationCodeAsync(string email)
        {
            try
            {
                _mainWindowViewModel.IsLoading = true;

                
                var isConnected = await IsConnectedAsync();
                if (!isConnected)
                {
                    _mainWindowViewModel.IsLoading = false;

                    return false; // If not connected, return false
                }

                _httpClient.DefaultRequestHeaders.Clear();


                // Serialize the usernameOrEmail as a plain string
                var usernameOrEmail = JsonSerializer.Serialize($"{email}");
                var content = new StringContent(usernameOrEmail, Encoding.UTF8, "application/json");

                // Make an HTTP POST request to the API to send the verification code
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/Authentication/send-verification-code", content);

                // If the response is successful (status code 200 OK), the code was sent
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {

                    _mainWindowViewModel.IsLoading = false;
                    return false;

                }
            }
            catch (Exception ex)
            {

                _mainWindowViewModel.IsLoading = false;
                return false;

            }

        }

        public async Task<bool> ValidateVerificationCodeAsync(string email, string enteredText)
        {
            try
            {
                _mainWindowViewModel.IsLoading = true;

                var isConnected = await IsConnectedAsync();
                if (!isConnected)
                {
                    _mainWindowViewModel.IsLoading = false;

                    return false; // If not connected, return false
                }

                _httpClient.DefaultRequestHeaders.Clear();

                // Prepare the request payload
                var request = new
                {
                    usernameOrEmail = email,
                    enteredVerificationCode = enteredText
                };

                // Serialize the request object to JSON
                var jsonString = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Use the correct endpoint for validating the verification code
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/Authentication/validate-verification-code", content);

                // Check if the response is successful (status code 200 OK)
                if (response.IsSuccessStatusCode)
                {
                    _mainWindowViewModel.IsLoading = false;

                    return true;
                }
                else
                {
                    _mainWindowViewModel.IsLoading = false;

                    return false;
                }
            }
            catch (Exception ex)
            {
                _mainWindowViewModel.IsLoading = false;

                // Log the exception if necessary
                // Optionally handle the error (e.g., show a generic error message)
                return false;
            }
        }


        public async Task<bool> UpdatePasswordAsync(string email, string password, string enteredCode)
        {
            // Send a request to the API to update the password
            try
            {
                _mainWindowViewModel.IsLoading = true;
                var isConnected = await IsConnectedAsync();
                if (!isConnected)
                {
                    _mainWindowViewModel.IsLoading = false;

                    return false; // If not connected, return false
                }

                _httpClient.DefaultRequestHeaders.Clear();

                // Prepare the request payload
                var request = new
                {
                    usernameOrEmail = email,
                    newPassword = password,
                    enteredVerificationCode = enteredCode
                };

                // Serialize the request object to JSON
                var jsonString = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Use the correct endpoint for validating the verification code
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/Authentication/forget-password", content);

                // Check if the response is successful (status code 200 OK)
                if (response.IsSuccessStatusCode)
                {
                    _mainWindowViewModel.IsLoading = false;
                    await _dialogService.ShowDialogAsync($"Password updated successfully"); // Wait for dialog to close
                    return true;
                }
                else
                {
                    _mainWindowViewModel.IsLoading = false;

                    return false;
                }
            }
            catch (Exception ex)
            {
                _mainWindowViewModel.IsLoading = false;

                return false;
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

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);

                var response = await _httpClient.GetAsync("/Weapons");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                _dialogService.ShowDialog($"Failed to fetch weapon data. Status code: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _dialogService.ShowDialog($"Error fetching weapon data: {ex.Message}");
                return null;
            }
        }

        public async Task<(bool, string)> RegisterNewUser(string firstName, string lastName, string username, string email, string password, DateTime? dateOfBirth)
        {
            try
            {
                _mainWindowViewModel.IsLoading = true;

                var isConnected = await IsConnectedAsync();
                if (!isConnected) return (false, string.Empty);

                // Create the request object that matches the RegisterUserRequestDTO structure
                var request = new
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Username = username,
                    Email = email,
                    Password = password,
                    RetypedPassword = password, // Assuming password and retyped password should be the same
                    DateOfBirth = dateOfBirth,
                    Country = string.Empty, // You can set this based on your form fields if available
                    City = string.Empty, // You can set this based on your form fields if available
                    Gender = string.Empty // Optional, you can pass gender if required
                };

                // Serialize the request object to JSON
                var jsonString = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Send the POST request to the Register API endpoint
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/Authentication/register", content);

                if (response.IsSuccessStatusCode)
                {
                    // If successful, deserialize the response to get the user DTO
                    var userDto = await response.Content.ReadFromJsonAsync<User>();

                    _mainWindowViewModel.IsLoading = false;

                    // Log user created successfully
                    await _dialogService.ShowDialogAsync($"User registered successfully: {userDto?.Username}"); // Wait for dialog to close

                    return (true, userDto.Email);
                }
                else
                {
                    // If the response is not successful, show the error message
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _mainWindowViewModel.IsLoading = false;
                    await _dialogService.ShowDialogAsync($"Registration failed: {errorMessage}");
                    return (false, string.Empty);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the registration process
                _mainWindowViewModel.IsLoading = false;
                await _dialogService.ShowDialogAsync($"Error during registration: {ex.Message}");
                return (false, string.Empty);
            }
        }

        public async Task<bool> VerifyUserAsync(string email, string enteredText)
        {
            try
            {
                _mainWindowViewModel.IsLoading = true;

                var isConnected = await IsConnectedAsync();
                if (!isConnected)
                {
                    _mainWindowViewModel.IsLoading = false;

                    return false; // If not connected, return false
                }

                _httpClient.DefaultRequestHeaders.Clear();

                // Prepare the request payload
                var request = new
                {
                    usernameOrEmail = email,
                    enteredVerificationCode = enteredText
                };

                // Serialize the request object to JSON
                var jsonString = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Use the correct endpoint for validating the verification code
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/Authentication/verify-user", content);

                // Check if the response is successful (status code 200 OK)
                if (response.IsSuccessStatusCode)
                {
                    _mainWindowViewModel.IsLoading = false;

                    return true;
                }
                else
                {
                    _mainWindowViewModel.IsLoading = false;

                    return false;
                }
            }
            catch (Exception ex)
            {
                _mainWindowViewModel.IsLoading = false;

                // Log the exception if necessary
                // Optionally handle the error (e.g., show a generic error message)
                return false;
            }
        }


        private void StoreToken(string token)
        {
            // Store the token in memory or some secure storage
            // For example, we can store it in a class-level variable (e.g., `_authToken`).
            _authToken = token;

            // Alternatively, you can store it in secure local storage if required
            // e.g., for web applications, you can use localStorage or sessionStorage
        }

  
    }
}
