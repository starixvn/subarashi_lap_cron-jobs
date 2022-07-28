using Fanex.Athena.CronJobs.App.Helpers;
using Hangfire.Common;
using Hangfire.Logging;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using System;

namespace Fanex.Athena.CronJobs.App.Filters
{
    public class GlobalHangfireJobExceptionFilter : JobFilterAttribute, IElectStateFilter
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public void OnStateElection(ElectStateContext context)
        {
            var failedState = context.CandidateState as FailedState;
            if (failedState != null)
            {
                var jobId = context.BackgroundJob.Id;
                var exception = failedState.Exception;
                Logger.ErrorException($"Exception with job {jobId} \n" +
                    $"{ErrorMessageHelper.GenerateMessage(exception)}"
                    , exception);
            }
        }
    }
}
