namespace TouchBox
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            this.Events.Items.Clear();
        }

        private void OnTouch(object sender, TouchEventArgs e)
        {
            this.Events.Items.Add($"{e.RoutedEvent.Name} Position: {e.GetTouchPoint((IInputElement)e.Source).Position.ToString(CultureInfo.InvariantCulture)}");
        }

        private void OnManipulation(object sender, InputEventArgs e)
        {
            this.Events.Items.Add($"{e.RoutedEvent.Name}");
        }

        private async void OnTouchClick(object sender, RoutedEventArgs e)
        {
            // delay so we don't mix with the mouse click that got us here.
            await Task.Delay(TimeSpan.FromMilliseconds(200));
            try
            {
                Touch.Tap(100, 120);
            }
            catch (Exception exception)
            {
                this.Events.Items.Add($"Exception: {exception.Message}");
            }
        }
    }
}
