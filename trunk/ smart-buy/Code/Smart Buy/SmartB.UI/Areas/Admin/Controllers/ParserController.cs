using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using SmartB.UI.Areas.Admin.Helper;
using SmartB.UI.Areas.Admin.Models;
using SmartB.UI.Models.EntityFramework;
using PagedList;

namespace SmartB.UI.Areas.Admin.Controllers
{
    public class ParserController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();
        private const int PageSize = 10;

        //
        // GET: /Admin/ParseWeb/

        public ActionResult Index(int page = 1)
        {
            var parsers = context.ParseInfoes
                .Include(x => x.Market)
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .ToPagedList(page, PageSize);
            return View(parsers);
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
            document.DocumentNode.Descendants().Where(x => x.Name == "script").ToList().ForEach(x => x.Remove());

            string fileName = "tmp.html";
            string path = Path.Combine(Server.MapPath("~/Areas/Admin/SavedPages"), fileName);
            document.Save(path, new UTF8Encoding());

            TempData["link"] = parseLink;
            return RedirectToAction("CreateParser");
        }

        public ActionResult CreateParser()
        {
            var markets = context.Markets
                .OrderBy(x => x.Name)
                .Where(x => x.IsActive)
                .ToList();
            var marketList = new List<SelectListItem>();
            foreach (var market in markets)
            {
                var item = new SelectListItem
                               {
                                   Text = market.Name,
                                   Value = market.Id.ToString()
                               };
                marketList.Add(item);
            }

            ViewBag.Markets = marketList;
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult CreateParser(ParserCreator model)
        {
            var parser = new ParseInfo
                             {
                                 MarketId = model.MarketId,
                                 ParseLink = model.ParseLink,
                                 ProductNameXpath = model.ProductNameXpath,
                                 PriceXpath = model.PriceXpath,
                                 PagingXpath = model.PagingXpath,
                                 IsActive = true
                             };
            context.ParseInfoes.Add(parser);
            context.SaveChanges();
            TempData["create"] = "Success";
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
