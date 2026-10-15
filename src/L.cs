using System;
using System.IO;

namespace Poliekran
{
    static class L
    {
        static readonly string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Poliekran");
        public static readonly string File = Path.Combine(dir, "log.txt");
        static readonly object k = new object();

        public static void W(string s)
        {
            try
            {
                lock (k)
                {
                    Directory.CreateDirectory(dir);
                    var fi = new FileInfo(File);
                    if (fi.Exists && fi.Length > 1 << 20) fi.Delete();
                    System.IO.File.AppendAllText(File, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "  " + s + Environment.NewLine);
                }
            }
            catch { }
        }

        public static void E(string where, Exception e) => W("ERROR " + where + ": " + e);
    }
}
