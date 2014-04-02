using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using SmartB.UI.Helper;

namespace SmartB.UI.Infrastructure
{
    public class CalculateDistanceJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            CalculateDistanceHelper.CalculateDistance();
        }
    }
}