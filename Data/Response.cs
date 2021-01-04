using AutoWrapper.Wrappers;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace RSSReader.Data
{
    public static class Response
    {
        public static string MsgErrUsernameTaken = "Username is already taken.";
        public static string MsgErrEmailTaken = "Email is already taken.";
        public static string MsgErrRequestFailed = "Request failed, internal server error.";
        public static string MsgErrWrongCredentials = "Wrong credentials.";

        public static string MsgCreatedRecord = "New record has been created in the database.";
        public static string MsgSucceed = "Succeed.";

        public static ApiResponse ErrRequestFailed = new ApiResponse(MsgErrRequestFailed, null, Status400BadRequest);
        public static ApiResponse ErrWrongCredentials = new ApiResponse(MsgErrWrongCredentials, null, Status401Unauthorized);
    }
}
