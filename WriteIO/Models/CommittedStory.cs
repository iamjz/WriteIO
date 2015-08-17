using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WriteIO.Models
{
    public class CommittedStory
    {
        public int StoryID { get; set; }
        public int PromptID { get; set; }
        public int SectionID { get; set; }
        public string Author { get; set; }
        public DateTime WrittenDate { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public string SectionContent { get; set; }
        public bool IsCommitted { get; set; }
    }
}