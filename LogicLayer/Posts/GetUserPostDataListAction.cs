using System.Collections.Generic;
using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;

using LogicLayer._GenericActions;

namespace LogicLayer.Posts
{
    public class GetUserPostDataListAction :
        ActionErrors,
        IActionAsync<int, IEnumerable<UserPostData>>
    {
        private string _userId;
        private IUnitOfWork _unitOfWork;

        public GetUserPostDataListAction(string userId, IUnitOfWork unitOfWork)
        {
            _userId = userId;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserPostData>> ActionAsync(int blogId)
        {
            ApiUser user = await _unitOfWork.UserRepo.GetByID(_userId);
            if (user == null)
            {
                AddError("Unauthorized.");
                return null;
            }

            IEnumerable<UserPostData> user_post_datas = await _unitOfWork.UserPostDataRepo
                .GetListWithPosts(x => x.User.Id == _userId && x.Post.Blog.Id == blogId);

            return user_post_datas;
        }
    }
}
