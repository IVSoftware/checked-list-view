# Checked List View

Your xaml demonstrates an intent to display the `DesignationComande` property and respond when the check box changes (either programmatically or though user interaction). This will require a model for the items in your `ListView` bound to those properties. 

For example:

[![responding to checks][1]][1]

___
**Finish the bindings in xaml**

Bind the `IsChecked` property in your `DataTemplate`
```
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="checked_list_view.MainPage">
    <Grid
        RowDefinitions="*,*" >
        <ListView
            Grid.Row="0"
            x:Name="listViewComande"                  
            BackgroundColor="Azure">
            <ListView.ItemTemplate >
                <DataTemplate>
                    <ViewCell>
                        <Grid 
                            RowDefinitions="Auto" 
                            ColumnDefinitions="Auto,20*,45*,10*,10*,10*" 
                            Padding="3,10,3,10" 
                            ColumnSpacing="8">
                            <CheckBox 
                                IsChecked="{Binding IsChecked}"
                                Grid.Row="1" 
                                Grid.Column="0" 
                                Color="CornflowerBlue" />
                            <Label 
                                Grid.Row="1" 
                                Grid.Column="2"  
                                BackgroundColor="LightGreen"
                                Text="{Binding DesignationComande}"      
                                FontSize="13" VerticalTextAlignment="Center"                            
                                HorizontalTextAlignment="Start" MaxLines="1"  
                                HorizontalOptions="Fill"
                                FontAttributes="None"
                                Padding="10,0"/>
                        </Grid>
                    </ViewCell>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>
        <Editor 
            Grid.Row="1"
            Text="{Binding EditorText}"/>
    </Grid>
</ContentPage>
```
___

**Make a Model for items in the `ListView`**

This functionality requires a bindable property for `IsChecked` that fires a `PropertyChanged` event for the check boxes.

```
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
```
___
**Glue it all together**

In this sample, the code-behind for the MainPage sets the `ItemsSource` of the `ListView` (you could also do this in xaml) to an observable collection of `CheckableCommand` and subscribes to the `PropertyChanged` notification of the items. 

```
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
            listViewComande.ItemSelected += (sender, e) =>
            {
                // Option to check the checkbox when item is selected.
                if (e.SelectedItem is CheckableCommand cmd) cmd.IsChecked = true;
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
```

  [1]: https://i.stack.imgur.com/xU2Sp.png