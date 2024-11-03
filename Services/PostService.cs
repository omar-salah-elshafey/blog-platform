using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserAuthentication.Data;
using UserAuthentication.DTO_s;
using UserAuthentication.Models;

namespace UserAuthentication.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        //private readonly IMapper _mapper;
        public PostService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<PostModel>> GetAllPostsAsync()
        {
            return await _context.Posts.Select(p=> new PostModel
            {
                Id = p.Id,
                AuthorName = p.Author.UserName,
                Title = p.Title,
                Content = p.Content,
                CreatedDate = p.CreatedDate,
                ModifiedDate = p.ModifiedDate,
                Comments = p.Comments.Select(c => new CommentsPostModel
                {
                    UserName = c.User.UserName,
                    Content = c.Content,
                    CreatedDate = c.CreatedDate
                }).ToList()
            }).ToListAsync();
        }

        public async Task<IEnumerable<PostModel>> GetPostsByUserAsync(string UserName)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null) return null;
            return await _context.Posts.Where(x => x.Author.UserName == UserName)
                .Select(p => new PostModel
            {
                Id = p.Id,
                AuthorName = p.Author.UserName,
                Title = p.Title,
                Content = p.Content,
                CreatedDate = p.CreatedDate,
                ModifiedDate = p.ModifiedDate,
                Comments = p.Comments.Select(c => new CommentsPostModel
                {
                    UserName = c.User.UserName,
                    Content = c.Content,
                    CreatedDate = c.CreatedDate
                }).ToList()
            }).ToListAsync();
        }

        public async Task<PostModel> GetPostByIdAsync(int id)
        {
            var post = await _context.Posts
                .Where(x => x.Id == id)
                .Select(p => new PostModel
                {
                    Id = p.Id,
                    AuthorName = p.Author.UserName,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedDate = p.CreatedDate,
                    ModifiedDate = p.ModifiedDate,
                    Comments = p.Comments.Select(c => new CommentsPostModel
                    {
                        UserName = c.User.UserName,
                        Content = c.Content,
                        CreatedDate = c.CreatedDate
                    }).ToList()
                }).FirstOrDefaultAsync();
            return post;
        }
        public async Task<PostModel> CreatePostAsync(PostDto postDto, string authId, string authUserName)
        {
            var post = new Post();
            post.AuthorId = authId;
            post.Content = postDto.Content;
            post.Title = postDto.Title;
            post.CreatedDate = DateTime.Now.ToLocalTime();
            // Add the post to the context
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return new PostModel
            {
                Id = post.Id,
                AuthorName = authUserName,
                Title = postDto.Title,
                Content = postDto.Content,
                CreatedDate = post.CreatedDate,
                ModifiedDate = post.ModifiedDate,
                Comments = _context.Comments.Where(x => x.PostId == post.Id).Select(c => new CommentsPostModel
                {
                    UserName = c.User.UserName,
                    Content = c.Content,
                    CreatedDate = c.CreatedDate
                }).ToList()
            };
        }

        public async Task<bool> DeletePostAsync(int id, string userId, bool isAdmin)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return false;
            if (!isAdmin && post.AuthorId != userId)
                return false;
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PostModel> UpdatePostAsync(int id, PostDto postDto, string authId, string authUserName)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return null;
            if (post.AuthorId != authId)
                return null;
            post.Title = postDto.Title;
            post.Content = postDto.Content;
            post.ModifiedDate = DateTime.Now.ToLocalTime();
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            return new PostModel
            {
                Id = id,
                AuthorName = authUserName,
                Title = postDto.Title,
                Content = postDto.Content,
                CreatedDate = post.CreatedDate,
                ModifiedDate = post.ModifiedDate
            };
        }
    }
}
