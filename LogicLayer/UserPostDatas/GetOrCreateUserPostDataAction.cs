using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;

using LogicLayer._GenericActions;

namespace LogicLayer.UserPostDatas
{
    public class GetOrCreateUserPostDataAction :
        ActionErrors,
        IActionAsync<int, UserPostData>
    {
        private IUnitOfWork _unitOfWork;
        private string _userId;

        public GetOrCreateUserPostDataAction(string userId, IUnitOfWork unitOfWork)
        {
            _userId = userId;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserPostData> ActionAsync(int postId)
        {
            ApiUser user = await _unitOfWork.UserRepo.GetByID(_userId);
            if (user == null)
            {
                AddError("Unauthorized.");
                return null;
            }
            
            Post post = await _unitOfWork.PostRepo.GetByID(postId);
            if (post == null)
            {
                AddError("Can't find post entity.");
                return null;
            }

            Subscription sub = await _unitOfWork.SubscriptionRepo.GetByUserIdAndBlogId(_userId, post.BlogId);
            if (sub == null)
            {
                AddError("Can't find sub entity.");
                return null;
            }

            UserPostData user_post_data = await _unitOfWork.UserPostDataRepo.GetWithPost(
                    x => x.User == user && x.Post == post
                    );

            if (user_post_data == null)
            {
                user_post_data = new UserPostData(post, user, sub);
                _unitOfWork.UserPostDataRepo.AddNoSave(user_post_data);
            }

            return user_post_data;
        }
    }
}
