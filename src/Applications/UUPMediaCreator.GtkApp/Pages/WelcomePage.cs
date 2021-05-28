namespace UUPMediaCreator.GtkApp.Pages
{
    public class WelcomePage : PageBase
    {
        public WelcomePage(IPageDelegate pageDelegate) : base(pageDelegate)
        {
            Title = "Welcome";
            Subtitle = "This wizard will guide you through creating a Windows Medium";
        }

        public override void Presented()
        {
            PageDelegate.BackEnabled = false;
            PageDelegate.NextEnabled = true;
        }

        public override void HandleNextButton()
        {
            PageDelegate.Navigate<ArchitecturePage>();
        }
    }
}