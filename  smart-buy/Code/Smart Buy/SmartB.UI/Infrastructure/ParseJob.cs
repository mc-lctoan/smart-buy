using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using SmartB.UI.Areas.Admin.Helper;

namespace SmartB.UI.Infrastructure
{
    public class ParseJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ParseHelper.ParseData();
        }
    }
}