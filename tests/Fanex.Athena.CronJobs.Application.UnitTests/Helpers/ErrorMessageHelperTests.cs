namespace Fanex.Athena.CronJobs.Application.UnitTests.Helpers;

using Application.Helpers;
using Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using NSubstitute;
using NUnit.Framework;

public class ErrorMessageHelperTests
{
    [Test]
    public void GenerateMessage_WhenCallWithGeneralException_ReturnCorrectString()
    {
        //Arrange
        var msg = "test";
        var exception = new Exception(msg);

        //Action
        var result = ErrorMessageHelper.GenerateMessage(exception);

        //Assert
        Assert.That(result, Does.Contain(msg));
    }

    [Test]
    public void GenerateMessage_WhenCallWithSqlException_ReturnStringHaveSqlExceptionInfo()
    {
        //Arrange
        var connectionString =
            "Data Source=127.0.0.1;Initial Catalog=testDB; integrated security=false; trustservercertificate=true; User Id=test; Password=test";
        SqlException sqlException = null;
        using (var conn = new SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
            }
            catch (SqlException ex)
            {
                sqlException = ex;
            }
        }

        //Action
        var result = ErrorMessageHelper.GenerateMessage(sqlException);

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(sqlException, Is.Not.Null);
            Assert.That(result, Does.Contain("Server"));
            Assert.That(result, Does.Contain("SpName"));
        });
    }

    [Test]
    public void GenerateMessage_WhenCallWithExceptionHaveInnerException_ReturnStringHaveInnerExcetion()
    {
        //Arrange
        var innerException = new Exception("Inner");
        var exception = new Exception("Outer", innerException);

        //Action
        var result = ErrorMessageHelper.GenerateMessage(exception);

        //Assert
        Assert.That(result, Does.Contain("Outer"));
        Assert.That(result, Does.Contain("Inner"));
        Assert.That(result.IndexOf("Inner"), Is.GreaterThan(result.IndexOf("Outer")));
    }

    [Test]
    public void GenerateMessage_WhenCallWithContext_ReturnStringHaveContextInfo()
    {
        //Arrange
        var exception = new Exception("test");
        var context = Substitute.For<HttpContext>();
        var httpRequest = Substitute.For<HttpRequest>();
        httpRequest.Host.ReturnsForAnyArgs(new HostString("host"));
        httpRequest.Path.ReturnsForAnyArgs(new PathString("/path"));
        context.Request.ReturnsForAnyArgs(httpRequest);

        //Action
        var result = ErrorMessageHelper.GenerateMessage(exception, context);

        //Assert
        Assert.That(result, Does.Contain("host"));
        Assert.That(result, Does.Contain("/path"));
        Assert.That(result, Does.Contain("Remote IP"));
    }

    [Test]
    public void GenerateMessage_WhenCallWithContextNotHaveRequest_ReturnStringHaveNoRequestInfo()
    {
        //Arrange
        var exception = new Exception("test");
        var context = Substitute.For<HttpContext>();
        HttpRequest httpRequest = null;
        context.Request.ReturnsForAnyArgs(httpRequest);

        //Action
        var result = ErrorMessageHelper.GenerateMessage(exception, context);

        //Assert
        Assert.That(result, Does.Not.Contain(nameof(HttpRequest.Host)));
        Assert.That(result, Does.Not.Contain(nameof(HttpRequest.Path)));
        Assert.That(result, Does.Not.Contain("Remote IP"));
    }

    [Test]
    public void GenerateMessage_WhenCallWithObjectDbException_ReturnStringHaveObjectDBExcptionInfo()
    {
        //Arrange
        var excption = new ObjectDbException("test", new Exception());

        //Action
        var result = ErrorMessageHelper.GenerateMessage(excption);

        //Assert
        Assert.That(result, Does.Contain("Server"));
        Assert.That(result, Does.Contain("DbName"));
        Assert.That(result, Does.Contain("SpName"));
        Assert.That(result, Does.Contain("UserId"));
        Assert.That(result, Does.Contain("Parameters"));
        Assert.That(result, Does.Contain("CommandTimeout"));
    }
}