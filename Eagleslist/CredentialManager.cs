using System;
using System.IO;
using System.Xml.Serialization;

namespace Eagleslist
{
    public static class CredentialManager
    {
        private static User _nonPersistentUser;

        public static void Burninate()
        {
            var currentUser = GetCurrentUser();
            if (!File.Exists(GetWriteFilePath()) && currentUser?.SessionID != null)
            {
                RequestManager.AttemptLogout(currentUser.SessionID);
            }
        }

        public static bool UserIsLoggedIn => GetCurrentUser() != null;

        public static User GetCurrentUser()
        {
            if (_nonPersistentUser != null)
            {
                return _nonPersistentUser;
            }

            return File.Exists(GetWriteFilePath()) ? ReadUser() : null;
        }

        public static bool IsPersisting
        {
            get
            {
                return _nonPersistentUser == null && File.Exists(GetWriteFilePath());
            }
        }

        public static void SetCurrentUser(User user, bool persist)
        {
            if (persist)
            {
                _nonPersistentUser = null;

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
                _nonPersistentUser = user;
            }
        }

        private static User ReadUser()
        {
            var serializer = new XmlSerializer(typeof(User));

            if (File.Exists(GetWriteFilePath()))
            {
                using (var reader = new StreamReader(GetWriteFilePath()))
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
            var serializer = new XmlSerializer(typeof(User));
            MakeWritePath(); // Will create all intermediate directories and override the existing file.

            using (var writer = new StreamWriter(GetWriteFilePath()))
            {
                serializer.Serialize(writer, user);
            }
        }

        private static void MakeWritePath()
        {
            var info = Directory.CreateDirectory(GetWriteFolderPath());

            if (!info.Exists) return;
            var stream = File.Create(GetWriteFilePath());
            stream.Dispose();
        }

        private static string GetWriteFolderPath()
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            const string eagleslistMainComponent = "Eagleslist";
            const string eagleslistDataComponent = "data";

            return Path.Combine(basePath, eagleslistMainComponent, eagleslistDataComponent);
        }

        private static string GetWriteFilePath()
        {
            const string filePath = "user.xml";
            return Path.Combine(GetWriteFolderPath(), filePath);
        }
    }
}
