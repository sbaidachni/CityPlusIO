using AdminCrossPlatformClient.Models;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminCrossPlatformClient.ViewModels
{
    public class MainPageViewModel
    {
        static MainPageViewModel  _instance = null;

        MobileServiceClient client;

        List<Resources> tableData;

        public MobileServiceClient CurrentClient
        {
            get
            {
                return client;
            }
        }

        public ObservableCollection<MainPageItem> Items { get; private set; }

        public async Task RefreshDataAsync()
        {
            var table = client.GetTable<Resources>();

            tableData = await (from a in table select a).ToListAsync();

            Items.Clear();

            foreach(var item in tableData)
            {
                MainPageItem newItem = new MainPageItem()
                {
                    Id = item.Id,
                    Name=item.Name,
                    Address=item.Address,
                    Lat=item.Lat,
                    Lon=item.Lon,
                    Food=Convert.ToBoolean(item.Food),
                    Medicine=Convert.ToBoolean(item.Medicine),
                    Clothes=Convert.ToBoolean(item.Clothes),
                    Shelter=Convert.ToBoolean(item.Shelter)
                };
                Items.Add(newItem);
            }
        }

        public async Task LoadMoreDataAsync()
        {
            var table = client.GetTable<Resources>();

            tableData = await (from a in table select a).Skip(Items.Count).ToListAsync();

            foreach (var item in tableData)
            {
                MainPageItem newItem = new MainPageItem()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Address = item.Address,
                    Lat = item.Lat,
                    Lon = item.Lon,
                    Food = Convert.ToBoolean(item.Food),
                    Medicine = Convert.ToBoolean(item.Medicine),
                    Clothes = Convert.ToBoolean(item.Clothes),
                    Shelter = Convert.ToBoolean(item.Shelter)
                };
                Items.Add(newItem);
            }
        }

        public async Task DeleteItemAsync(MainPageItem item)
        {
            var table = client.GetTable<Resources>();
            var resourceDelete = (await (from a in table where a.Id == item.Id select a).ToListAsync()).FirstOrDefault();
            
            await table.DeleteAsync(resourceDelete);
            Items.Remove(item);
        }

        public async Task AddItemAsync(MainPageItem newItem)
        {

            var table = client.GetTable<Resources>();

            var resources = new Resources()
            {
                Name = newItem.Name,
                Address = newItem.Address,
                Lon = newItem.Lon,
                Lat = newItem.Lat,
                Shelter = Convert.ToInt32(newItem.Shelter),
                Food = Convert.ToInt32(newItem.Food),
                Clothes = Convert.ToInt32(newItem.Clothes),
                Medicine = Convert.ToInt32(newItem.Medicine)
            };

            await table.InsertAsync(resources);
            newItem.Id = resources.Id;
            Items.Add(newItem);
        }

        public async Task UpdateItemAsync(MainPageItem updateItem)
        {
            var table = client.GetTable<Resources>();
            var resourceUpdate = (await (from a in table where a.Id == updateItem.Id select a).ToListAsync()).FirstOrDefault();
            
           
            resourceUpdate.Address = updateItem.Address;
            resourceUpdate.Name = updateItem.Name;
            resourceUpdate.Lat = updateItem.Lat;
            resourceUpdate.Lon = updateItem.Lon;
            resourceUpdate.Food = Convert.ToInt32(updateItem.Food);
            resourceUpdate.Shelter = Convert.ToInt32(updateItem.Shelter);
            resourceUpdate.Medicine = Convert.ToInt32(updateItem.Medicine);
            resourceUpdate.Clothes = Convert.ToInt32(updateItem.Clothes);

            await table.UpdateAsync(resourceUpdate);
        }

        public static MainPageViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainPageViewModel();
                return _instance;
            }
        }

        private MainPageViewModel()
        {
            Items = new ObservableCollection<MainPageItem>();
            client = new MobileServiceClient("https://adminlayercity.azurewebsites.net");
        }
    }
}
