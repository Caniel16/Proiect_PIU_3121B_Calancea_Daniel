using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Media_file
{
    // Un capitol dintr-o carte audio (un singur fisier MP3).
    // Anunta interfata cand se schimba pozitia, durata sau starea,
    // ca sa se actualizeze singura.
    public class Capitol : INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Path { get; set; }

        private double _savedPosition;
        // Secunda la care a ramas ultima oara redarea, ca sa poata continua de acolo.
        public double SavedPosition
        {
            get => _savedPosition;
            set
            {
                if (_savedPosition == value) return;
                _savedPosition = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressPercent));
            }
        }

        public Gen Gen { get; set; }
        public Stare Stare { get; set; }

        private bool _isCompleted = false;
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted == value) return;
                _isCompleted = value;
                OnPropertyChanged();
            }
        }

        private double _duration = 0;
        // Durata totala in secunde, aflata la prima ascultare.
        public double Duration
        {
            get => _duration;
            set
            {
                if (_duration == value) return;
                _duration = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressPercent));
            }
        }

        // Cat la suta din capitol a fost ascultat. Folosit la bara de progres.
        // Returneaza 0 daca nu stim inca durata totala.
        public double ProgressPercent
        {
            get
            {
                if (Duration <= 0) return 0;
                return (SavedPosition / Duration) * 100.0;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
