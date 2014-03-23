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
            ParseHelper.LoadWeb(parseLink);
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

        public ActionResult EditParser(int id)
        {
            var parser = context.ParseInfoes.FirstOrDefault(x => x.Id == id);

            if (parser != null)
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

                ParseHelper.LoadWeb(parser.ParseLink);
                TempData["link"] = parser.ParseLink;
            }

            return View(parser);
        }

        [HttpPost]
        public RedirectToRouteResult EditParser(ParseInfo model)
        {
            var parser = context.ParseInfoes.FirstOrDefault(x => x.Id == model.Id);
            string message;
            if (parser != null)
            {
                parser.MarketId = model.MarketId;
                parser.ParseLink = model.ParseLink;
                parser.ProductNameXpath = model.ProductNameXpath;
                parser.PriceXpath = model.PriceXpath;
                parser.PagingXpath = model.PagingXpath;

                context.SaveChanges();
                message = "Success";
            }
            else
            {
                message = "Failed";
            }
            TempData["edit"] = message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public RedirectToRouteResult DeleteParser(int[] ids)
        {
            foreach (int id in ids)
            {
                var parser = context.ParseInfoes.FirstOrDefault(x => x.Id == id);
                if (parser != null)
                {
                    parser.IsActive = false;
                }
            }
            context.SaveChanges();
            TempData["delete"] = "Done";
            return RedirectToAction("Index");
        }

        //[HttpPost]
        public ActionResult ParseData()
        {
            ParseHelper.ParseData();
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
