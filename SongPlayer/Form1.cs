using AxWMPLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SongPlayer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.Visible = false;
            //  player = new SoundPlayer();
        }
        bool isplaying = false;
        static int q = 0;
       private void button5_Click(object sender, EventArgs e)
        {

            //button5.BackgroundImage = Properties.Resources.pause;
            if (isplaying==false)
            {
                button5.BackgroundImage = Properties.Resources.pause;
                axWindowsMediaPlayer1.Ctlcontrols.play();
                isplaying = true;
                if (songg.bored == s.song&&q==0)//q==0
                {
                   
                        axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Bored! (NINGNING Solo) - aespa.mp3";
                       // pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\Pictures\Screenshots\Screenshot 2025-10-21 191554.png");
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        q++;
                   
                }

                else if (songg.pretty == s.song&&q==0)
                {  axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\KATSEYE (캣츠아이) I'm Pretty Lyric Video.mp3";
                        pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 113923.png");
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        q++;


                }
                else if (songg.love == s.song && q==0)
                {
                    
                        axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Keyshia Cole - Love (Lyrics).mp3";
                        pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 114842.png");
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        q++;
                   
                }
            }
            else
            {
                button5.BackgroundImage = Properties.Resources.play;
                axWindowsMediaPlayer1.Ctlcontrols.pause();
                isplaying = false;
                
            }
            
        }
        bool ns=false;
        private void button6_Click(object sender, EventArgs e)
        {
            if (isplaying == true)
            {
                if (songg.bored == s.song && ns == false)
                {
                    //pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\Pictures\Screenshots\Screenshot 2025-10-21 191705.png");
                    axWindowsMediaPlayer1.URL = @"C:\\Users\\ratro\\OneDrive\\Desktop\\Billie Eilish - Bored (Official Audio) - BillieEilishVEVO.mp3\";
                    ns = true; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.bored == s.song && ns == true)
                {
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\Pictures\Screenshots\Screenshot 2025-10-21 191554.png");

                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Bored! (NINGNING Solo) - aespa.mp3";
                    ns = false; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
               else if (songg.love == s.song && ns == false)
                {
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 114904.png");
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\5 Seconds of Summer - Lover Of Mine (Official Audio).mp3";
                    ns = true; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.love == s.song && ns == true)
                {
                   pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 114842.png");
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Keyshia Cole - Love (Lyrics).mp3";
                    ns = false; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.pretty == s.song && ns == false)
                {
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 113944.png");
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Daya - Sit Still, Look Pretty (Lyrics).mp3";
                    ns = true; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.pretty == s.song && ns == true)
                {
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 113923.png");
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\KATSEYE (캣츠아이) I'm Pretty Lyric Video.mp3";
                    ns = false; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }

            }
            else
            {
                if (songg.bored == s.song && ns == false)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;


                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\Pictures\Screenshots\Screenshot 2025-10-21 191705.png");
                    axWindowsMediaPlayer1.URL = @"C:\\Users\\ratro\\OneDrive\\Desktop\\Billie Eilish - Bored (Official Audio) - BillieEilishVEVO.mp3\";
                    ns = true; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.bored == s.song && ns == true)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Bored! (NINGNING Solo) - aespa.mp3";

                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\Pictures\Screenshots\Screenshot 2025-10-21 191554.png");

                    ns = false; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.love == s.song && ns == false)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;

                     axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\5 Seconds of Summer - Lover Of Mine (Official Audio).mp3";
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 114904.png");

                    ns = true; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.love == s.song && ns == true)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;

                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Keyshia Cole - Love (Lyrics).mp3";
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 114842.png");

                    ns = false; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.pretty == s.song && ns == false)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Daya - Sit Still, Look Pretty (Lyrics).mp3";
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 113944.png");

                    ns = true; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.pretty == s.song && ns == true)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\KATSEYE (캣츠아이) I'm Pretty Lyric Video.mp3";
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 113923.png");

                    ns = false; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }

            }
            
                
               
            
        }
       // SoundPlayer player;
        Sonnngg s=new Sonnngg();
        private void button1_Click(object sender, EventArgs e)
        {
            q = 0;
            s.bo();
            
               
              
          
           
        }

        private void axWindowsMediaPlayer1_Enter(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        bool ps = false;
        private void button4_Click(object sender, EventArgs e)
        {
            if (isplaying)
            {
                if (songg.bored == s.song && ps == false)
                {
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\Pictures\Screenshots\Screenshot 2025-10-21 191554.png");
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Bored! (NINGNING Solo) - aespa.mp3";
                    ps = false;
                }
                else if (songg.bored == s.song && ps == true)
                {
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\Pictures\Screenshots\Screenshot 2025-10-21 191705.png");
                    axWindowsMediaPlayer1.URL = @"C:\\Users\\ratro\\OneDrive\\Desktop\\Billie Eilish - Bored (Official Audio) - BillieEilishVEVO.mp3\";
                    ps = true;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                }
                else if (songg.love == s.song && ps == false)
                {
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 114904.png");
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\5 Seconds of Summer - Lover Of Mine (Official Audio).mp3";
                    ps = false; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.love == s.song && ps == true)
                {
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 114842.png");
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Keyshia Cole - Love (Lyrics).mp3";
                    ps = true; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.pretty == s.song && ps == false)
                {
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 113944.png");
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Daya - Sit Still, Look Pretty (Lyrics).mp3";
                    ps = false; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.pretty == s.song && ps == true)
                {
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 113923.png");
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\KATSEYE (캣츠아이) I'm Pretty Lyric Video.mp3";
                    ps = true; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            else
            {
                if (songg.bored == s.song && ps == false)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;

                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\Pictures\Screenshots\Screenshot 2025-10-21 191554.png");
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                   
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Bored! (NINGNING Solo) - aespa.mp3";
                    

                    ps = false;
                }
                else if (songg.bored == s.song && ps == true)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;

                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\Pictures\Screenshots\Screenshot 2025-10-21 191705.png");
                    
                    axWindowsMediaPlayer1.URL = @"C:\\Users\\ratro\\OneDrive\\Desktop\\Billie Eilish - Bored (Official Audio) - BillieEilishVEVO.mp3\";
                    ps = true;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                }
                else if (songg.love == s.song && ps == false)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;

                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\5 Seconds of Summer - Lover Of Mine (Official Audio).mp3";
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 114904.png");

                    ps = false; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.love == s.song && ps == true)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;

                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Keyshia Cole - Love (Lyrics).mp3";
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 114842.png");

                    ps = true; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.pretty == s.song && ps == false)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Daya - Sit Still, Look Pretty (Lyrics).mp3";
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 113944.png");

                   ps = false; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else if (songg.pretty == s.song && ps == true)
                {
                    isplaying = true;
                    button5.BackgroundImage = Properties.Resources.pause;
                    axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\KATSEYE (캣츠아이) I'm Pretty Lyric Video.mp3";
                    pictureBox1.Image = Image.FromFile(@"C:\Users\ratro\OneDrive\Pictures\Screenshots\Screenshot 2025-11-22 113923.png");

                    ps = true; pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }

            }
            /*if (songg.love == s.song && ps == false)
            {
                axWindowsMediaPlayer1.URL = @"C:\\Users\\ratro\\OneDrive\\Desktop\\Billie Eilish - Bored (Official Audio) - BillieEilishVEVO.mp3\";
                ns = true;
            }
            else if (songg.love == s.song && ns == true)
            {
                axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Bored! (NINGNING Solo) - aespa.mp3";
                ns = false;
            }
            if (songg.pretty == s.song && ns == false)
            {
                axWindowsMediaPlayer1.URL = @"C:\\Users\\ratro\\OneDrive\\Desktop\\Billie Eilish - Bored (Official Audio) - BillieEilishVEVO.mp3\";
                ns = true;
            }
            else if (songg.pretty == s.song && ns == true)
            {
                axWindowsMediaPlayer1.URL = @"C:\Users\ratro\OneDrive\Desktop\Bored! (NINGNING Solo) - aespa.mp3";
                ns = false;
            }
        */}

        private void button2_Click(object sender, EventArgs e)

        {
            q = 0;
            s.pr();

        

        }

        private void button3_Click(object sender, EventArgs e)
        {
            q = 0;
            s.lo();
        }
    }
}
