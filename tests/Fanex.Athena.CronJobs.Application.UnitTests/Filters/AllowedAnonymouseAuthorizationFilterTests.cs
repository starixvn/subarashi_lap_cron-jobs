namespace Fanex.Athena.CronJobs.Application.UnitTests.Filters;

using Application.Filters;
using NUnit.Framework;

public class AllowedAnonymouseAuthorizationFilterTests
{
    [Test]
    public void Authorize_WhenCall_ReturnTrue()
    {
        //Arrange
        var filter = new AllowedAnonymouseAuthorizationFilter();

        //Action
        var result = filter.Authorize(default);

        //Assert
        Assert.That(result, Is.True);
    }
}