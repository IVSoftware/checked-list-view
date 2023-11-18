using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace checked_list_view
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            BindingContext = this;
            InitializeComponent();
            listViewComande.ItemsSource = new ObservableCollection<CheckableCommand>
            {
                new CheckableCommand{ DesignationComande = "Command 1" },
                new CheckableCommand{ DesignationComande = "Command 2" },
                new CheckableCommand{ DesignationComande = "Command 3" },
            };
            listViewComande.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem is CheckableCommand cmd) cmd.IsChecked = true;
            };
            foreach (CheckableCommand command in listViewComande.ItemsSource)
            {
                command.PropertyChanged += (sender, e) =>
                {
                    switch (e.PropertyName) 
                    {
                        case nameof(CheckableCommand.IsChecked):
                            var checkedCommands = 
                                listViewComande
                                .ItemsSource
                                .OfType<CheckableCommand>()
                                .Where (_ => _.IsChecked)
                                .ToList();
                            if(checkedCommands.Any())
                            {
                                EditorText = string.Join(Environment.NewLine, checkedCommands);
                            }
                            else
                            {
                                EditorText = "No checked commands at this time.";
                            }
                            break;
                    }
                };
            }
            EditorText = "Hello, I'll be keeping track of your checked commands.";
        }
        public string EditorText
        {
            get => _editorText;
            set
            {
                if (!Equals(_editorText, value))
                {
                    _editorText = value;
                    OnPropertyChanged();
                }
            }
        }
        string _editorText = string.Empty;
    }
    class CheckableCommand : INotifyPropertyChanged
    {
        public string DesignationComande { get; set; }
        public override string ToString() => DesignationComande;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (!Equals(_isChecked, value))
                {
                    _isChecked = value;
                    OnPropertyChanged();
                }
            }
        }
        bool _isChecked = false;
        private void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
