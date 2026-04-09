using CSharpFunctionalExtensions;
using DigitalSalaryService.Application.Features.Kyc.CreateKycCommand;
using DigitalSalaryService.Application.Models.Kyc;
using DigitalSalaryService.Application.Models.Sima;
using DigitalSalaryService.Application.Services.Abstract;
using DigitalSalaryService.Models.Configurations;
using DigitalSalaryService.Models.DTO_s.IB;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace DigitalSalaryService.Application.Services.Concrete
{
    public class KycClient : IKycClient
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<KycClient> _logger;

        public KycClient(HttpClient httpClient, ILogger<KycClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Result<PersonInfoResponseModel>> GetPersonalInfoAsync(PersonInfoRequestModel requestModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var url =
                    $"api/DigitalKyc/getPersonalInfo?" +
                    $"PinCode={requestModel.PinCode}&" +
                    $"DocumentNumber={requestModel.DocumentNumber}&" +
                    $"ClientCode={requestModel.ClientCode}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

                    _logger.LogError(
                        "PersonalInfo request failed. StatusCode: {StatusCode}, Response: {Response}",
                        (int)response.StatusCode,
                        errorContent);

                    return Result.Failure<PersonInfoResponseModel>(
                        $"Request failed. StatusCode: {(int)response.StatusCode}, Response: {errorContent}");
                }

                var responseContent =
                    await response.Content.ReadAsStringAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(responseContent))
                    return Result.Failure<PersonInfoResponseModel>(
                        "Empty response received from DigitalKyc service.");

                KycResponseModel<PersonInfoResponseModel> ? responseModel;

                try
                {
                    responseModel =
                        JsonConvert.DeserializeObject<KycResponseModel<PersonInfoResponseModel>>(responseContent);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex,
                        "Failed to deserialize PersonalInfo response. Response: {Response}",
                        responseContent);

                    return Result.Failure<PersonInfoResponseModel>(
                        $"Failed to deserialize response. {ex.Message}");
                }

                if (responseModel == null)
                    return Result.Failure<PersonInfoResponseModel>(
                        "Invalid response received from DigitalKyc service.");

                return Result.Success(responseModel.Data);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("PersonalInfo request timeout.");

                return Result.Failure<PersonInfoResponseModel>(
                    "PersonalInfo request timeout.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "HTTP error while requesting PersonalInfo.");

                return Result.Failure<PersonInfoResponseModel>(
                    $"HTTP error while requesting PersonalInfo: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error while requesting PersonalInfo.");

                return Result.Failure<PersonInfoResponseModel>(
                    $"Unexpected error: {ex.Message}");
            }

        }

        public async Task<Result<bool>> CheckKycActualityAsync(string customerCode, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"api/DigitalKyc/checkKycActuality/{customerCode}";
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

                    _logger.LogError(
                        "CheckKycActuality request failed. StatusCode: {StatusCode}, Response: {Response}",
                        (int)response.StatusCode,
                        errorContent);

                    return Result.Failure<bool>(
                        $"Request failed. StatusCode: {(int)response.StatusCode}, Response: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                KycResponseModel<bool>? kycResponse;
                try
                {
                    kycResponse = JsonConvert.DeserializeObject<KycResponseModel<bool>>(responseContent);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex,
                        "Failed to deserialize CheckKycActuality response: {Response}",
                        responseContent);

                    return Result.Failure<bool>($"Failed to deserialize response: {ex.Message}");
                }

                if (kycResponse == null)
                {
                    return Result.Failure<bool>("Received null response from KYC service.");
                }

                return Result.Success(kycResponse.Data);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("CheckKycActuality request timeout.");
                return Result.Failure<bool>("CheckKycActuality request timeout.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while requesting CheckKycActuality.");
                return Result.Failure<bool>($"HTTP error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while checking KYC actuality.");
                return Result.Failure<bool>($"Unexpected error: {ex.Message}");
            }
        }
        public async Task<Result<KycCreateResponseModel>> CreateKycAsync(
    KycCreateRequestModel command,
    CancellationToken cancellationToken = default)
        {
            try
            {
                var url = "api/DigitalKyc/CreateDigitalKycForSalary";

                KycCreateRequestModel requestModel = new KycCreateRequestModel
                {
                    CustomerCode = command.CustomerCode,
                    FieldOfActivityId = command.FieldOfActivityId,
                    HasPoliticalRelativeRelation = command.HasPoliticalRelativeRelation,
                    IsPolitical = command.IsPolitical,
                    PreviousFullName = command.PreviousFullName,
                };

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(requestModel),
                        Encoding.UTF8,
                        "application/json")
                };

                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);

                    _logger.LogError(
                        "CreateDigitalKycForSalary request failed. StatusCode: {StatusCode}, Response: {Response}",
                        (int)response.StatusCode,
                        errorContent);

                    return Result.Failure<KycCreateResponseModel>(
                        $"Request failed. StatusCode: {(int)response.StatusCode}, Response: {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                KycResponseModel<KycCreateResponseModel>? kycResponse;

                try
                {
                    kycResponse = JsonConvert.DeserializeObject<KycResponseModel<KycCreateResponseModel>>(responseContent);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex,
                        "Failed to deserialize CreateDigitalKycForSalary response: {Response}",
                        responseContent);

                    return Result.Failure<KycCreateResponseModel>($"Failed to deserialize response: {ex.Message}");
                }

                if (kycResponse == null)
                {
                    return Result.Failure<KycCreateResponseModel>("Received null response from KYC service.");
                }

                if (!kycResponse.IsSuccess)
                {
                    return Result.Failure<KycCreateResponseModel>(
                        string.Join(", ", kycResponse.Errors ?? new List<string>()));
                }

                return Result.Success(kycResponse.Data);
            }
            catch (TaskCanceledException)
            {
                _logger.LogError("CreateDigitalKycForSalary request timeout.");
                return Result.Failure<KycCreateResponseModel>("CreateDigitalKycForSalary request timeout.");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while requesting CreateDigitalKycForSalary.");
                return Result.Failure<KycCreateResponseModel>($"HTTP error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating Digital KYC for salary.");
                return Result.Failure<KycCreateResponseModel>($"Unexpected error: {ex.Message}");
            }
        }


    }
}