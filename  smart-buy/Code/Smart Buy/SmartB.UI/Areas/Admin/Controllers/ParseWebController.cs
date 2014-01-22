using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using SmartB.UI.Areas.Admin.Helper;
using SmartB.UI.Areas.Admin.Models;

namespace SmartB.UI.Areas.Admin.Controllers
{
    public class ParseWebController : Controller
    {
        //
        // GET: /Admin/ParseWeb/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult LoadWeb(ParserCreator model)
        {
            // Create Firefox browser
            var web = new HtmlWeb {UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:26.0) Gecko/20100101 Firefox/26.0"};

            // Load website
            var document = web.Load(model.ParseLink);

            // Correct links
            var src = new List<HtmlNode>(document.DocumentNode.Descendants().Where(x => x.Attributes["src"] != null));
            var link = new List<HtmlNode>(document.DocumentNode.Descendants().Where(x => x.Attributes["href"] != null));
            ParseHelper.CorrectLink(src, model.ParseLink, "src");
            ParseHelper.CorrectLink(link, model.ParseLink, "href");

            document.DocumentNode.Descendants().Where(x => x.Name == "script").ToList().ForEach(x => x.Remove());

            string fileName = "tmp.html";
            string path = Path.Combine(Server.MapPath("~/Areas/Admin/SavedPages"), fileName);
            document.Save(path, new UTF8Encoding());

            return RedirectToAction("Index");
        }

        public ActionResult ParseData()
        {
            HtmlWeb web = new HtmlWeb();
            web.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:26.0) Gecko/20100101 Firefox/26.0";
            HtmlDocument document = new HtmlDocument();
            document.OptionFixNestedTags = true;
            document = web.Load("http://www.chobinhtay.gov.vn/PriceBoard.aspx");
            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes(".//*[@id='ctl00_Mnt_Cnt_Gr_List']/tr/td[1]");
            return View(nodes);
        }
    }
}
