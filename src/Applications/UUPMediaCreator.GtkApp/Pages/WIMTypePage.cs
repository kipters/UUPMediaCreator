using System;
using Gtk;

namespace UUPMediaCreator.GtkApp.Pages
{
    public class WIMTypePage : PageBase
    {
        private readonly Gtk.RadioButton _lzxRadio;
        private readonly Gtk.RadioButton _lzmsRadio;
        private readonly Gtk.RadioButton _xpressRadio;

        public WIMTypePage(IPageDelegate pageDelegate) : base(pageDelegate)
        {
            Title = "Select compression type";
            Subtitle =
                "This compression type will be used for the Windows Recovery Environment, Windows Preinstallation Environment (unless it's LZMS in which case it will fallback to LZX) and the Windows Install image.";

            _lzxRadio = new Gtk.RadioButton(null, "LZX");
            _lzmsRadio = new Gtk.RadioButton(_lzxRadio, "LZMS");
            _xpressRadio = new Gtk.RadioButton(_lzmsRadio, "ZPRESS");
            
            PackStart(_lzxRadio, false, false, 0);
            PackStart(new Gtk.Label("Default option. Provides a good balance between size, performance, and resource utilization."){Halign = Align.Start}, false, false, 5);
            PackStart(_lzmsRadio, false, false, 0);
            PackStart(new Gtk.Label("Most compact option. the resulting media size will be the smallest possible, but will use much more resources."){Halign = Align.Start}, false, false, 5);
            PackStart(_xpressRadio, false, false, 0);
            PackStart(new Gtk.Label("Quickest option. Lean on resource utilization, but will not be as compact as LZX or LZMS."){Halign = Align.Start}, false, false, 5);

        }

        public override void Presented()
        {
            PageDelegate.BackEnabled = true;
            PageDelegate.NextEnabled = true;
        }

        public override void HandleNextButton()
        {
            App.ConversionPlan.InstallationWIMMediumType =
                (_lzxRadio.Active, _lzmsRadio.Active, _xpressRadio.Active) switch
                {
                    (true, _, _) => InstallationWIMMediumType.LZX,
                    (_, true, _) => InstallationWIMMediumType.LZMS,
                    (_, _, true) => InstallationWIMMediumType.XPRESS,
                    _ => throw new NotImplementedException()
                };
            
            PageDelegate.Navigate<RecapPage>();
        }
    }
}