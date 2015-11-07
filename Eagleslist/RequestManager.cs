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

        private static async Task<User> FetchUserByID(AuthResponse auth, HttpClient client)
        {
            string url = string.Format("https://sourcekitserviceterminated.com/apidb/users/id/{0}", auth.UserID);
            Dictionary<string, List<User>> root = await SendObjectAsJSON<Dictionary<string, List<User>>>(auth, url, client, client.PutAsync);

            User user = root["Users"][0];
            user.AddAuth(auth);
                
            return user;
        }

        public static async Task<User> AttemptLogin(LoginRequest request)
        {
            using (HttpClient client = new HttpClient(DefaultRequestHandler()))
            {
                string url = "https://sourcekitserviceterminated.com/apidb/users/auth";
                AuthResponse response = await SendObjectAsJSON<AuthResponse>(request, url, client, client.PutAsync);

                if (response.UserID <= 0)
                {
                    return null;
                }

                return await FetchUserByID(response, client);
            }
        }

        public async Task<AuthResponse> AttemptRegistration(RegistrationSubmission registration)
        {
            using (HttpClient client = new HttpClient(DefaultRequestHandler()))
            {
                string url = "https://sourcekitserviceterminated.com/apidb/users/new";
                return await SendObjectAsJSON<AuthResponse>(registration, url, client, client.PostAsync);
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

        private static WebRequestHandler DefaultRequestHandler()
        {
            WebRequestHandler handler = new WebRequestHandler();
            handler.ServerCertificateValidationCallback = (sender, certificate, chain, errors) =>
            {
                return true;
            };

            return handler;
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

        private static async Task<T> GetJSON<T>(string url)
        {
            using (WebRequestHandler handler = new WebRequestHandler())
            {
                handler.ServerCertificateValidationCallback = (sender, certificate, chain, errors) =>
                {
                    return true;
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        string responseString = await client.GetStringAsync(url);
                        return JsonConvert.DeserializeObject<T>(responseString);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return default(T);
                    }
                }
            }
        }

        private static async Task<T> SendObjectAsJSON<T>(object obj, string url, HttpClient client, Func<string, HttpContent, Task<HttpResponseMessage>> action)
        {
            try
            {
                string json = JsonConvert.SerializeObject(obj);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await action(url, content);
                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default(T);
            }
        }
    }
}
