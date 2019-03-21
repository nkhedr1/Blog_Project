using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Blog.Models.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [AllowHtml]
        [Required]
        public string Body { get; set; }

        public bool Published { get; set; }
        public HttpPostedFileBase UploadedFile { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string MediaUrl { get; set; }
        public string SlugTitle { get; set; }
        Random rnd = new Random();

        public string SlugRoute(string title)
        {
            int titleNum = rnd.Next(1, 99999999);
            string titleStringNum = Convert.ToString(titleNum);

            title = title.ToLower();
            // removing invalid charachers           
            title = Regex.Replace(title, @"[^a-z0-9\s-]", "");
            // converting multiple spaces into one space   
            title = Regex.Replace(title, @"\s+", " ").Trim();
            // adding - instead of spaces
            title = Regex.Replace(title, @"\s", "-");
            title = String.Concat(title, "-", titleStringNum);
            return title;
        }
    }
}