using System;
using Gtk;

namespace UUPMediaCreator.GtkApp.Pages
{
    public class RecapPage : PageBase
    {
        private readonly Entry _pathEntry;

        public RecapPage(IPageDelegate pageDelegate) : base(pageDelegate)
        {
            Title = "To Recap";
            Subtitle =
                "The tool will proceed to create an ISO media using the details you previously selected. If any information is incorrect, go back, and change it before clicking next. Once you click next, you cannot go back.";

            var plan = App.ConversionPlan;
            
            PushLabel("Getting", plan.BuildString ?? plan.UpdateData.Xml.LocalizedProperties.Title ?? "");
            PushLabel("Edition", plan.Edition);
            PushLabel("In the following language", plan.LanguageTitle);
            PushLabel("For the following architecture", plan.MachineType.ToString());
            PushLabel("As an", plan.MediumType.ToString());
            PushLabel("Using the following compression", plan.InstallationWIMMediumType.ToString());

            var saveLabel = new Label("Where do you want to save your ISO image?")
            {
                Halign = Align.Start,
                Valign = Align.Start
            };

            _pathEntry = new Entry
            {
                PlaceholderText = "Path to the resulting ISO image...",
                Valign = Align.Start,
                Halign = Align.Start
            };
            _pathEntry.Changed += OnPathChanged;

            var browseButton = new Button
            {
                Label = "Browse"
            };
            browseButton.Clicked += OnBrowseClicked;

            PackStart(saveLabel, false, false, 0);
            PackStart(_pathEntry, false, false, 0);
            PackStart(browseButton, false, false, 0);
        }

        private void OnBrowseClicked(object? sender, EventArgs e)
        {
            var dialog = new FileChooserDialog("Save file", (Window) PageDelegate, FileChooserAction.Save,
                "_Cancel", ResponseType.Cancel,
                "_Save", ResponseType.Accept)
            {
                DoOverwriteConfirmation = true,
                CurrentName = "Windows.iso",
                
            };
            if ((ResponseType) dialog.Run() == ResponseType.Accept)
            {
                _pathEntry.Text = dialog.Filename;
            }
            dialog.Destroy();
        }

        private void OnPathChanged(object? sender, EventArgs e)
        {
            var path = _pathEntry.Text;
            App.ConversionPlan.ISOPath = path;
            PageDelegate.NextEnabled = !string.IsNullOrEmpty(path);
        }

        private void PushLabel(string title, string text)
        {
            var label = new Label($"**{title}**: {text}")
            {
                Halign = Align.Start,
                Valign = Align.Start,
                UseMarkup = true
            };
            PackStart(label, false, false, 0);
        }

        public override void Presented()
        {
            PageDelegate.BackEnabled = true;
            PageDelegate.NextEnabled = false;
        }

        public override void HandleNextButton()
        {
        }
    }
}