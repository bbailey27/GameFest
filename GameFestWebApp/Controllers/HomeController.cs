using GameFestWebApp.Components;
using GameFestWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GameFestWebApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //public ActionResult InputDetails()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult InputDetails(HomeViewModel model)
        //{
        //    //InputModel input = new InputModel(model);

        //    //return View(input);
        //}

        public ActionResult AssignTables()
        {
            InputModel input = new InputModel();
            return View(input);
        }


        [HttpPost]
        public ActionResult AssignTables(InputModel model)
        {
            //InputModel input = new InputModel(model);
            //input.Tables.Add(new TableModel("", 4, false));
            //OutputViewModel vm = new OutputViewModel(model);
            //vm.GetResults();
            //return RedirectToAction("InputDetails");
            Organizer organizer = new Organizer(model);
            organizer.RunOptimizer();
            return View(model);

        }

        public ActionResult AddTable(InputModel model)
        {
            TableModel newTable = new TableModel("", 4, false);
            return PartialView("TableView", newTable);
        }
    }
}