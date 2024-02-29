namespace PluginDemocracy.API.UrlRegistry
{
    public static class ApiEndPoints
    {
        public static readonly string Base = "/api/";
        #region Test
        public static readonly string TestController = Base + "test/";
        public static readonly string GetTestMessagesPage = TestController + "testmessagespage";
        #endregion
        #region User
        public static readonly string UsersController = Base + "users/";
        public static readonly string PostSignUp = UsersController + "signup";
        public static readonly string ConfirmEmail = UsersController + "confirmemail";
        public static readonly string PostLogin = UsersController + "login";
        public static readonly string PostSendForgotPasswordEmail = UsersController + "sendforgotpasswordemail";
        public static readonly string PostResetPassword = UsersController + "resetpassword";
        public static readonly string PostToggleUserCulture = UsersController + "toggleuserculture";
        public static readonly string PostUpdateAccount = UsersController + "updateaccount";
        public static readonly string UpdateProfilePicture = UsersController + "updateprofilepicture";
        #endregion
        #region Community
        public static readonly string CommunityController = Base + "community/";
        public static readonly string RegisterCommunity = CommunityController + "registercommunity";
        #endregion
    }
}