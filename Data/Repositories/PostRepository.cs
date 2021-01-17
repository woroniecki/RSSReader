using Microsoft.EntityFrameworkCore;
using RSSReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RSSReader.Data.Repositories
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        public static Expression<Func<Post, bool>> BY_POSTID(int id) => q => q.Id == id;
        public static Expression<Func<Post, bool>> BY_POSTURL(string url) => q => q.Url == url;

        private readonly DataContext _context;

        public PostRepository(DataContext context)
            : base(context.Posts)
        {
            _context = context;
        }
    }

    public interface IPostRepository : IBaseRepository<Post>
    {
    }
}
