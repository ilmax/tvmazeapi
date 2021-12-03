using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace TvMaze.IntegrationTests.Infrastructure;
class WebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Action<IServiceCollection> _testServiceConfiguration;
    
    public WebApplicationFactory(Action<IServiceCollection> testServiceConfiguration)
    {
        _testServiceConfiguration = testServiceConfiguration;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ServiceCollection = services;
            services.RemoveAll(typeof(IHostedService));
            _testServiceConfiguration?.Invoke(services);
        });
    }

    public IServiceCollection ServiceCollection { get; private set; }
}