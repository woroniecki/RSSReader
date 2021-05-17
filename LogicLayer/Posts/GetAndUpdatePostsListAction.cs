using System.Collections.Generic;
using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;
using LogicLayer._const;
using LogicLayer._GenericActions;

namespace LogicLayer.Groups
{
    public class GetAndUpdatePostsListAction :
        ActionErrors,
        IActionAsync<int, IEnumerable<Post>>
    {
        private string _userId;
        private int _blogId;
        private IUnitOfWork _unitOfWork;

        public GetAndUpdatePostsListAction(string userId, int blogId, IUnitOfWork unitOfWork)
        {
            _userId = userId;
            _blogId = blogId;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Post>> ActionAsync(int page)
        {
            ApiUser user = await _unitOfWork.UserRepo.GetByID(_userId);
            if (user == null)
            {
                AddError("Unauthorized.");
                return null;
            }

            IEnumerable<Post> posts = await _unitOfWork.PostRepo
                .GetLatest(_blogId, page * RssConsts.POSTS_PER_CALL, RssConsts.POSTS_PER_CALL);

            return posts;
        }
    }
}
