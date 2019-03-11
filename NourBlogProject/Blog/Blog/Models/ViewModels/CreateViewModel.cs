using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }
    }
}