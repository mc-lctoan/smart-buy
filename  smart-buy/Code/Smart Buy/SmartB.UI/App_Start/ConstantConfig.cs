using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using SmartB.UI.Infrastructure;

namespace SmartB.UI
{
    public static class ConstantConfig
    {
        public static void Register(HttpServerUtility server)
        {
            ConstantManager.LogPath = server.MapPath("~/Areas/Admin/LogFiles/");
            ConstantManager.ConfigPath = server.MapPath("~/Areas/Admin/AdminConfig.xml");
            ConstantManager.SavedPath = server.MapPath("~/Areas/Admin/SavedPages");
            ConstantManager.TrainingFilePath = server.MapPath("~/UploadedExcelFiles/ProductName.txt");
            ConstantManager.DistanceFilePath = server.MapPath("~/CalculateMarketDistance.xml");
            ConstantManager.IsParserRunning = false;
        }
    }
}