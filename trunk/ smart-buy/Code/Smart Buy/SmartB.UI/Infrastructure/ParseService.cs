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
        public ParseService()
        {
            Schedule(ParseHelper.ParseData).ToRunEvery(1).Days().At(4, 0);
        }
    }
}