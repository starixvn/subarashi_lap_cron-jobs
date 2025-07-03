namespace Fanex.Athena.CronJobs.Domain.DTO;

using System.Net;

public class LogRequest
{
    public string Message { get; set; }

    public string Path { get; set; }

    public string Query { get; set; }

    public HttpStatusCode? StatusCode { get; set; }

    public bool? IsSuccess { get; set; }

    public long DurationMs { get; set; }

    public string Request { get; set; }

    public string Response { get; set; }

    public string Exception { get; set; }
}