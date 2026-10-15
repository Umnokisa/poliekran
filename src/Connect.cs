using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Poliekran
{
    [ComImport, Guid("B65AD801-ABAF-11D0-BB8B-00A0C90F2744"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IDTExtensibility2
    {
        [DispId(1)] void OnConnection([In, MarshalAs(UnmanagedType.IDispatch)] object app, [In] int mode, [In, MarshalAs(UnmanagedType.IDispatch)] object inst, [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref Array custom);
        [DispId(2)] void OnDisconnection([In] int mode, [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref Array custom);
        [DispId(3)] void OnAddInsUpdate([In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref Array custom);
        [DispId(4)] void OnStartupComplete([In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref Array custom);
        [DispId(5)] void OnBeginShutdown([In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref Array custom);
    }

    [ComImport, Guid("000C0396-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IRibbonExtensibility
    {
        [DispId(1)] [return: MarshalAs(UnmanagedType.BStr)] string GetCustomUI([In, MarshalAs(UnmanagedType.BStr)] string id);
    }

    [ComVisible(true), Guid("CFE2C7D5-5DBE-4027-903C-A3FB8B5B3890"), ProgId("Poliekran.Connect"), ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class Connect : IDTExtensibility2, IRibbonExtensibility
    {
        static readonly string Ver = typeof(Connect).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "?";
        static bool hooked;
        ShowManager mgr;
        ControlForm form;

        public Connect()
        {
            if (hooked) return;
            hooked = true;
            try { AppDomain.CurrentDomain.UnhandledException += (s, e) => L.W("UNHANDLED: " + e.ExceptionObject); }
            catch (Exception ex) { L.E("ctor", ex); }
        }

        public void OnConnection(object app, int mode, object inst, ref Array custom)
        {
            L.W("OnConnection v" + Ver + " mode=" + mode + " clr=" + Environment.Version + " x64=" + Environment.Is64BitProcess);
            try { mgr = new ShowManager(app); }
            catch (Exception ex) { L.E("OnConnection", ex); }
        }

        public void OnDisconnection(int mode, ref Array custom)
        {
            L.W("OnDisconnection mode=" + mode);
            try { mgr?.StopAll(); } catch (Exception ex) { L.E("OnDisconnection.Stop", ex); }
            try { form?.Dispose(); } catch (Exception ex) { L.E("OnDisconnection.Form", ex); }
            mgr = null; form = null;
        }

        public void OnAddInsUpdate(ref Array custom) { }
        public void OnStartupComplete(ref Array custom) { L.W("OnStartupComplete"); }

        public void OnBeginShutdown(ref Array custom)
        {
            L.W("OnBeginShutdown");
            try { mgr?.StopShows(); } catch (Exception ex) { L.E("OnBeginShutdown", ex); }
        }

        public string GetCustomUI(string id)
        {
            L.W("GetCustomUI " + id);
            return
@"<customUI xmlns=""http://schemas.microsoft.com/office/2009/07/customui"">
  <ribbon>
    <tabs>
      <tab idMso=""TabSlideShow"">
        <group id=""gPoliekran"" label=""Полиэкран"">
          <button id=""bPoliekran"" label=""Полиэкран"" size=""large"" imageMso=""SlideShowFromBeginning"" onAction=""OnOpen""/>
        </group>
      </tab>
    </tabs>
  </ribbon>
</customUI>";
        }

        public void OnOpen(object c)
        {
            L.W("OnOpen");
            try
            {
                if (mgr == null) { MessageBox.Show("Надстройка не запустилась. Подробности в журнале:\n" + L.File, "Полиэкран"); return; }
                if (form == null || form.IsDisposed) form = new ControlForm(mgr);
                form.Show();
                form.Activate();
            }
            catch (Exception ex) { L.E("OnOpen", ex); MessageBox.Show(ex.Message, "Полиэкран"); }
        }
    }
}
