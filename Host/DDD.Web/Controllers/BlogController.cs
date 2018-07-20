using DDD.Domain.Core.Model.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWeb.Controllers
{
    public class BlogController : BaseController
    {
        private readonly IBlogRepository _blogRepository;

        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        // GET: Blog
        public ActionResult Index()
        {
            var posts = _blogRepository.GetAllPosts(1);

            return View(posts);
        }
    }
}