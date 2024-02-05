using FluentAssertions;
using Host.TestApps.Oidc.IntegrationTests.Pom;

namespace Host.TestApps.Oidc.IntegrationTests;

public class OidcIntegrationTests : IClassFixture<HostApplication>
{
    [Fact]
    public async Task ItShouldIntegrateWithIdentityServer()
    {
        const string subYoda = "test-user-2";
        
        var app = new App();

        try
        {
            await app.NavigateToProxy();

            await app.IdSvrLoginPage.BtnYodaLogin.ClickAsync();
            await Task.Delay(2000);

            app.CurrentPage.Text.Should().Contain(subYoda);
        
            // Assert the user was logged in
            await app.GoTo("/.auth/me");
            await Task.Delay(1000);

            app.CurrentPage.Text.Should().Contain(subYoda);
            
            // Test ASP.NET Core authorization pipeline
            await app.GoTo("/custom/me");
            await Task.Delay(1000);

            app.CurrentPage.Text.Should().Contain(subYoda);

            // Assert the user was logged in
            await app.GoTo("/api/echo");
            await Task.Delay(1000);

            app.CurrentPage.Text.Should().Contain("Bearer ey");

            // Log out
            await app.GoTo("/.auth/end-session");
            await Task.Delay(1000);
        
            await app.IdSvrSignOutPage.BtnYes.ClickAsync();
            await app.WaitForNavigationAsync();
        
            // Assert user details flushed
            await app.GoTo("/.auth/me");
            await Task.Delay(1000);

            app.CurrentPage.Text.Should().NotContain(subYoda);
            
            // Assert token removed
            await app.GoTo("/api/echo");
            await Task.Delay(1000);
        
            app.CurrentPage.Text.Should().NotContain("Bearer ey");
        }
        finally
        {
            await app.CloseBrowser();
        }
    }
}