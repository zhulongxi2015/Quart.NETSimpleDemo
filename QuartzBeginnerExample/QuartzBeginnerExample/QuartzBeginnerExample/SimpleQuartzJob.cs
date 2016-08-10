using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Quartz;

namespace QuartzBeginnerExample
{
    public class SimpleQuartzJob : IJob
    {

        private static ILog _log = LogManager.GetLogger(typeof(SimpleQuartzJob));

        /// <summary>
        /// Called by the <see cref="IScheduler" /> when a
        /// <see cref="Trigger" /> fires that is associated with
        /// the <see cref="IJob" />.
        /// </summary>
        public virtual void Execute(JobExecutionContext context)
        {
            try
            {
                // This job simply prints out its job name and the
                // date and time that it is running
                string jobName = context.JobDetail.FullName;
                _log.Info("Executing job: " + jobName + " executing at " + DateTime.Now.ToString("r"));
            }
			catch (Exception e)
			{
				_log.Info("--- Error in job!");
				JobExecutionException e2 = new JobExecutionException(e);
				// this job will refire immediately
				e2.RefireImmediately = true;
				throw e2;
			}
        }
    }
}
