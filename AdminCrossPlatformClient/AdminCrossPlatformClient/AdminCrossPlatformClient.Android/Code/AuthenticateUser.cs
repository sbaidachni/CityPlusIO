using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AdminCrossPlatformClient.Models;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using AdminCrossPlatformClient.ViewModels;

namespace AdminCrossPlatformClient.Droid.Code
{
    public class AuthenticateUser:IAuthenticate
    {
        // Define a authenticated user.
        private MobileServiceUser user;

        private Context _context;

        public AuthenticateUser(Context context)
        {
            _context = context;
        }

        public async Task<bool> Authenticate()
        {
            var success = false;
            var message = string.Empty;
            try
            {
                user = await MainPageViewModel.Instance.CurrentClient.LoginAsync(_context,
                    MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
                if (user != null)
                {
                    message = string.Format("you are now signed-in as {0}.",
                        user.UserId);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            // Display the success or failure message.
            AlertDialog.Builder builder = new AlertDialog.Builder(_context);
            builder.SetMessage(message);
            builder.SetTitle("Sign-in result");
            builder.Create().Show();

            return success;
        }
    }
}