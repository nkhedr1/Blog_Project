using Blog.Models;
using Blog.Models.Domain;
using Blog.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class CommentController : Controller
    {
        private ApplicationDbContext DbContext;

        public CommentController()
        {
            DbContext = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddComment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddComment(int id, AddCommentViewModel commentData)
        {

            return SaveComment(id, commentData);

        }

        private ActionResult SaveComment(int id, AddCommentViewModel commentData)
        {

            //if (!ModelState.IsValid)
            //{
            //    return View();
            //}

            var userId = User.Identity.GetUserId();

            Comment currentComment;

            currentComment = new Comment();
            currentComment.UserId = userId;

            //if (currentComment == null)
            //{
            //    return RedirectToAction(nameof(CommentController.AddComment));
            //}
            currentComment.PostId = id;
            currentComment.Body = commentData.Body;
            currentComment.UpdatedReason = commentData.UpdatedReason;
            currentComment.DateCreated = DateTime.Today;
            currentComment.DateUpdated = DateTime.Today;
            DbContext.Comments.Add(currentComment);
            //Comments.Add(currentComment);
            DbContext.SaveChanges();

            //return RedirectToAction(nameof(PostController.ViewPost));
            return View();
        }
    }
}