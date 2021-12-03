using Microsoft.Extensions.DependencyInjection;
using TvMaze.IntegrationTests.Infrastructure;
using Xunit;

namespace TvMaze.IntegrationTests.Web;

public class DependencyInjectionTests : BaseWebApplicationTest
{
    [Fact]
    public void All_services_can_be_resolved()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();

        // Act && Assert
        foreach (var serviceDescriptor in Factory.ServiceCollection)
        {
            // Exclude framework services and open generics
            if (!IsFrameworkService(serviceDescriptor) && !serviceDescriptor.ServiceType.IsGenericTypeDefinition)
            {
                Assert.NotNull(scope.ServiceProvider.GetService(serviceDescriptor.ServiceType));
            }
        }

        static bool IsFrameworkService(ServiceDescriptor serviceDescriptor)
        {
            return serviceDescriptor.ServiceType.FullName.StartsWith("Microsoft") ||
                   serviceDescriptor.ServiceType.FullName.StartsWith("System");
        }
    }
}