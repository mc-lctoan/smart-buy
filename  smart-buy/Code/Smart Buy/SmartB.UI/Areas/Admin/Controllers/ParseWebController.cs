using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

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

        public ActionResult ParseDataSelenium()
        {
            IWebDriver driver = new FirefoxDriver();
            driver.Navigate().GoToUrl("http://www.chobinhtay.gov.vn/PriceBoard.aspx");
            IList<IWebElement> nameList = driver.FindElements(By.XPath(".//*[@id='ctl00_Mnt_Cnt_Gr_List']/tbody/tr/td[1]/span"));
            driver.Close();
            return View(nameList);
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
