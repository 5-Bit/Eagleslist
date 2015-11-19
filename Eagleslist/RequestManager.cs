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
        private const string RootUrl = "https://5bitstudios.com/";

        public async Task<List<User>> GetUsers()
        {
            const string url = RootUrl + "apidb/users";
            var responseString = await Request(url);
            return await UsersFromJson(responseString);
        }

        public async Task<List<Listing>> GetListings()
        {
            const string url = RootUrl + "apidb/listings";
            var responseString = await Request(url);
            return await ListingsFromJson(responseString);
        }

        public static async Task<BitmapImage> GetBitmapFromUri(Uri uri)
        {
            var bytes = await new WebClient().DownloadDataTaskAsync(uri);

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnDemand;
            image.StreamSource = new MemoryStream(bytes);
            image.EndInit();

            return image;
        }

        private static async Task<User> FetchUserById(AuthResponse auth, HttpClient client)
        {
            string url = $"{RootUrl}apidb/users/id/{auth.UserID}";
            var user = await SendObjectAsJson<User>(auth, url, client.PutAsync);

            user?.AddAuth(auth);

            return user;
        }

        public static async Task<NewListingResponse> PostNewListing(Listing listing, string sessionId)
        {
            using (var client = new HttpClient(DefaultRequestHandler()))
            {
                const string url = RootUrl + "apidb/listings/new";
                var validated = new ValidatedListing(sessionId, listing);

                return await SendObjectAsJson<NewListingResponse>(validated, url, client.PostAsync);
            }
        }

        public static async Task<User> AttemptLogin(LoginRequest request)
        {
            using (var client = new HttpClient(DefaultRequestHandler()))
            {
                const string url = RootUrl + "apidb/users/auth";
                var response = await SendObjectAsJson<AuthResponse>(request, url, client.PutAsync);

                if (response.UserID <= 0)
                {
                    return null;
                }

                return await FetchUserById(response, client);
            }
        }

        public static async void AttemptLogout(string sessionId)
        {
            using (var client = new HttpClient(DefaultRequestHandler()))
            {
                var payload = new Dictionary<string, string> {
                    { "SessionID", sessionId }
                };

                const string url = RootUrl + "apidb/users/logout";
                var json = JsonConvert.SerializeObject(payload);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await client.PutAsync(url, content);
            }
        }

        public static async Task<User> AttemptRegistration(RegistrationSubmission registration)
        {
            using (var client = new HttpClient(DefaultRequestHandler()))
            {
                const string url = RootUrl + "apidb/users/new";
                var response = await SendObjectAsJson<AuthResponse>(registration, url, client.PostAsync);

                if (!string.IsNullOrEmpty(response.Error))
                {
                    return new User(
                        0, null, null, 
                        registration.Username, registration.Email, null,
                        false, false, response.Error
                    );
                }
                else
                {
                    var request = new LoginRequest(registration.Email, registration.Password);

                    return await AttemptLogin(request);
                }
            }
        }

        private static Task<List<User>> UsersFromJson(string json)
        {
            return Task.Run(() => JsonConvert.DeserializeObject<Dictionary<string, List<User>>>(json)["Users"]);
        }

        private static Task<List<Listing>> ListingsFromJson(string json)
        {
            return Task.Run(() => JsonConvert.DeserializeObject<Dictionary<string, List<Listing>>>(json)["Listings"]);
        }

        private static WebRequestHandler DefaultRequestHandler()
        {
            var handler = new WebRequestHandler
            {
                ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true
            };

            return handler;
        }

        private static async Task<string> Request(string url)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

                using (var client = new HttpClient(handler))
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

        private static async Task<T> GetJson<T>(string url)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

                using (var client = new HttpClient(handler))
                {
                    try
                    {
                        var responseString = await client.GetStringAsync(url);
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

        private static async Task<T> SendObjectAsJson<T>(object obj, string url, Func<string, HttpContent, Task<HttpResponseMessage>> action)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await action(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

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
