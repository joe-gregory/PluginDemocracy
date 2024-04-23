namespace PluginDemocracy.API.UrlRegistry
{
    public static class ApiEndPoints
    {
        public const string Base = "/api/";
        #region Test
        public static readonly string TestController = Base + "test/";
        public static readonly string GetTestMessagesPage = TestController + "testmessagespage";
        #endregion
        #region User
        public const string UsersController = Base + "users/";
        public const string PostSignUp = UsersController + "signup";
        public const string ConfirmEmail = UsersController + "confirmemail";
        public const string PostLogin = UsersController + "login";
        public const string PostSendForgotPasswordEmail = UsersController + "sendforgotpasswordemail";
        public const string PostResetPassword = UsersController + "resetpassword";
        public const string PostToggleUserCulture = UsersController + "toggleuserculture";
        public const string PostUpdateAccount = UsersController + "updateaccount";
        public const string UpdateProfilePicture = UsersController + "updateprofilepicture";
        public const string MarkNotificationAsRead = UsersController + "marknotificationasread";
        public const string RefreshUserData = UsersController + "refreshuserdata";
        #endregion
        #region Community
        public const string CommunityController = Base + "community/";
        public const string RegisterCommunity = CommunityController + "registercommunity";
        public const string GetListOfAllCommunities = CommunityController + "getlistofallcommunities";
        public const string GetListOfHomesForCommunity = CommunityController + "getlistofhomesforcommunity";
        public const string JoinCommunityRequest = CommunityController + "joincommunityrequest";
        public const string CreateNewPost = CommunityController + "createnewpost";
        public const string GetFeed = CommunityController + "getfeed";
        #endregion
        #region Admin
        public const string AdminController = Base + "admin/";
        public const string AdminGetListOfAllSimpleCommunitiesDtos = AdminController + "getlistofallsimplecommunitiesdtos";
        public const string AdminIsCurrentUserAdmin = AdminController +"iscurrentuseradmin";
        public const string AdminGetPendingJoinCommunityRequests = AdminController + "getpendingjoincommunityrequests"; 
        public const string AdminRejectJoinRequest = AdminController + "rejectjoinrequest";
        public const string AdminAcceptJoinRequest = AdminController + "acceptjoinrequest";
        #endregion
    }
}