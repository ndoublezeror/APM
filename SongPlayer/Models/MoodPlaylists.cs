using System.Collections.Generic;

namespace SongPlayer.Models
{
    public class MoodPlaylists
    {
        public Dictionary<Mood, List<string>> Playlists { get; set; } = new Dictionary<Mood, List<string>>();

        public MoodPlaylists()
        {
            // ensure keys exist
            foreach (Mood m in (Mood[])System.Enum.GetValues(typeof(Mood)))
            {
                if (!Playlists.ContainsKey(m)) Playlists[m] = new List<string>();
            }
        }
    }
}
