using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WriteIO.Models;
using WriteIO.DAL;

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
                    dataModel.Downvotes = 0;
                    dataModel.Upvotes = 0;
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
                                     sec.Upvotes,
                                     sec.Downvotes,
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
                                     sec.Upvotes,
                                     sec.Downvotes,
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

        public static bool UpvoteSection(int sectionID)
        {
            SectionsVM section = getSectionFromId(sectionID);

            if (IsValidStory(section.StoryID)
                && GetCurrentSeq(section.StoryID) == section.SequenceNumber)
            {
                try
                {
                    using (var db = new WriteEntities())
                    {
                        var s = db.Sections.Find(sectionID);

                        if (s != null)
                        {
                            s.Upvotes += 1;
                            db.SaveChanges();
                            return true;
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

        public static bool DownvoteSection(int sectionID)
        {
            SectionsVM section = getSectionFromId(sectionID);

            if (IsValidStory(section.StoryID)
                && GetCurrentSeq(section.StoryID) == section.SequenceNumber)
            {
                try
                {
                    using (var db = new WriteEntities())
                    {
                        var s = db.Sections.Find(sectionID);

                        if (s != null)
                        {
                            s.Downvotes += 1;
                            db.SaveChanges();
                            return true;
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
                        sectionObj.Upvotes = section.Upvotes;
                        sectionObj.Author = section.Author;
                        sectionObj.Downvotes = section.Downvotes;
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
                                                   sec.Upvotes,
                                                   sec.Downvotes,
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
                            Upvotes = cs.Upvotes,
                            Downvotes = cs.Downvotes,
                            SequenceNumber = cs.SequenceNumber,
                            StoryID = cs.StoryID,
                            SectionContent = cs.SectionContent,
                            SortOrder = (cs.Upvotes - cs.Downvotes)
                        }).ToList();

                        storyResults = storyResults.OrderByDescending(s => s.SortOrder);
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