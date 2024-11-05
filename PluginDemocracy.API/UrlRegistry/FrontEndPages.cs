using PluginDemocracy.API.Controllers;

namespace PluginDemocracy.API.UrlRegistry
{
    public static class FrontEndPages
    {
        //Generic pages
        public const string Home = "/";
        public const string GenericMessage = "/message";
        public const string ReportABug = "/reportabug";

        //User pages
        public const string Login = "/login";
        public const string SignUp = "/signup";
        public const string ConfirmEmail = "/confirmemail";
        public const string ForgotPassword = "/forgotpassword";
        public const string ResetPassword = "/resetpassword";
        public const string Notifications = "/notifications";
        public const string PetitionDrafts = "/petitiondrafts";
        public const string ProposalDrafts = "/proposaldrafts";
        public const string CreateProposal = "/createproposal";

        //Community pages
        public const string Feed = "/feed";
        public const string RegisterCommunity = "/registercommunity";
        public const string JoinCommunity = "/joincommunity";
        public const string JoinOrRegisterCommunity = "/joinorregistercommunity";
        public const string ProposalCreate = "/proposal/create";
        public const string CreatePetition = "/createpetition";
        public const string JoinCommunityRequests = "/joincommunityrequests";
        public const string CurrentJoinCommunityRequests = "/currentjoincommunityrequests";
        public const string AboutCommunity = "/aboutcommunity";
        public const string Petition = "/petition";
        /// <summary>
        /// This takes you to the page where individuals with Roles can see pending join community requests. 
        /// </summary>
        public const string RolesPendingJCRequests = "/rolespendingjcrequests";

        //Admin pages
        public const string AppAdmin = "/appadmin";

        //About pages
        public const string AboutUser = "/aboutuser";
    }
}
