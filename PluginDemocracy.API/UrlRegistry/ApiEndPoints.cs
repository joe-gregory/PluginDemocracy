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
        public const string AboutUser = UsersController + "aboutuser";
        public const string GetUserDTOFromEmail = UsersController + "getuserdtofromemail";
        public const string GetUserPetitionDrafts = UsersController + "getuserpetitiondrafts";
        public const string SavePetitionDraft = UsersController + "savepetitiondraft";
        public const string DeletePetitionDraft = UsersController + "deletepetitiondraft";
        public const string GetPetitionDraft = UsersController + "getpetitiondraft";
        public const string DeleteDocumentFromPetition = UsersController + "deletedocumentfrompetition";
        public const string AuthorReadyToPublishPetition = UsersController + "authorreadytopublishpetition";
        public const string GetAllJoinCommunityRequestsForUser = UsersController + "getalljoincommunityrequestsforuser";
        
         
        #endregion
        #region Community
        public const string CommunityController = Base + "community/";
        public const string RegisterCommunity = CommunityController + "registercommunity";
        public const string GetListOfAllCommunities = CommunityController + "getlistofallcommunities";
        public const string GetListOfHomesForCommunity = CommunityController + "getlistofhomesforcommunity";
        public const string JoinCommunityRequest = CommunityController + "joincommunityrequest";
        public const string GetJoinCommunityRequest = CommunityController + "getjoincommunityrequest";
        public const string ProposalCreate = CommunityController + "/proposal/create";
        public const string GetHomeForJoinCommunityRequestInfo = CommunityController + "gethomeforjoincommunityrequestinfo";
        public const string AddAdditionalSupportingDocumentsToJoinCommunityRequest = CommunityController + "addadditionalsupportingdocumentstojoincommunityrequest";
        public const string AddMessageToJoinCommunityRequest = CommunityController + "addmessagetojoincommunityrequest";
        public const string GetListOfAvatarUsersForACommunity = CommunityController + "getlistofavatarusersforacommunity";
        public const string AcceptOrRejectJoinCommunityRequest = CommunityController + "acceptorrejectjoincommunityrequest";

        #region Feed
        public const string CreateNewPost = CommunityController + "createnewpost";
        public const string GetFeed = CommunityController + "getfeed";
        public const string DeletePost = CommunityController + "deletepost";
        public const string ReactToPost = CommunityController + "ReactToPost";
        public const string AddCommentToPost = CommunityController + "addcommenttopost";
        public const string DeleteComment = CommunityController + "deletecomment";
        #endregion
        #endregion
        #region Admin
        public const string AdminController = Base + "admin/";
        public const string AdminGetListOfAllSimpleCommunitiesDTOsWithRoles = AdminController + "getlistofallsimplecommunitiesdtos";
        public const string AdminIsCurrentUserAdmin = AdminController +"iscurrentuseradmin";
        public const string AdminGetPendingJoinCommunityRequestsForACommunity = AdminController + "getpendingjoincommunityrequests"; 
        public const string AdminRejectJoinRequest = AdminController + "rejectjoinrequest";
        public const string AdminAcceptJoinRequest = AdminController + "acceptjoinrequest";
        
        public const string AdminCreateAndAssignRole = AdminController + "createandassignrole";
        public const string AdminDeleteAndUnassignRole = AdminController + "deleteandunassignrole";
        #endregion

    }
}