using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WriteIO.Models;
using WriteIO.DAL;
using WriteIO.Helpers;

namespace WriteIO.Controllers
{
    public class SectionController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult WriteSection(SectionsVM submission)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                if (ModelState.IsValid
                    && Session["StoryID"].ToString() != null
                    && Session["CurrentSeq"].ToString() != null)
                {
                    var StoryID = Session["StoryID"];
                    int formattedID = 0;
                    bool isIdNumeric = int.TryParse(StoryID.ToString(), out formattedID);

                    //check seq number
                    var SeqNumber = Session["CurrentSeq"];
                    int formattedSeqNumber = 0;
                    bool isSeqNumNumeric = int.TryParse(SeqNumber.ToString(), out formattedSeqNumber);

                    if (isIdNumeric && isSeqNumNumeric)
                    {
                        submission.StoryID = (int)StoryID;
                        submission.SequenceNumber = (int)SeqNumber;
                        Helpers.SectionHelper.InsertSection(submission);

                        IEnumerable<SectionsVM> s = Helpers.SectionHelper.ShowCurrentSections(submission.StoryID);
                        return PartialView("_CurrentSections", s);
                    }

                    return View();
                    //if insert failed, redirect to failed submission partial view
                }

                return View();
            }
        }

        [HttpPost]
        public ActionResult Upvote(int id)
        {

            if (Session["StoryID"].ToString() != null
                && Session["CurrentSeq"].ToString() != null
                && Helpers.SectionHelper.UpvoteSection(id))
            {
                var StoryID = Session["StoryID"];
                int formattedID = 0;
                bool isIdNumeric = int.TryParse(StoryID.ToString(), out formattedID);

                if (isIdNumeric && Helpers.SectionHelper.IsValidStory((int)StoryID))
                {
                    IEnumerable<SectionsVM> s = Helpers.SectionHelper.ShowCurrentSections((int)StoryID);
                    return PartialView("_CurrentSections", s);
                }
            }

            return null;
        }

        [HttpPost]
        public ActionResult Downvote(int id)
        {
            if (Session["StoryID"].ToString() != null
                && Session["CurrentSeq"].ToString() != null
                && Helpers.SectionHelper.DownvoteSection(id))
            {
                var StoryID = Session["StoryID"];
                int formattedID = 0;
                bool isIdNumeric = int.TryParse(StoryID.ToString(), out formattedID);

                if (isIdNumeric && Helpers.SectionHelper.IsValidStory((int)StoryID))
                {
                    IEnumerable<SectionsVM> s = Helpers.SectionHelper.ShowCurrentSections((int)StoryID);
                    return PartialView("_CurrentSections", s);
                }
            }

            return null;
        }

        [HttpGet]
        public ActionResult WriteSection(int id)
        {
            return PartialView("_WriteSection");
        }

        [HttpGet]
        public ActionResult GetCurrentSections(int id)
        {
            IEnumerable<SectionsVM> s = Helpers.SectionHelper.ShowCurrentSections(id);
            return PartialView("_CurrentSections", s);
        }
    }
}