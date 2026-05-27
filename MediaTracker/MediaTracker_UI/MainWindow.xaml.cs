using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Media_file;
using Media_playlist;
using StocareDate;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;

namespace MediaTracker_UI
{
    public partial class MainWindow : Window
    {
        private Storage storage = new Storage();

        // Lista completa de carti. La cautare, lista afisata pointeaza
        // temporar la o lista filtrata, dar aceasta ramane intacta.
        private System.Collections.ObjectModel.ObservableCollection<Carte> books = new();

        // Toate capitolele din toate cartile, intr-o singura lista.
        private List<Capitol> allChapters = new List<Capitol>();

        private MediaPlayer player = new MediaPlayer();

        // Ruleaza la fiecare jumatate de secunda ca sa actualizeze
        // sliderul si pozitia salvata a capitolului.
        private DispatcherTimer timer = new DispatcherTimer();

        private Carte? currentBook = null;
        private Capitol? currentChapter = null;
        private bool isPlaying = false;

        // Adevarat cat timp utilizatorul trage de slider,
        // ca sa nu fie miscat de cod in acelasi timp.
        private bool isDragging = false;

        // Adevarat cand un capitol este selectat din cod (nu de utilizator),
        // ca sa nu porneasca redarea cand alegi doar o carte.
        private bool _isProgrammaticSelection = false;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            SetupPlayer();
        }

        // Citeste capitolele si cartile salvate, la pornirea aplicatiei.
        private void LoadData()
        {
            allChapters = storage.Load();
            List<Carte> loaded = storage.LoadPlaylists(allChapters);
            foreach (Carte c in loaded)
                books.Add(c);

            BooksListBox.ItemsSource = books;
        }

        // Pregateste playerul audio si timerul care actualizeaza interfata.
        private void SetupPlayer()
        {
            player.MediaOpened += (s, e) =>
            {
                double duration = player.NaturalDuration.TimeSpan.TotalSeconds;
                ProgressSlider.Maximum = duration;
                DurationText.Text = FormatTime(player.NaturalDuration.TimeSpan);

                if (currentChapter != null)
                {
                    currentChapter.Duration = duration;

                    if (currentChapter.SavedPosition > 0)
                        player.Position = TimeSpan.FromSeconds(currentChapter.SavedPosition);
                }

                player.Volume = VolumeSlider.Value;
                player.Play();
            };

            player.MediaEnded += (s, e) =>
            {
                SaveCurrentPosition();
                PlayNextChapter();
            };

            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (s, e) =>
            {
                if (!isDragging && isPlaying)
                {
                    ProgressSlider.Value = player.Position.TotalSeconds;
                    PositionText.Text = FormatTime(player.Position);

                    if (currentChapter != null)
                        currentChapter.SavedPosition = player.Position.TotalSeconds;
                }
            };
            timer.Start();
        }

