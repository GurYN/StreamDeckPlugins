using System;
using System.Net.Http;
using System.Threading.Tasks;
using Crypto.Actions.Watcher.Models;
using Newtonsoft.Json;

namespace Crypto.Actions.Watcher.Services
{
    public class ApiService
    {
        public static async Task<CoinModel> GetCoinData(string name)
        {
            var url = string.Format("https://api.coingecko.com/api/v3/coins/{0}?localization=false&tickers=false&community_data=false&developer_data=false&sparkline=false", name);
            using (var httpClient = new HttpClient())
            {
                var result = await httpClient.GetAsync(url);
                if (result.IsSuccessStatusCode)
                {
                    var data = await result.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<CoinModel>(data);
                }
            }

            return null;
        }
    }
}
