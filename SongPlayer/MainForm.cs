using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using AxWMPLib;
using SongPlayer.Controls;
using SongPlayer.Models;
using SongPlayer.Services;

using System.Linq;

namespace SongPlayer
{
    public class MainForm : Form
    {
        private AxWindowsMediaPlayer axPlayer;
        private readonly Timer playbackTimer = new Timer { Interval = 250 };
        private Label lblElapsed;
        private Label lblRemaining;
        private Label lblSongLine;
        private readonly List<Song> playlist = new List<Song>();
        private int currentIndex = -1;
        private bool isPlaying;

        // mood playlists persistence
        private MoodPlaylists playlistsData;
        private Mood currentMood = Mood.Bored;

        // UI
        private System.Windows.Forms.ListView playlistView;
        private IconButton btnAdd;
        private IconButton btnPlayPause;
        private SeekBar seekBar;
        private Label lblTitle;
        private Label lblArtist;
        private Label lblCurrentMood;
        private Button btnAddVisible;
        private Button btnChangeImage;
        private Button btnRemoveTrack;
        private Label statusLabel;

        // migrated controls from previous Form1
        private SongPlayer.Controls.AvatarBox pictureBox1;
        private Button btnBored;
        private Button btnPretty;
        private Button btnLove;
        private Button btnPlayLarge;
        private Button btnPrevAlt;
        private Button btnNextAlt;

        // migrated playback state (kept for compatibility with migrated handlers)
        private bool isplaying = false; // migrated play state
        private bool ns = false;
        private bool ps = false;
        private int q = 0;

        private enum MigratedSong { None, Bored, Pretty, Love }
        private MigratedSong currentMigrated = MigratedSong.None;

        // hardcoded migrated paths (kept from original migration)
        private readonly string boredPrimary = @"C:\Users\ratro\OneDrive\Desktop\Bored! (NINGNING Solo) - aespa.mp3";
        private readonly string boredAlternate = @"C:\Users\ratro\OneDrive\Desktop\Billie Eilish - Bored (Official Audio) - BillieEilishVEVO.mp3";
        private readonly string prettyPrimary = @"C:\Users\ratro\OneDrive\Desktop\KATSEYE (캣츠아이) I'm Pretty Lyric Video.mp3";
        private readonly string prettyAlternate = @"C:\Users\ratro\OneDrive\OneDrive\Desktop\Daya - Sit Still, Look Pretty (Lyrics).mp3";
        private readonly string lovePrimary = @"C:\Users\ratro\OneDrive\Desktop\Keyshia Cole - Love (Lyrics).mp3";
        private readonly string loveAlternate = @"C:\Users\ratro\OneDrive\Desktop\5 Seconds of Summer - Lover Of Mine (Official Audio).mp3";

        public MainForm()
        {
            Text = "SongPlayer";
            Size = new Size(900, 560);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(13, 13, 18);

            InitializeComponents();
            // load persisted mood playlists
            playlistsData = PlaylistManager.Load();
            BuildAudioEngine();

            playbackTimer.Tick += PlaybackTimer_Tick;
            playbackTimer.Start();

            // default to selected mood and show its playlist
            SetCurrentMood(Mood.Bored);
            // polish: update global theme colors
            BackColor = ColorTranslator.FromHtml("#121212");

            FormClosing += (s, e) => PlaylistManager.Save(playlistsData);
            // Ensure Add button also refreshes/selects the added song when used.
            try
            {
                if (btnAdd != null)
                {
                    btnAdd.Click += (s, e) =>
                    {
                        try
                        {
                            if (playlist != null && playlist.Count > 0)
                            {
                                if (currentIndex < 0) currentIndex = playlist.Count - 1;
                                PlayIndex(currentIndex);
                            }
                            RepositionSongLabelsUnderPicture();
                        }
                        catch { }
                    };
                }
            }
            catch { }
        }

