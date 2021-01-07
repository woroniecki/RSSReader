using AutoWrapper.Wrappers;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace RSSReader.Data
{
    public static class Response
    {
        public static string MsgErrUsernameTaken = "Username is already taken.";
        public static string MsgErrEmailTaken = "Email is already taken.";
        public static string MsgErrRequestFailed = "Request failed, internal server error.";
        public static string MsgErrBadRequest = "Bad request.";
        public static string MsgErrWrongCredentials = "Wrong credentials.";
        public static string MsgErrUnauthorized = "Unauthorized user.";
        public static string MsgErrSubAlreadyDisabled = "Subscription is already disabled.";
        public static string MsgErrSubAlreadyEnabled = "Subscription is already enabled.";
        public static string MsgErrEntityNotExists = "Entity doesn't exists.";

        public static string MsgCreatedRecord = "New record has been created in the database.";
        public static string MsgSucceed = "Succeed.";
        public static string MsgCreated = "Created new entity.";

        public static ApiResponse ErrRequestFailed = new ApiResponse(MsgErrRequestFailed, null, Status400BadRequest);
        public static ApiResponse ErrBadRequest = new ApiResponse(MsgErrBadRequest, null, Status400BadRequest);
        public static ApiResponse ErrWrongCredentials = new ApiResponse(MsgErrWrongCredentials, null, Status401Unauthorized);
        public static ApiResponse ErrUnauhtorized = new ApiResponse(MsgErrUnauthorized, null, Status401Unauthorized);
        public static ApiResponse ErrSubAlreadyDisabled = new ApiResponse(MsgErrSubAlreadyDisabled, null, Status400BadRequest);
        public static ApiResponse ErrSubAlreadyEnabled = new ApiResponse(MsgErrSubAlreadyEnabled, null, Status400BadRequest);
        public static ApiResponse ErrEntityNotExists = new ApiResponse(MsgErrEntityNotExists, null, Status400BadRequest);
    }
}
