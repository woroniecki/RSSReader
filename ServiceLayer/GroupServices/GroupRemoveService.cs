using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DbAccess.Core;
using Dtos.Groups;
using LogicLayer.Groups;
using ServiceLayer._Commons;
using ServiceLayer._Runners;

namespace ServiceLayer.GroupServices
{
    public class GroupRemoveService : IGroupRemoveService
    {
        private RemoveGroupAction _action;
        private IUnitOfWork _unitOfWork;

        public IImmutableList<ValidationResult> Errors => _action != null ? _action.Errors : null;

        public GroupRemoveService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Remove(RemoveGroupRequestDto data, string userId)
        {
            _action = new RemoveGroupAction(userId, _unitOfWork);

            var runner = new RunnerWriteDbAsync<RemoveGroupRequestDto, bool>(
                _action,
                _unitOfWork.Context
                );

            var result = await runner.RunActionAsync(data);

            return;
        }
    }

    public interface IGroupRemoveService : IValidatedService
    {
        Task Remove(RemoveGroupRequestDto data, string userId);
    }
}
