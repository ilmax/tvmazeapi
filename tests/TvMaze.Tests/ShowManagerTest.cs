using System;
using TvMaze.ApplicationServices;
using Xunit;

namespace TvMaze.Tests;

public class ShowManagerTest
{
    [Fact]
    public void Ctor_should_check_for_null()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ShowManager(null));
    }
}