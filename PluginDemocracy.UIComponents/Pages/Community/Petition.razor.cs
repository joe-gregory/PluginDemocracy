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

        protected override async Task OnInitializedAsync()
        {
            await RefreshPetition();
        }
        private async Task RefreshPetition()
        {
            if (PetitionId != null) PetitionDTO = await Services.GetDataGenericAsync<PetitionDTO>($"{ApiEndPoints.GetPetition}?petitionId={PetitionId}");
            if (PetitionDTO == null) Services.AddSnackBarMessage("warning", "No petition data received.");
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
                Intent = AppState.Translate(ResourceKeys.ESignModalWindowCheckBox)
            };
            string url = $"{ApiEndPoints.ESign}?petitionId={PetitionId}";
            await Services.PostDataAsync<ESignatureDTO>(url, ESignatureDTO);
            await RefreshPetition();
            disableAll = false;
            ShowESignDialog = false;
            StateHasChanged();
        }
    }
}
