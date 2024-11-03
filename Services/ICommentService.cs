using UserAuthentication.DTO_s;
using UserAuthentication.Models;

namespace UserAuthentication.Services
{
    public interface ICommentService
    {
        Task<CommentModel> CreateCommentAsync(CommentDto commentDto, string userId, string userName);
        Task<IEnumerable<CommentModel>> GetAllCommentsAsync();
        Task<IEnumerable<CommentModel>> GetCommentsByUserAsync(string UserName);
        Task<bool> DeleteCommentAsync(int id, string userId, bool isAdmin);

        Task<CommentModel> UpdateCommentAsync(int id, CommentDto commentDto, string userId, string UserName);
    }
}
