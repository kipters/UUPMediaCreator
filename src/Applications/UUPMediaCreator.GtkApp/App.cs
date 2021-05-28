using System;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsUpdateLib;

namespace UUPMediaCreator.GtkApp
{
    public class App : Gtk.Application
    {
        private Gtk.Window _window;
        internal static ConversionPlan ConversionPlan = new();

        public App() : base("dev.kipters.uupmediacreator", GLib.ApplicationFlags.None)
        {
            SynchronizationContext.SetSynchronizationContext(new GLib.GLibSynchronizationContext());
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            _window ??= RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new AppWindow(this)
                : new UnsupportedPlatformWindow(this);
                _window.ShowAll();
            _window.Present();
        }
    }
    
    public enum MediumType
    {
        WindowsInstallationMedium,
        WindowsFeatureOnDemandMedium,
        WindowsLanguagePackMedium
    }

    public enum InstallationMediumType
    {
        ISO,
        InstallWIM,
        BootWIM,
        VHD
    }

    public enum InstallationWIMMediumType
    {
        XPRESS,
        LZX,
        LZMS
    }

    public class ConversionPlan
    {
        public UpdateData UpdateData { get; set; }
        public string Language { get; set; }
        public string LanguageTitle { get; set; }
        public Uri FlagUri { get; set; }
        public string Edition { get; set; }
        public string BuildString { get; set; }

        public MachineType MachineType { get; set; }

        public MediumType MediumType { get; set; }
        public InstallationMediumType InstallationMediumType { get; set; }
        public InstallationWIMMediumType InstallationWIMMediumType { get; set; }
        
        public string TmpOutputFolder { get; set; }

        public string ISOPath { get; set; }
    }
}