using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Blog.Models.ViewModels
{
    public class ListPostsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool Published { get; set; }
        public string SlugTitle { get; set; }
        //Random rnd = new Random();

        //public string SlugRoute()
        //{
        //    int titleNum = rnd.Next(1, 9999999);
        //    string titleStringNum = Convert.ToString(titleNum);
        //    //string phrase = string.Format("{0}-{1}", Id, Title);

        //    string str = Title.ToLower();
        //    // invalid chars           
        //    str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        //    // convert multiple spaces into one space   
        //    str = Regex.Replace(str, @"\s+", " ").Trim();
        //    // cut and trim 
        //    str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
        //    str = Regex.Replace(str, @"\s", "-"); // hyphens 
        //    str = String.Concat(str, titleStringNum);
        //    return str;
        //}

        //private string RemoveAccent(string text)
        //{
        //    byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
        //    return System.Text.Encoding.ASCII.GetString(bytes);
        //}
    }
}