        private System.Drawing.Bitmap CreateMoodImage(Mood mood, System.Drawing.Size size)
        {
            int w = Math.Max(64, size.Width);
            int h = Math.Max(64, size.Height);
            var bmp = new System.Drawing.Bitmap(w, h);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                // background
                using (var bg = new System.Drawing.SolidBrush(ColorTranslator.FromHtml("#252525")))
                    g.FillRectangle(bg, 0, 0, w, h);
                // circle with mood color
                Color c;
                switch (mood)
                {
                    case Mood.Happy: c = ColorTranslator.FromHtml("#1DB954"); break;
                    case Mood.Love: c = ColorTranslator.FromHtml("#E75480"); break;
                    case Mood.Sad: c = ColorTranslator.FromHtml("#3B82F6"); break;
                    case Mood.Thankful: c = ColorTranslator.FromHtml("#F59E0B"); break;
                    case Mood.Bored: c = ColorTranslator.FromHtml("#6B7280"); break;
                    default: c = ColorTranslator.FromHtml("#8A6AFF"); break;
                }
                var rect = new System.Drawing.Rectangle((w - Math.Min(w, h)) / 2, (h - Math.Min(w, h)) / 2, Math.Min(w, h), Math.Min(w, h));
                using (var grad = new System.Drawing.Drawing2D.LinearGradientBrush(rect, ControlPaint.Light(c), c, 45f))
                    g.FillEllipse(grad, rect);
                // letter
                string letter = mood.ToString().Substring(0, 1).ToUpper();
                using (var f = new Font("Segoe UI", Math.Max(18, rect.Width / 4), FontStyle.Bold))
                using (var br = new SolidBrush(Color.White))
                {
                    var sz = g.MeasureString(letter, f);
                    g.DrawString(letter, f, br, rect.X + (rect.Width - sz.Width) / 2f, rect.Y + (rect.Height - sz.Height) / 2f - 2);
                }
            }
            return bmp;
        }

        private void PlaylistView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var item = e.Item;
            var bounds = e.Bounds;
            // background
            if (e.State.HasFlag(ListViewItemStates.Selected))
                using (var sel = new SolidBrush(Color.FromArgb(40, 40, 48))) g.FillRectangle(sel, bounds);
            else
                using (var bg = new SolidBrush(Color.FromArgb(26, 26, 35))) g.FillRectangle(bg, bounds);
            // icon / thumbnail placeholder
            var thumbRect = new Rectangle(bounds.X + 8, bounds.Y + 6, 36, 36);
            using (var br = new LinearGradientBrush(thumbRect, Color.FromArgb(48, 48, 56), Color.FromArgb(32, 32, 36), 45f))
                g.FillEllipse(br, thumbRect);

            // title and artist
            var song = item.Tag as Song;
            string title = song?.Title ?? item.Text;
            string artist = song?.Artist ?? string.Empty;
            using (var titleFont = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (var artistFont = new Font("Segoe UI", 8f))
            using (var titleBrush = new SolidBrush(Color.White))
            using (var artistBrush = new SolidBrush(ColorTranslator.FromHtml("#B3B3B3")))
            {
                g.DrawString(title, titleFont, titleBrush, thumbRect.Right + 8, bounds.Y + 6);
                if (!string.IsNullOrEmpty(artist))
                {
                    g.DrawString(artist, artistFont, artistBrush, thumbRect.Right + 8, bounds.Y + 24);
                }
            }
        }

        private void InitializeComponents()
        {
            // Set global form font and base colors to match 2026 premium dark theme
            try { Font = new Font("Segoe UI Variable", 10f, FontStyle.Regular); } catch { Font = new Font("Segoe UI", 10f, FontStyle.Regular); }
            BackColor = ColorTranslator.FromHtml("#121212");

            // Add padding around content
            Padding = new Padding(16);

            // top-left add button (compact)
            btnAdd = new IconButton
            {
                Icon = AppIcon.Plus,
                IdleColor = Color.FromArgb(36, 36, 38),
                HoverColor = ColorTranslator.FromHtml("#2A2A2E"),
                Size = new Size(36, 36),
                Location = new Point(16, 16)
            };
            btnAdd.Click += BtnAdd_Click;

            // primary play/pause control (used in toolbar)
            btnPlayPause = new IconButton
            {
                Icon = AppIcon.Play,
                IdleColor = Color.FromArgb(36, 36, 38),
                HoverColor = ColorTranslator.FromHtml("#2A2A2E"),
                Size = new Size(48, 48)
            };
            btnPlayPause.Click += (s, e) => TogglePlayPause();

            // seek bar (styled)
            seekBar = new SeekBar { Width = 520, Height = 18 };
            seekBar.ValueChangedByUser += (s, e) =>
            {
                if (axPlayer?.currentMedia != null)
                {
                    double dur = axPlayer.currentMedia.duration;
                    axPlayer.Ctlcontrols.currentPosition = seekBar.Value * dur;
                }
            };

            // Title / artist labels
            lblTitle = new Label
            {
                ForeColor = Color.White,
                AutoSize = true,
                Font = new Font(Font.FontFamily, 14f, FontStyle.Bold)
            };
            lblArtist = new Label { ForeColor = ColorTranslator.FromHtml("#B3B3B3"), AutoSize = true, Font = new Font(Font.FontFamily, 10f) };

            // left panel: moods / playlists
            var leftPanel = new Panel { Dock = DockStyle.Left, Width = 280, BackColor = ColorTranslator.FromHtml("#1C1C1E") };
            var moodFlow = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 220, FlowDirection = FlowDirection.TopDown, Padding = new Padding(12), AutoScroll = true, BackColor = Color.Transparent };

