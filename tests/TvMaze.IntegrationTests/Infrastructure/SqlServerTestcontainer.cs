using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Containers.Configurations;
using DotNet.Testcontainers.Containers.Modules.Abstractions;

namespace TvMaze.IntegrationTests.Infrastructure;

public sealed class SqlServerTestcontainer : TestcontainerDatabase
{
    internal SqlServerTestcontainer(ITestcontainersConfiguration configuration)
        : base(configuration)
    { }

    public override string ConnectionString => $"Server=127.0.0.1,{Port};Database={Database};User Id={Username};Password={Password};";

    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        bool retry = true;
        tryAgain:
        try
        {
            await base.StartAsync(cancellationToken);
        }
        catch (DockerApiException dockerApiException) when (retry && dockerApiException.StatusCode == HttpStatusCode.Conflict)
        {
            retry = false;
            await NukeItAsync("sql-server-db");
            goto tryAgain;
        }
    }

    private async Task NukeItAsync(string name)
    {
        var uri = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");
        var dockerClient = new DockerClientConfiguration(uri).CreateClient();

        // Stop the container if it's running and remove it
        await dockerClient.Containers.RemoveContainerAsync(name, new ContainerRemoveParameters { Force = true });
    }
}