using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using System.Threading;

namespace QuartzBeginnerExample
{
    public class CronTriggerRunner : IExample
    {
        public string Name
        {
            get { return GetType().Name; }
        }

        public virtual void Run()
        {
            ILog log = LogManager.GetLogger(typeof(CronTriggerRunner));

            log.Info("------- Initializing -------------------");

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            log.Info("------- Initialization Complete --------");

            log.Info("------- Scheduling Jobs ----------------");

            // jobs can be scheduled before sched.start() has been called

            // job 1 will run every 20 seconds
            JobDetail job = new JobDetail("job1", "group1", typeof(SimpleQuartzJob));
            CronTrigger trigger = new CronTrigger("trigger1", "group1", "job1", "group1");
            trigger.CronExpressionString = "0/20 * * * * ?";
            sched.AddJob(job, true);
            DateTime ft = sched.ScheduleJob(trigger);

            log.Info(string.Format("{0} has been scheduled to run at: {1} and repeat based on expression: {2}", job.FullName, ft.ToString("r"), trigger.CronExpressionString));

            log.Info("------- Starting Scheduler ----------------");

            // All of the jobs have been added to the scheduler, but none of the
            // jobs
            // will run until the scheduler has been started
            sched.Start();

            log.Info("------- Started Scheduler -----------------");

            log.Info("------- Waiting five minutes... ------------");
            try
            {
                // wait five minutes to show jobs
                Thread.Sleep(300 * 1000);
                // executing...
            }
            catch (ThreadInterruptedException)
            {
            }

            log.Info("------- Shutting Down ---------------------");

            sched.Shutdown(true);

            log.Info("------- Shutdown Complete -----------------");

            SchedulerMetaData metaData = sched.GetMetaData();
            log.Info(string.Format("Executed {0} jobs.", metaData.NumJobsExecuted));
        }

    }
}
