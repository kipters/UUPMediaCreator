using System;
using WindowsUpdateLib;
using Gtk;

namespace UUPMediaCreator.GtkApp.Pages
{
    public class ArchitecturePage : PageBase
    {
        private readonly RadioButton _amd64Radio;
        private readonly RadioButton _arm64Radio;
        private readonly RadioButton _x86Radio;

        public ArchitecturePage(IPageDelegate pageDelegate) : base(pageDelegate)
        {
            Title = "First, let's select the architecture";
            Subtitle = "The selected architecture will be used for the final medium";

            _amd64Radio = new Gtk.RadioButton(null, "x64 (x86__64 / AMD64)");
            _arm64Radio = new Gtk.RadioButton(_amd64Radio, "a64 (AArch64 / ARM64)");
            _x86Radio = new Gtk.RadioButton(_arm64Radio, "x86 (i386)");

            PackStart(_amd64Radio, false, false, 0);
            PackStart(
                new Gtk.Label("This architecture is most commonly used on modern Laptops, Tablets and Desktops.")
                    {Halign = Align.Start}, false, false, 5);
            PackStart(_arm64Radio, false, false, 0);
            PackStart(
                new Gtk.Label(
                        "This architecture is most commonly used on always connected Laptops, Tablets and Phones.")
                    {Halign = Align.Start}, false, false, 5);
            PackStart(_x86Radio, false, false, 0);
            PackStart(
                new Gtk.Label("This architecture is most commonly used on older Laptops and Desktops.")
                    {Halign = Align.Start}, false, false, 5);
        }

        public override void Presented()
        {
            PageDelegate.BackEnabled = true;
            PageDelegate.NextEnabled = true;
        }

        public override void HandleNextButton()
        {
            App.ConversionPlan.MachineType = (_amd64Radio.Active, _arm64Radio.Active, _x86Radio.Active) switch
            {
                (true, _, _) => MachineType.amd64,
                (_, true, _) => MachineType.arm64,
                (_, _, true) => MachineType.x86,
                _ => throw new NotImplementedException()
            };
            
            PageDelegate.Navigate<BuildPage>();
        }
    }
}