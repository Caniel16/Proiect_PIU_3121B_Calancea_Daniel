using System.IO;
using System.Windows;
using System.Windows.Media;
using Media_file;
using StocareDate;

namespace MediaTracker_UI
{
    public partial class MainWindow : Window
    {
        private Storage storage = new Storage();

        private const int MIN_TITLE_LENGTH = 2;
        private const int MAX_TITLE_LENGTH = 100;

        private SolidColorBrush normalColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private SolidColorBrush errorColor = new SolidColorBrush(Color.FromRgb(233, 69, 96));

        public MainWindow()
        {
            InitializeComponent();
        }

        // --- Validare ---
        private bool ValidateInputs()
        {
            bool isValid = true;

            // Reset
            TitleLabel.Foreground = normalColor;
            PathLabel.Foreground = normalColor;
            GenLabel.Foreground = normalColor;
            TitleError.Visibility = Visibility.Collapsed;
            PathError.Visibility = Visibility.Collapsed;
            GenError.Visibility = Visibility.Collapsed;

            // Title
            if (string.IsNullOrWhiteSpace(TitleInput.Text) ||
                TitleInput.Text.Length < MIN_TITLE_LENGTH)
            {
                TitleLabel.Foreground = errorColor;
                TitleError.Text = $"Title must be at least {MIN_TITLE_LENGTH} characters.";
                TitleError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (TitleInput.Text.Length > MAX_TITLE_LENGTH)
            {
                TitleLabel.Foreground = errorColor;
                TitleError.Text = $"Title must be under {MAX_TITLE_LENGTH} characters.";
                TitleError.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Path
            if (string.IsNullOrWhiteSpace(PathInput.Text))
            {
                PathLabel.Foreground = errorColor;
                PathError.Text = "Path cannot be empty.";
                PathError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (!File.Exists(PathInput.Text))
            {
                PathLabel.Foreground = errorColor;
                PathError.Text = "File does not exist on disc.";
                PathError.Visibility = Visibility.Visible;
                isValid = false;
            }

            // Gen - verificam daca un RadioButton e selectat
            if (GetSelectedGen() == null)
            {
                GenLabel.Foreground = errorColor;
                GenError.Text = "Please select a genre.";
                GenError.Visibility = Visibility.Visible;
                isValid = false;
            }

            return isValid;
        }

        // Returnează Gen-ul selectat din RadioButtons, sau null dacă niciunul
        private Gen? GetSelectedGen()
        {
            if (RadioFantezie.IsChecked == true) return Gen.Fantezie;
            if (RadioSciFi.IsChecked == true) return Gen.ScientificFiction;
            if (RadioMister.IsChecked == true) return Gen.Mister;
            if (RadioRomance.IsChecked == true) return Gen.Romance;
            if (RadioNonFictiune.IsChecked == true) return Gen.NonFictiune;
            return null;
        }

        // Construiește Stare din CheckBoxes bifate
        private Stare GetSelectedStare()
        {
            Stare stare = Stare.None;
            if (CheckAscultat.IsChecked == true) stare |= Stare.Ascultat;
            if (CheckFavorit.IsChecked == true) stare |= Stare.Favorit;
            if (CheckDeAscultat.IsChecked == true) stare |= Stare.DeAscultat;
            if (CheckArhivat.IsChecked == true) stare |= Stare.Arhivat;
            return stare;
        }

        // --- Evenimente butoane ---
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            SuccessText.Visibility = Visibility.Collapsed;

            if (!ValidateInputs())
                return;

            MediaFile file = new MediaFile();
            file.Title = TitleInput.Text;
            file.Path = PathInput.Text;
            file.SavedPosition = 0;
            file.GenAudio = GetSelectedGen()!.Value;
            file.StareAudio = GetSelectedStare();

            List<MediaFile> files = storage.Load();
            files.Add(file);
            storage.Save(files);

            SuccessText.Text = $"✔ '{file.Title}' added successfully!";
            SuccessText.Visibility = Visibility.Visible;

            ClearForm();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            TitleInput.Text = "";
            PathInput.Text = "";
            RadioFantezie.IsChecked = false;
            RadioSciFi.IsChecked = false;
            RadioMister.IsChecked = false;
            RadioRomance.IsChecked = false;
            RadioNonFictiune.IsChecked = false;
            CheckAscultat.IsChecked = false;
            CheckFavorit.IsChecked = false;
            CheckDeAscultat.IsChecked = false;
            CheckArhivat.IsChecked = false;

            TitleLabel.Foreground = normalColor;
            PathLabel.Foreground = normalColor;
            GenLabel.Foreground = normalColor;
            TitleError.Visibility = Visibility.Collapsed;
            PathError.Visibility = Visibility.Collapsed;
            GenError.Visibility = Visibility.Collapsed;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            DetailsPanel.Visibility = Visibility.Collapsed;
            NotFoundText.Visibility = Visibility.Collapsed;
            SearchError.Visibility = Visibility.Collapsed;

            string searchTitle = SearchInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchTitle))
            {
                SearchError.Text = "Please enter a title to search.";
                SearchError.Visibility = Visibility.Visible;
                return;
            }

            List<MediaFile> files = storage.Load();
            MediaFile found = null;

            foreach (MediaFile file in files)
            {
                if (file.Title == searchTitle)
                {
                    found = file;
                    break;
                }
            }

            if (found == null)
            {
                NotFoundText.Visibility = Visibility.Visible;
                return;
            }

            TitleText.Text = found.Title;
            PathText.Text = found.Path;
            PositionText.Text = found.SavedPosition + " seconds";
            GenreText.Text = found.GenAudio.ToString();
            StateText.Text = found.StareAudio.ToString();
            DetailsPanel.Visibility = Visibility.Visible;
        }

        // --- Meniu ---
        private void MenuAdd_Click(object sender, RoutedEventArgs e)
        {
            AddPanel.Visibility = Visibility.Visible;
        }

        private void MenuSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchPanel.Visibility = Visibility.Visible;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            List<MediaFile> files = storage.Load();
            if (files.Count == 0)
            {
                MessageBox.Show("No files found in library.");
                return;
            }
            MediaFile file = files[0];
            TitleText.Text = file.Title;
            PathText.Text = file.Path;
            PositionText.Text = file.SavedPosition + " seconds";
            GenreText.Text = file.GenAudio.ToString();
            StateText.Text = file.StareAudio.ToString();
            DetailsPanel.Visibility = Visibility.Visible;
            SearchPanel.Visibility = Visibility.Visible;
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("MediaTracker v1.0\nA personal audio library manager.",
                            "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}