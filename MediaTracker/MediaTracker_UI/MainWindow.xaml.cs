using System.Windows;
using Media_file;
using StocareDate;

namespace MediaTracker_UI
{
    public partial class MainWindow : Window
    {
        private Storage storage = new Storage();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            List<MediaFile> files = storage.Load();

            if (files.Count == 0)
            {
                MessageBox.Show("No files found in library.");
                return;
            }

            // Afișăm primul fișier din bibliotecă
            MediaFile file = files[0];

            TitleText.Text = file.Title;
            PathText.Text = file.Path;
            PositionText.Text = file.SavedPosition + " seconds";
            GenreText.Text = file.GenAudio.ToString();
            StateText.Text = file.StareAudio.ToString();
        }
    }
}