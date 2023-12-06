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
            new CleanWarning().Show();
        }

        private void StartCleaning()
        {
            long total_found = 0;
            long total_deleted = 0;
            List<string> clean_paths = new List<string>();

            if (downloads.IsChecked == true) clean_paths.Add(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads");
            if (tempfiles.IsChecked == true) clean_paths.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Temp");

            PrintTextToLog(String.Format("Cleaning {0} {1}...", clean_paths.Count, clean_paths.Count != 1 ? "paths" : "path"));

            bool br = true;
            string last_drive = "c";
            foreach (string path in clean_paths)
            {
                // get how many storage is used by path
                long local_found = 0;
                long local_deleted = 0;

                // loop through all files in path
                foreach (string file in Directory.GetFiles(path))
                {
                    local_found += new FileInfo(file).Length;
                    total_found += new FileInfo(file).Length;

                    if (IsFileDeletable(file))
                    {
                        local_deleted += new FileInfo(file).Length;
                        total_deleted += new FileInfo(file).Length;
                    }
                }

                if (last_drive.ToLower() != path.Substring(0, 1).ToLower()) br = true;

                long local_deleted_percentage = (local_deleted * 100) / local_found;
                PrintTextToLog(String.Format("Cleaned up {0} of space at {1} (which is {2}% of the directory)", ConvertBytesToReadableSize(local_deleted), path, local_deleted_percentage), br);
                br = false;
            }

            long deleted_percentage = (total_deleted * 100) / total_found;
            PrintTextToLog(String.Format("Finished! Freed {0} ({1}) of space, with a total of {2} of checked files.", ConvertBytesToReadableSize(total_deleted), deleted_percentage + "%", ConvertBytesToReadableSize(total_found)), true);
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
        private void PrintTextToLog(string text, bool begin_new_block = false)
        {
            // Add paragraph
            Paragraph paragraph = new Paragraph();
            Run r = new Run(text);
            paragraph.Inlines.Add(r);
            paragraph.FontSize = 14;
            paragraph.LineHeight = 18;
            paragraph.Margin = new Thickness(0, 0, 0, 0);

            // Add paragraph to document
            FlowDocument flowDocument = log.Document as FlowDocument;

            if (flowDocument != null)
            {
                if (begin_new_block)
                {
                    Paragraph empty = new Paragraph();
                    empty.Inlines.Add(new Run(""));
                    empty.FontSize = 2;
                    empty.LineHeight = 2;

                    flowDocument.Blocks.Add(empty);

                }

                flowDocument.Blocks.Add(paragraph);
            }

            // Scroll to bottom
            log.ScrollToEnd();
        }

    }
}
