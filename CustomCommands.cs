using System.Windows.Input;

//CTRL + M plus CTRL + O  - Collapse All
//TODO: Version number  13.2.11.11 is displayed in Version Collumn as 13.2.1111
//TODO: Handling when unable to connect because already someone connected to Miniserver
namespace WPFConfigUpdater
{
    //private void CheckBoxEnableMultiSelect_Click(object sender, RoutedEventArgs e)
    //{
    //    if(CheckBoxEnableMultiSelect.IsChecked == true)
    //    {
    //        listView_Miniserver.SelectionMode = SelectionMode.Multiple;
    //    }
    //    else
    //    {
    //        listView_Miniserver.SelectionMode = SelectionMode.Extended;
    //    }
    //}



    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
            (
                "Exit",
                "Exit",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4, ModifierKeys.Alt)
                }
            );

        public static readonly RoutedUICommand DeselectAll = new RoutedUICommand
            (
                "DeselectAll",
                "DeselectAll",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D, ModifierKeys.Control)
                }
            );

        //Define more commands here, just like the one above
    }

}
