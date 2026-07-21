using System;
using System.IO;
using System.Xml.Serialization;
using SongPlayer.Models;

namespace SongPlayer.Services
{
    public static class PlaylistManager
    {
        private static readonly string appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SongPlayer");
        private static readonly string filePath = Path.Combine(appFolder, "playlists.xml");

        public static MoodPlaylists Load()
        {
            try
            {
                if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
                if (!File.Exists(filePath)) return new MoodPlaylists();
                var ser = new XmlSerializer(typeof(MoodPlaylists));
                using (var fs = File.OpenRead(filePath))
                {
                    return (MoodPlaylists)ser.Deserialize(fs) ?? new MoodPlaylists();
                }
            }
            catch
            {
                return new MoodPlaylists();
            }
        }

        public static void Save(MoodPlaylists data)
        {
            try
            {
                if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
                var ser = new XmlSerializer(typeof(MoodPlaylists));
                using (var fs = File.Create(filePath))
                {
                    ser.Serialize(fs, data);
                }
            }
            catch
            {
                // swallow for now
            }
        }
    }
}
