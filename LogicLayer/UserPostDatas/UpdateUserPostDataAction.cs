using System.Threading.Tasks;

using DataLayer.Models;
using DbAccess.Core;
using Dtos.UserPostData;
using LogicLayer._GenericActions;

namespace LogicLayer.UserPostDatas
{
    public class UpdateUserPostDataAction :
        ActionErrors,
        IAction<UpdateUserPostDataRequestDto, bool>
    {
        private UserPostData _userPostData;
        private IUnitOfWork _unitOfWork;

        public UpdateUserPostDataAction(UserPostData userPostData, IUnitOfWork unitOfWork)
        {
            _userPostData = userPostData;
            _unitOfWork = unitOfWork;
        }

        public bool Action(UpdateUserPostDataRequestDto dto)
        {
            if (dto.Readed.HasValue)
            {
                _userPostData.Readed = dto.Readed.Value;
            }

            if (dto.Favourite.HasValue)
            {
                if (_userPostData.Favourite != dto.Favourite.Value)
                    _userPostData.Post.FavouriteAmount += dto.Favourite.Value ? 1 : -1;

                _userPostData.Favourite = dto.Favourite.Value;
            }

            return true;
        }
    }
}
