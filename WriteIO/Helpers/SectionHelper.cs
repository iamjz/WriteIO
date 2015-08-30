using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WriteIO.Models;
using WriteIO.DAL;
using System.Web.Mvc;
using System.Security.Principal;
using WriteIO.Controllers;

namespace WriteIO.Helpers
{
    public class SectionHelper
    {
        public static bool InsertSection(SectionsVM submission)
        {
            if (IsValidStory(submission.StoryID))
            {
                if (GetCurrentSeq(submission.StoryID) == submission.SequenceNumber)
                {
                    //map submission back to data model: Section
                    Section dataModel = new Section();

                    dataModel.SectionContent = submission.SectionContent;
                    dataModel.SequenceNumber = submission.SequenceNumber;
                    dataModel.StoryID = submission.StoryID;

                    //default properties upon submission:
                    dataModel.IsCommitted = false;
                    dataModel.WrittenDate = DateTime.Now;
                    //dataModel.Downvotes = 0;
                    //dataModel.Upvotes = 0;
                    dataModel.Author = "Frontend User";

                    try
                    {
                        using (var db = new WriteEntities())
                        {
                            db.Sections.Add(dataModel);
                            db.SaveChanges();

                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        public static bool IsValidStory(int storyID)
        {
            //grab all the sections ordered by sequence number descending.
            //take the top section (latest sequence) and add one to it.
            try
            {
                using (var db = new WriteEntities())
                {
                    var story = (from sto in db.Stories
                                 join sec in db.Sections
                                 on sto.SectionID equals sec.SectionID
                                 where sto.StoryID == storyID
                                 where sec.IsCommitted == true
                                 orderby sec.SequenceNumber ascending
                                 select new
                                 {
                                     sto.StoryID,
                                     sto.PromptID,
                                     sto.SectionID,
                                     sec.Author,
                                     sec.WrittenDate,
                                     sec.SectionContent,
                                     sec.SequenceNumber,
                                     sec.IsCommitted
                                 }).ToList();

                    if (story != null && story.Count > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Add error handling
                return false;
            }

            return false;
        }

        public static int GetCurrentSeq(int storyID)
        {
            try
            {
                using (var db = new WriteEntities())
                {
                    //grab all the sections ordered by sequence number descending.
                    //take the top section (latest sequence) and add one to it.
                    var story = (from sto in db.Stories
                                 join sec in db.Sections
                                 on sto.SectionID equals sec.SectionID
                                 where sto.StoryID == storyID
                                 where sec.IsCommitted == true
                                 orderby sec.SequenceNumber descending
                                 select new
                                 {
                                     sto.StoryID,
                                     sto.PromptID,
                                     sto.SectionID,
                                     sec.Author,
                                     sec.WrittenDate,
                                     sec.SectionContent,
                                     sec.SequenceNumber,
                                     sec.IsCommitted
                                 }).ToList();

                    if (story != null && story.Count > 0)
                    {
                        var LatestSection = story.FirstOrDefault();

                        if (LatestSection.SequenceNumber >= 0)
                        {
                            return LatestSection.SequenceNumber + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Add error handling
                return 0;
            }

            return 0;
        }

        public static bool UpdateSection(SectionsVM submission)
        {
            //make sure storyID and sectionID already exists.
            //make sure sequence is current.

            return false;
        }

        public static bool VoteSection(int sectionID, string username, bool votetype)
        {
            if (String.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            SectionsVM section = getSectionFromId(sectionID);

            if (section != null
                && IsValidStory(section.StoryID)
                && GetCurrentSeq(section.StoryID) == section.SequenceNumber)
            {
                try
                {
                    using (var db = new WriteEntities())
                    {

                        List<SectionTransaction> t = db.SectionTransactions.Where(tr => tr.Username == username
                                    && tr.SectionID == section.SectionID).ToList();

                        if (t != null && t.Count == 1)
                        {
                            if (votetype == true)
                            {
                                t.FirstOrDefault().Vote = 1;
                            }
                            else
                            {
                                t.FirstOrDefault().Vote = -1;
                            }
                            
                            db.SaveChanges();

                            return true;
                        }
                        else if (t != null && t.Count == 0)
                        {
                            SectionTransaction firstTrans = new SectionTransaction();

                            if (votetype == true)
                            {
                                firstTrans.Vote = 1;
                            }
                            else
                            {
                                firstTrans.Vote = -1;
                            }

                            firstTrans.Username = username;
                            firstTrans.SectionID = sectionID;

                            db.SectionTransactions.Add(firstTrans);
                            db.SaveChanges();

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public static int calculateTotUpvotes(int sectionId)
        {
            int totalVotes = 0;

            using (var db = new WriteEntities())
            {
                int? result = db.SectionTransactions.Where(t => t.SectionID == sectionId).Sum(r => r.Vote);

                totalVotes = result ?? 0;
            }

            return totalVotes;
        }

        public static SectionsVM getSectionFromId(int id)
        {
            try
            {
                using (var db = new WriteEntities())
                {
                    var section = db.Sections.Find(id);

                    if (section != null)
                    {
                        SectionsVM sectionObj = new SectionsVM();

                        sectionObj.SectionID = section.SectionID;
                        sectionObj.SectionContent = section.SectionContent;
                        sectionObj.SequenceNumber = section.SequenceNumber;
                        sectionObj.Author = section.Author;
                        sectionObj.Votes = calculateTotUpvotes(section.SectionID);
                        sectionObj.WrittenDate = section.WrittenDate;
                        sectionObj.StoryID = section.StoryID;

                        return sectionObj;
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Add error handling
                return null;
            }

            return null;
        }

        public static IEnumerable<SectionsVM> ShowCurrentSections(int storyID)
        {

            IEnumerable<SectionsVM> storyResults = null;

            try
            {
                using (var db = new WriteEntities())
                {
                    if (IsValidStory(storyID))
                    {
                        int currentSeq = GetCurrentSeq(storyID);

                        var currentSections = (from sec in db.Sections
                                               where sec.StoryID == storyID
                                               where sec.SequenceNumber == currentSeq
                                               select new
                                               {
                                                   sec.StoryID,
                                                   sec.SectionID,
                                                   sec.Author,
                                                   sec.WrittenDate,
                                                   sec.SectionContent,
                                                   sec.SequenceNumber,
                                                   sec.IsCommitted
                                               }).ToList();

                        //map query results to viewmodel.
                        storyResults = currentSections.Select(cs => new SectionsVM
                        {
                            SectionID = cs.SectionID,
                            Author = cs.Author,
                            WrittenDate = cs.WrittenDate,
                            SequenceNumber = cs.SequenceNumber,
                            StoryID = cs.StoryID,
                            SectionContent = cs.SectionContent,
                            Votes = calculateTotUpvotes(cs.SectionID)
                        }).ToList();

                        storyResults = storyResults.OrderByDescending(s => s.Votes);
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Add error handling
                return storyResults;
            }

            return storyResults;
        }
    }
}