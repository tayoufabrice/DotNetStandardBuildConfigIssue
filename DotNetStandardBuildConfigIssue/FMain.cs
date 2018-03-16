using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DotNetStandardBuildConfigIssue
{
    public partial class FMain : Form
    {
        public FMain()
        {
            InitializeComponent();
        }

        private void FMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] d = e.Data.GetFormats();
            string fn = ((string[])e.Data.GetData("FileDrop"))[0];
            string[] lines = File.ReadAllLines(fn);
            string[] txt = lines.ToArray();
            List<string> replacements = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                string l = lines[i];
                int p1 = l.IndexOf(".Ad-Hoc");
                if (p1 < 0)
                    p1 = l.IndexOf(".AppStore");
                if (p1 < 0)
                    p1 = l.IndexOf(".Release");
                if (p1 < 0)
                    continue;
                int p2 = l.IndexOf("=", p1);
                if (p2 < 0)
                    continue;
                int p3 = l.IndexOf("Debug", p2);
                if (p3 < 0)
                    continue;
                txt[i] = l.Replace("Debug", "Release");
            }
            string name = Path.GetFileNameWithoutExtension(fn);
            var fi = new FileInfo(fn);
            string folder = fi.Directory.FullName;

            string fn_backup = Path.Combine(folder, name + "_Backup" + fi.Extension);
            File.Delete(fn_backup);
            File.Copy(fn, fn_backup);

            Encoding encoding;
            using (var reader = new StreamReader(fn))
            {
                encoding = reader.CurrentEncoding;
            }
            //File.WriteAllLines(fn_backup, lines);

            File.WriteAllLines(fn, txt, encoding);
            System.Diagnostics.Process.Start(
                @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\devenv.exe", 
                string.Format( "/diff \"{0}\" \"{1}\"", fn_backup, fn));
            //string new_txt = txt.Replace("", "");
        }
        private void FMain_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Link;
        }
    }
}
