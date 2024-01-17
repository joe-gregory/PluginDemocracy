﻿using PluginDemocracy.API.UrlRegistry;
using PluginDemocracy.DTOs;
using System.Drawing;

namespace PluginDemocracy.UIComponents.Pages
{
    public partial class Settings
    {
        private bool _checked;
        private MudBlazor.Color thumbIconColor;
        private bool disabled = false;
        private string selectedFlag = string.Empty;
        private const string usaFlag = "https://upload.wikimedia.org/wikipedia/en/thumb/a/a4/Flag_of_the_United_States.svg/2880px-Flag_of_the_United_States.svg.png";
        private const string mxnFlag = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Flag_of_Mexico.svg/2880px-Flag_of_Mexico.svg.png";
        /// <summary>
        /// Checked true equals es-MX, not checked false = USA
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (AppState.Culture.Name == "es-MX") _checked = true;
            else if (AppState.Culture.Name == "en-US") _checked = false;
            else disabled = true;
            SetLook();
            if(AppState.IsLoggedIn)
            {
                #pragma warning disable CS8602 // Dereference of a possibly null reference. AppState.IsLoggedIn checks if AppState.User is null
                userDto.FirstName = AppState.User.FirstName;
                #pragma warning restore CS8602 // Dereference of a possibly null reference.
                userDto.MiddleName = AppState.User.MiddleName;
                userDto.LastName = AppState.User.LastName;
                userDto.SecondLastName = AppState.User.SecondLastName;
                userDto.Email = AppState.User.Email;
                userDto.Password = AppState.User.Password;
                userDto.PhoneNumber = AppState.User.PhoneNumber;
                userDto.Address = AppState.User.Address;
                userDto.DateOfBirth = AppState.User.DateOfBirth;
                userDto.Culture = AppState.User.Culture;
                userDto.Admin = AppState.User.Admin;
            };
            
        }
        private void SetLook()
        {
            //true == Mexican
            if (_checked)
            {
                thumbIconColor = MudBlazor.Color.Success;
                selectedFlag = mxnFlag;
            }
            else
            {
                thumbIconColor = MudBlazor.Color.Info;
                selectedFlag = usaFlag;
            }
        }
        private async Task OnSwitchToggled()
        {

            if (AppState.IsLoggedIn)
            {
                try
                {
                    #pragma warning disable CS8604 // Possible null reference argument warning disabled because AppState.IsLoggedIn checks that AppState.User != null.
                    await Services.PostDataAsync<UserDto>(ApiEndPoints.PostToggleUserCulture, AppState.User);
                    #pragma warning restore CS8602 // Dereference of a possibly null reference.
                    _checked = !_checked;
                }
                catch(Exception ex)
                {
                    Services.AddSnackBarMessage("error", ex.Message);
                }
            }
            else
            {
                _checked = !_checked;
                if (_checked) AppState.SetCulture(new System.Globalization.CultureInfo("es-MX"));
                else AppState.SetCulture(new System.Globalization.CultureInfo("en-US"));
            }
            SetLook();
        }
        private async void PostForm()
        {
            disable = true;
            Services.PostDataAsync<UserDto>(cualEsElEndPoint, userDto);
        }
    }
}
