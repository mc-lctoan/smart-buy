using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SmartB.UI.Areas.Admin.Models;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Areas.Admin.Helper
{
    public static class LogFileHelper
    {
        public static void GenerateLogFile(List<LogInfo> infos)
        {
            string path = ConstantManager.LogPath;

            int max = infos.Max(x => x.Link.Length);
            max++;
            string link = "Link".PadRight(max);
            string fileName = Path.GetRandomFileName() + ".txt";
            string horizontalLine = HorizontalLine(max);

            string content = "SMART BUY LOG FILE\n" +
                             "Tạo file lúc: " + DateTime.Now.ToShortDateString() + ", " +
                             DateTime.Now.ToShortTimeString() + "\n";
            content += horizontalLine;
            content += string.Format("|{0,-3}|{1}|{2,-15}|{3,-13}|{4,-19}|\n", "STT", link, "Thời gian parse",
                                     "Tổng sản phẩm", "Insert vào database");
            content += horizontalLine;

            for (int i = 0; i < infos.Count; i++)
            {
                content += string.Format("|{0,-3}|{1}|{2,-15}|{3,-13}|{4,-19}|\n",
                                         i + 1, infos[i].Link.PadRight(max), infos[i].ElapsedTime, infos[i].TotalItems,
                                         infos[i].ToDatabase);
                content += horizontalLine;
            }
            int totalTime = infos.Sum(x => x.ElapsedTime);
            int totalParsedItems = infos.Sum(x => x.TotalItems);
            content += "Tổng thời gian parse: " + totalTime + " mili giây\n";
            content += "Tổng sản phẩm parse được: " + totalParsedItems;

            File.WriteAllText(path + fileName, content, new UnicodeEncoding());
            
            using (var context = new SmartBuyEntities())
            {
                var log = new LogFile
                              {
                                  FileName = fileName,
                                  CreatedTime = DateTime.Now,
                                  IsActive = true
                              };

                context.LogFiles.Add(log);
                context.SaveChanges();
            }
        }

        private static string HorizontalLine(int max)
        {
            int length = 56 + max;
            string line = "";
            for (int i = 0; i < length; i++)
            {
                line += "_";
            }
            line += "\n";
            return line;
        }
    }
}