        // Filtreaza cartile dupa nume (indiferent de litere mari sau mici).
        // Lista originala ramane neschimbata.
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(query))
                BooksListBox.ItemsSource = books;
            else
                BooksListBox.ItemsSource = books.Where(b => b.Name.ToLower().Contains(query)).ToList();
        }

        // Cand alegi o carte: afiseaza capitolele ei si selecteaza vizual
        // ultimul capitol inceput, fara sa porneasca redarea.
        private void BooksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentBook = BooksListBox.SelectedItem as Carte;
            if (currentBook == null) return;

            BookTitleText.Text = currentBook.Name;
            BookInfoText.Text = $"{currentBook.Files.Count} chapters  •  {currentBook.Tip}";
            BookInfoPanel.Visibility = Visibility.Visible;

            ChaptersListBox.ItemsSource = currentBook.Files;

            var lastChapter = currentBook.Files
                .FirstOrDefault(f => f.SavedPosition > 0 && !f.IsCompleted);

            if (lastChapter != null)
            {
                _isProgrammaticSelection = true;
                ChaptersListBox.SelectedItem = lastChapter;
                _isProgrammaticSelection = false;
            }
        }

        private void ChaptersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isProgrammaticSelection) return;

            var selected = ChaptersListBox.SelectedItem as Capitol;
            if (selected == null) return;

            LoadChapter(selected);
        }

        // Salveaza pozitia capitolului curent, apoi pregateste noul capitol.
        // Redarea efectiva incepe cand fisierul este gata.
        private void LoadChapter(Capitol chapter)
        {
            SaveCurrentPosition();

            currentChapter = chapter;
            player.Open(new Uri(chapter.Path));

            NowPlayingTitle.Text = chapter.Title;
            NowPlayingBook.Text = currentBook?.Name ?? "";

            isPlaying = true;
            PlayPauseButton.Content = "⏸";
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentChapter == null) return;

            if (isPlaying)
            {
                player.Pause();
                isPlaying = false;
                PlayPauseButton.Content = "▶";
                SaveCurrentPosition();
            }
            else
            {
                player.Play();
                isPlaying = true;
                PlayPauseButton.Content = "⏸";
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentBook == null || currentChapter == null) return;

            int index = currentBook.Files.IndexOf(currentChapter);
            if (index > 0)
            {
                ChaptersListBox.SelectedItem = currentBook.Files[index - 1];
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PlayNextChapter();
        }

        private void PlayNextChapter()
        {
            if (currentBook == null || currentChapter == null) return;

            int index = currentBook.Files.IndexOf(currentChapter);
            if (index < currentBook.Files.Count - 1)
            {
                Dispatcher.Invoke(() =>
                {
                    ChaptersListBox.SelectedItem = currentBook.Files[index + 1];
                });
            }
            else
            {
                isPlaying = false;
                PlayPauseButton.Content = "▶";
            }
        }

        private void ProgressSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDragging = true;
        }

        private void ProgressSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDragging = false;
            player.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
            PositionText.Text = FormatTime(player.Position);
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isDragging)
                PositionText.Text = FormatTime(TimeSpan.FromSeconds(ProgressSlider.Value));
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = VolumeSlider.Value;
        }

        // Adauga o carte noua: cere numele, alegi un folder cu MP3-uri,
        // si pentru fiecare fisier se creeaza un capitol nou.
        private void MenuAddBook_Click(object sender, RoutedEventArgs e)
        {
            string bookName = Microsoft.VisualBasic.Interaction.InputBox("Enter book name:", "New Book", "");

            if (string.IsNullOrWhiteSpace(bookName)) return;

            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                Description = "Select folder with MP3 chapters"
            };

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            string[] mp3Files = Directory.GetFiles(dialog.SelectedPath, "*.mp3")
                                         .OrderBy(f => f)
                                         .ToArray();

            if (mp3Files.Length == 0)
            {
                MessageBox.Show("No MP3 files found in that folder.");
                return;
            }

            List<Capitol> chapters = new List<Capitol>();
            foreach (string path in mp3Files)
            {
                Capitol chapter = new Capitol();
                chapter.Title = Path.GetFileNameWithoutExtension(path);
                chapter.Path = path;
                chapter.SavedPosition = 0;
                chapter.Gen = Gen.None;
                chapter.Stare = Stare.None;

                allChapters.Add(chapter);
                chapters.Add(chapter);
            }

            Carte book = new Carte();
            book.Name = bookName;
            book.Tip = TipCarte.Personal;
            book.Optiuni = OptiuniCarte.None;
            book.Files = chapters;

            books.Add(book);
            storage.Save(allChapters);
            storage.SavePlaylists(books.ToList());
            BooksListBox.ItemsSource = null;
            BooksListBox.ItemsSource = books;

            MessageBox.Show($"'{bookName}' added with {chapters.Count} chapters!");
        }

        // Sterge cartea selectata din lista.
        // Capitolele raman salvate pe disc, doar nu mai apartin niciunei carti.
        private void MenuRemoveBook_Click(object sender, RoutedEventArgs e)
        {
            if (currentBook == null)
            {
                MessageBox.Show("Select a book first.", "Remove Book", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Remove \"{currentBook.Name}\"?", "Remove Book",
                                         MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            books.Remove(currentBook);
            storage.SavePlaylists(books.ToList());

            currentBook = null;
            currentChapter = null;
            player.Stop();

            BookInfoPanel.Visibility = Visibility.Collapsed;
            ChaptersListBox.ItemsSource = null;
            NowPlayingTitle.Text = "No chapter selected";
            NowPlayingBook.Text = "";
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentPosition();
            Application.Current.Shutdown();
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("AudioBook Player v1.0", "About",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Salveaza pe disc pozitia curenta din capitol.
        // Interfata se actualizeaza singura (vezi clasa Capitol).
        private void SaveCurrentPosition()
        {
            if (currentChapter == null) return;
            currentChapter.SavedPosition = player.Position.TotalSeconds;
            storage.Update(currentChapter);
        }

        private string FormatTime(TimeSpan ts)
        {
            if (ts.Hours > 0)
                return ts.ToString(@"h\:mm\:ss");
            return ts.ToString(@"m\:ss");
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveCurrentPosition();
            base.OnClosing(e);
        }
    }
}
