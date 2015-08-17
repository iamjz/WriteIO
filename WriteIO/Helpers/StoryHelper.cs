using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WriteIO.DAL;
using WriteIO.Models;

namespace WriteIO.Helpers
{
    public class StoryHelper
    {
        public static IEnumerable<CommittedStory> GetFullStory(int storyID)
        {
            IEnumerable<CommittedStory> storyResults = null;

            try
            {
                using (var db = new WriteEntities())
                {
                    //pull from datasource
                    var storyQuery = (from sto in db.Stories
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
                                          sec.IsCommitted
                                      });

                    //map to view-model.
                    storyResults = storyQuery.Select(fs => new CommittedStory
                    {
                        StoryID = fs.StoryID,
                        PromptID = fs.PromptID,
                        SectionID = fs.SectionID,
                        Author = fs.Author,
                        WrittenDate = fs.WrittenDate,
                        Upvotes = fs.Upvotes,
                        Downvotes = fs.Downvotes,
                        SectionContent = fs.SectionContent,
                        IsCommitted = fs.IsCommitted
                    }).ToList();
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