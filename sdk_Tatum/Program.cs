
using System.Text;
using Newtonsoft.Json.Linq;

public class Program
{
    private interface IWallet
    {
        public string Xpub { get; set; }
        public string Mnemonic { get; set; }
    }

    public class Wallet : IWallet
    {
        public string Xpub { get; set; }
        public string Mnemonic { get; set; }
    }

    private interface IAddress
    {
       public string  address { get; set; }
    }

    public class Address : IAddress
    {
        public string address { get; set; }
    }

    private interface IBalance
    {
        public string balance { get; set; }
    }

    public class Balance :IBalance
    {
        public string balance { get; set; }
    }

    public static async Task Main(string[] args)
    {
        var walletInfoJson = await CreateWallet();
        var privaryKey = await CreatePrivaryKey(walletInfoJson.Mnemonic);
        var address = await CreateAddress(walletInfoJson.Xpub,0);
        var balance = await GetBalance(address.address);

        Console.WriteLine(privaryKey);
        Console.WriteLine("Address : " + address.address);
        Console.WriteLine("balance : " + balance.balance);
        Console.ReadLine();
    }

    public static async Task<Wallet> CreateWallet()
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://api.tatum.io/v3/ethereum/wallet");
            var responseBody = await response.Content.ReadAsStringAsync();
            var wallet = JObject.Parse(responseBody).ToObject<Wallet>();
            return wallet;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }


    public static async Task<object> CreatePrivaryKey(string mnemonic)
    {
        try
        {
            using var httpClient = new HttpClient();
            JObject json = JObject.Parse(@"{
                index: 0,
                mnemonic: """ + mnemonic + @"""
              }");


            var postData = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://api.tatum.io/v3/ethereum/wallet/priv", postData);
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }

    public static async Task<Address> CreateAddress(string xpub,int index)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://api.tatum.io/v3/ethereum/address/" + xpub + "/" + index);
            var responseBody = await response.Content.ReadAsStringAsync();
            var address = JObject.Parse(responseBody).ToObject<Address>();
            return address;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }


    public static async Task<Balance> GetBalance(string Address)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://api.tatum.io/v3/ethereum/account/balance/" + Address);
            var responseBody = await response.Content.ReadAsStringAsync();
            var balance = JObject.Parse(responseBody).ToObject<Balance>();
            return balance;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null;
        }
    }


}
