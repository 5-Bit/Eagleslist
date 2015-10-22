using System;
using Newtonsoft.Json;

namespace Eagleslist
{
    public class User
    {
        public int ID { get; private set; }

        public string Handle { get; private set; }
        public string Email { get; private set; }
        public string Bio { get; private set; }
        public bool IsMod { get; private set; }
        public bool IsFaculty { get; private set; }
    }
}
