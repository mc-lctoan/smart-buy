using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace SmartB.UI.Areas.Admin.Helper
{
    public static class ParseHelper
    {
        public static void CorrectLink(List<HtmlNode> nodes, string url, string attName)
        {
            Uri uri = new Uri(url);
            string host = uri.GetLeftPart(UriPartial.Authority);
            string path = uri.AbsolutePath;
            if (path.EndsWith(".aspx") || path.EndsWith(".html") || path.EndsWith(".htm"))
            {
                path = path.Substring(0, path.LastIndexOf('/'));
            }

            foreach (HtmlNode node in nodes)
            {
                if (node.Attributes[attName] != null && !node.Attributes[attName].Value.StartsWith("http"))
                {
                    string tmp = node.Attributes[attName].Value;
                    if (path.Length > 1)
                    {
                        tmp = host + "/" + path + "/" + tmp;
                    }
                    else
                    {
                        tmp = host + "/" + tmp;
                    }
                    tmp = tmp.Replace("//", "/");
                    tmp = tmp.Replace("http:/", "http://");
                    node.Attributes[attName].Value = tmp;
                }
            }
        }
    }
}