﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace CrossPlatformBotClient.UserControls
{
    public partial class BotTextMessage : ContentView
    {
        private string textMessage;
        public string TextMessage
        {
            set
            {
                textMessage = value;
                textBlock.Text = textMessage;
            }
        }
        public BotTextMessage()
        {
            InitializeComponent();
        }
    }
}