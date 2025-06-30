namespace Fanex.Athena.CronJobs.Domain.Params;

using System;

public class InsertBellNotificationParam
{
    public int MessageType { get; set; }

    public string LicenseeNames { get; set; }

    public string SubjectEn { get; set; }

    public string SubjectCs { get; set; }

    public string ContentEn { get; set; }

    public string ContentCs { get; set; }

    public DateTime EventDate { get; set; }
}