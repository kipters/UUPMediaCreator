using System;
using System.Linq;
using System.Threading.Tasks;
using WindowsUpdateLib;

namespace UUPMediaCreator.GtkApp.Pages
{
    public class LanguagePage : PageBase
    {
        private readonly Gtk.Spinner _progress;
        private readonly Gtk.ListStore _store;
        private readonly Gtk.TreeView _list;
        private readonly Gtk.Label _descriptionLabel;
        private BuildFetcher.AvailableBuildLanguages[] _updates;
        private BuildFetcher.AvailableBuildLanguages _selectedUpdate;

        public LanguagePage(IPageDelegate pageDelegate) : base(pageDelegate)
        {
            Title = "Ciao, Hola, Bonjour, Hello oder Hallo?";
            Subtitle = "What language do you speak?";

            _progress = new Gtk.Spinner();
            CenterWidget = _progress;

            _store = new Gtk.ListStore(typeof(string), typeof(string), typeof(string));
            _list = new Gtk.TreeView(_store);
            _descriptionLabel = new Gtk.Label
            {
                Halign = Gtk.Align.Start,
                SingleLineMode = false
            };

            _list.AppendColumn(new Gtk.TreeViewColumn("_", new Gtk.CellRendererText(), "text", 0));
            _list.AppendColumn(new Gtk.TreeViewColumn("Name", new Gtk.CellRendererText(), "text", 1));
            _list.AppendColumn(new Gtk.TreeViewColumn("Code", new Gtk.CellRendererText(), "text", 2));
            
            PackStart(_list, true, true, 0);
            _list.Selection.Changed += TreeSelectionChanged;
            PackStart(_descriptionLabel, true, false, 0);
            
            ShowProgress();
        }

        private void TreeSelectionChanged(object? sender, EventArgs e)
        {
            var selection = sender as Gtk.TreeSelection;
            if (!selection.GetSelected(out var model, out var iter))
            {
                PageDelegate.NextEnabled = false;
                return;
            }

            var flagUri = model.GetValue(iter, 0) as string;
            var name = model.GetValue(iter, 1) as string;
            var code = model.GetValue(iter, 2) as string;

            var update = _updates.Single(x => x.FlagUri.ToString() == flagUri && x.Title == name && x.LanguageCode == code);
            _selectedUpdate = update;
            PageDelegate.NextEnabled = true;
        }

        private void ShowProgress()
        {
            _progress.Show();
            _progress.Start();
            _list.Hide();
            _descriptionLabel.Hide();
        }
        
        private void ShowGrid()
        {
            _progress.Stop();
            _progress.Hide();
            _list.ShowAll();
            _descriptionLabel.Show();
        }

        public override void Presented()
        {
            Title = "Loading languages";
            Subtitle = "This process might take a few minutes to complete.";
            PageDelegate.BackEnabled = false;
            PageDelegate.NextEnabled = false;
            
            ShowProgress();

            Task.Run(async () =>
            {
                _updates = await BuildFetcher.GetAvailableBuildLanguagesAsync(App.ConversionPlan.UpdateData)
                    .ConfigureAwait(false);
                
                foreach (var update in _updates)
                {
                    _store.AppendValues(update.FlagUri.ToString(), update.Title, update.LanguageCode);
                }
                
                ShowGrid();
                Title = "Ciao, Hola, Bonjour, Hello oder Hallo?";
                Subtitle = "What language do you speak?";
                PageDelegate.BackEnabled = true;
                PageDelegate.NextEnabled = true;
            });
        }

        public override void HandleNextButton()
        {
            if (_selectedUpdate is null)
            {
                return;
            }

            App.ConversionPlan.Language = _selectedUpdate.LanguageCode;
            App.ConversionPlan.LanguageTitle = _selectedUpdate.Title;
            App.ConversionPlan.FlagUri = _selectedUpdate.FlagUri;
            
            PageDelegate.Navigate<EditionPage>();
        }

        public override void HandleBackButton()
        {
        }
    }
}