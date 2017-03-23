using AdminCrossPlatformClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AdminCrossPlatformClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResourceDetails : ContentPage
    {
        private const string queueName = "";

        private MainPageItem Parameter { get; set; }

        private MainPageViewModel ViewModel { get; set; }

        private ResourceDetails()
        {
            InitializeComponent();
        }

        public ResourceDetails(MainPageItem item,MainPageViewModel model):this()
        {
            Parameter = item;
            ViewModel = model;
        }

        protected override void OnAppearing()
        {
            this.BindingContext = Parameter;

            base.OnAppearing();
        }

        private async void Cancel_Button_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PopAsync();
        }

        private async void Save_Button_Clicked(object sender, EventArgs e)
        {
            if (!(await Parameter.CheckAddressAsync()))
            {
                await DisplayAlert("Address is not recognized", "Please, check the address again", "Close");
            }
            else
            {
                if (Parameter.Id == 0)
                {
                    await ViewModel.AddItemAsync(Parameter);
                }
                else
                {
                    await ViewModel.UpdateItemAsync(Parameter);
                }

                if (sendSwitch.IsToggled)
                {
                    await Parameter.NotifyUsersAsync();
                }

                await this.Navigation.PopAsync();
            }
        }
    }
}
