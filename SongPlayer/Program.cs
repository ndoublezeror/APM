using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SongPlayer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Global exception handlers to capture and display unhandled errors during debugging/runtime
            Application.ThreadException += (s, e) => HandleException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                HandleException(ex);
            };

            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private static void HandleException(Exception ex)
        {
            try
            {
                var msg = ex == null ? "Unknown error" : ex.ToString();
                try
                {
                    var appFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SongPlayer");
                    if (!System.IO.Directory.Exists(appFolder)) System.IO.Directory.CreateDirectory(appFolder);
                    var log = System.IO.Path.Combine(appFolder, "error.log");
                    System.IO.File.AppendAllText(log, DateTime.Now.ToString("s") + " - " + msg + System.Environment.NewLine + Environment.NewLine);
                }
                catch { }
                System.Windows.Forms.MessageBox.Show("An unhandled exception occurred. See %AppData%\\SongPlayer\\error.log for details.\n\n" + msg, "Unhandled Exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch { }
            finally
            {
                try { Application.Exit(); } catch { }
            }
    }
}
}
