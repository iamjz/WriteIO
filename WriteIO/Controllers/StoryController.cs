using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WriteIO.Models;
using WriteIO.Helpers;
using WriteIO.DAL;

namespace WriteIO.Controllers
{
    public class StoryController : Controller
    {
        [HttpGet]
        public ActionResult ShowFullStory(int id)
        {
            IEnumerable<CommittedStory> fs = Helpers.StoryHelper.GetFullStory(id);

            Session["CurrentSeq"] = Helpers.SectionHelper.GetCurrentSeq(id);
            Session["StoryID"] = id;

            return View(fs);
        }
    }
}