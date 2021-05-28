using System;
using System.Threading.Tasks;
using Gdk;
using Gtk;
using UUPMediaCreator.GtkApp.Pages;
using Key = Gdk.Key;

namespace UUPMediaCreator.GtkApp
{
    public class AppWindow : Gtk.ApplicationWindow, IPageDelegate
    {
        private readonly Gtk.HeaderBar _headerBar;
        private readonly Gtk.Button _cancelButton;
        private readonly Gtk.Button _backButton;
        private readonly Gtk.Button _nextButton;
        private readonly Gtk.Stack _stack;
        private PageBase _top;

        public AppWindow(Gtk.Application application) : base(application)
        {
            Resizable = false;
            ResizeWindow();
            BorderWidth = 25;
            
            _headerBar = new Gtk.HeaderBar
            {
                Title = "UUP Media Creator"
            };
            Titlebar = _headerBar;

            KeyPressEvent += HandleKeyPress; 

            _cancelButton = Gtk.Button.NewWithMnemonic("_Cancel");
            // _cancelButton.StyleContext.AddClass("destructive-action");
            _cancelButton.Clicked += (sender, args) => HandleClose();
            
            _backButton = Gtk.Button.NewWithMnemonic("_Back");
            _backButton.Clicked += (sender, args) => _top?.HandleBackButton();

            _nextButton = Gtk.Button.NewWithMnemonic("_Next");
            _nextButton.StyleContext.AddClass("suggested-action");
            _nextButton.Clicked += (sender, args) => _top?.HandleNextButton();
            
            _headerBar.PackStart(_cancelButton);
            _headerBar.PackEnd(_nextButton);
            // _headerBar.PackEnd(_backButton);

            _stack = new Gtk.Stack
            {
                Valign = Gtk.Align.Start,
                Halign = Gtk.Align.Start,
                TransitionType = Gtk.StackTransitionType.SlideLeft
            };

            Add(_stack);
            Navigate<WelcomePage>();
        }

        private void HandleKeyPress(object o, KeyPressEventArgs args)
        {
            if (args.Event.State.HasFlag(ModifierType.ControlMask) && args.Event.Key == Key.q)
            {
                HandleClose();
            }
        }

        private async void HandleClose()
        {
            var dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo,
                "Do you really want to exit the application?");
            
            var tcs = new TaskCompletionSource();
            dialog.Response += HandleCloseDialog;
            dialog.Show();

            void HandleCloseDialog(object o, ResponseArgs args)
            {
                dialog.Response -= HandleCloseDialog;
                if (args.ResponseId == ResponseType.Yes)
                {
                    Close();
                }

                dialog.Destroy();
                tcs.TrySetResult();
            }

            await tcs.Task;
        }

        public bool BackEnabled
        {
            set => _backButton.Sensitive = value;
        }
        public bool NextEnabled
        {
            set => _nextButton.Sensitive = value;
        }

        public void Navigate<TPage>() where TPage : PageBase
        {
            var widget = (PageBase) Activator.CreateInstance(typeof(TPage), new[] {this});
            _top = widget ?? throw new InvalidOperationException();
            
            widget.ShowAll();
            widget.Presented();
            _stack.Remove(_stack.VisibleChild);
            _stack.AddNamed(widget, typeof(TPage).Name);
            _stack.VisibleChild = widget;
            widget.SetSizeRequest(700, 500);
            ResizeWindow();
        }

        private void ResizeWindow()
        {
            Resize(800, 600);
        }

        public void GoBack()
        {
        }
    }

    public interface IPageDelegate
    {
        public bool BackEnabled { set; }
        public bool NextEnabled { set; }
        void Navigate<TPage>() where TPage : PageBase;
        void GoBack();
    }
}