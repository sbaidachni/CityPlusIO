using CrossPlatformBotClient.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CrossPlatformBotClient.UserControls
{
    public partial class ConfirmationMessage : ContentView
    {
        public delegate void ButtonClickedHandler(string message);

        public event ButtonClickedHandler ButtonClicked;

        private List<HeroButton> buttons;
        public List<HeroButton> Buttons
        {
            set
            {
                buttons = value;

                foreach (var b in buttons)
                {
                    var but = new Button() { Text = b.title, CommandParameter = b.value };
                    but.Clicked += But_Clicked;
                    buttonStack.Children.Add(but);
                }
            }
        }

        private void But_Clicked(object sender, EventArgs e)
        {
            if (ButtonClicked != null)
            {
                foreach (var b in buttonStack.Children)
                {
                    if (b is Button)
                    {
                        (b as Button).IsEnabled = false;
                    }
                }
                ButtonClicked(((Button)sender).CommandParameter.ToString());
            }
        }

        private string textMessage;
        public string TextMessage
        {
            set
            {
                textMessage = value;
                textBlock.Text = textMessage;
            }
        }

        public ConfirmationMessage()
        {
            InitializeComponent();
        }
    }
}
