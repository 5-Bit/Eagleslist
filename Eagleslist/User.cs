using System;
using Newtonsoft.Json;

namespace Eagleslist
{
    public class User
    {
        public int ID { get; private set; }
        public string ImageURL { get; set; }
        public string Handle { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public bool IsMod { get; set; }
        public bool IsFaculty { get; set; }

        public override string ToString()
        {
            return "User: " + Handle;
        }
    }
}
