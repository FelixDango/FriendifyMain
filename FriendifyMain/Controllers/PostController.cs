using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FriendifyMain.Models;

namespace FriendifyMain.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        // GET: api/Post
        [HttpGet]
        public IActionResult List()
        {
            // TODO: Get the posts from the database
            var posts = new List<Post>();
            return Json(posts);
        }

        // GET: api/Post/5
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            // TODO: Get the post details from the database
            var post = new Post();
            return Json(post);
        }

        // POST: api/Post
        [HttpPost]
        public IActionResult Create([FromBody] Post post)
        {
            // TODO: Validate and save the post to the database
            return CreatedAtAction(nameof(Details), new { id = post.Id }, post);
        }

        // PUT: api/Post/5
        [HttpPut("{id}")]
        public IActionResult Edit(int id, [FromBody] Post post)
        {
            // TODO: Validate and update the post in the database
            return NoContent();
        }

        // DELETE: api/Post/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // TODO: Delete the post from the database
            return NoContent();
        }
    }
}