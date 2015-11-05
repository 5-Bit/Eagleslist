using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows.Media.Imaging;
using System.Net;
using System.IO;
using System.Text;

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
            //string url = "https://sourcekitserviceterminated.com/apidb/listings";
            string url = "https://sourcekitserviceterminated.com/static/magic.json";
            string responseString = await Request(url);
            return await ListingsFromJSON(responseString);
        }

        public static async Task<BitmapImage> GetBitmapFromURI(Uri uri)
        {
            byte[] bytes = await new WebClient().DownloadDataTaskAsync(uri);

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnDemand;
            image.StreamSource = new MemoryStream(bytes);
            image.EndInit();

            return image;
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

        public async Task<HttpResponseMessage> AttemptRegistration(RegistrationSubmission registration)
        {
            using (WebRequestHandler handler = new WebRequestHandler())
            {
                handler.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        string json = JsonConvert.SerializeObject(registration);
                        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                        return await client.PostAsync("https://sourcekitserviceterminated.com/apidb/users/new", content);
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
