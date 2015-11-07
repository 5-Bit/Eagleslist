using System;
using Newtonsoft.Json;
using System.Reflection;

namespace Eagleslist
{
    public class User
    {
        public int ID { get; set; }
        public string SessionID { get; private set; }
        public string ImageURL { get; private set; }
        public string Handle { get; private set; }
        public string Email { get; private set; }
        public string Bio { get; private set; }
        public bool IsMod { get; private set; }
        public bool IsFaculty { get; private set; }

        public User(int id, string sessionID, string imageURL, string handle, string email, string bio, bool isMod, bool isFaculty)
        {
            this.ID = id;
            this.SessionID = sessionID;
            this.ImageURL = imageURL;
            this.Handle = handle;
            this.Email = email;
            this.Bio = bio;
            this.IsMod = isMod;
            this.IsFaculty = isFaculty;
        }

        public void AddAuth(AuthResponse auth)
        {
            this.SessionID = auth.SessionID;
            this.ID = auth.UserID;
        }

        public void MergeUser(User otherUser)
        {
            foreach(PropertyInfo property in typeof(User).GetProperties())
            {
                property.SetValue(this, property.GetValue(otherUser, null));
            }
        }
    }
}
