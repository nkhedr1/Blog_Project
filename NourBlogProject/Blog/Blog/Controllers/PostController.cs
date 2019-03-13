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
    public class PostController : Controller
    {

        private ApplicationDbContext DbContext;

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

        public PostController()
        {
            DbContext = new ApplicationDbContext();
        }

        [HttpGet]
        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateViewModel postData)
        {

            return SavePost(null, postData);

        }

        private ActionResult SavePost(int? id, CreateViewModel postData)
        {

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

            //if (postData.UploadedFile.ContentLength > 0 && postData.UploadedFile != null)
            //{
            //    var fileName = Path.GetFileName(postData.UploadedFile.FileName);
            //    var path = Path.Combine(Server.MapPath("~/uploads"), fileName);
            //    postData.UploadedFile.SaveAs(path);
            //}

            currentPost.Title = postData.Title;
            currentPost.Body = postData.Body;
            currentPost.DateCreated = DateTime.Today;
            currentPost.DateUpdated = DateTime.Today;
            DbContext.SaveChanges();

            return RedirectToAction(nameof(PostController.ListPosts));
        }

        [HttpGet]
        public ActionResult ViewPost(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(PostController.ListPosts));
            }


            var userId = User.Identity.GetUserId();

            var selectedPost = DbContext.Posts.FirstOrDefault(post =>
            post.Id == id.Value &&
            post.UserId == userId);

            if (selectedPost == null)
            {
                return RedirectToAction(nameof(PostController.ListPosts));
            }


            var postToView = new ViewPostViewModel();
            postToView.Title = selectedPost.Title;
            postToView.Body = selectedPost.Body;
            postToView.DateCreated = selectedPost.DateCreated;
            postToView.DateUpdated = DateTime.Today;

            return View(postToView);
        }

        [HttpGet]
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditAdmin(int? id)
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
        public ActionResult Edit(int id, CreateViewModel postData)
        {
            return SavePost(id, postData);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditAdmin(int id, CreateViewModel postData)
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

            var blogPosts = DbContext.Posts
                .Where(post => post.UserId == userId)
                .Select(post => new ListPostsViewModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    Body = post.Body,
                    DateCreated = post.DateCreated
                }).ToList();

            return View(blogPosts);


        }


    }
}