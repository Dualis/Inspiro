﻿using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inspiro.Controllers
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;

        private AzureManager()
        {
            this.client = new MobileServiceClient("MOBILE_APP_URL");
        }

        public MobileServiceClient AzureClient
        {
            get { return client; }
        }

        public static AzureManager AzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AzureManager();
                }

                return instance;
            }
        }
    }
}