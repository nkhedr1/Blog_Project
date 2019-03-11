using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Microsoft.AspNet.Identity;
using Blog.Models.Domain;
using Blog.Models.ViewModels;

namespace Blog.Controllers
{
    public class PostController : Controller
    {

        private ApplicationDbContext DbContext;

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
            //validates the required inputs from the paramter that they are all there
            if (!ModelState.IsValid)
            {

                return View();
            }

            var userId = User.Identity.GetUserId();

            //if (DbContext.Posts.Any(post => post.UserId == userId &&
            //post.Title == postData.Title &&
            //(!id.HasValue || post.Id != id.Value)))
            //{
            //    ModelState.AddModelError(nameof(RegisterEditMovieViewModel.MovieName),
            //        "Movie name should be unique");

            //    PopulateViewBag();
            //    return View();
            //}

            Post post;

            //if (!id.HasValue)
            //{
            post = new Post();
            post.UserId = userId;
            DbContext.Posts.Add(post);
            //}
            //else
            //{
            //    movie = DbContext.Movies.FirstOrDefault(
            //   p => p.Id == id);

            //    if (movie == null)
            //    {
            //        return RedirectToAction(nameof(MovieController.Index));
            //    }
            //}

            post.Title = postData.Title;
            post.Body = postData.Body;
            post.DateCreated = DateTime.Today;
            post.DateUpdated = DateTime.Today;
            DbContext.SaveChanges();

            //return RedirectToAction(nameof(HomeController.Index));
            return View();
        }

        public ActionResult ViewPost()
        {


            return View();
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }

            var userId = User.Identity.GetUserId();

            var postToModify = DbContext.Posts.FirstOrDefault(
                post => post.Id == id && post.UserId == userId);

            if (postToModify == null)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }


            var model = new RegisterEditMovieViewModel();
            model.Category = movie.Category;
            model.Rating = movie.Rating;
            model.MovieName = movie.Name;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(int id, RegisterEditMovieViewModel formData)
        {
            return SaveMovie(id, formData);
        }

        public ActionResult Delete()
        {


            return View();
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

        public ActionResult LogInPage()
        {

            return View();
        }

        public ActionResult Register()
        {

            return View();
        }
    }
}