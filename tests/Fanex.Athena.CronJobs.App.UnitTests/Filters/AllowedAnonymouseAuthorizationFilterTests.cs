using Fanex.Athena.CronJobs.App.Filters;
using NUnit.Framework;

namespace Fanex.Athena.CronJobs.App.UnitTests.Filters
{
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
}
