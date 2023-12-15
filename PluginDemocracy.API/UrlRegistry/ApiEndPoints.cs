namespace PluginDemocracy.API.UrlRegistry
{
    public static class ApiEndPoints
    {
        public static readonly string Base = "/api/";

        public static readonly string TestController = Base + "test/";
        public static readonly string GetTestMessagesPage = TestController + "testmessagespage";

        public static readonly string UsersController = Base + "users/";
        public static readonly string PostSignUp = UsersController + "signup";
    }
}
