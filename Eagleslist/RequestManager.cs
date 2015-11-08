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
        private const string RootURL = "https://sourcekitserviceterminated.com/";

        public async Task<List<User>> GetUsers()
        {
            string url = RootURL + "apidb/users";
            string responseString = await Request(url);
            return await UsersFromJSON(responseString);
        }

        public async Task<List<Listing>> GetListings()
        {
            //string url = "https://sourcekitserviceterminated.com/apidb/listings";
            string url = RootURL + "static/magic.json";
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
            string url = string.Format("{0}apidb/users/id/{1}", RootURL, auth.UserID);
            User user = await SendObjectAsJSON<User>(auth, url, client, client.PutAsync);

            if (user != null)
            {
                user.AddAuth(auth);
            }

            return user;
        }

        public static async Task<NewListingResponse> PostNewListing(Listing listing, string sessionID)
        {
            using (HttpClient client = new HttpClient(DefaultRequestHandler()))
            {
                string url = RootURL + "apidb/listings/new";
                ValidatedListing validated = new ValidatedListing(sessionID, listing);

                return await SendObjectAsJSON<NewListingResponse>(validated, url, client, client.PostAsync);
            }
        }

        public static async Task<User> AttemptLogin(LoginRequest request)
        {
            using (HttpClient client = new HttpClient(DefaultRequestHandler()))
            {
                string url = RootURL + "apidb/users/auth";
                AuthResponse response = await SendObjectAsJSON<AuthResponse>(request, url, client, client.PutAsync);

                if (response.UserID <= 0)
                {
                    return null;
                }

                return await FetchUserByID(response, client);
            }
        }

        public static async Task<User> AttemptRegistration(RegistrationSubmission registration)
        {
            using (HttpClient client = new HttpClient(DefaultRequestHandler()))
            {
                string url = RootURL + "apidb/users/new";
                AuthResponse response = await SendObjectAsJSON<AuthResponse>(registration, url, client, client.PostAsync);

                if (response.Error != null && response.Error.Length > 0)
                {
                    return new User(
                        0, null, null, 
                        registration.Username, registration.Email, null,
                        false, false, response.Error
                    );
                }
                else
                {
                    LoginRequest request = new LoginRequest(registration.Email, registration.Password);

                    return await AttemptLogin(request);
                }
            }
        }

        private static Task<List<User>> UsersFromJSON(String JSON)
        {
            return Task.Run(() =>
            {
                return JsonConvert.DeserializeObject<Dictionary<string, List<User>>>(JSON)["Users"];
            });
        }

        private static Task<List<Listing>> ListingsFromJSON(String JSON)
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
