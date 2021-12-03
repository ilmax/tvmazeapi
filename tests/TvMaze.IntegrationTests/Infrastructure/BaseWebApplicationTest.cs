using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace TvMaze.IntegrationTests.Infrastructure;

public abstract class BaseWebApplicationTest : IDisposable
{
    protected BaseWebApplicationTest(Action<IServiceCollection> replaceServices = null)
    {
        Factory = new WebApplicationFactory(replaceServices);
        Client = Factory.CreateClient();
    }

    private protected WebApplicationFactory Factory { get; }
    protected HttpClient Client { get; }

    public void Dispose()
    {
        Factory.Dispose();
        Client.Dispose();
    }
}