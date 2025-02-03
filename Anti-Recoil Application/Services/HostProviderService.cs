using Anti_Recoil_Application.Core.Services;
using Anti_Recoil_Application.Enums;
using Anti_Recoil_Application.Models;
using Anti_Recoil_Application.ViewModels;
using Anti_Recoil_Application.ViewModels.DialogViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
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

        public HostProviderService(DialogService dialogService, MainWindowViewModel mainWindowViewModel, SettingsService settingsService)
        {
            _dialogService = dialogService;

            _httpClient = new HttpClient();
            _mainWindowViewModel = mainWindowViewModel;

            // Load the token from the settings
            settingsService.Load();
            _authToken = settingsService.Settings.Token;
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

                _dialogService.ShowErrorDialog($"API returned status code: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Error checking connection: {ex.Message}");
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

                    _dialogService.ShowErrorDialog("Login failed: No token received.");

                    return (true, false); // Login success, no connection issue
                }

                // If login fails, show an error message
                _mainWindowViewModel.IsLoading = false;
                var errorMessage = await response.Content.ReadAsStringAsync();

                // if error is Email not confirmed. then show confirmation dialog CreateEnterFieldDialogViewModel
                if (errorMessage.Contains("Email not confirmed"))
                {

                    var dialogViewModel = _dialogService.CreateEnterFieldDialogViewModel(
                        "Please, Enter Code Sent to Your Mail.",
                        string.Empty,
                        "Enter Verification Code",
                        async (enteredText) =>
                        {
                            var isVerified = await VerifyUserAsync(username, enteredText);
                            if (isVerified)
                            {
                                await _dialogService.ShowDialogAsync("Email verified successfully");
                            }
                            else
                            {
                                await _dialogService.ShowErrorDialogAsync("Email verification failed");
                            }
                        });


                    await _dialogService.ShowDialogAsync(dialogViewModel);

                    return (false, false); // Login failed, no connection issue
                }

                _dialogService.ShowErrorDialog($"Login failed: {errorMessage}");
                return (false, false); // Login failed, no connection issue
            }
            catch (Exception ex)
            {
                // Handle any exception and show the error
                _mainWindowViewModel.IsLoading = false;
                _dialogService.ShowErrorDialog($"Error during login: {ex.Message}");
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
                    // If the response is not successful, show the error message
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await _dialogService.ShowErrorDialogAsync($"Registration failed: {errorMessage}");
                    return null; // Return null in case of failed validation
                }
            }
            catch (Exception ex)
            {
                // Log the exception (if necessary)
                _mainWindowViewModel.IsLoading = false;
                _dialogService.ShowErrorDialog($"Validation failed: {ex.Message}");

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
                    // If the response is not successful, show the error message
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await _dialogService.ShowErrorDialogAsync($"Registration failed: {errorMessage}");
                    return false;

                }
            }
            catch (Exception ex)
            {

                _mainWindowViewModel.IsLoading = false;
                _dialogService.ShowErrorDialog($"Validation failed: {ex.Message}");
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
                    // If the response is not successful, show the error message
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await _dialogService.ShowErrorDialogAsync($"Registration failed: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _mainWindowViewModel.IsLoading = false;

                // Log the exception if necessary
                // Optionally handle the error (e.g., show a generic error message)
                _dialogService.ShowErrorDialog($"Validation failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string password, string enteredCode = null)
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
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/Authentication/reset-password", content);

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
                    // If the response is not successful, show the error message
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await _dialogService.ShowErrorDialogAsync($"Registration failed: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _mainWindowViewModel.IsLoading = false;
                // Log the exception
                return false;
            }
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
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

                // Clear headers and make API call
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                // Prepare the request payload
                var request = new
                {
                    usernameOrEmail = email,
                    password = password,
                };

                // Serialize the request object to JSON
                var jsonString = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Use the correct endpoint for validating the verification code
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/Authentication/check-password", content);

                // Check if the response is successful (status code 200 OK)
                if (response.IsSuccessStatusCode)
                {
                    _mainWindowViewModel.IsLoading = false;
                    return true;
                }
                else
                {
                    _mainWindowViewModel.IsLoading = false;
                    // If the response is not successful, show the error message
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await _dialogService.ShowErrorDialogAsync($"Registration failed: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _mainWindowViewModel.IsLoading = false;
                // Log the exception
                return false;
            }
        }

        public async Task<(bool, string)> RegisterNewUser(string firstName, string lastName, string username, string email, string password, DateTime? dateOfBirth, string selectedGender, string country, string city, RegisterViewModel registerViewModel)
        {
            try
            {
                _mainWindowViewModel.IsLoading = true;

                var isConnected = await IsConnectedAsync();
                if (!isConnected)
                {
                    _mainWindowViewModel.IsLoading = false;

                    return (false, string.Empty);
                }

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
                    Country = selectedGender,
                    City = country,
                    Gender = city
                };

                // Serialize the request object to JSON
                var jsonString = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Send the POST request to the Register API endpoint
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/Authentication/register", content);

                _mainWindowViewModel.IsLoading = false;

                if (response.IsSuccessStatusCode)
                {

                    var registerResponse = await response.Content.ReadFromJsonAsync<RegisterResponse>();
                    if (registerResponse?.User != null)
                    {

                        var dialogViewModel = _dialogService.CreateEnterFieldDialogViewModel(
                        "Please, Enter Code Sent to Your Mail.",
                        string.Empty,
                        "Enter Verification Code",
                        async (enteredText) =>
                        {
                            var isVerified = await VerifyUserAsync(username, enteredText);
                            if (isVerified)
                            {
                                var dialogViewModel = new MainDialogViewModel(() =>
                                {

                                    registerViewModel.HasRegistered();
                                    _dialogService.CloseDialog();
                                })
                                {
                                    HeaderText = "Email verified successfully",
                                    ButtonText = "Close"
                                };
                                await _dialogService.ShowDialogAsync(dialogViewModel);
                            }
                            else
                            {
                                await _dialogService.ShowErrorDialogAsync("Email verification failed");
                            }
                        });


                        await _dialogService.ShowDialogAsync(dialogViewModel);


                        return (true, registerResponse.User.Email);
                    }
                    await _dialogService.ShowDialogAsync("User registration successful, User not confirmed!");
                    return (true, string.Empty);
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await _dialogService.ShowErrorDialogAsync($"Registration failed: {errorMessage}");
                    return (false, string.Empty);
                }

            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the registration process
                _mainWindowViewModel.IsLoading = false;
                await _dialogService.ShowErrorDialogAsync($"Error during registration: {ex.Message}");
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

        public async Task<bool> UpdateUserAsync(string email, string? value, UpdateUserOption? option)
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

                // Clear headers and make API call
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);

                // convert otpion to string
                string newOption = option switch
                {
                    UpdateUserOption.FirstName => "FirstName",
                    UpdateUserOption.LastName => "LastName",
                    UpdateUserOption.Username => "Username",
                    UpdateUserOption.Email => "Email",
                    UpdateUserOption.Password => "Password",
                };

                // Prepare the request payload
                var request = new
                {
                    usernameOrEmail = email,
                    option = newOption,
                    newValue = value
                };

                // Serialize the request object to JSON
                var jsonString = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                // Use the correct endpoint for validating the verification code
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/Authentication/update-user", content);

                // Check if the response is successful (status code 200 OK)
                if (response.IsSuccessStatusCode)
                {
                    _mainWindowViewModel.IsLoading = false;
                    await _dialogService.ShowDialogAsync($"User {newOption} updated successfully");
                    return true;
                }
                else
                {
                    _mainWindowViewModel.IsLoading = false;
                    // If the response is not successful, show the error message
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await _dialogService.ShowErrorDialogAsync($"Registration failed: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _mainWindowViewModel.IsLoading = false;
                // Log the exception
                return false;
            }
        }

        // get user data from token
        public async Task<User?> GetUserAsync()
        {
            try
            {
                _mainWindowViewModel.IsLoading = true;

                var tokenHandler = new JwtSecurityTokenHandler();

                var jwtToken = tokenHandler.ReadJwtToken(_authToken);
                var userName = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.NameIdentifier ||
                    c.Type == ClaimTypes.Name ||
                    c.Type == "sub" || // Common in JWTs
                    c.Type == "unique_name" || // Some APIs use this
                    c.Type == "email" // If email is used as username
                )?.Value;


                if (string.IsNullOrEmpty(userName))
                {
                    throw new Exception("Username not found in token");
                }

                var user = await ValidateUsernameOrEmailAsync(userName);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                return user;
            }
            catch (Exception ex)
            {
                _dialogService.ShowErrorDialog($"Error fetching user data: {ex.Message}");
                return null; // Return null instead of an unhandled exception
            }
            finally
            {
                _mainWindowViewModel.IsLoading = false;
            }
        }

        public async Task<List<Weapon>> GetWeaponsAsync()
        {
            try
            {
                _mainWindowViewModel.IsLoading = true;

                // Check connectivity
                var isConnected = await IsConnectedAsync();
                if (!isConnected)
                {
                    _dialogService.ShowErrorDialog("No internet connection. Please check your network settings.");
                    return new List<Weapon>();
                }

                // Clear headers and make API call
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);

                var response = await _httpClient.GetAsync($"{_baseApiUrl}/Weapons");

                if (response.IsSuccessStatusCode)
                {
                    // Read and deserialize response content
                    var weapons = await response.Content.ReadFromJsonAsync<List<Weapon>>();
                    if (weapons != null)
                    {
                        return weapons;
                    }
                }

                // throw an exception if the response is not successful
                throw new HttpRequestException($"API returned status code: {response.StatusCode}");
            }
            catch (HttpRequestException httpEx)
            {
                // Ensure IsLoading is reset regardless of outcome
                _mainWindowViewModel.IsLoading = false;
                throw new Exception($"Error fetching weapon data: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                // Ensure IsLoading is reset regardless of outcome
                _mainWindowViewModel.IsLoading = false;
                throw new Exception($"Error parsing weapon data: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                // Ensure IsLoading is reset regardless of outcome
                _mainWindowViewModel.IsLoading = false;
                throw new Exception($"Error fetching weapon data: {ex.Message}");
            }
            finally
            {
                // Ensure IsLoading is reset regardless of outcome
                _mainWindowViewModel.IsLoading = false;

            }
        }


        private void StoreToken(string token)
        {
            _authToken = token;
            var settings = App.AppHost?.Services.GetRequiredService<SettingsService>();
            settings?.UpdateSettings(token: token);

        }


    }
}
