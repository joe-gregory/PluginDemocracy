namespace PluginDemocracy.API.UrlRegistry
{
    public static class ApiEndPoints
    {
        public static readonly string Base = "/api/";

        public static readonly string TestController = Base + "test/";
        public static readonly string GetTestMessagesPage = TestController + "testmessagespage";

        public static readonly string UsersController = Base + "users/";
        public static readonly string PostSignUp = UsersController + "signup";
        public static readonly string ConfirmEmail = UsersController + "confirmemail";
        public static readonly string PostLogin = UsersController + "login";
        public static readonly string PostSendForgotPasswordEmail = UsersController + "sendforgotpasswordemail";
        public static readonly string PostResetPassword = UsersController + "resetpassword";
        public static readonly string PostToggleUserCulture = UsersController + "toggleuserculture";
    }
}
