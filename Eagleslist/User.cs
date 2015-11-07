using System;
using Newtonsoft.Json;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace Eagleslist
{
    public class User: IXmlSerializable
    {
        public int ID { get; private set; }
        public string SessionID { get; private set; }
        public string ImageURL { get; private set; }
        public string Handle { get; private set; }
        public string Email { get; private set; }
        public string Bio { get; private set; }
        public bool IsMod { get; private set; }
        public bool IsFaculty { get; private set; }

        public string AuthError { get; private set; }

        public User()
        {

        }

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
            this.AuthError = auth.Error;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            ID = Convert.ToInt32(reader.GetAttribute("ID"));
            SessionID = reader.GetAttribute("SessionID");
            ImageURL = reader.GetAttribute("ImageURL");
            Handle = reader.GetAttribute("Handle");
            Email = reader.GetAttribute("Email");
            Bio = reader.GetAttribute("Bio");
            IsMod = Convert.ToBoolean(reader.GetAttribute("IsMod"));
            IsFaculty = Convert.ToBoolean(reader.GetAttribute("IsFaculty"));
            AuthError = reader.GetAttribute("AuthError");
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ID", Convert.ToString(ID));
            writer.WriteAttributeString("SessionID", SessionID);
            writer.WriteAttributeString("ImageURL", ImageURL);
            writer.WriteAttributeString("Handle", Handle);
            writer.WriteAttributeString("Email", Email);
            writer.WriteAttributeString("Bio", Bio);
            writer.WriteAttributeString("IsMod", Convert.ToString(IsMod));
            writer.WriteAttributeString("IsFaculty", Convert.ToString(IsFaculty));
            writer.WriteAttributeString("AuthError", AuthError);
        }
    }
}
