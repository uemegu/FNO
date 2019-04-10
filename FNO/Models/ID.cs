using System;
using System.Linq;
using Xamarin.Auth;

namespace FNO.Models
{
    public class ID
    {
        public string Id { get; set; }
        public void Save()
        {
            var account = AccountStore.Create().FindAccountsForService("chu2name").FirstOrDefault<Account>();
            if (account == null) account = new Account();
            account.Properties["Id"] = Id;
            AccountStore.Create().Save(account, "chu2name");
        }
        public static ID Load()
        {
            var account = AccountStore.Create().FindAccountsForService("chu2name").FirstOrDefault<Account>();
            if (account != null && account.Properties.Count > 0)
            {
                return new ID() { Id = account.Properties["Id"] };
            }
            return null;
        }
        public static void Delete()
        {
            var account = AccountStore.Create().FindAccountsForService("chu2name").FirstOrDefault<Account>();
            if (account != null && account.Properties.Count > 0)
            {
                AccountStore.Create().Delete(account, "chu2name");
            }
        }
    }
}
