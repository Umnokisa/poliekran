using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Poliekran
{
    class Show
    {
        public string Path, Tmp;
        public Screen Scr;
        public bool Full, Reload;
        public int Sec;
        public dynamic Pres, Win;
        public IntPtr Hwnd;
        public Guard G;
        public HashSet<IntPtr> Before;
        public int Last = 1, Miss, PtrErr, ReloadErr;
        public bool Stopping, Dead, Pending;
        public DateTime DeadAt;
        public readonly List<DateTime> Hist = new List<DateTime>();
        public DateTime Stamp, SeenT;
        public long Size = -1, SeenL = -1;
        public Task<Probe> Job;
        public string Title => System.IO.Path.GetFileName(Path);
    }

    class Probe
    {
        public DateTime T;
        public long N;
        public string Tmp;
    }

    class ShowManager
    {
        const int T = -1, F = 0, ppPointerArrow = 1, ppShowTypeSpeaker = 1, ppShowTypeWindow = 2, ppShowAll = 1, ppUseSlideTimings = 2, ppAlertsNone = 1;
        const int MaxFails = 5, FailWindow = 60, RetryPause = 60;
        static readonly string TmpDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Poliekran");
        dynamic app;
        public readonly List<Show> Shows = new List<Show>();
        public event Action Changed;
        public bool ManualStops = true;
        Timer hb;
        int tick;
        bool inHb, busy;
        Show launching;

        public ShowManager(object a)
        {
            app = a;
            try { if (Directory.Exists(TmpDir)) foreach (var f in Directory.GetFiles(TmpDir)) try { File.Delete(f); } catch { } } catch { }
            Hook.Input += OnInput;
            Hook.On();
            hb = new Timer { Interval = 500 };
            hb.Tick += (s, e) => Safe("Heartbeat", Heartbeat);
            hb.Start();
        }

        void OnInput(IntPtr root, bool exitKey)
        {
            foreach (var sh in Shows)
                if (sh.G != null && sh.Hwnd == root)
                {
                    if (exitKey) sh.G.ExitKey = DateTime.Now; else sh.G.RClick = DateTime.Now;
                }
        }

        bool Blocked(Show sh) => busy || Shows.Any(x => x != sh && x.Win != null && x.Hwnd == IntPtr.Zero);

        public Show Start(string path, Screen scr, bool full, int sec, bool reload)
        {
            if (Blocked(null)) throw new InvalidOperationException(UiText.PreviousShowStarting);
            L.W("Start " + path + " on " + scr.DeviceName + " " + scr.Bounds + (full ? " fullscreen" : " window") + " sec=" + sec + " reload=" + reload);
            var sh = new Show { Path = path, Scr = scr, Full = full, Sec = sec, Reload = reload };
            try
            {
                busy = true;
                try { Fresh(sh); } finally { busy = false; }
                Shows.Add(sh);
                Launch(sh, 1);
            }
            catch (Exception ex)
            {
                L.E("Start", ex);
                Shows.Remove(sh);
                var w = sh.Win;
                Drop(sh);
                try { w?.View.Exit(); } catch { }
                Close(sh);
                throw;
            }
            Fire();
            return sh;
        }

        static string Copy(string src)
        {
            Directory.CreateDirectory(TmpDir);
            var dst = System.IO.Path.Combine(TmpDir, Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(src));
            File.Copy(src, dst);
            return dst;
        }

        static readonly List<string> trash = new List<string>();

        static void Del(string f)
        {
            if (f == null) return;
            try { if (File.Exists(f)) File.Delete(f); }
            catch { lock (trash) if (!trash.Contains(f)) trash.Add(f); }
        }

        static void Sweep()
        {
            string[] a;
            lock (trash) { a = trash.ToArray(); trash.Clear(); }
            foreach (var f in a) Del(f);
        }

        void Fresh(Show sh)
        {
            try { var fi = new FileInfo(sh.Path); sh.Stamp = fi.LastWriteTimeUtc; sh.Size = fi.Length; } catch { sh.Size = -1; }
            sh.SeenL = -1;
            var tmp = Copy(sh.Path);
            try { sh.Pres = OpenFile(tmp, sh.Sec); }
            catch { Del(tmp); throw; }
            Del(sh.Tmp);
            sh.Tmp = tmp;
        }

        dynamic OpenFile(string file, int sec)
        {
            object old = null;
            try { old = app.DisplayAlerts; app.DisplayAlerts = ppAlertsNone; } catch { }
            dynamic p;
            try { p = app.Presentations.Open(file, T, T, F); }
            finally { if (old != null) try { app.DisplayAlerts = old; } catch { } }
            if (sec > 0)
            {
                try
                {
                    int n = (int)p.Slides.Count, k = 0;
                    if (n > 1)
                        for (int i = 1; i <= n; i++)
                        {
                            var tr = p.Slides.Item(i).SlideShowTransition;
                            if ((int)tr.AdvanceOnTime == F) { tr.AdvanceOnTime = T; tr.AdvanceTime = (float)sec; k++; }
                        }
                    if (k > 0) L.W("Timing " + sec + "s set on " + k + " of " + n + " slides");
                }
                catch (Exception ex) { L.E("Timing", ex); }
            }
            return p;
        }

        void Close(Show sh)
        {
            try { if (sh.Pres != null) { sh.Pres.Saved = T; sh.Pres.Close(); } } catch { }
            sh.Pres = null;
            Del(sh.Tmp);
            sh.Tmp = null;
        }

        void EnsurePres(Show sh)
        {
            try { if (sh.Pres != null && (int)sh.Pres.Slides.Count >= 0) return; } catch { }
            L.W("Reopen " + sh.Path);
            try { if (sh.Pres != null) { sh.Pres.Saved = T; sh.Pres.Close(); } } catch { }
            sh.Pres = null;
            if (sh.Tmp != null && File.Exists(sh.Tmp)) sh.Pres = OpenFile(sh.Tmp, sh.Sec);
            else Fresh(sh);
        }

        void Launch(Show sh, int slide)
        {
            sh.Pending = true;
            busy = true;
            launching = sh;
            try
            {
                EnsurePres(sh);
                var ss = sh.Pres.SlideShowSettings;
                ss.ShowType = sh.Full ? ppShowTypeSpeaker : ppShowTypeWindow;
                if (sh.Full) { try { ss.ShowPresenterView = F; } catch (Exception ex) { L.W("ShowPresenterView: " + ex.Message); } }
                ss.ShowScrollbar = F;
                ss.LoopUntilStopped = T;
                ss.RangeType = ppShowAll;
                ss.AdvanceMode = ppUseSlideTimings;
                Drop(sh);
                sh.Before = new HashSet<IntPtr>(W.Tops());
                sh.Miss = 0;
                sh.Win = ss.Run();
                if (!sh.Stopping) Attach(sh);
                if (sh.Stopping)
                {
                    L.W("Stopped while starting: " + sh.Title);
                    var w = sh.Win;
                    Drop(sh);
                    try { w?.View.Exit(); } catch { }
                    Close(sh);
                    return;
                }
                if (slide > 1 && slide <= Count(sh)) { try { sh.Win.View.GotoSlide(slide); } catch (Exception ex) { L.E("GotoSlide", ex); } }
                Pointer(sh);
            }
            finally { sh.Pending = false; busy = false; launching = null; }
        }

        void Pointer(Show sh)
        {
            try { if ((int)sh.Win.View.PointerType != ppPointerArrow) sh.Win.View.PointerType = ppPointerArrow; }
            catch (Exception ex) { if (sh.PtrErr++ < 3) L.W("PointerType: " + ex.Message); }
            W.CursorOn();
        }

        bool Attach(Show sh)
        {
            var h = Find(sh);
            if (h == IntPtr.Zero) { if (sh.Miss++ < 3) L.W("Attach: window not found yet"); return false; }
            sh.Hwnd = h;
            sh.Before = null;
            sh.Miss = 0;
            sh.G = new Guard(h, sh.Scr);
            sh.G.Fit();
            L.W("Attached hwnd=" + h + " class=" + W.Cls(h) + " title=" + W.Txt(h));
            return true;
        }

        static bool HasName(string title, string nm)
        {
            int i = title.IndexOf(nm, StringComparison.OrdinalIgnoreCase);
            while (i >= 0)
            {
                int e = i + nm.Length;
                if (e >= title.Length || !char.IsDigit(title[e])) return true;
                i = title.IndexOf(nm, i + 1, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        IntPtr Find(Show sh)
        {
            try
            {
                var h = new IntPtr((int)sh.Win.HWND);
                if (h != IntPtr.Zero && W.IsWindow(h)) return W.GetAncestor(h, W.GA_ROOT);
            }
            catch (Exception ex) { if (sh.Miss < 3) L.W("HWND unavailable: " + ex.Message); }
            if (sh.Before == null) return IntPtr.Zero;
            var used = new HashSet<IntPtr>(Shows.Where(x => x != sh).Select(x => x.Hwnd));
            string cls = sh.Full ? "screenClass" : "PPTFrameClass";
            var c = W.Tops().Where(h => !sh.Before.Contains(h) && !used.Contains(h) && W.IsWindowVisible(h) && W.Cls(h) == cls).ToList();
            if (c.Count == 0) return IntPtr.Zero;
            if (c.Count == 1) return c[0];
            string nm = "";
            try { nm = System.IO.Path.GetFileNameWithoutExtension((string)sh.Pres.Name); } catch { }
            var byName = nm.Length > 0 ? c.Where(h => HasName(W.Txt(h), nm)).ToList() : new List<IntPtr>();
            if (sh.Miss < 3) L.W("  " + c.Count + " candidates, " + byName.Count + " match '" + nm + "'");
            if (byName.Count == 1) return byName[0];
            return sh.Miss >= 4 ? c[0] : IntPtr.Zero;
        }

        void Drop(Show sh)
        {
            sh.G?.Release(); sh.G = null;
            sh.Win = null; sh.Hwnd = IntPtr.Zero; sh.Before = null;
        }

        int Slide(Show sh)
        {
            if (sh.Win == null) return 0;
            try { return (int)sh.Win.View.Slide.SlideIndex; } catch { }
            try { return (int)sh.Win.View.CurrentShowPosition; } catch { }
            return 0;
        }

        string Why(Show sh)
        {
            if (sh.Win == null && sh.Hwnd == IntPtr.Zero) return "no show";
            if (sh.Hwnd == IntPtr.Zero)
            {
                if (busy) return null;
                if (Attach(sh)) return null;
                return sh.Miss > 10 ? "window never appeared" : null;
            }
            if (!W.IsWindow(sh.Hwnd)) return "window destroyed";
            int i = Slide(sh);
            if (i > 0 && i != sh.Last) { sh.Last = i; Fire(); }
            return null;
        }

        void Heartbeat()
        {
            if (inHb) return;
            inHb = true;
            try
            {
                tick++;
                bool enf = tick % 4 == 0, files = tick % 10 == 0;
                if (tick % 20 == 0) Sweep();
                if (Shows.Count > 0) W.CursorOn();
                foreach (var sh in Shows.ToList())
                {
                    if (sh.Stopping || sh.Pending || !Shows.Contains(sh)) continue;
                    if (sh.Dead)
                    {
                        if ((DateTime.Now - sh.DeadAt).TotalSeconds >= RetryPause) { L.W("Retry after pause: " + sh.Title); sh.Dead = false; sh.Hist.Clear(); Revive(sh); }
                        else if (tick % 2 == 0) Fire();
                        continue;
                    }
                    var why = Why(sh);
                    if (why == null)
                    {
                        if (enf) { sh.G?.Enforce(); Pointer(sh); }
                        if (sh.Job != null || files) CheckFile(sh);
                        continue;
                    }
                    var g = sh.G;
                    bool user = ManualStops && g != null && why == "window destroyed" &&
                        ((DateTime.Now - g.ExitKey).TotalSeconds < 5 || (DateTime.Now - g.RClick).TotalSeconds < 15);
                    L.W("Show lost (" + why + (user ? ", ended by user" : "") + "): " + sh.Title);
                    if (user) Stop(sh); else Lost(sh);
                }
            }
            finally { inHb = false; }
        }

        void Lost(Show sh)
        {
            sh.Pending = true;
            var w = sh.Win;
            Drop(sh);
            try { w?.View.Exit(); } catch { }
            var now = DateTime.Now;
            sh.Hist.RemoveAll(t => (now - t).TotalSeconds > FailWindow);
            if (sh.Hist.Count >= MaxFails)
            {
                L.W("Gave up after " + sh.Hist.Count + " restarts, retry in " + RetryPause + "s: " + sh.Title);
                sh.Dead = true; sh.DeadAt = now; sh.Pending = false;
                Fire();
                return;
            }
            int d = sh.Hist.Count == 0 ? 300 : Math.Min(2000 * sh.Hist.Count, 15000);
            Fire();
            Later(d, () => Revive(sh));
        }

        void Revive(Show sh)
        {
            sh.Pending = false;
            if (sh.Stopping || !Shows.Contains(sh)) { Close(sh); return; }
            if (sh.Dead) return;
            if (Blocked(sh)) { sh.Pending = true; Later(500, () => Revive(sh)); return; }
            sh.Hist.Add(DateTime.Now);
            L.W("Revive " + sh.Title + " slide=" + sh.Last + " n=" + sh.Hist.Count);
            try { Launch(sh, sh.Last); }
            catch (Exception ex) { L.E("Revive", ex); Lost(sh); }
            Fire();
        }

        void CheckFile(Show sh)
        {
            if (!sh.Reload) return;
            if (sh.Job == null)
            {
                string p = sh.Path;
                DateTime st = sh.Stamp, seenT = sh.SeenT;
                long sz = sh.Size, seenL = sh.SeenL;
                sh.Job = Task.Run(() =>
                {
                    var fi = new FileInfo(p);
                    if (!fi.Exists) return null;
                    var r = new Probe { T = fi.LastWriteTimeUtc, N = fi.Length };
                    if (sz >= 0 && r.T == st && r.N == sz) return r;
                    if (sz >= 0 && r.T == seenT && r.N == seenL)
                    {
                        try { r.Tmp = Copy(p); }
                        catch { r.Tmp = null; }
                    }
                    return r;
                });
                return;
            }
            if (!sh.Job.IsCompleted) return;
            var j = sh.Job;
            sh.Job = null;
            if (j.IsFaulted || j.Result == null) return;
            var q = j.Result;
            if (sh.Size < 0) { sh.Stamp = q.T; sh.Size = q.N; Del(q.Tmp); return; }
            if (q.T == sh.Stamp && q.N == sh.Size) { sh.SeenL = -1; Del(q.Tmp); return; }
            if (q.Tmp == null) { sh.SeenT = q.T; sh.SeenL = q.N; return; }
            if (q.T != sh.SeenT || q.N != sh.SeenL || sh.Pending || sh.Stopping || !Shows.Contains(sh) || Blocked(sh)) { Del(q.Tmp); return; }
            DoReload(sh, q);
        }

        void DoReload(Show sh, Probe q)
        {
            dynamic np;
            sh.Pending = true;
            busy = true;
            try { np = OpenFile(q.Tmp, sh.Sec); }
            catch (Exception ex)
            {
                Del(q.Tmp);
                if (sh.ReloadErr++ < 3) L.E("Reload open", ex);
                return;
            }
            finally { busy = false; sh.Pending = false; }
            if (sh.Stopping || !Shows.Contains(sh))
            {
                try { np.Saved = T; np.Close(); } catch { }
                Del(q.Tmp);
                return;
            }
            L.W("File changed, reloading: " + sh.Title);
            sh.ReloadErr = 0;
            sh.Pending = true;
            var w = sh.Win;
            Drop(sh);
            try { w?.View.Exit(); } catch { }
            Close(sh);
            sh.Pres = np;
            sh.Tmp = q.Tmp;
            sh.Stamp = q.T; sh.Size = q.N; sh.SeenL = -1;
            try
            {
                int c = Count(sh);
                Launch(sh, sh.Last <= c ? sh.Last : 1);
            }
            catch (Exception ex) { L.E("Reload", ex); Lost(sh); }
            Fire();
        }

        public int Pos(Show sh) => Slide(sh);

        public int Count(Show sh)
        {
            try { return (int)sh.Pres.Slides.Count; } catch { return 0; }
        }

        public void Next(Show sh) => Nav(sh, true);
        public void Prev(Show sh) => Nav(sh, false);

        void Nav(Show sh, bool fwd)
        {
            if (sh.Win == null) return;
            try { if (fwd) sh.Win.View.Next(); else sh.Win.View.Previous(); return; }
            catch (Exception ex) { L.W("Nav via COM failed: " + ex.Message); }
            var h = sh.G?.Pane ?? IntPtr.Zero;
            if (h == IntPtr.Zero) h = sh.Hwnd;
            if (h == IntPtr.Zero) return;
            var k = new IntPtr(fwd ? W.VK_RIGHT : W.VK_LEFT);
            W.PostMessage(h, W.WM_KEYDOWN, k, new IntPtr(1));
            W.PostMessage(h, W.WM_KEYUP, k, new IntPtr(unchecked((int)0xC0000001)));
        }

        public void Restart(Show sh)
        {
            if (sh.Pending || Blocked(sh)) throw new InvalidOperationException(UiText.ShowRestarting(sh.Title));
            L.W("Restart " + sh.Title);
            sh.Dead = false;
            sh.Hist.Clear();
            if (sh.Win != null)
            {
                sh.Stopping = true;
                var w = sh.Win;
                Drop(sh);
                try { w.View.Exit(); } catch { }
                sh.Stopping = false;
            }
            Revive(sh);
        }

        public void Stop(Show sh)
        {
            L.W("Stop " + sh.Title);
            sh.Stopping = true;
            Shows.Remove(sh);
            var j = sh.Job;
            sh.Job = null;
            j?.ContinueWith(t => { if (t.Status == TaskStatus.RanToCompletion && t.Result != null) Del(t.Result.Tmp); });
            if (busy && launching == sh) { Fire(); return; }
            var w = sh.Win;
            Drop(sh);
            try { w?.View.Exit(); } catch { }
            Close(sh);
            Fire();
        }

        public void StopShows()
        {
            foreach (var s in Shows.ToList()) Stop(s);
        }

        public void StopAll()
        {
            if (hb != null) { hb.Stop(); hb.Dispose(); hb = null; }
            StopShows();
            Hook.Input -= OnInput;
            Hook.Off();
            app = null;
        }

        void Fire() { try { Changed?.Invoke(); } catch (Exception ex) { L.E("Changed", ex); } }

        static void Safe(string n, Action a) { try { a(); } catch (Exception ex) { L.E(n, ex); } }

        static void Later(int ms, Action a)
        {
            var t = new Timer { Interval = ms };
            t.Tick += (s, e) => { t.Stop(); t.Dispose(); Safe("Later", a); };
            t.Start();
        }
    }
}
