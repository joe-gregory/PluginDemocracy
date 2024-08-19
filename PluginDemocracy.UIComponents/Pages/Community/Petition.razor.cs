using Microsoft.AspNetCore.Components;
using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using PluginDemocracy.Translations;
using SignaturePad;
using System.Text;

namespace PluginDemocracy.UIComponents.Pages.Community
{
    public partial class Petition
    {
        [SupplyParameterFromQuery]
        public int? PetitionId { get; set; }
        protected PetitionDTO? PetitionDTO;
        private bool _showESignDialog;
        private bool disableAll;
        private bool ShowESignDialog
        {
            get
            {
                return _showESignDialog;
            }
            set
            {
                if (_showESignDialog != value) _showESignDialog = value;
                if (_showESignDialog == false)
                {
                    Signature = Array.Empty<byte>();
                    agreeToESignCheckBox = false;
                }
            }
        }
        private bool agreeToESignCheckBox;
        private byte[] Signature { get; set; } = Array.Empty<byte>();
        private readonly SignaturePadOptions _options = new()
        {
            BackgroundColor = "#fcf3cf",
        };
        private ESignatureDTO? ESignatureDTO = null;
        //Stats
        private double amountOfHomesThatHaveSigned;
        private double amountOfHomesNeededForMajority;
        private double amountOfHomesThatHaventSigned;
        private double amountOfSignaturesFromHomeOwners;
        private double amountOfSignaturesFromNonOwningResidents;
        private bool majorityHomeOwnerSigned;

        protected override async Task OnInitializedAsync()
        {
            await RefreshPetition();
        }
        private async Task RefreshPetition()
        {
            if (PetitionId != null) PetitionDTO = await Services.GetDataGenericAsync<PetitionDTO>($"{ApiEndPoints.GetPetition}?petitionId={PetitionId}");
            if (PetitionDTO == null) Services.AddSnackBarMessage("warning", "No petition data received.");
            CalculateStats();
        }
        private void CalculateStats()
        {
            amountOfHomesThatHaveSigned = 0;
            amountOfHomesNeededForMajority = 0;
            amountOfHomesThatHaventSigned = 0;
            amountOfSignaturesFromHomeOwners = 0;
            amountOfSignaturesFromNonOwningResidents = 0;
            //amountOfHomesThatHaveSigned
            double homesThatSigned = 0;
            if (PetitionDTO?.CommunityDTO != null)
            {
                foreach (HomeDTO homeDTO in PetitionDTO?.CommunityDTO?.Homes ?? [])
                {
                    foreach (KeyValuePair<UserDTO, double> ownersOwnerships in homeDTO.OwnersOwnerships)
                    {
                        if (PetitionDTO?.Signatures.Any(s => s?.Signer?.Id == ownersOwnerships.Key.Id) ?? false)
                        {
                            homesThatSigned += ownersOwnerships.Value;
                        }
                    }
                }
            }
            amountOfHomesThatHaveSigned = homesThatSigned/100;

            //amountOfHomesNeededForMajority
            int totalHomes = PetitionDTO?.CommunityDTO?.Homes.Count ?? 0;
            amountOfHomesNeededForMajority = totalHomes / 2 + 1;

            //amountOfHomesThatHaventSigned
            double totalHomesPercentagesTotals = PetitionDTO?.CommunityDTO?.Homes.Count ?? 0;
            amountOfHomesThatHaventSigned = totalHomesPercentagesTotals - amountOfHomesThatHaveSigned;

            //amountOfSignaturesFromHomeOwners &
            //amountOfSignaturesFromNonOwningResidents
            double signaFromHomeOwners = 0;
            double signaFromResidents = 0;
            foreach (ESignatureDTO eSignatureDTO in PetitionDTO?.Signatures ?? [])
            {
                if (PetitionDTO?.CommunityDTO?.Homes.Any(h => h.OwnersOwnerships.Any(o => o.Key.Id == eSignatureDTO.Signer?.Id)) ?? false)
                {
                    signaFromHomeOwners++;
                }
                else if (PetitionDTO?.CommunityDTO?.Homes.Any(h => h.Residents.Any(r => r.Id == eSignatureDTO.Signer?.Id)) ?? false)
                {
                    signaFromResidents++;
                }
            }
            amountOfSignaturesFromHomeOwners = signaFromHomeOwners;
            amountOfSignaturesFromNonOwningResidents = signaFromResidents;

            //majorityHomeOwnerSigned
            majorityHomeOwnerSigned = amountOfHomesThatHaveSigned >= amountOfHomesNeededForMajority;
        }
        protected void StartSignPetitionProcess()
        {
            ShowESignDialog = true;
        }
        private async void ESign()
        {
            disableAll = true;
            //Create E-Signature DTO:
            ESignatureDTO = new ESignatureDTO
            {
                SignatureImage = Encoding.UTF8.GetString(Signature),
                SignatureImageBase64 = Convert.ToBase64String(Signature),
                Intent = AppState.Translate(ResourceKeys.ESignModalWindowCheckBox)
            };
            string url = $"{ApiEndPoints.ESign}?petitionId={PetitionId}";
            await Services.PostDataAsync<ESignatureDTO>(url, ESignatureDTO);
            await RefreshPetition();
            disableAll = false;
            ShowESignDialog = false;
            StateHasChanged();
        }
        private void GetPDF()
        {
            string url = $"{ApiEndPoints.GeneratePDFOfPetition}?petitionId={PetitionId}";
            Navigation.NavigateTo($"{AppState.ApiBaseUrl}{url}", forceLoad: true);
        }
    }
}
