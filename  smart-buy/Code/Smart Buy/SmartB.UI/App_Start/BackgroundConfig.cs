using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Core;
using Quartz.Impl;
using SmartB.UI.Infrastructure;

namespace SmartB.UI.App_Start
{
    public static class BackgroundConfig
    {
        private static IScheduler scheduler;

        public static void StartScheduler()
        {
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
        }

        public static void ScheduleParser()
        {
            IJobDetail job = JobBuilder.Create<ParseJob>()
                .WithIdentity("Parser", "Job")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("ParserTrigger", "Trigger")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(4, 0))
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        public static void RescheduleParser(int hour, int minute)
        {
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("ParserTrigger", "Trigger")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute))
                .Build();
            //scheduler.RescheduleJob(parserTrigger.Key, trigger);
        }
    }
}