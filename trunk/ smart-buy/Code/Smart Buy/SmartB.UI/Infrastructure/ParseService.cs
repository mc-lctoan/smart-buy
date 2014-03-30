using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentScheduler;
using SmartB.UI.Areas.Admin.Helper;

namespace SmartB.UI.Infrastructure
{
    public class ParseService : Registry
    {
        public ParseService(int hour, int minute)
        {
            Schedule(ParseHelper.ParseData).WithName("Parser").ToRunEvery(1).Days().At(hour, minute);
        }
    }
}