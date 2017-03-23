using AdminCrossPlatformClient.ViewModels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AdminCrossPlatformClient
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            if (this.BindingContext == null)
            {
                await RefreshDataAsync();
            }
            else
            {
                RebindData();
            }
            base.OnAppearing();
        }

        private void RebindData()
        {
            this.BindingContext = null;
            this.BindingContext = MainPageViewModel.Instance;
            editButton.IsEnabled = false;
            deleteButton.IsEnabled = false;
        }

        private async Task RefreshDataAsync()
        {
            this.BindingContext = MainPageViewModel.Instance;
            await MainPageViewModel.Instance.RefreshDataAsync();
            editButton.IsEnabled = false;
            deleteButton.IsEnabled = false;
        }

        private void listView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem!=null)
            {
                editButton.IsEnabled = true;
                deleteButton.IsEnabled = true;
            }
            else
            {
                editButton.IsEnabled = false;
                deleteButton.IsEnabled = false;
            }
        }

        private async void addButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ResourceDetails(new MainPageItem(), MainPageViewModel.Instance));
        }

        private async void editButton_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            await Navigation.PushAsync(new ResourceDetails(listView.SelectedItem as MainPageItem, MainPageViewModel.Instance));
        }

        private async void deleteButton_Clicked(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            await MainPageViewModel.Instance.DeleteItemAsync(listView.SelectedItem as MainPageItem);
        }

        private async void refreshButton_Clicked(object sender, EventArgs e)
        {
            await RefreshDataAsync();
        }
    }
}