            // current mood label
            lblCurrentMood = new Label { Text = "Mood: Bored", ForeColor = Color.White, Font = new Font(Font.FontFamily, 11f, FontStyle.Bold), AutoSize = true, Location = new Point(16, 12) };
            leftPanel.Controls.Add(lblCurrentMood);

            // create mood tiles (polished custom control)
            foreach (Mood m in (Mood[])Enum.GetValues(typeof(Mood)))
            {
                var tile = new SongPlayer.Controls.MoodTile
                {
                    Tag = m,
                    Mood = m,
                    Width = leftPanel.Width - 36,
                    Height = 56,
                    Accent = ColorTranslator.FromHtml("#1DB954")
                };
                tile.Click += (s, e) => SetCurrentMood((Mood)((MoodTile)s).Mood);
                moodFlow.Controls.Add(tile);
            }
            leftPanel.Controls.Add(moodFlow);
            // visible textual add button so it's obvious in the UI
            btnAddVisible = new Button
            {
                Text = $"Add tracks to {currentMood}",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = ColorTranslator.FromHtml("#252525"),
                Location = new Point(16, moodFlow.Bottom + 12),
                Size = new Size(leftPanel.Width - 40, 36)
            };
            btnAddVisible.FlatAppearance.BorderSize = 0;
            btnAddVisible.Click += (s, e) => BtnAdd_Click(s, e);
            leftPanel.Controls.Add(btnAddVisible);
            // keep the icon button as a compact control too (optional)
            btnAdd.Location = new Point(16, moodFlow.Bottom + 12);
            leftPanel.Controls.Add(btnAdd);

