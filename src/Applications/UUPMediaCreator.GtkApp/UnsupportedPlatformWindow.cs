namespace UUPMediaCreator.GtkApp
{
    public class UnsupportedPlatformWindow : Gtk.ApplicationWindow
    {
        public UnsupportedPlatformWindow(Gtk.Application application) : base(application)
        {
            BorderWidth = 25;
            SetSizeRequest(400, 300);
            Resizable = false;
            
            var header = new Gtk.HeaderBar
            {
                Title = "Unsupported platform",
                ShowCloseButton = false
            };
            Titlebar = header;

            var errorLabel = new Gtk.Label("Sorry, only Linux is supported at the moment.")
            {
                Valign = Gtk.Align.Center,
                Halign = Gtk.Align.Center
            };
            
            Add(errorLabel);

            var exitButton = Gtk.Button.NewWithMnemonic("_Close");
            exitButton.StyleContext.AddClass("suggested-action");
            exitButton.Clicked += (sender, args) => Close();
            header.PackEnd(exitButton);
        }
    }
}