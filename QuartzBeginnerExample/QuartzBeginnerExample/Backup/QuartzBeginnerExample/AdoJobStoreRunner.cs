using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using Quartz;
using Common.Logging;
using Quartz.Impl;
using System.Threading;

namespace QuartzBeginnerExample
{
    public class AdoJobStoreRunner : IExample
    {
        public string Name
        {
            get { return GetType().Name; }
        }

        private static ILog _log = LogManager.GetLogger(typeof(AdoJobStoreRunner));

        public virtual void CleanUp(IScheduler inScheduler)
        {
            _log.Warn("***** Deleting existing jobs/triggers *****");

            // unschedule jobs
            string[] groups = inScheduler.TriggerGroupNames;
            for (int i = 0; i < groups.Length; i++)
            {
                String[] names = inScheduler.GetTriggerNames(groups[i]);
                for (int j = 0; j < names.Length; j++)
                    inScheduler.UnscheduleJob(names[j], groups[i]);
            }

            // delete jobs
            groups = inScheduler.JobGroupNames;
            for (int i = 0; i < groups.Length; i++)
            {
                String[] names = inScheduler.GetJobNames(groups[i]);
                for (int j = 0; j < names.Length; j++)
                    inScheduler.DeleteJob(names[j], groups[i]);
            }
        }

        public virtual void Run(bool inClearJobs, bool inScheduleJobs)
        {
            NameValueCollection properties = new NameValueCollection();

            properties["quartz.scheduler.instanceName"] = "TestScheduler";
            properties["quartz.scheduler.instanceId"] = "instance_one";
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "5";
            properties["quartz.threadPool.threadPriority"] = "Normal";
            properties["quartz.jobStore.misfireThreshold"] = "60000";
            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.StdAdoDelegate, Quartz";
            properties["quartz.jobStore.useProperties"] = "false";
            properties["quartz.jobStore.dataSource"] = "default";
            properties["quartz.jobStore.tablePrefix"] = "QRTZ_";
            properties["quartz.jobStore.clustered"] = "true";
            // if running MS SQL Server we need this
            properties["quartz.jobStore.selectWithLockSQL"] = "SELECT * FROM {0}LOCKS UPDLOCK WHERE LOCK_NAME = @lockName";

            properties["quartz.dataSource.default.connectionString"] = @"Server=LIJUNNIN-PC\SQLEXPRESS;Database=quartz;Trusted_Connection=True;";
            properties["quartz.dataSource.default.provider"] = "SqlServer-20";

            // First we must get a reference to a scheduler
            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            IScheduler sched = sf.GetScheduler();
            if (inClearJobs)
            {
                CleanUp(sched);
            }


            _log.Info("------- Initialization Complete -----------");

            if (inScheduleJobs)
            {
                _log.Info("------- Scheduling Jobs ------------------");

                string schedId = sched.SchedulerInstanceId;

                int count = 1;

                JobDetail job = new JobDetail("job_" + count, schedId, typeof(SimpleQuartzJob));
                // ask scheduler to re-Execute this job if it was in progress when
                // the scheduler went down...
                job.RequestsRecovery = true;
                SimpleTrigger trigger = new SimpleTrigger("trig_" + count, schedId, 20, 5000L);

                trigger.StartTime = DateTime.Now.AddMilliseconds(1000L);
                sched.ScheduleJob(job, trigger);
                _log.Info(string.Format("{0} will run at: {1} and repeat: {2} times, every {3} seconds", job.FullName, trigger.GetNextFireTime(), trigger.RepeatCount, (trigger.RepeatInterval / 1000)));

                count++;
                job = new JobDetail("job_" + count, schedId, typeof(SimpleQuartzJob));
                // ask scheduler to re-Execute this job if it was in progress when
                // the scheduler went down...
                job.RequestsRecovery = (true);
                trigger = new SimpleTrigger("trig_" + count, schedId, 20, 5000L);

                trigger.StartTime = (DateTime.Now.AddMilliseconds(2000L));
                sched.ScheduleJob(job, trigger);
                _log.Info(string.Format("{0} will run at: {1} and repeat: {2} times, every {3} seconds", job.FullName, trigger.GetNextFireTime(), trigger.RepeatCount, (trigger.RepeatInterval / 1000)));

                count++;
                job = new JobDetail("job_" + count, schedId, typeof(SimpleQuartzJob));
                // ask scheduler to re-Execute this job if it was in progress when
                // the scheduler went down...
                job.RequestsRecovery = (true);
                trigger = new SimpleTrigger("trig_" + count, schedId, 20, 3000L);

                trigger.StartTime = (DateTime.Now.AddMilliseconds(1000L));
                sched.ScheduleJob(job, trigger);
                _log.Info(string.Format("{0} will run at: {1} and repeat: {2} times, every {3} seconds", job.FullName, trigger.GetNextFireTime(), trigger.RepeatCount, (trigger.RepeatInterval / 1000)));

                count++;
                job = new JobDetail("job_" + count, schedId, typeof(SimpleQuartzJob));
                // ask scheduler to re-Execute this job if it was in progress when
                // the scheduler went down...
                job.RequestsRecovery = (true);
                trigger = new SimpleTrigger("trig_" + count, schedId, 20, 4000L);

                trigger.StartTime = (DateTime.Now.AddMilliseconds(1000L));
                sched.ScheduleJob(job, trigger);
                _log.Info(string.Format("{0} will run at: {1} & repeat: {2}/{3}", job.FullName, trigger.GetNextFireTime(), trigger.RepeatCount, trigger.RepeatInterval));

                count++;
                job = new JobDetail("job_" + count, schedId, typeof(SimpleQuartzJob));
                // ask scheduler to re-Execute this job if it was in progress when
                // the scheduler went down...
                job.RequestsRecovery = (true);
                trigger = new SimpleTrigger("trig_" + count, schedId, 20, 4500L);

                trigger.StartTime = (DateTime.Now.AddMilliseconds(1000L));
                sched.ScheduleJob(job, trigger);
                _log.Info(string.Format("{0} will run at: {1} & repeat: {2}/{3}", job.FullName, trigger.GetNextFireTime(), trigger.RepeatCount, trigger.RepeatInterval));
            }
            // jobs don't start firing until start() has been called...
            _log.Info("------- Starting Scheduler ---------------");
            sched.Start();
            _log.Info("------- Started Scheduler ----------------");

            _log.Info("------- Waiting for one hour... ----------");

            Thread.Sleep(TimeSpan.FromHours(1));


            _log.Info("------- Shutting Down --------------------");
            sched.Shutdown();
            _log.Info("------- Shutdown Complete ----------------");
        }

        public void Run()
        {
            bool clearJobs = true;
            bool scheduleJobs = true;

            AdoJobStoreRunner example = new AdoJobStoreRunner();
            example.Run(clearJobs, scheduleJobs);

        }
    }
}
