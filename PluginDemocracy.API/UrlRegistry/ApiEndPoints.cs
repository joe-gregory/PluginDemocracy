using System.Reflection.Metadata;

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
        public const string GetUserDTOFromEmail = UsersController + "getuserdtofromemail";
        public const string GetUserPetitionDrafts = UsersController + "getuserpetitiondrafts";
        public const string SavePetitionDraft = UsersController + "savepetitiondraft";
        public const string DeletePetitionDraft = UsersController + "deletepetitiondraft";
        public const string GetPetitionDraft = UsersController + "getpetitiondraft";
        public const string DeleteDocumentFromPetition = UsersController + "deletedocumentfrompetition";
        public const string AuthorReadyToPublishPetition = UsersController + "authorreadytopublishpetition";
        public const string GetAllJoinCommunityRequestsForUser = UsersController + "getalljoincommunityrequestsforuser";
        public const string GetUserAbout = UsersController + "getuserabout";
        public const string UnmarkPetitionReadyToPublish = UsersController + "unmarkpetitionreadytopublish";
        public const string ESign = UsersController + "esign";
        public const string SaveProposalDraft = UsersController + "saveproposaldraft";
        public const string GetProposalDraft = UsersController + "getproposaldraft";
        public const string GetUserProposalDrafts = UsersController + "getuserproposaldrafts";
        public const string DeleteProposalDraft = UsersController + "deleteproposaldraft";
        public const string PublishProposal = UsersController + "publishproposal";

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
        public const string GetCommunityAbout = CommunityController + "getcommunityabout";
        public const string GetPetition = CommunityController + "getpetition";
        public const string GeneratePDFOfPetition = CommunityController + "generatepdfofpetition";

        #region Feed
        public const string CreateNewPost = CommunityController + "createnewpost";
        public const string GetFeed = CommunityController + "getfeed";
        public const string DeletePost = CommunityController + "deletepost";
        public const string ReactToPost = CommunityController + "ReactToPost";
        public const string AddCommentToPost = CommunityController + "addcommenttopost";
        public const string DeleteComment = CommunityController + "deletecomment";
        #endregion
        #endregion
        #region Roles
        public const string RolesGetListOfJCRequestsForGivenCommunity = CommunityController + "getlistofjcrrequestsforgivencommunity";
        #endregion
        #region Admin
        public const string AdminController = Base + "admin/";
        public const string AdminGetListOfAllSimpleCommunitiesDTOsWithRoles = AdminController + "getlistofallsimplecommunitiesdtos";
        public const string AdminIsCurrentUserAdmin = AdminController +"iscurrentuseradmin";
        public const string AdminRejectJoinRequest = AdminController + "rejectjoinrequest";
        public const string AdminAcceptJoinRequest = AdminController + "acceptjoinrequest";
        
        public const string AdminCreateAndAssignRole = AdminController + "createandassignrole";
        public const string AdminDeleteAndUnassignRole = AdminController + "deleteandunassignrole";

        public const string AdminUpdateCommunityInfo = AdminController + "updatecommunityinfo";
        public const string AdminUpdateCommunityPicture = AdminController + "updatecommunitypicture";
        public const string AdminGetFullCommunityDTOObject = AdminController + "getfullcommunitydtoobject";
        public const string AdminRemoveHomeOwnership = AdminController + "removehomeownership";
        public const string AdminRemoveResidencyFromHome = AdminController + "removeresidencyfromhome";
        public const string AdminDeleteHome = AdminController + "deletehome";
        public const string AdminEditHome = AdminController + "edithome";
        public const string AdminAddHome = AdminController + "addhome";
        public const string AdminReportABug = AdminController + "reportabug";
        #endregion

    }
}