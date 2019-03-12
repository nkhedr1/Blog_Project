using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class ViewPostViewModel
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool Published { get; set; }
    }
}