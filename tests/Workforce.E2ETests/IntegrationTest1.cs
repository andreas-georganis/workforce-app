using Microsoft.Playwright;

namespace Workforce.E2ETests.Tests;

public sealed class HomePageTests
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Fact]
    public async Task HomePageDisplaysWelcomeHeading()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.Workforce_AppHost>(cancellationToken);

        await using var app = await appHost.BuildAsync(cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.ResourceNotifications
            .WaitForResourceHealthyAsync("workforce-web", cancellationToken)
            .WaitAsync(DefaultTimeout, cancellationToken);

        using var webClient = app.CreateHttpClient("workforce-web");
        var webUri = webClient.BaseAddress
            ?? throw new InvalidOperationException("The workforce-web resource has no HTTP endpoint.");

        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        var page = await browser.NewPageAsync();

        await page.GotoAsync(webUri.ToString());

        var heading = page.GetByRole(AriaRole.Heading, new() { Name = "Hello, world!" });

        Assert.True(await heading.IsVisibleAsync());
    }
}
