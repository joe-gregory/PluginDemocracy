using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using MudBlazor;
using PluginDemocracy.DTOs;
using PluginDemocracy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static MudBlazor.CategoryTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PluginDemocracy.UIComponents.Components
{
    /// <summary>
    /// This is a MudDataGrid that displays the homes of a community and allows editing. Used in RegisterCommunity.razor
    /// The <MudDataGrid> component is designed to efficiently display lists of data in a tabular format. Think of it as a smart HTML table.
    /// The key property here is Items. You feed it a collection (e.g., an IEnumerable<HomeDto>) which the DataGrid will then use to generate rows.
    ///Columns and Templates: Columns in a DataGrid define the structure of each row.PropertyColumns automatically bind to properties of your data 
    ///items, while TemplateColumns let you customize the content within cells.
    ///The TemplateColumn gives you complete control over what each cell within a row should look like. Within a TemplateColumn, you have a <CellTemplate>.
    ///Think of the <CellTemplate> as a mini-template for constructing each individual cell.
    ///When the DataGrid renders, it iterates through your Items collection. For each item in your list, it creates a row.
    ///Cell Generation: Within each row, it then uses your<CellTemplate> to generate the cells.
    ///Contextual Magic: Here's the key part: During each cell generation, MudBlazor provides a special variable named context. 
    ///This context variable holds a reference to the current item from the Items collection that's being used to create that cell.
    ///Since your <CellTemplate> is used inside a DataGrid with its Items property bound to a collection of HomeDto, the context variable in your template will have the following:
    ///Type: HomeDto
    ///Item: The specific HomeDto object related to the row containing the cell being generated.
    ///This means context.Item gives you direct access to the individual HomeDto object of that cell's row!
    ///You can bind UI elements and display specific properties of the HomeDto using context.Item.PropertyName.
    ///As demonstrated in our delete button example, you can use @(() => HandleDelete(context.Item)) within an OnClick handler to have the button 
    ///'know' exactly which HomeDto it needs to operate on.
    /// </summary>
    public partial class CommunityDtoHomesDataGrid
    {
        [Parameter]
        public CommunityDTO? Community { get; set; }
        public void DeleteHome(HomeDTO home)
        {
            Community?.Homes.Remove(home);
            StateHasChanged();
        }
    }
}
