using UserAuthentication.Data;
using UserAuthentication.DTO_s;
using UserAuthentication.Models;

namespace UserAuthentication.Services
{
    public interface IPostService
    {
        Task<IEnumerable<PostModel>> GetAllPostsAsync();
        Task<PostModel> GetPostByIdAsync(int id);
        Task<IEnumerable<PostModel>> GetPostsByUserAsync(string UserName);
        Task<PostModel> CreatePostAsync(PostDto postDto, string authId, string authUserName);
        Task<PostModel> UpdatePostAsync(int id, PostDto postDto, string authId, string authUserName);
        Task<bool> DeletePostAsync(int id, string userId, bool isAdmin);
    }
}
