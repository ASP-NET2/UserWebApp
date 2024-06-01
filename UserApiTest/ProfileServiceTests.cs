using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Http.Json;
using System.Net;
using UserApi.Models;
using UserApi.Services;
using Moq.Protected;

namespace UserApiTest;

public class ProfileServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<ILogger<ProfileService>> _loggerMock;
    private readonly ProfileService _profileService;

    public ProfileServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _loggerMock = new Mock<ILogger<ProfileService>>();
        _profileService = new ProfileService(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllProfilesAsync_ReturnsProfiles()
    {
        var profiles = new List<ProfileModel> { new ProfileModel { FirstName = "John", LastName = "Doe" } };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(profiles)
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        var result = await _profileService.GetAllProfilesAsync();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("John", result[0].FirstName);
    }
}
