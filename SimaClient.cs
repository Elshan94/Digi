using CSharpFunctionalExtensions;
using DigitalSalaryService.Application.Models.Sima;
using DigitalSalaryService.Application.Services.Abstract;
using DigitalSalaryService.Models.Configurations;
using Microsoft.Extensions.Options;
using System.Threading;
using Newtonsoft.Json;
using System.Text;
using DigitalSalaryService.Persistence.Repositories.Abstract;

namespace DigitalSalaryService.Application.Services.Concrete
{
    public class SimaClient : ISimaClient
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<SimaClient> _logger;

        private readonly SimaConfig _simaConfig;

        public SimaClient(HttpClient httpClient, ILogger<SimaClient> logger, IOptions<SimaConfig> simaSignerClientConfig)
        {
            _httpClient = httpClient;
            _logger = logger;
            _simaConfig = simaSignerClientConfig.Value;
        }

        public async Task<Result<string>> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "v2/operation/auth");

                var tokenRequestModel = new TokenRequestModel()
                {
                    Key = _simaConfig.Key,
                    Service = _simaConfig.Service,
                };

                var jsonModel = JsonConvert.SerializeObject(tokenRequestModel);

                request.Content = new StringContent(jsonModel, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

                    return Result.Failure<string>(
                        $"Token request failed. StatusCode: {(int)response.StatusCode}, Response: {errorContent}");
                }

                var responseContent =
                    await response.Content.ReadAsStringAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(responseContent))
                    return Result.Failure<string>("Empty response received from auth service.");

                TokenResponseModel? tokenResponse;

                try
                {
                    tokenResponse =
                        JsonConvert.DeserializeObject<TokenResponseModel>(responseContent);
                }
                catch (JsonException ex)
                {
                    return Result.Failure<string>(
                        $"Failed to deserialize token response. {ex.Message}");
                }

                if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.Token))
                    return Result.Failure<string>("Token was not returned by sima service.");

                if (!string.Equals(tokenResponse.Status, "success",
                      StringComparison.OrdinalIgnoreCase))
                {
                    return Result.Failure<string>(
                        $"Unsuccess data received from sima. Status: {tokenResponse.Status}");
                }

                return Result.Success(tokenResponse.Token);
            }
            catch (TaskCanceledException)
            {
                return Result.Failure<string>("Token request timeout.");
            }
            catch (HttpRequestException ex)
            {
                return Result.Failure<string>($"HTTP error while requesting token: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Failure<string>($"Unexpected error: {ex.Message}");
            }
        }
        public async Task<Result<bool>> ValidateIdentificationAsync(ValidateIdentificationRequestModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "/v2/onboarding/get-document-data-aze-scan");

                request.Content = new StringContent(
                    JsonConvert.SerializeObject(model),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.SendAsync(request, cancellationToken);

                var responseContent =
                    await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return Result.Failure<bool>(
                        $"Government scan failed. StatusCode: {(int)response.StatusCode}, Response: {responseContent}");
                }

                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    return Result.Failure<bool>("Empty response from government service.");
                }

                ValidateIdentificationResponseModel? scanResponse;

                try
                {
                    scanResponse =
                        JsonConvert.DeserializeObject<ValidateIdentificationResponseModel>(responseContent);
                }
                catch (JsonException ex)
                {
                    return Result.Failure<bool>(
                        $"Failed to deserialize response: {ex.Message}");
                }

                if (scanResponse == null)
                {
                    return Result.Failure<bool>("Invalid response received.");
                }

                if (!string.Equals(scanResponse.Status, "success",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return Result.Failure<bool>(
                        $"Government validation failed. Status: {scanResponse.Status}");
                }

                return Result.Success(true);
            }
            catch (TaskCanceledException)
            {
                return Result.Failure<bool>("Government scan request timeout.");
            }
            catch (HttpRequestException ex)
            {
                return Result.Failure<bool>(
                    $"HTTP error while calling government service: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"Unexpected error: {ex.Message}");
            }
        }
        public async Task<Result<bool>> UploadUserDataAsync(UploadUserDataRequest model, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Token))
                {
                    return Result.Failure<bool>("Token is required.");
                }

                model.StatusCallbackUrl = _simaConfig.CallbackUrl;

                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "/v2/onboarding/upload-data");

                request.Content = new StringContent(
                    JsonConvert.SerializeObject(model),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.SendAsync(request, cancellationToken);

                var responseContent =
                    await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return Result.Failure<bool>(
                        $"Upload user data failed. StatusCode: {(int)response.StatusCode}, Response: {responseContent}");
                }

                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    return Result.Failure<bool>("Empty response from onboarding service.");
                }

                UploadUserDataResponse? uploadResponse;

                try
                {
                    uploadResponse =
                        JsonConvert.DeserializeObject<UploadUserDataResponse>(responseContent);
                }
                catch (JsonException ex)
                {
                    return Result.Failure<bool>(
                        $"Failed to deserialize response: {ex.Message}");
                }

                if (uploadResponse == null)
                {
                    return Result.Failure<bool>("Invalid response received.");
                }

                if (!string.Equals(uploadResponse.Status,
                        "success",
                        StringComparison.OrdinalIgnoreCase))
                {
                    return Result.Failure<bool>(
                        $"Upload failed. Status: {uploadResponse.Status}");
                }

                return Result.Success(true);
            }
            catch (TaskCanceledException)
            {
                return Result.Failure<bool>("Upload user data request timeout.");
            }
            catch (HttpRequestException ex)
            {
                return Result.Failure<bool>(
                    $"HTTP error while uploading user data: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"Unexpected error: {ex.Message}");
            }
        }
    }
}
