using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Impl;
using SmartB.UI.Infrastructure;

namespace SmartB.UI.App_Start
{
    public static class BackgroundConfig
    {
        private static IScheduler scheduler;

        private static ITrigger parserTrigger;

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
            parserTrigger = trigger;

            scheduler.ScheduleJob(job, trigger);
        }

        public static void RescheduleParser(int hour, int minute)
        {
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("ParserTrigger", "Trigger")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute))
                .Build();
            scheduler.RescheduleJob(parserTrigger.Key, trigger);
        }

        public static void ScheduleDistanceService()
        {
            IJobDetail job = JobBuilder.Create<CalculateDistanceJob>()
                .WithIdentity("Distance", "DistanceJ")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("DistanceTrigger", "DistanceT")
                .StartNow()
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(1, 0))
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}