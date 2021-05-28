using System;
using System.Linq;
using System.Threading.Tasks;
using WindowsUpdateLib;

namespace UUPMediaCreator.GtkApp.Pages
{
    public class EditionPage : PageBase
    {
        private readonly Gtk.Spinner _progress;
        private readonly Gtk.ListStore _store;
        private readonly Gtk.TreeView _list;
        private readonly Gtk.Label _descriptionLabel;
        private BuildFetcher.AvailableEdition[] _editions;
        private BuildFetcher.AvailableEdition _selectedEdition;

        public EditionPage(IPageDelegate pageDelegate) : base(pageDelegate)
        {
            Title = "Select the Windows edition";
            Subtitle = "";

            _progress = new Gtk.Spinner();
            CenterWidget = _progress;

            _store = new Gtk.ListStore(typeof(string));
            _list = new Gtk.TreeView(_store);
            _descriptionLabel = new Gtk.Label
            {
                Halign = Gtk.Align.Start,
                SingleLineMode = false
            };

            _list.AppendColumn(new Gtk.TreeViewColumn("Edition", new Gtk.CellRendererText(), "text", 0));
            
            PackStart(_list, true, true, 0);
            _list.Selection.Changed += TreeSelectionChanged;
            PackStart(_descriptionLabel, true, false, 0);
            
            ShowProgress();
        }

        private void TreeSelectionChanged(object? sender, EventArgs e)
        {
            if (sender is Gtk.TreeSelection selection && selection.GetSelected(out var model, out var iter))
            {
                var editionId = model.GetValue(iter, 0) as string;
                var edition = _editions.Single(x => x.Edition == editionId);
                _selectedEdition = edition;
                PageDelegate.NextEnabled = true;
            }
            else
            {
                PageDelegate.NextEnabled = false;
            }
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
            Title = "Loading editions";
            Subtitle = "This process might take a few minutes to complete.";
            PageDelegate.BackEnabled = false;
            PageDelegate.NextEnabled = false;
            
            ShowProgress();
            Task.Run(async () =>
            {
                _editions = await BuildFetcher
                    .GetAvailableEditions(App.ConversionPlan.UpdateData, App.ConversionPlan.Language)
                    .ConfigureAwait(false);

                foreach (var edition in _editions)
                {
                    _store.AppendValues(edition.Edition);
                }
                
                ShowGrid();
                Title = "Select the Windows edition";
                Subtitle = "";
                PageDelegate.BackEnabled = true;
                PageDelegate.NextEnabled = false;
            });
        }

        public override void HandleNextButton()
        {
            App.ConversionPlan.Edition = _selectedEdition.Edition;
            PageDelegate.Navigate<WIMTypePage>();
        }
    }
}