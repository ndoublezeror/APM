using System.IO;
using System;
using System.Drawing;
using System.Security.Cryptography;
using TagLib;

namespace SongPlayer.Models
{
    public class Song
    {
        public string FilePath { get; }
        public string Title { get; }
        public string Artist { get; }
        public byte[] AlbumArtData { get; private set; }
        public string AlbumArtPath { get; private set; }

        public Image GetAlbumArtImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(AlbumArtPath) && System.IO.File.Exists(AlbumArtPath))
                {
                    return Image.FromFile(AlbumArtPath);
                }
                if (AlbumArtData == null) return null;
                using (var ms = new MemoryStream(AlbumArtData))
                {
                    var img = Image.FromStream(ms);
                    var bmp = new Bitmap(img);
                    img.Dispose();
                    return bmp;
                }
            }
            catch { return null; }
        }

        public Song(string path)
        {
            FilePath = path;
            Title = Path.GetFileNameWithoutExtension(path);
            Artist = string.Empty;
            AlbumArtData = null;
            try
            {
                var tfile = TagLib.File.Create(path);
                try
                {
                    if (!string.IsNullOrEmpty(tfile.Tag.Title)) Title = tfile.Tag.Title;
                    if (tfile.Tag.Performers != null && tfile.Tag.Performers.Length > 0)
                        Artist = tfile.Tag.Performers[0];
                    else if (!string.IsNullOrEmpty(tfile.Tag.FirstPerformer))
                        Artist = tfile.Tag.FirstPerformer;

                    if (tfile.Tag.Pictures != null && tfile.Tag.Pictures.Length > 0)
                    {
                        try { AlbumArtData = tfile.Tag.Pictures[0].Data.Data; } catch { AlbumArtData = null; }
                    }
                }
                finally { tfile.Dispose(); }
            }
            catch
            {
                // fallback: try to parse "Artist - Title" from filename
                var name = Path.GetFileNameWithoutExtension(path);
                if (!string.IsNullOrEmpty(name) && name.Contains(" - "))
                {
                    var parts = name.Split(new[] { " - " }, 2, System.StringSplitOptions.None);
                    Artist = parts[0].Trim();
                    Title = parts.Length > 1 ? parts[1].Trim() : parts[0].Trim();
                }
            }
            // if we have album art bytes, persist to AppData cache for faster reuse
            try
            {
                if (AlbumArtData != null && AlbumArtData.Length > 0)
                {
                    var appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SongPlayer", "art");
                    if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
                    // create a reproducible filename based on file path hash
                    using (var md5 = MD5.Create())
                    {
                        var key = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(path));
                        var fname = BitConverter.ToString(key).Replace("-", "") + ".jpg";
                        var dest = Path.Combine(appFolder, fname);
                        if (!System.IO.File.Exists(dest))
                        {
                            System.IO.File.WriteAllBytes(dest, AlbumArtData);
                        }
                        AlbumArtPath = dest;
                    }
                }
            }
            catch { }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
