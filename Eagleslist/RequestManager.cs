using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace Eagleslist
{
    public class RequestManager
    {
        public bool RequestIsActive { get; private set; }

        public async Task<List<User>> GetUsers()
        {
            string url = "https://sourcekitserviceterminated.com/apidb/users";
            string responseString = await Request(url);
            return await UsersFromJSON(responseString);
        }

        public async Task<List<Listing>> GetListings()
        {
            string url = "https://sourcekitserviceterminated.com/apidb/listings";
            string responseString = await Request(url);
            return await ListingsFromJSON(responseString);
        }

        private async Task<string> Request(string url)
        {
            using (WebRequestHandler handler = new WebRequestHandler())
            {
                handler.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        return await client.GetStringAsync(url);
                    }
                    catch (HttpRequestException exception)
                    {
                        Console.WriteLine(exception.Message);
                        return null;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                        return null;
                    }
                }
            }
        }

        private Task<List<User>> UsersFromJSON(String JSON)
        {
            return Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<Dictionary<string, List<User>>>(JSON)["Users"];
            });
        }

        private Task<List<Listing>> ListingsFromJSON(String JSON)
        {
            return Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<Dictionary<string, List<Listing>>>(JSON)["Listings"];
            });
        }
    }
}
