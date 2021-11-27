using DataLayer.Code;
using DataLayer.Models;
using Dtos.Auth.Register;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using static Microsoft.AspNetCore.Http.StatusCodes;
using ServiceLayer._Commons;

namespace ServiceLayer._CQRS.UserCommands
{
    public class RegisterUserCommand : ICommand
    {
        public RegisterNewUserRequestDto Data { get; set; }
        public string Id { get; set; }
    }

    public class RegisterUserCommandHandler : IHandleCommand<RegisterUserCommand>
    {
        private DataContext _context;
        private UserManager<ApiUser> _userManager;

        public RegisterUserCommandHandler(DataContext context, UserManager<ApiUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task Handle(RegisterUserCommand command)
        {
            using (var tx = _context.Database.BeginTransaction())
            {
                if (await GetUser(x => x.UserName == command.Data.Username) != null)
                    throw ApiExceptions.FieldValidation(
                            nameof(RegisterNewUserRequestDto.Username),
                            "Username is already taken.",
                            Status400BadRequest
                        );

                if (await GetUser(x => x.Email == command.Data.Email) != null)
                    throw ApiExceptions.FieldValidation(
                            nameof(RegisterNewUserRequestDto.Email),
                            "Email is already taken.",
                            Status400BadRequest
                        );

                ApiUser registered_user = new ApiUser
                {
                    Id = command.Id,
                    UserName = command.Data.Username,
                    Email = command.Data.Email,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(
                    registered_user, 
                    command.Data.Password
                    );

                if (!result.Succeeded)
                    throw ApiExceptions.General(
                            "Internale server error.",
                            Status500InternalServerError
                        );

                _context.SaveChanges();

                await tx.CommitAsync();
            }
        }

        private async Task<ApiUser> GetUser(Expression<Func<ApiUser, bool>> predicate)
        {
            return await _context.Users.FirstOrDefaultAsync(predicate);
        }
    }
}
