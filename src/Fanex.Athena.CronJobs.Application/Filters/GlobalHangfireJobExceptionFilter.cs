namespace Fanex.Athena.CronJobs.Application.Filters;

using System;
using System.Text;
using Hangfire.Common;
using Hangfire.States;
using Helpers;
using Logging;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
public class GlobalHangfireJobExceptionFilterAttribute(ILogger logger) : JobFilterAttribute, IElectStateFilter
{
    public void OnStateElection(ElectStateContext context)
    {
        if (context.CandidateState is FailedState failedState)
        {
            var exception = failedState.Exception;
            logger.Error(GenerateJobException(context), exception);
        }
    }

    private static string GenerateJobException(ElectStateContext context)
    {
        var failedState = context.CandidateState as FailedState;
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Job Name: {context.BackgroundJob.Job.Type.Name}");
        stringBuilder.AppendLine($"Job Id: {context.BackgroundJob.Id}");
        stringBuilder.AppendLine($"Created Time: {context.BackgroundJob.CreatedAt}");
        stringBuilder.AppendLine(ErrorMessageHelper.GenerateMessage(failedState.Exception));
        return stringBuilder.ToString();
    }
}