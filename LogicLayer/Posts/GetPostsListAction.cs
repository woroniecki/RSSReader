using System.Collections.Generic;
using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;
using LogicLayer._const;
using LogicLayer._GenericActions;

namespace LogicLayer.Groups
{
    public class GetPostsListAction :
        ActionErrors,
        IActionAsync<int, IEnumerable<Post>>
    {
        private int _blogId;
        private IUnitOfWork _unitOfWork;

        public GetPostsListAction(int blogId, IUnitOfWork unitOfWork)
        {
            _blogId = blogId;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Post>> ActionAsync(int page)
        {
            IEnumerable<Post> posts = await _unitOfWork.PostRepo
                .GetLatest(_blogId, page * RssConsts.POSTS_PER_CALL, RssConsts.POSTS_PER_CALL);

            return posts;
        }
    }
}
