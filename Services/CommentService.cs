using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserAuthentication.Data;
using UserAuthentication.DTO_s;
using UserAuthentication.Models;

namespace UserAuthentication.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CommentService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<CommentModel>> GetAllCommentsAsync()
        {
            return await _context.Comments.Select(x => new CommentModel
            {
                Id = x.Id,
                PostId = x.PostId,
                UserName = x.User.UserName,
                Content = x.Content,
                CreatedDate = x.CreatedDate,
            }).ToListAsync();
        }

        public async Task<IEnumerable<CommentModel>> GetCommentsByUserAsync(string UserName)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null) return null;
            return await _context.Comments.Where(c => c.User.UserName == UserName)
                .Select(x => new CommentModel
            {
                Id = x.Id,
                PostId = x.PostId,
                UserName = x.User.UserName,
                Content = x.Content,
                CreatedDate = x.CreatedDate,
            }).ToListAsync();
        }

        public async Task<CommentModel> CreateCommentAsync(CommentDto commentDto, string userId, string userName)
        {
            var post = await _context.Posts.FindAsync(commentDto.PostId);
            if (post == null)
                return null;
            var comment = new Comment();
            comment.UserId = userId;
            comment.PostId = commentDto.PostId;
            comment.Content = commentDto.content;
            comment.CreatedDate = DateTime.Now.ToLocalTime();
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return new CommentModel
            {
                Id= comment.Id,
                PostId= comment.PostId,
                UserName = userName,
                Content = comment.Content,
                CreatedDate = DateTime.Now.ToLocalTime()
            };
        }

        public async Task<bool> DeleteCommentAsync(int id, string userId, bool isAdmin)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return false;
            var post = await _context.Posts.FindAsync(comment.PostId);
            if (post == null) return false;
            if (!isAdmin && comment.UserId != userId && post.AuthorId != userId) return false;
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CommentModel> UpdateCommentAsync(int id, CommentDto commentDto, string userId, string UserName)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null) return null;
            if (comment.UserId != userId) return null;
            comment.Content = commentDto.content;
            _context.Update(comment);
            await _context.SaveChangesAsync();
            return new CommentModel
            {
                Id = comment.Id,
                PostId = comment.PostId,
                UserName = UserName,
                Content = comment.Content,
                CreatedDate = comment.CreatedDate,
            };
        }

    }
}
