﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserAuthentication.Data;
using UserAuthentication.DTO_s;
using UserAuthentication.Models;
using UserAuthentication.Services;

namespace UserAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly UserManager<ApplicationUser> _userManager;
        public PostController(IPostService postService, UserManager<ApplicationUser> userManager)
        {
            _postService = postService;
            _userManager = userManager;
        }

        [HttpGet("get-all-posts")]
        public async Task<IActionResult> GetAllPostsAsync()
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }

        [HttpPost("create-post")]
        [Authorize(Roles = "Author, Admin")]
        public async Task<IActionResult> CreatePostAsync(PostDto postDto)
        {
            var authId = Request.Cookies["userId"];
            var authUserName = Request.Cookies["UserName"];
            var createdPost = await _postService.CreatePostAsync(postDto, authId, authUserName);
            if (createdPost == null)
                return BadRequest("An error occurred!");
            return Ok(createdPost);
        }

        [HttpPost("delete-post")]
        [Authorize(Roles = "Author, Admin")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = Request.Cookies["userId"];
            var user = await _userManager.FindByIdAsync(userId);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            var deletedPost = await _postService.DeletePostAsync(id, userId, isAdmin);
            if (!deletedPost) return NotFound(string.Empty);
            return Ok(deletedPost);
        }

        [HttpPost]
        [Authorize(Roles = "Author, Admin")]
        public async Task<IActionResult> UpdatePostAsync(int id, PostDto postDto)
        {
            var authId = Request.Cookies["userId"];
            var userName = Request.Cookies["userName"];
            var UpdatedPost = await _postService.UpdatePostAsync(id, postDto, authId, userName);
            if (UpdatedPost == null) return BadRequest(string.Empty);
            return Ok(UpdatedPost);
        }

    }
}
