using System;
using System.IO;
using System.Xml.Serialization;

namespace Eagleslist
{
    public class CredentialManager
    {
        ~CredentialManager()
        {

        }

        public static User getCurrentUser()
        {
            return ReadUser();
        }

        public static void setCurrentUser(User user)
        {
            if (user == null)
            {
                if (File.Exists(GetWriteFilePath()))
                {
                    File.Delete(GetWriteFilePath());
                }
            }
            else
            {
                WriteUser(user);
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
