using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WriteIO.Models
{
    public class SectionsVM
    {
        public Nullable<int> SectionID { get; set; }

        public string Author { get; set; }

        public DateTime WrittenDate { get; set; }

        public int Votes { get; set; }

        public int SequenceNumber { get; set; }

        public int StoryID { get; set; }

        [Required]
        [StringLength(50000)]
        public string SectionContent { get; set; }

        public int SortOrder { get; set; }
    }
}