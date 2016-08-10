using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using System.Threading;

namespace QuartzBeginnerExample
{
    public class SimpleTriggerRunner : IExample
    {
        public string Name
        {
            get { return GetType().Name; }
        }

        public virtual void Run()
        {
            ILog log = LogManager.GetLogger(typeof(SimpleTriggerRunner));

            log.Info("------- Initializing -------------------");

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler();

            log.Info("------- Initialization Complete --------");

            log.Info("------- Scheduling Jobs ----------------");

            // jobs can be scheduled before sched.start() has been called

            // get a "nice round" time a few seconds in the future...
            DateTime ts = TriggerUtils.GetNextGivenSecondDate(null, 15);

            // job1 will only fire once at date/time "ts"
            JobDetail job = new JobDetail("job1", "group1", typeof(SimpleQuartzJob));
            SimpleTrigger trigger = new SimpleTrigger("trigger1", "group1");
            // set its start up time
            trigger.StartTime = ts;
            // set the interval, how often the job should run (10 seconds here) 
            trigger.RepeatInterval = 10000;
            // set the number of execution of this job, set to 10 times. 
            // It will run 10 time and exhaust.
            trigger.RepeatCount = 100;


            // schedule it to run!
            DateTime ft = sched.ScheduleJob(job, trigger);
            log.Info(string.Format("{0} will run at: {1} and repeat: {2} times, every {3} seconds",
                job.FullName, ft.ToString("r"), trigger.RepeatCount, (trigger.RepeatInterval / 1000)));

            
            log.Info("------- Starting Scheduler ----------------");

            // All of the jobs have been added to the scheduler, but none of the jobs
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

            // display some stats about the schedule that just ran
            SchedulerMetaData metaData = sched.GetMetaData();
            log.Info(string.Format("Executed {0} jobs.", metaData.NumJobsExecuted));
        }
    }
}