            // change mood image button
            btnChangeImage = new Button
            {
                Text = "Change image",
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = ColorTranslator.FromHtml("#252525"),
                Location = new Point(16, moodFlow.Bottom + 56),
                Size = new Size(leftPanel.Width - 40, 36)
            };
            btnChangeImage.FlatAppearance.BorderSize = 0;
            btnChangeImage.Click += (s, e) =>
            {
                using (var dlg = new OpenFileDialog())
                {
                    dlg.Filter = "Image files|*.png;*.jpg;*.jpeg|All files|*.*";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            var imgPath = dlg.FileName;
                            var appFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SongPlayer", "mood_images");
                            if (!System.IO.Directory.Exists(appFolder)) System.IO.Directory.CreateDirectory(appFolder);
                            var dest = System.IO.Path.Combine(appFolder, currentMood.ToString() + System.IO.Path.GetExtension(imgPath));
                            System.IO.File.Copy(imgPath, dest, true);
                            // apply immediately
                            try { pictureBox1.CrossfadeTo(System.Drawing.Image.FromFile(dest)); } catch { }
                            UpdateStatus($"Mood image updated for {currentMood}");
                        }
                        catch { UpdateStatus("Failed to update mood image"); }
                    }
                }
            };
            leftPanel.Controls.Add(btnChangeImage);

            // center area: album art + metadata + seek
            var centerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            // picture container holds the album art and forces it to be above the metadata
            var pictureContainer = new Panel { Dock = DockStyle.Top, Height = 460, BackColor = Color.Transparent };
            pictureBox1 = new SongPlayer.Controls.AvatarBox { Size = new Size(420, 420), Anchor = AnchorStyles.None };
            // center the avatar inside pictureContainer
            pictureBox1.Location = new Point(Math.Max(16, (pictureContainer.Width - pictureBox1.Width) / 2), 20);
            pictureContainer.Resize += (s, e) => { pictureBox1.Location = new Point(Math.Max(16, (pictureContainer.Width - pictureBox1.Width) / 2), 20); };
            pictureContainer.Controls.Add(pictureBox1);

            // metadata container (positioned under album art)
            var metaPanel = new Panel { Height = 80, Dock = DockStyle.Top, BackColor = Color.Transparent };
            lblTitle.Location = new Point(8, 6);
            lblArtist.Location = new Point(8, 32);
            lblSongLine = new Label { ForeColor = ColorTranslator.FromHtml("#9A9A9A"), AutoSize = true, Location = new Point(8, 52), Font = new Font(Font.FontFamily, 8f) };
            metaPanel.Controls.Add(lblTitle);
            metaPanel.Controls.Add(lblArtist);
            metaPanel.Controls.Add(lblSongLine);

            centerPanel.Controls.Add(pictureContainer);
            centerPanel.Controls.Add(metaPanel);

            // seek container 
            var seekContainer = new Panel { Height = 48, Dock = DockStyle.Bottom, BackColor = Color.Transparent };
            seekBar.Location = new Point(80, 12);
            seekContainer.Resize += (s, e) => { seekBar.Location = new Point(80, 12); seekBar.Width = Math.Max(280, seekContainer.Width - 180); };
            // elapsed and remaining labels
            lblElapsed = new Label { Text = "00:00", ForeColor = ColorTranslator.FromHtml("#B3B3B3"), AutoSize = false, Width = 64, TextAlign = ContentAlignment.MiddleLeft, Location = new Point(8, 12) };
            lblRemaining = new Label { Text = "-00:00", ForeColor = ColorTranslator.FromHtml("#B3B3B3"), AutoSize = false, Width = 64, TextAlign = ContentAlignment.MiddleRight, Location = new Point(seekContainer.Width - 72, 12), Anchor = AnchorStyles.Top | AnchorStyles.Right };
            seekContainer.Controls.Add(lblElapsed);
            seekContainer.Controls.Add(lblRemaining);
            seekContainer.Controls.Add(seekBar);

            // transport controls panel
            var transportPanel = new Panel { Height = 110, Dock = DockStyle.Bottom, BackColor = Color.Transparent };
            var prevBtn = new IconButton { Icon = AppIcon.Previous, Size = new Size(52, 52), IdleColor = Color.FromArgb(36, 36, 38), CircleBackground = true, MinimumSize = new Size(44, 44) };
            var playBtn = new IconButton { Icon = AppIcon.Play, Size = new Size(84, 84), IdleColor = ColorTranslator.FromHtml("#1DB954"), Filled = true, CircleBackground = true, IconScale = 1.1f, MinimumSize = new Size(64, 64) };
            // expose central play button via field so TogglePlayPause updates the visible control
            btnPlayPause = playBtn;
            var nextBtn = new IconButton { Icon = AppIcon.Next, Size = new Size(52, 52), IdleColor = Color.FromArgb(36, 36, 38), CircleBackground = true, MinimumSize = new Size(44, 44) };
            playBtn.Click += (s, e) => TogglePlayPause();
            prevBtn.Click += (s, e) => { if (playlist.Count == 0) return; int prev = (currentIndex <= 0) ? playlist.Count - 1 : currentIndex - 1; PlayIndex(prev); };
            nextBtn.Click += (s, e) => PlayNext();

            // layout transport - center horizontally
            transportPanel.Resize += (s, e) =>
            {
                int cx = transportPanel.Width / 2;
                playBtn.Location = new Point(cx - playBtn.Width / 2, 8);
                prevBtn.Location = new Point(playBtn.Left - prevBtn.Width - 24, 16);
                nextBtn.Location = new Point(playBtn.Right + 24, 16);
            };
            centerPanel.Controls.Add(seekContainer);
            // add metadata panel above transport so song title/artist sits under the album art
            centerPanel.Controls.Add(metaPanel);
            transportPanel.Controls.Add(prevBtn);
            transportPanel.Controls.Add(playBtn);
            transportPanel.Controls.Add(nextBtn);
            centerPanel.Controls.Add(transportPanel);

            // right panel: playlist and controls
            var rightPanel = new Panel { Dock = DockStyle.Right, Width = 340, BackColor = ColorTranslator.FromHtml("#252525"), MinimumSize = new Size(220, 200) };
            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Color.Transparent };
            var rightTitle = new Label { Text = "Playlist", ForeColor = Color.White, Font = new Font(Font.FontFamily, 12f, FontStyle.Bold), Location = new Point(16, 12), AutoSize = true };
            btnRemoveTrack = new Button { Text = "Remove", FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.Transparent, Size = new Size(84, 32) };
            btnRemoveTrack.FlatAppearance.BorderSize = 0;
            btnRemoveTrack.Dock = DockStyle.Right;
            btnRemoveTrack.Click += (s, e) =>
            {
                // if any items are checked, remove them; otherwise remove selected
                try
                {
                    var toRemove = new System.Collections.Generic.List<int>();
                    if (playlistView.CheckedItems.Count > 0)
                    {
                        foreach (ListViewItem it in playlistView.CheckedItems) toRemove.Add(it.Index);
                    }
                    else if (playlistView.SelectedIndices.Count > 0)
                    {
                        toRemove.Add(playlistView.SelectedIndices[0]);
                    }
                    if (toRemove.Count == 0) return;
                    // remove highest indexes first
                    toRemove.Sort(); toRemove.Reverse();
                    foreach (var idx in toRemove)
                    {
                        if (idx >= 0 && idx < playlist.Count)
                        {
                            var song = playlist[idx];
                            playlist.RemoveAt(idx);
                            playlistView.Items.RemoveAt(idx);
                            if (playlistsData?.Playlists != null && playlistsData.Playlists.ContainsKey(currentMood))
                                playlistsData.Playlists[currentMood].Remove(song.FilePath);
                        }
                    }
                    PlaylistManager.Save(playlistsData);
                    UpdateStatus($"Removed {toRemove.Count} track(s)");
                    SetCurrentMood(currentMood);
                }
                catch { }
            };
            headerPanel.Controls.Add(rightTitle);
            headerPanel.Controls.Add(btnRemoveTrack);
            rightPanel.Controls.Add(headerPanel);

            // playlistView configuration: use Details view for reliable Title/Artist columns
            playlistView = new System.Windows.Forms.ListView
            {
                View = View.Details,
                Dock = DockStyle.Fill,
                BackColor = ColorTranslator.FromHtml("#252525"),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                FullRowSelect = true,
                MultiSelect = false,
                HeaderStyle = ColumnHeaderStyle.Clickable
            };
            playlistView.Columns.Add("Title", 220);
            playlistView.Columns.Add("Artist", 100);
            playlistView.CheckBoxes = true;
            playlistView.DoubleClick += (s, e) => { if (playlistView.SelectedIndices.Count > 0) PlayIndex(playlistView.SelectedIndices[0]); };
            rightPanel.Controls.Add(playlistView);

            // add controls to the form
            Controls.Add(centerPanel);
            Controls.Add(rightPanel);
            Controls.Add(leftPanel);

            // ensure meta panel is positioned under the album art immediately
            try
            {
                metaPanel.Width = Math.Min(centerPanel.Width - 48, pictureBox1.Width);
                metaPanel.Location = new Point(pictureBox1.Left + (pictureBox1.Width - metaPanel.Width) / 2, pictureBox1.Bottom + 12);
            }
            catch { }

            // context menu for playlistView
            var ctx = new ContextMenuStrip();
            var miRemove = new ToolStripMenuItem("Remove");
            var miReveal = new ToolStripMenuItem("Reveal in Explorer");
            miRemove.Click += (s, e) =>
            {
                if (playlistView.SelectedIndices.Count == 0) return;
                int idx = playlistView.SelectedIndices[0];
                if (idx >= 0 && idx < playlist.Count)
                {
                    var song = playlist[idx];
                    playlist.RemoveAt(idx);
                    playlistView.Items.RemoveAt(idx);
                    if (playlistsData?.Playlists != null && playlistsData.Playlists.ContainsKey(currentMood))
                        playlistsData.Playlists[currentMood].Remove(song.FilePath);
                    PlaylistManager.Save(playlistsData);
                    SetCurrentMood(currentMood);
                }
            };
            miReveal.Click += (s, e) =>
            {
                if (playlistView.SelectedIndices.Count == 0) return;
                int idx = playlistView.SelectedIndices[0];
                if (idx >= 0 && idx < playlist.Count)
                {
                    var path = playlist[idx].FilePath;
                    if (System.IO.File.Exists(path))
                        System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{path}\"");
                }
            };
            ctx.Items.Add(miRemove);
            ctx.Items.Add(miReveal);
            playlistView.ContextMenuStrip = ctx;

            // drag-drop support
            playlistView.AllowDrop = true;
            playlistView.DragEnter += (s, e) => { if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy; };
            playlistView.DragDrop += (s, e) =>
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var f in files)
                {
                    // update persisted playlists only; UI will be rebuilt by SetCurrentMood
                    if (playlistsData == null) playlistsData = new MoodPlaylists();
                    if (!playlistsData.Playlists.ContainsKey(currentMood)) playlistsData.Playlists[currentMood] = new System.Collections.Generic.List<string>();
                    playlistsData.Playlists[currentMood].Add(f);
                }
                PlaylistManager.Save(playlistsData);
                // rebuild playlist UI from persisted data
                SetCurrentMood(currentMood);
            };

            // Seek bar and title in center
            seekBar.Location = new Point(40, 420);
            centerPanel.Controls.Add(seekBar);
            // don't add lblTitle directly to centerPanel; it is inside metaPanel positioned under the album art

            // status label at bottom
            statusLabel = new Label
            {
                Text = "",
                Dock = DockStyle.Bottom,
                Height = 24,
                ForeColor = ColorTranslator.FromHtml("#B3B3B3"),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            Controls.Add(statusLabel);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // debug helper: show a brief confirmation that handler is invoked
            try { System.Diagnostics.Debug.WriteLine("BtnAdd_Click invoked"); } catch { }
            using (var dlg = new OpenFileDialog())
            {
                dlg.Multiselect = true;
                dlg.Filter = "Audio files|*.mp3;*.wav;*.wma;*.flac|All files|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (var f in dlg.FileNames)
                    {
                        var song = new Song(f);
                        playlist.Add(song);
                        var itext = string.IsNullOrEmpty(song.Artist) ? song.Title : $"{song.Title} — {song.Artist}";
                        // do not add to UI here; SetCurrentMood will rebuild the playlistView from persisted data
                        // keep playlistsData updated

                        if (playlistsData == null) playlistsData = new MoodPlaylists();
                        if (!playlistsData.Playlists.ContainsKey(currentMood)) playlistsData.Playlists[currentMood] = new System.Collections.Generic.List<string>();
                        playlistsData.Playlists[currentMood].Add(f);
                    }
                    PlaylistManager.Save(playlistsData);
                    UpdateStatus($"Added {dlg.FileNames.Length} track(s) to {currentMood}");
                    // reload persisted playlists to avoid any serialization timing issues and rebuild UI
                    try { playlistsData = PlaylistManager.Load(); } catch { }
                    SetCurrentMood(currentMood);
                    // ensure playlist view redraws immediately after adding tracks
                    playlistView.Invalidate();
                }
            }
        }

        private void SetCurrentMood(Mood mood)
        {
            currentMood = mood;
            // refresh playlist view for this mood
            playlist.Clear();
            // clear the current playlist view
            playlistView.Items.Clear();
            if (playlistsData == null) playlistsData = new MoodPlaylists();
            if (!playlistsData.Playlists.ContainsKey(mood)) playlistsData.Playlists[mood] = new System.Collections.Generic.List<string>();
                foreach (var path in playlistsData.Playlists[mood])
            {
                var s = new Song(path);
                playlist.Add(s);
                var lvi = new ListViewItem(s.Title) { Tag = s };
                lvi.SubItems.Add(s.Artist ?? string.Empty);
                playlistView.Items.Add(lvi);
            }
            // ensure first item is visible and UI refreshed to avoid missing-first-item symptom
            try { if (playlistView.Items.Count > 0) { playlistView.Items[0].EnsureVisible(); playlistView.Refresh(); } } catch { }
            // clear current playing title/artist when switching mood
            lblTitle.Text = string.Empty;
            if (lblArtist != null) lblArtist.Text = string.Empty;
            if (lblSongLine != null) lblSongLine.Text = string.Empty;
            // update visible current mood label and add-button text
            try
            {
                if (lblCurrentMood != null) lblCurrentMood.Text = $"Mood: {mood}";
                if (btnAddVisible != null) btnAddVisible.Text = $"Add tracks to {mood}";
            }
            catch { }
            // update album art to mood image (prefer custom uploaded image)
            try
            {
                var appFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SongPlayer", "mood_images");
                System.Drawing.Image toUse = null;
                if (System.IO.Directory.Exists(appFolder))
                {
                    var files = System.IO.Directory.GetFiles(appFolder, mood.ToString() + ".*");
                    if (files.Length > 0)
                    {
                        try
                        {
                            // load image into memory and create a Bitmap copy to avoid locking the source file
                            var bytes = System.IO.File.ReadAllBytes(files[0]);
                            using (var ms = new System.IO.MemoryStream(bytes))
                            using (var img = System.Drawing.Image.FromStream(ms))
                            {
                                toUse = new System.Drawing.Bitmap(img);
                            }
                        }
                        catch { toUse = null; }
                    }
                }
                if (toUse == null)
                {
                    toUse = CreateMoodImage(mood, pictureBox1.Size);
                }
                if (pictureBox1 != null && toUse != null)
                {
                    pictureBox1.CrossfadeTo(toUse);
                    try { pictureBox1.Invalidate(); pictureBox1.Refresh(); } catch { }
                }
            }
            catch { }
            // ensure persistence of mood playlists
            try { PlaylistManager.Save(playlistsData); } catch { }

            // update mood tile counts if left panel exists
            try
            {
                foreach (Control c in Controls)
                {
                    if (c is Panel left && left.Dock == DockStyle.Left)
                    {
                        // find the FlowLayoutPanel among left panel children
                        FlowLayoutPanel flow = null;
                        foreach (Control lc in left.Controls)
                        {
                            if (lc is FlowLayoutPanel fp) { flow = fp; break; }
                        }
                        if (flow != null)
                        {
                            foreach (Control ctl in flow.Controls)
                            {
                                if (ctl is SongPlayer.Controls.MoodTile mt)
                                {
                                    var mm = mt.Mood;
                                    int count = playlistsData.Playlists.ContainsKey(mm) ? playlistsData.Playlists[mm].Count : 0;
                                    mt.Count = count;
                                    mt.Selected = (mm == currentMood);
                                }
                                else if (ctl is Button b)
                                {
                                    var mm = (Mood)b.Tag;
                                    int count = playlistsData.Playlists.ContainsKey(mm) ? playlistsData.Playlists[mm].Count : 0;
                                    b.Text = $"{mm} ({count})";
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void UpdateStatus(string text)
        {
            try
            {
                if (statusLabel != null)
                {
                    statusLabel.Text = text;
                    var t = new Timer { Interval = 3000 };
                    t.Tick += (s, e) => { statusLabel.Text = ""; t.Stop(); t.Dispose(); };
                    t.Start();
                }
            }
            catch { }
        }

        // Migrated control handlers (simplified and consolidated)
        private void BtnBored_Click(object sender, EventArgs e)
        {
            q = 0;
            currentMigrated = MigratedSong.Bored;
            lblTitle.Text = "Bored";
            ns = ps = false;
        }

        private void BtnPretty_Click(object sender, EventArgs e)
        {
            q = 0;
            currentMigrated = MigratedSong.Pretty;
            lblTitle.Text = "Pretty";
            ns = ps = false;
        }

        private void BtnLove_Click(object sender, EventArgs e)
        {
            q = 0;
            currentMigrated = MigratedSong.Love;
            lblTitle.Text = "Love";
            ns = ps = false;
        }

        private void BtnPlayLarge_Click(object sender, EventArgs e)
        {
            // Toggle migrated play/pause
            if (!isplaying)
            {
                try
                {
                    if (btnPlayLarge != null && Properties.Resources.pause != null)
                        btnPlayLarge.BackgroundImage = Properties.Resources.pause;
                }
                catch { }

                string url = null;
                switch (currentMigrated)
                {
                    case MigratedSong.Bored:
                        url = ps || ns ? boredAlternate : boredPrimary;
                        TrySetPicture(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-10-21 191554.png");
                        RepositionSongLabelsUnderPicture();
                        break;
                    case MigratedSong.Pretty:
                        url = ps || ns ? prettyAlternate : prettyPrimary;
                        TrySetPicture(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 113923.png");
                        RepositionSongLabelsUnderPicture();
                        break;
                    case MigratedSong.Love:
                        url = ps || ns ? loveAlternate : lovePrimary;
                        TrySetPicture(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 114842.png");
                        RepositionSongLabelsUnderPicture();
                        break;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    axPlayer.URL = url;
                    axPlayer.Ctlcontrols.play();
                    isplaying = true;
                    q++;
                }
            }
            else
            {
                try
                {
                    if (btnPlayLarge != null && Properties.Resources.play != null)
                        btnPlayLarge.BackgroundImage = Properties.Resources.play;
                }
                catch { }

                try { axPlayer.Ctlcontrols.pause(); } catch { }
                isplaying = false;
            }
        }

        private void BtnPrevAlt_Click(object sender, EventArgs e)
        {
            ps = !ps;
            if (isplaying && currentMigrated != MigratedSong.None)
            {
                BtnPlayLarge_Click(this, EventArgs.Empty);
            }
        }

        private void BtnNextAlt_Click(object sender, EventArgs e)
        {
            ns = !ns;
            if (isplaying && currentMigrated != MigratedSong.None)
            {
                BtnPlayLarge_Click(this, EventArgs.Empty);
            }
        }

        private void TrySetPicture(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    pictureBox1.Image = Image.FromFile(path);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            catch { }
        }

        private void BuildAudioEngine()
        {
            ((System.ComponentModel.ISupportInitialize)(axPlayer = new AxWindowsMediaPlayer())).BeginInit();
            axPlayer.Location = new Point(-500, -500);
            axPlayer.Size = new Size(1, 1);
            axPlayer.Enabled = true;
            Controls.Add(axPlayer);
            ((System.ComponentModel.ISupportInitialize)(axPlayer)).EndInit();

            axPlayer.uiMode = "none";
            axPlayer.settings.autoStart = false;
            axPlayer.settings.volume = 70;
            axPlayer.PlayStateChange += AxPlayer_PlayStateChange;
        }

        private void AxPlayer_PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            try
            {
                if (e.newState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
                {
                    BeginInvoke((Action)(() => { PlayNext(); }));
                }
                else if (e.newState == (int)WMPLib.WMPPlayState.wmppsPlaying)
                {
                    BeginInvoke((Action)(() => { isPlaying = true; if (btnPlayPause != null) btnPlayPause.Icon = AppIcon.Pause; }));
                }
                else if (e.newState == (int)WMPLib.WMPPlayState.wmppsPaused || e.newState == (int)WMPLib.WMPPlayState.wmppsStopped)
                {
                    BeginInvoke((Action)(() => { isPlaying = false; if (btnPlayPause != null) btnPlayPause.Icon = AppIcon.Play; }));
                }
            }
            catch { }
        }

        private void PlaybackTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (axPlayer?.currentMedia == null) return;
                double duration = axPlayer.currentMedia.duration;
                double position = axPlayer.Ctlcontrols.currentPosition;
                if (duration > 0)
                {
                    var target = position / duration;
                    // smooth interpolation towards target
                    var curr = seekBar.Value;
                    var diff = target - curr;
                    var step = Math.Sign(diff) * Math.Min(0.06, Math.Abs(diff));
                    if (!seekBar.IsDragging) seekBar.Value = Math.Max(0, Math.Min(1, curr + step));

                    // update time labels
                    try { if (lblElapsed != null) lblElapsed.Text = TimeSpan.FromSeconds(position).ToString(@"mm\:ss"); } catch { }
                    try { if (lblRemaining != null) lblRemaining.Text = "-" + TimeSpan.FromSeconds(Math.Max(0, duration - position)).ToString(@"mm\:ss"); } catch { }
                }
            }
            catch { }
        }

        // utility to set avatar image from current track if available
        private void TrySetCoverFromCurrent()
        {
            try
            {
                if (currentIndex >= 0 && currentIndex < playlist.Count)
                {
                    var song = playlist[currentIndex];
                    // prefer embedded album art from tags
                    try
                    {
                        var art = song?.GetAlbumArtImage();
                        if (art != null)
                        {
                            pictureBox1.CrossfadeTo(art);
                            return;
                        }
                    }
                    catch { }

                    // attempt to find image in same folder named cover.jpg/png or album art
                    var path = playlist[currentIndex].FilePath;
                    var dir = Path.GetDirectoryName(path);
                    if (dir != null)
                    {
                        var candidates = new[] { "cover.jpg", "cover.png", "album.jpg", "folder.jpg" };
                        foreach (var c in candidates)
                        {
                            var p = Path.Combine(dir, c);
                            if (File.Exists(p))
                            {
                                try { pictureBox1.CrossfadeTo(Image.FromFile(p)); } catch { pictureBox1.CrossfadeTo(null); }
                                return;
                            }
                        }
                    }
                    // fallback: leave current (mood) image in place
                }
            }
            catch { }
        }

        private void PlayIndex(int index)
        {
            if (index < 0 || index >= playlist.Count) return;
            currentIndex = index;
            axPlayer.URL = playlist[index].FilePath;
            axPlayer.Ctlcontrols.play();
            isPlaying = true;
            btnPlayPause.Icon = AppIcon.Pause;
            lblTitle.Text = playlist[index].Title;
            // select item in playlistView
            try
            {
                playlistView.SelectedIndices.Clear();
                if (index >= 0 && index < playlistView.Items.Count) playlistView.Items[index].Selected = true;
            }
            catch { }
            // try to show album art from song tags
            try { TrySetCoverFromCurrent(); } catch { }
            // update artist label as well
            try { if (lblArtist != null) lblArtist.Text = playlist[index].Artist; } catch { }
            // show the file name under artist
            try { if (lblSongLine != null) lblSongLine.Text = System.IO.Path.GetFileName(playlist[index].FilePath); } catch { }
            // removed debug messagebox
        }

        private void TogglePlayPause()
        {
            // If no items in the playlist, do not invoke UI click handler directly
            // The large play button is for migrated/demo songs; avoid calling the click
            // handler during initialization or when controls may not be ready.
            if (playlist.Count == 0)
            {
                return;
            }

            if (currentIndex < 0) PlayIndex(0);
            else
            {
                if (isPlaying)
                {
                    axPlayer.Ctlcontrols.pause();
                    isPlaying = false;
                    btnPlayPause.Icon = AppIcon.Play;
                }
                else
                {
                    axPlayer.Ctlcontrols.play();
                    isPlaying = true;
                    btnPlayPause.Icon = AppIcon.Pause;
                }
            }
        }

        // Helper: recursively get all controls
        private IEnumerable<Control> GetAllControls(Control parent)
        {
            var list = new List<Control>();
            foreach (Control c in parent.Controls)
            {
                list.Add(c);
                if (c.HasChildren) list.AddRange(GetAllControls(c));
            }
            return list;
        }

        // Move song/artist labels (if present) to be under the first picture box found.
        private void RepositionSongLabelsUnderPicture()
        {
            try
            {
                var all = GetAllControls(this).ToList();
                var pic = all.OfType<PictureBox>().FirstOrDefault();
                if (pic == null) return;

                var labels = all.OfType<Label>().Where(l =>
                    (l.Name ?? string.Empty).IndexOf("song", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (l.Name ?? string.Empty).IndexOf("title", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (l.Name ?? string.Empty).IndexOf("artist", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (l.Name ?? string.Empty).IndexOf("by", StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                if (labels.Count == 0) return;

                int y = pic.Bottom + 6;
                int left = pic.Left;
                foreach (var lbl in labels)
                {
                    lbl.Left = left;
                    lbl.Top = y;
                    y += lbl.Height + 2;
                }
            }
            catch { }
        }

        private void PlayNext()
        {
            if (playlist.Count == 0) return;
            int next = (currentIndex + 1) % playlist.Count;
            PlayIndex(next);
        }
    }
}
