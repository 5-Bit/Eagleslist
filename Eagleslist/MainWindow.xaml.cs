using System.Windows;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eagleslist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Request();
        }

        private async void Request()
        {
            string url = "http://sourcekitserviceterminated.com:8080/api";
            HttpClient client = new HttpClient();

            string responseString = await client.GetStringAsync(url);
            //6Dictionary<string, string> htmlAttributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Console.WriteLine(responseString);

            Dictionary<string, User[]> users = JsonConvert.DeserializeObject<Dictionary<string, User[]>>(responseString);
            foreach (User user in users["users"])
            {
                Console.WriteLine(user.ToString());
            }
        }
    }
}
