using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cleanup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Clean Button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> clean_paths = new List<string>();

            // check if downloads is checked
            if (downloads.IsChecked == true)
            {
                clean_paths.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads");
            }

            // check if tempfiles is checked
            if (tempfiles.IsChecked == true)
            {
                clean_paths.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp");
            }

            PrintTextToLog(String.Format("Selected {0} paths: {1}", clean_paths.Count, String.Join(", ", clean_paths)));
        }

        private void UpdateSummary(List<string> paths)
        {
            foreach (string path in paths)
            {
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(new Run(path));
            }
        }

        // print text to log
        private void PrintTextToLog(string text)
        {
            // Add paragraph
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(text));
            paragraph.FontSize = 14;
            paragraph.LineHeight = 18;
            paragraph.Margin = new Thickness(0, 0, 0, 0);

            // Add paragraph to document
            FlowDocument flowDocument = log.Document as FlowDocument;
            if (flowDocument != null)
            {
                flowDocument.Blocks.Add(paragraph);
            }

            // Scroll to bottom
            log.ScrollToEnd();
        }

        private void exitbtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // On Drag of "dragbar" move window
        private void dragbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

    }
}
