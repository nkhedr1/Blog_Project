using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class AddCommentViewModel
    {
        public int Id { get; set; }

        public string Body { get; set; }

        public string UpdatedReason { get; set; }

        public int PostId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}