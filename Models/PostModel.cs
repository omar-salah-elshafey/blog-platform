using UserAuthentication.Data;

namespace UserAuthentication.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } 
        public DateTime? ModifiedDate { get; set; } 
        public IEnumerable<CommentsPostModel> Comments { get; set; }
    }
}
