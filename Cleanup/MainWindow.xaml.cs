using System.IO;
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
            long total_deleted = 0;
            List<string> clean_paths = new List<string>();

            if (downloads.IsChecked == true) clean_paths.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads");
            if (tempfiles.IsChecked == true) clean_paths.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp");

            PrintTextToLog(String.Format("Cleaning {0} {1}...", clean_paths.Count, clean_paths.Count != 1 ? "paths" : "path"));


            foreach (string path in clean_paths)
            {
                // get how many storage is used by path
                long storage_used = 0;

                // loop through all files in path
                foreach (string file in Directory.GetFiles(path))
                {
                    storage_used += new FileInfo(file).Length;
                    if (IsFileDeletable(file))
                        total_deleted += new FileInfo(file).Length;
                    else PrintTextToLog(String.Format("Can not delete {0}", file));
                }

                if (storage_used == total_deleted) PrintTextToLog(String.Format("Freed {0} of space.", ConvertBytesToReadableSize(storage_used)));
                else PrintTextToLog(String.Format("Freed {0} of space, with a total of {1} of checked files.", ConvertBytesToReadableSize(total_deleted), ConvertBytesToReadableSize(storage_used)));
            }
        }

        public void UpdateSummary(object sender, RoutedEventArgs e)
        {

        }

        private bool IsFileDeletable(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            FileStream stream = null;

            try
            {
                // Versuche, die Datei mit exklusivem Zugriff zu öffnen
                stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                return true;
            }
            catch (IOException)
            {
                // Ein IOException bedeutet, dass die Datei in Gebrauch ist oder ein anderer Fehler aufgetreten ist
                return false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        private string ConvertBytesToReadableSize(long bytes)
        {
            string[] sizes = { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB" };
            double formattedSize = bytes;
            int sizeIndex = 0;

            while (formattedSize >= 1024 && sizeIndex < sizes.Length - 1)
            {
                formattedSize /= 1024;
                sizeIndex++;
            }

            return string.Format("{0:0.##} {1}", formattedSize, sizes[sizeIndex]);
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

    }
}
