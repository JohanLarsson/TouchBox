namespace TouchBox
{
    using System.Globalization;
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

        private void OnTouchClick(object sender, RoutedEventArgs e)
        {
            Touch.Tap(100, 120);
        }
    }
}
