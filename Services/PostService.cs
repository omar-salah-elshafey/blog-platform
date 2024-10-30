using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserAuthentication.Data;
using UserAuthentication.DTO_s;
using UserAuthentication.Models;

namespace UserAuthentication.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        //private readonly IMapper _mapper;
        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostModel>> GetAllPostsAsync()
        {
            return await _context.Posts.Select(x => new PostModel
            {
                AuthorName = x.Author.UserName,
                Title = x.Title,
                Content = x.Content,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
            }).ToListAsync();
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
                AuthorName = authUserName,
                Title = postDto.Title,
                Content = postDto.Content,
                CreatedDate = post.CreatedDate,
                ModifiedDate = post.ModifiedDate,
            };
        }

        public async Task<bool> DeletePostAsync(int id, string currentUserId, bool isAdmin)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
                return false;
            if (!isAdmin && post.AuthorId != currentUserId)
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
                AuthorName = authUserName,
                Title = postDto.Title,
                Content = postDto.Content,
                CreatedDate = post.CreatedDate,
                ModifiedDate = post.ModifiedDate
            };
        }
    }
}
