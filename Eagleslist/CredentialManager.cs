using System;
using System.IO;
using System.Xml.Serialization;

namespace Eagleslist
{
    public class CredentialManager
    {
        private static User NonPersistentUser = null;

        public static bool UserIsLoggedIn
        {
            get
            {
                return GetCurrentUser() != null;
            }
        }

        public static User GetCurrentUser()
        {
            if (File.Exists(GetWriteFilePath()))
            {
                return ReadUser();
            }
            else
            {
                return NonPersistentUser;
            }
        }

        public static void SetCurrentUser(User user, bool persist)
        {
            if (persist)
            {
                if (user == null)
                {
                    DeletePersistingUser();
                }
                else
                {
                    WriteUser(user);
                }
            }
            else
            {
                DeletePersistingUser();
                NonPersistentUser = user;
            }
        }

        private static User ReadUser()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(User));

            if (File.Exists(GetWriteFilePath()))
            {
                using (StreamReader reader = new StreamReader(GetWriteFilePath()))
                {
                    return (User)serializer.Deserialize(reader);
                }
            }

            return null;
        }

        private static void DeletePersistingUser()
        {
            if (File.Exists(GetWriteFilePath()))
            {
                File.Delete(GetWriteFilePath());
            }
        }

        private static void WriteUser(User user)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(User));
            MakeWritePath(); // Will create all intermediate directories and override the existing file.

            using (StreamWriter writer = new StreamWriter(GetWriteFilePath()))
            {
                serializer.Serialize(writer, user);
                Console.WriteLine("Wrote to: {0}", GetWriteFilePath());
            }
        }

        private static void MakeWritePath()
        {
            DirectoryInfo info = Directory.CreateDirectory(GetWriteFolderPath());

            if (info.Exists)
            {
                FileStream stream = File.Create(GetWriteFilePath());
                stream.Dispose();
            }
        }

        private static string GetWriteFolderPath()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string eagleslistMainComponent = "Eagleslist";
            string eagleslistDataComponent = "data";

            return Path.Combine(basePath, eagleslistMainComponent, eagleslistDataComponent);
        }

        private static string GetWriteFilePath()
        {
            string filePath = "user.xml";
            return Path.Combine(GetWriteFolderPath(), filePath);
        }
    }
}
