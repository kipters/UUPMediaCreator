using System;
using Gdk;
using Gtk;
using WrapMode = Pango.WrapMode;

namespace UUPMediaCreator.GtkApp
{
    public abstract class PageBase : Gtk.Box
    {
        protected IPageDelegate PageDelegate { get; }

        protected PageBase(IPageDelegate pageDelegate) 
            : base(Gtk.Orientation.Vertical, 5)
        {
            PageDelegate = pageDelegate;
            Margin = 5;

            TitleLabel = new Gtk.Label
                {Halign = Align.Start, Valign = Align.Start, LineWrap = true, LineWrapMode = WrapMode.Word};
            SubtitleLabel = new Gtk.Label
                {Halign = Align.Start, Valign = Align.Start, LineWrap = true, LineWrapMode = WrapMode.Word};
            
            PackStart(TitleLabel, false, false, 5);
            PackStart(SubtitleLabel, false, false, 5);
        }

        protected Gtk.Label TitleLabel { get; }
        protected Gtk.Label SubtitleLabel { get; }
        
        public string Title
        {
            get => TitleLabel.Text;
            set => TitleLabel.Text = value;
        }

        public string Subtitle
        {
            get => SubtitleLabel.Text;
            set => SubtitleLabel.Text = value;
        }

        public abstract void Presented();
        public abstract void HandleNextButton();

        public virtual void HandleBackButton()
        {
            PageDelegate.GoBack();
        }

        // protected override void OnSizeAllocated(Rectangle allocation)
        // {
        //     var width = Math.Min(700, allocation.Width);
        //     var height = Math.Max(500, allocation.Height);
        //     
        //     base.OnSizeAllocated(new Rectangle(allocation.X, allocation.Y, width, height));
        // }
    }
}