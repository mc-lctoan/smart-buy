using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SmartB.UI.Areas.Admin.Helper;
using SmartB.UI.Areas.Admin.Models;
using SmartB.UI.Infrastructure;
using SmartB.UI.Models.EntityFramework;
using PagedList;

namespace SmartB.UI.Areas.Admin.Controllers
{
    [MyAuthorize(Roles = "staff")]
    public class ParserController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();
       // private const int PageSize = 10;

        //
        // GET: /Admin/ParseWeb/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var parsers = from p in context.ParseInfoes
                          .Include(x => x.Market)
                          .OrderBy(x => x.Id)
                           select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                parsers = parsers.Where(s => s.Market.Name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    parsers = parsers.OrderByDescending(s => s.Market.Name);
                    break;
                default:
                    parsers = parsers.OrderBy(s => s.Market.Name);
                    break;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(parsers.ToPagedList(pageNumber, pageSize));
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

        [HttpPost]
        public RedirectToRouteResult ParseData()
        {
            Task.Factory.StartNew(ParseHelper.ParseData);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult SetActive(int id)
        {
            var parser = context.ParseInfoes.FirstOrDefault(x => x.Id == id);
            bool statusFlag = false;
            if (ModelState.IsValid)
            {
                if (parser.IsActive == true)
                {
                    parser.IsActive = false;
                    statusFlag = false;
                }
                else
                {
                    parser.IsActive = true;
                    statusFlag = true;
                }
                context.SaveChanges();
            }

            // Display the confirmation message
            var results = new ParseInfo
            {
                IsActive = statusFlag
            };

            return Json(results);
        }
    }
}
