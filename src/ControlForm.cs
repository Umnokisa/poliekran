using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Poliekran
{
    class ControlForm : Form
    {
        static readonly int[] Secs = { 0, 5, 10, 15, 30, 60 };
        ShowManager mgr;
        Screen[] scr;
        ComboBox cb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        ComboBox md = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 130 };
        ComboBox sc = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 110 };
        CheckBox rl = new CheckBox { Text = "Перезапускать показ, когда файл изменился", AutoSize = true, Checked = true, Margin = new Padding(16, 7, 3, 3) };
        ListView lv = new ListView { View = View.Details, FullRowSelect = true, Dock = DockStyle.Fill, HideSelection = false };

        public ControlForm(ShowManager m)
        {
            mgr = m;
            Text = "Полиэкран";
            Width = 900; Height = 460;
            StartPosition = FormStartPosition.CenterScreen;
            try
            {
                using (var s = typeof(ControlForm).Assembly.GetManifestResourceStream("Poliekran.app.ico"))
                    if (s != null) Icon = new Icon(s);
            }
            catch (Exception ex) { L.E("Icon", ex); }

            lv.Columns.Add("Файл", 230);
            lv.Columns.Add("Монитор", 190);
            lv.Columns.Add("Слайд", 60);
            lv.Columns.Add("Статус", 160);
            lv.Columns.Add("Настройки", 220);

            md.Items.AddRange(new object[] { "Полный экран", "Окно" });
            md.SelectedIndex = 0;
            sc.Items.AddRange(Secs.Select(x => x == 0 ? "не листать" : x + " с").ToArray());
            sc.SelectedIndex = Array.IndexOf(Secs, 15);

            var top = new TableLayoutPanel { Dock = DockStyle.Top, Height = 34, ColumnCount = 4 };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < 3; i++) top.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var add = new Button { Text = "Запустить файл…", AutoSize = true };
            add.Click += (s, e) => Add();
            var rs = new Button { Text = "Обновить мониторы", AutoSize = true };
            rs.Click += (s, e) => LoadScreens();
            top.Controls.Add(cb, 0, 0);
            top.Controls.Add(md, 1, 0);
            top.Controls.Add(add, 2, 0);
            top.Controls.Add(rs, 3, 0);

            var opt = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 34 };
            opt.Controls.Add(new Label { Text = "Слайды без времени смены листать через:", AutoSize = true, Margin = new Padding(3, 8, 3, 3) });
            opt.Controls.Add(sc);
            opt.Controls.Add(rl);

            var bar = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 38 };
            bar.Controls.Add(Btn("< Назад", mgr.Prev));
            bar.Controls.Add(Btn("Вперёд >", mgr.Next));
            bar.Controls.Add(Btn("Перезапустить", mgr.Restart));
            bar.Controls.Add(Btn("Остановить", mgr.Stop));
            var all = new Button { Text = "Остановить все", AutoSize = true };
            all.Click += (s, e) => mgr.StopShows();
            bar.Controls.Add(all);
            var ms = new CheckBox { Text = "Esc в показе останавливает его", AutoSize = true, Checked = mgr.ManualStops, Margin = new Padding(12, 7, 3, 3) };
            ms.CheckedChanged += (s, e) => mgr.ManualStops = ms.Checked;
            bar.Controls.Add(ms);

            Controls.Add(lv);
            Controls.Add(opt);
            Controls.Add(top);
            Controls.Add(bar);

            LoadScreens();
            mgr.Changed += Rebuild;
            FormClosing += (s, e) => { if (e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; Hide(); } };
            Rebuild();
        }

        static string Desc(Screen s, int i)
        {
            var b = s.Bounds;
            return $"{i + 1}: {b.Width}×{b.Height}, позиция {b.X},{b.Y}{(s.Primary ? ", основной" : "")}";
        }

        void LoadScreens()
        {
            scr = Screen.AllScreens;
            cb.Items.Clear();
            for (int i = 0; i < scr.Length; i++) cb.Items.Add(Desc(scr[i], i));
            cb.SelectedIndex = scr.Length > 1 ? 1 : 0;
        }

        string ScrName(Screen s)
        {
            int i = Array.FindIndex(scr, x => x.DeviceName == s.DeviceName);
            return i >= 0 ? Desc(scr[i], i) : s.DeviceName;
        }

        void Add()
        {
            using (var d = new OpenFileDialog { Filter = "PowerPoint|*.pptx;*.pptm;*.ppt;*.ppsx;*.ppsm;*.pps", Multiselect = false })
            {
                if (d.ShowDialog(this) != DialogResult.OK) return;
                try { mgr.Start(d.FileName, scr[cb.SelectedIndex], md.SelectedIndex == 0, Secs[sc.SelectedIndex], rl.Checked); }
                catch (Exception ex) { MessageBox.Show(this, ex.Message, "Не удалось запустить", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        Button Btn(string t, Action<Show> a)
        {
            var b = new Button { Text = t, AutoSize = true };
            b.Click += (s, e) =>
            {
                try
                {
                    var sel = lv.SelectedItems.Cast<ListViewItem>().Select(i => (Show)i.Tag).ToList();
                    if (sel.Count == 0 && mgr.Shows.Count == 1) sel.Add(mgr.Shows[0]);
                    if (sel.Count == 0) { MessageBox.Show(this, "Выберите показ в списке.", "Полиэкран"); return; }
                    foreach (var sh in sel) a(sh);
                }
                catch (Exception ex) { MessageBox.Show(this, ex.Message, "Полиэкран"); }
            };
            return b;
        }

        void Rebuild()
        {
            if (IsDisposed) return;
            var shows = mgr.Shows.ToList();
            bool same = lv.Items.Count == shows.Count;
            for (int i = 0; same && i < shows.Count; i++) same = lv.Items[i].Tag == shows[i];
            if (same)
            {
                for (int i = 0; i < shows.Count; i++)
                {
                    var r = Row(shows[i]);
                    for (int k = 0; k < r.Length; k++)
                        if (lv.Items[i].SubItems[k].Text != r[k]) lv.Items[i].SubItems[k].Text = r[k];
                }
                return;
            }
            var sel = new HashSet<Show>(lv.SelectedItems.Cast<ListViewItem>().Select(i => (Show)i.Tag));
            lv.BeginUpdate();
            lv.Items.Clear();
            foreach (var sh in shows) lv.Items.Add(new ListViewItem(Row(sh)) { Tag = sh, Selected = sel.Contains(sh) });
            lv.EndUpdate();
        }

        string[] Row(Show sh)
        {
            int pos = mgr.Pos(sh);
            string slide = pos > 0 ? pos + "/" + mgr.Count(sh) : "-";
            string st = sh.Dead ? "упал, повтор через " + Math.Max(0, 60 - (int)(DateTime.Now - sh.DeadAt).TotalSeconds) + " с"
                : sh.Pending || sh.Win == null ? "перезапуск" : "идёт";
            string cfg = (sh.Full ? "полный экран" : "окно") + ", " + (sh.Sec == 0 ? "не листать" : sh.Sec + " с") + (sh.Reload ? ", следит за файлом" : "");
            return new[] { sh.Title, ScrName(sh.Scr), slide, st, cfg };
        }
    }
}
