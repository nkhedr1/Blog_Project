using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Microsoft.AspNet.Identity;
using Blog.Models.Domain;
using Blog.Models.ViewModels;
using System.IO;

namespace Blog.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private List<string> AllowedExtenions = new List<string>
                { ".jpeg", ".jpg", ".gif", ".png" };

        private ApplicationDbContext DbContext;
        public List<Comment> Comments { get; set; }

        public PostController()
        {
            DbContext = new ApplicationDbContext();
            Comments = new List<Comment>();
        }

        [HttpGet]
        public ActionResult BlogIndex()
        {
            var userId = User.Identity.GetUserId();

            var allPosts = DbContext.Posts
                .Select(post => new ListPostsViewModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body,
                    DateCreated = post.DateCreated
                }).ToList();

            return View(allPosts);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(CreateViewModel postData)
        {

            return SavePost(null, postData);

        }

        private ActionResult SavePost(int? id, CreateViewModel postData)
        {
            var fileExtension = Path.GetExtension(postData.UploadedFile.FileName).ToLower();

            if (!AllowedExtenions.Contains(fileExtension))
            {
                ModelState.AddModelError("", "File extension is not allowed");
                return View();
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = User.Identity.GetUserId();


            Post currentPost;


            if (!id.HasValue)
            {
                currentPost = new Post();
                currentPost.UserId = userId;
                DbContext.Posts.Add(currentPost);
            }
            else
            {
                currentPost = DbContext.Posts.FirstOrDefault(
                post => post.Id == id);

                if (currentPost == null)
                {
                    return RedirectToAction(nameof(PostController.ListPosts));
                }
            }



            currentPost.Title = postData.Title;
            currentPost.Body = postData.Body;
            currentPost.SlugTitle = postData.SlugRoute(postData.Title);
            // Checking if the SlugTitle is a duplicate
            Post slugTitleDuplicateQuery = DbContext.Posts.FirstOrDefault(
               post => post.SlugTitle == currentPost.SlugTitle);

            if (slugTitleDuplicateQuery != null)
            {
                currentPost.SlugTitle = String.Concat(currentPost.SlugTitle, "1");
            }

            currentPost.DateCreated = DateTime.Today;
            currentPost.Published = postData.Published;
            currentPost.MediaUrl = UploadFile(postData.UploadedFile);

            DbContext.SaveChanges();

            return RedirectToAction(nameof(PostController.ListPosts));
        }

        [HttpGet]
        [Route("blog/{title}")]
        public ActionResult ViewPost(string title, int id)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return RedirectToAction(nameof(PostController.ListPosts));
            }


            var userId = User.Identity.GetUserId();

            var selectedPost = DbContext.Posts.FirstOrDefault(post =>
            post.SlugTitle == title &&
            post.UserId == userId);

            if (selectedPost == null)
            {
                return RedirectToAction(nameof(PostController.ListPosts));
            }

            var postToView = new ViewPostViewModel();
            postToView.Title = selectedPost.Title;
            postToView.Id = selectedPost.Id;
            postToView.Body = selectedPost.Body;
            postToView.DateCreated = selectedPost.DateCreated;
            postToView.DateUpdated = DateTime.Today;
            Comments = DbContext.Comments.ToList();

            var postComments = Comments
                               .Where(comment => comment.PostId == id)
                               .Select(comment => new Comment
                               {
                                   Id = comment.Id,
                                   Body = comment.Body,
                                   DateCreated = comment.DateCreated
                               }).ToList();

            postToView.Comments = postComments;

            return View(postToView);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(PostController.ListPosts));

            }

            var userId = User.Identity.GetUserId();

            var postToModify = DbContext.Posts.FirstOrDefault(
                post => post.Id == id && post.UserId == userId);

            if (postToModify == null)
            {
                return RedirectToAction(nameof(PostController.ListPosts));
            }


            var editModel = new CreateViewModel();
            editModel.Title = postToModify.Title;
            editModel.Body = postToModify.Body;

            return View(editModel);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, CreateViewModel postData)
        {
            return SavePost(id, postData);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(PostController.ListPosts));
            }

            var userId = User.Identity.GetUserId();

            var postToRemove = DbContext.Posts.FirstOrDefault(
                post => post.Id == id && post.UserId == userId);

            if (postToRemove != null)
            {
                DbContext.Posts.Remove(postToRemove);
                DbContext.SaveChanges();
            }

            return RedirectToAction(nameof(PostController.ListPosts));
        }

        public ActionResult ListPosts()
        {
            var userId = User.Identity.GetUserId();
            List<ListPostsViewModel> blogPosts;
            if (User.IsInRole("Admin"))
            {
                blogPosts = DbContext.Posts
                               .Where(post => post.UserId == userId)
                               .Select(post => new ListPostsViewModel
                               {
                                   Id = post.Id,
                                   Title = post.Title,
                                   Body = post.Body,
                                   DateCreated = post.DateCreated,
                                   SlugTitle = post.SlugTitle,
                                   Published = post.Published
                               }).ToList();
            }

            else
            {
                blogPosts = DbContext.Posts
                               .Where(post => post.Published)
                               .Select(post => new ListPostsViewModel
                               {
                                   Id = post.Id,
                                   Title = post.Title,
                                   Body = post.Body,
                                   DateCreated = post.DateCreated,
                                   Published = post.Published
                               }).ToList();
            }

            return View(blogPosts);

        }

        private string UploadFile(HttpPostedFileBase file)
        {
            if (file != null)
            {
                var uploadFolder = "~/Upload/";
                var mappedFolder = Server.MapPath(uploadFolder);

                if (!Directory.Exists(mappedFolder))
                {
                    Directory.CreateDirectory(mappedFolder);
                }

                file.SaveAs(mappedFolder + file.FileName);

                return uploadFolder + file.FileName;
            }

            return null;
        }

        [HttpGet]
        public ActionResult AddComment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddComment(int id, AddCommentViewModel commentData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = User.Identity.GetUserId();

            Comment currentComment;

            currentComment = new Comment();
            currentComment.UserId = userId;

            currentComment.PostId = id;
            currentComment.Body = commentData.Body;
            currentComment.UpdatedReason = commentData.UpdatedReason;
            currentComment.DateCreated = DateTime.Today;
            currentComment.DateUpdated = DateTime.Today;
            DbContext.Comments.Add(currentComment);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(PostController.ListPosts));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditComment(int id)
        {

            var userId = User.Identity.GetUserId();

            var commentToModify = DbContext.Comments.FirstOrDefault(
                comment => comment.PostId == id);

            var editCommentModel = new AddCommentViewModel();
            editCommentModel.Body = commentToModify.Body;
            editCommentModel.UpdatedReason = commentToModify.UpdatedReason;
            editCommentModel.DateUpdated = DateTime.Today;

            return View(editCommentModel);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditComment(int id, AddCommentViewModel commentData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userId = User.Identity.GetUserId();

            Comment currentComment;

            currentComment = new Comment();
            currentComment.UserId = userId;

            currentComment.PostId = id;
            currentComment.Body = commentData.Body;
            currentComment.UpdatedReason = commentData.UpdatedReason;
            currentComment.DateCreated = commentData.DateCreated;
            currentComment.DateUpdated = DateTime.Today;
            DbContext.Comments.Add(currentComment);
            DbContext.SaveChanges();

            return RedirectToAction(nameof(PostController.ListPosts));
        }
    }
}