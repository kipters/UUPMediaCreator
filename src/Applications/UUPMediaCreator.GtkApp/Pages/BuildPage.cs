using System;
using System.Linq;
using System.Threading;
using WindowsUpdateLib;
using Gtk;
using Task = System.Threading.Tasks.Task;

namespace UUPMediaCreator.GtkApp.Pages
{
    public class BuildPage : PageBase
    {
        private readonly Gtk.Spinner _progress;
        private readonly Gtk.ListStore _store;
        private readonly Gtk.TreeView _list;
        private BuildFetcher.AvailableBuild[] _updates;
        private BuildFetcher.AvailableBuild _selectedBuild;
        private readonly Gtk.Label _descriptionLabel;

        public BuildPage(IPageDelegate pageDelegate) : base(pageDelegate)
        {
            Title = "What version of windows do you want to build media for?";
            Subtitle = "The selected build will be used for the final medium";

            _progress = new Gtk.Spinner();
            CenterWidget = _progress;

            _store = new Gtk.ListStore(typeof(string), typeof(string));
            _list = new Gtk.TreeView(_store);
            _descriptionLabel = new Gtk.Label
            {
                Halign = Gtk.Align.Start,
                SingleLineMode = false
            };

            _list.AppendColumn(new Gtk.TreeViewColumn("Title", new Gtk.CellRendererText(), "text", 0)
                {Sizing = TreeViewColumnSizing.Autosize,Expand = false});
            _list.AppendColumn(new Gtk.TreeViewColumn("Created", new Gtk.CellRendererText(), "text", 1)
                {Sizing = TreeViewColumnSizing.Autosize});
            
            PackStart(_list, true, true, 0);
            _list.Selection.Changed += TreeSelectionChanged;
            PackStart(_descriptionLabel, true, false, 0);
            
            ShowProgress();
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

        private void TreeSelectionChanged(object? sender, EventArgs e)
        {
            var selection = sender as Gtk.TreeSelection;
            if (!selection.GetSelected(out var model, out var iter))
            {
                PageDelegate.NextEnabled = false;
                return;
            }
            
            var title = model.GetValue(iter, 0) as string;
            var created = model.GetValue(iter, 1) as string;

            var build = _updates.Single(x => x.Title == title && x.Created == created);
            _selectedBuild = build;
            _descriptionLabel.Text = build.Description;
            PageDelegate.NextEnabled = true;
        }

        public override void Presented()
        {
            Title = "Loading builds";
            Subtitle = "This process might take a few minutes to complete";
            PageDelegate.BackEnabled = false;
            PageDelegate.NextEnabled = false;
            _selectedBuild = null;

            ShowProgress();
            Task.Run(async () =>
            {
                var machineType = App.ConversionPlan.MachineType;
                _updates = await BuildFetcher.GetAvailableBuildsAsync(machineType).ConfigureAwait(false);
                
                foreach (var build in _updates)
                {
                    _store.AppendValues(build.Title, build.Created);
                }

                ShowGrid();
                Title = "What version of Windows do you want to build media for?";
                Subtitle = "The selected build will be used for the final medium";
                PageDelegate.BackEnabled = true;
                PageDelegate.NextEnabled = false;
            });
        }

        public override void HandleNextButton()
        {
            if (_selectedBuild is null)
            {
                return;
            }
            
            App.ConversionPlan.UpdateData = _selectedBuild.UpdateData;
            App.ConversionPlan.BuildString = _selectedBuild.BuildString;
            
            PageDelegate.Navigate<LanguagePage>();
        }
    }
}