using UserApi.Models;

namespace UserApi.Services;

public class ProfileService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProfileService> _logger;
    private readonly string _azureFunctionUrl = $"https://manerouserprovider.azurewebsites.net/api/GetAllFunction?code=CjhwMX-p044JIkqKmff7ABrsAJsk4NUtg3x_8SNXRkdOAzFuTo6Mvw%3D%3D";

    public ProfileService(HttpClient httpClient, ILogger<ProfileService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<ProfileModel>> GetAllProfilesAsync()
    {
        var response = await _httpClient.GetAsync(_azureFunctionUrl);
        if (response.IsSuccessStatusCode)
        {
            var profiles = await response.Content.ReadFromJsonAsync<List<ProfileModel>>();
            return profiles ?? new List<ProfileModel>();
        }
        else
        {
            _logger.LogError("Error fetching profiles from Azure Function");
            return new List<ProfileModel>(); 
        }
    }
}

