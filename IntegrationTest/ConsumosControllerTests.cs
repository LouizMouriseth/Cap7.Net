using System.Net.Http.Headers;
using Core;
using Core.Enums;
using Infrastructure.Extensions;
using Infrastructure.Options;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTest;

public class ConsumosControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ConsumosControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        var usuario = new User("Teste", "teste@email.com", "123", UserRole.Admin);
        
        var scopeFactory = factory.Services.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AuthTokenOptions>>().Value;

        var token = usuario.GenerateJwtToken(options);
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task GET_ReturnsHttpStatusCode200()
    {
        var request = "/api/Consumos";

        var response = await _client.GetAsync(request);

        response.EnsureSuccessStatusCode();
    }
}