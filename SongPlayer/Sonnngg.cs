using System;

namespace SongPlayer
{
    // Minimal compatibility shim for Form1 references.
    // Original project had a Sonnngg class and a songg static holder; re-create the minimal members used by Form1 so the project builds.
    public static class songg
    {
        public const int bored = 1;
        public const int pretty = 2;
        public const int love = 3;
    }

    public class Sonnngg
    {
        // public field used by Form1
        public int song;

        public Sonnngg()
        {
            // default to first song
            song = songg.bored;
        }
        // methods expected by Form1 — set current song
        public void bo() { song = songg.bored; }
        public void pr() { song = songg.pretty; }
        public void lo() { song = songg.love; }
    }
}
