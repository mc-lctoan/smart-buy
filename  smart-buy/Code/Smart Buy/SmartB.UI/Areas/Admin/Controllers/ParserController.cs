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
using SmartB.UI.Models.EntityFramework;

namespace SmartB.UI.Areas.Admin.Controllers
{
    public class ParserController : Controller
    {
        SmartBuyEntities context = new SmartBuyEntities();

        //
        // GET: /Admin/ParseWeb/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult LoadWeb(string parseLink)
        {
            // Create Firefox browser
            var web = new HtmlWeb {UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:26.0) Gecko/20100101 Firefox/26.0"};

            // Load website
            var document = web.Load(parseLink);

            // Correct links
            var src = new List<HtmlNode>(document.DocumentNode.Descendants().Where(x => x.Attributes["src"] != null));
            var link = new List<HtmlNode>(document.DocumentNode.Descendants().Where(x => x.Attributes["href"] != null));
            ParseHelper.CorrectLink(src, parseLink, "src");
            ParseHelper.CorrectLink(link, parseLink, "href");

            // TODO: Remove all script?
            //document.DocumentNode.Descendants().Where(x => x.Name == "script").ToList().ForEach(x => x.Remove());

            string fileName = "tmp.html";
            string path = Path.Combine(Server.MapPath("~/Areas/Admin/SavedPages"), fileName);
            document.Save(path, new UTF8Encoding());

            TempData["link"] = parseLink;
            return RedirectToAction("Index");
        }

        public RedirectToRouteResult CreateParser(ParserCreator model)
        {
            ParseInfo parser = new ParseInfo
                                   {
                                       // TODO: fix market for testing
                                       MarketId = 1,
                                       ParseLink = model.ParseLink,
                                       ProductNameXpath = model.ProductNameXpath,
                                       PriceXpath = model.PriceXpath,
                                       IsActive = true
                                   };
            context.ParseInfoes.Add(parser);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        //[HttpPost]
        public ActionResult ParseData()
        {
            ParseHelper.ParseData(HttpContext.Application["LogPath"].ToString());
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
