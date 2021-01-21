using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogApi.Domain.Blogs
{
    public class Blog : Entity
    {
        public string Url { get; set; }

        public List<Post> Posts { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

    public class Post : Entity
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public Guid BlogId { get; set; }
        public Blog Blog { get; set; }

        public List<PostTag> Tags { get; set; }
    }

    public class PostTag
    {
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }

        public Guid PostId { get; set; }
        public Post Post { get; set; }
    }

    public class Tag : Entity
    {
        public string Name { get; set; }

        public List<PostTag> Posts { get; set; }
    }

    public class User : Entity
    {
        public string UserName { get; set; }

        public List<Blog> OwnedBlogs { get; set; }
    }
}
