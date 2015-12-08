using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows.Media.Imaging;
using System.Net;
using System.IO;
using System.Text;
using System.Web;

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
            return ListingsFromJson(responseString);
        }

        public static async Task<List<Listing>> SearchForText(string text)
        {
            var url = $"{RootUrl}apidb/searchlistings/{HttpUtility.UrlEncode(text)}";
            var responseString = await Request(url);
            return ListingsFromJson(responseString);
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

        public static async Task<bool> IsAuthenticated()
        {
            var user = CredentialManager.GetCurrentUser();

            if (user == null || user.SessionID == null)
            {
                return false;
            }

            using (var client = new HttpClient(DefaultRequestHandler()))
            {
                var auth = new AuthResponse(null, user.SessionID, user.ID);
                var fetched = await FetchUserById(auth, client);

                if (fetched == null)
                {
                    return false;
                }
                else
                {
                    return !string.IsNullOrWhiteSpace(fetched.SessionID);
                }
            }
        }

        internal async static Task<ErrorResponse> SaveNewUserInformation(User user)
        {
            using (var client = new HttpClient(DefaultRequestHandler()))
            {
                var session = CredentialManager.GetCurrentUser()?.SessionID;
                const string url = RootUrl + "apidb/editProfile";
                var payload = new Dictionary<string, object>() {
                    { "SessionID", session },
                    { "User", user }
                };
                
                return await SendObjectAsJson<ErrorResponse>(payload, url, client.PutAsync);
            }
        }

        public static async Task<List<Listing>> FetchListingsByUser(User user)
        {
            using (var client = new HttpClient(DefaultRequestHandler()))
            {
                var session = CredentialManager.GetCurrentUser()?.SessionID;
                if (session == null)
                {
                    return new List<Listing>();
                }

                var payload = new Dictionary<string, object>()
                {
                    { "SessionID", session }
                };

                var url = $"{RootUrl}apidb/users/listings/{user.ID}";
                var returnObject = await SendObjectAsJson<Dictionary<string, List<Listing>>>(payload, url, client.PutAsync);

                return returnObject["Listings"] ?? new List<Listing>();
            }
        }

        public static async Task<User> FetchUserById(int userId)
        {
            var currentUser = CredentialManager.GetCurrentUser();
            if (currentUser == null)
            {
                return null;
            }

            using (var client = new HttpClient(DefaultRequestHandler()))
            {
                var auth = new AuthResponse(null, currentUser.SessionID, userId);
                return await FetchUserById(auth, client);
            }
        }

        private static async Task<User> FetchUserById(AuthResponse auth, HttpClient client)
        {
            var url = $"{RootUrl}apidb/users/id/{auth.UserID}";
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

        public static async Task<bool> DeleteListing(Listing listing)
        {
            using (var client = new HttpClient(DefaultRequestHandler()))
            {
                var session = CredentialManager.GetCurrentUser()?.SessionID;

                if (session == null || listing == null)
                {
                    return false;
                }

                var url = $"{RootUrl}apidb/listings/{listing.ListingID}/delete";

                var payload = new Dictionary<string, object>() {
                    { "SessionID", session }
                };

                var response = await SendObjectAsJson<ErrorResponse>(payload, url, client.PutAsync);

                return response != null && string.IsNullOrEmpty(response.Error);
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

                var request = new LoginRequest(registration.Email, registration.Password);
                return await AttemptLogin(request);
            }
        }

        //public static async Task<List<Comment>> GetMessagesForCurrentUser()
        //{
        //    using (var client = new HttpClient(DefaultRequestHandler()))
        //    {
        //        var session = CredentialManager.GetCurrentUser()?.SessionID;

        //        if (session == null)
        //        {
        //            return null;
        //        }

        //        var url = RootUrl + $"/apidb/listingcomments/{listing.ListingID}/getAll";
        //    }
        //}

        public static async Task<ErrorResponse> PostNewCommentOnListing(Comment comment, Listing listing)
        {
            var session = CredentialManager.GetCurrentUser()?.SessionID;
            if (comment == null || listing == null || session == null)
            {
                return null;
            }

            using (var client = new HttpClient(DefaultRequestHandler()))
            {
                var url = RootUrl + $"apidb/listingcomments/{listing.ListingID}/add";
                var commentRequest = new Dictionary<string, object>()
                {
                    { "SessionID", session },
                    { "Comment", comment }
                };

                var response = await SendObjectAsJson<ErrorResponse>(commentRequest, url, client.PostAsync);
                return !string.IsNullOrEmpty(response?.Error) ? null : response;
            }
        }

        public static async Task<ErrorResponse> DeleteComment(Comment comment)
        {
            var session = CredentialManager.GetCurrentUser()?.SessionID;
            if (comment == null || session == null)
            {
                return null;
            }

            using (var client = new HttpClient(DefaultRequestHandler()))
            {
                var url = RootUrl + $"apidb/deletecomment/{comment.ID}/";
                var commentRequest = new Dictionary<string, object>()
                {
                    { "SessionID", session }
                };

                return await SendObjectAsJson<ErrorResponse>(commentRequest, url, client.PutAsync);
            }
        }

        public static async Task<CommentRequestResponse> GetCommentsForListing(Listing listing)
        {
            if (listing == null)
            {
                return null;
            }

            var url = RootUrl + $"apidb/listingcomments/{listing.ListingID}/getAll";
            return await GetJson<CommentRequestResponse>(url);
        }

        private static Task<List<User>> UsersFromJson(string json)
        {
            return Task.Run(() => JsonConvert.DeserializeObject<Dictionary<string, List<User>>>(json)["Users"]);
        }

        private static List<Listing> ListingsFromJson(string json)
        {
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<Listing>>>(json);

            if (dictionary != null)
            {
                if (dictionary["Listings"] != null)
                {
                    return dictionary["Listings"];
                }

                return null;
            }

            return null;
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
                return default(T);
            }
        }

        public static async Task<List<GoogleBook>> GetBooksMatchingTitle(string title)
        {
            var url = $"https://www.googleapis.com/books/v1/volumes?q={HttpUtility.UrlEncode(title)}&maxResults=40&orderBy=relevance";
            var response = await GetJson<GoogleBookResponse>(url);

            return response?.items ?? new List<GoogleBook>();
        }

        public static async Task<List<GoogleBook>> GetBooksMatchingIsbn(string isbn)
        {
            var url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{HttpUtility.UrlEncode(isbn)}";
            var response = await GetJson<GoogleBookResponse>(url);

            return response?.items ?? new List<GoogleBook>();
        }
    }
}
