namespace PluginDemocracy.API.UrlRegistry
{
    public static class FrontEndPages
    {
        //Generic pages
        public const string Home = "/";
        public const string GenericMessage = "/message";

        //User pages
        public const string Login = "/login";
        public const string SignUp = "/signup";
        public const string ConfirmEmail = "/confirmemail";
        public const string ForgotPassword = "/forgotpassword";
        public const string ResetPassword = "/resetpassword";
        public const string Notifications = "/notifications";

        //Community pages
        public const string Feed = "/communityfeed";
        public const string RegisterCommunity = "/registercommunity";
        public const string JoinCommunity = "/joincommunity";
        public const string JoinOrRegisterCommunity = "/joinorregistercommunity";

        //Admin pages
        public const string AppAdmin = "/appadmin";
    }
}
