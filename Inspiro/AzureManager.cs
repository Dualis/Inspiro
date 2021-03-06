﻿using Inspiro.DataModels;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inspiro
{
    public class AzureManager
    {

        private static AzureManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<Auth> authTable;
        private IMobileServiceTable<Accounts> accountsTable;

        private AzureManager()
        {
            this.client = new MobileServiceClient("http://inspirodbhost.azurewebsites.net");
            this.authTable = this.client.GetTable<Auth>();
            this.accountsTable = this.client.GetTable<Accounts>();
        }

        public async Task AddAuth(Auth auth)
        {
            await this.authTable.InsertAsync(auth);
        }

        public async Task<List<Auth>> GetAuths()
        {
            return await this.authTable.ToListAsync();
        }

        public async Task UpdateAuths(Auth auth)
        {
            await this.authTable.UpdateAsync(auth);
        }

        public async Task DeleteAuths(Auth auth)
        {
            await this.authTable.DeleteAsync(auth);
        }


        public async Task AddAccounts(Accounts acc)
        {
            await this.accountsTable.InsertAsync(acc);
        }

        public async Task<List<Accounts>> GetAccounts()
        {
            return await this.accountsTable.ToListAsync();
        }

        public async Task UpdateAccounts(Accounts accounts)
        {
            await this.accountsTable.UpdateAsync(accounts);
        }

        public async Task DeleteAccounts(Accounts accounts)
        {
            await this.accountsTable.DeleteAsync(accounts);
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