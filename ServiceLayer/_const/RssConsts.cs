using System;
using System.Threading.Tasks;

namespace LogicLayer._const
{
    public static class RssConsts
    {
        public const int POSTS_PER_CALL = 10;

        public static double AUTH_TOKEN_EXPIRES_TIME_S = new TimeSpan(0, 6, 0, 0).TotalSeconds;
        public static double REFRESH_TOKEN_EXPIRES_TIME_S = AUTH_TOKEN_EXPIRES_TIME_S + new TimeSpan(30, 0, 0, 0).TotalSeconds;

        public static double UPDATE_BLOG_DELAY_S = new TimeSpan(0, 0, 5, 0).TotalSeconds;
    }
}
