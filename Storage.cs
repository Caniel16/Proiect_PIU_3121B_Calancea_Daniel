using Media_file;
using Media_playlist;

namespace StocareDate
{
    public class Storage
    {
        private string filePath = "C:\\Users\\user\\Documents\\MediaTracker\\MediaTracker\\bin\\Debug\\net8.0\\library.txt";

        public void Save(List<MediaFile> files)
        {
            List<string> lines = new List<string>();

            foreach (MediaFile file in files)
            {
                lines.Add(file.Title);
                lines.Add(file.Path);
                lines.Add(file.SavedPosition.ToString());
                lines.Add(file.GenAudio.ToString());
                lines.Add(file.StareAudio.ToString());
                lines.Add("---");
            }

            File.WriteAllLines(filePath, lines);
            Console.WriteLine("Library saved.");
        }

        public List<MediaFile> Load()
        {
            List<MediaFile> files = new List<MediaFile>();

            if (!File.Exists(filePath))
                return files;

            string[] lines = File.ReadAllLines(filePath);
            int i = 0;

            while (i + 4 < lines.Length)
            {
                MediaFile file = new MediaFile();
                file.Title = lines[i];
                file.Path = lines[i + 1];
                file.SavedPosition = double.Parse(lines[i + 2]);
                file.GenAudio = Enum.Parse<Gen>(lines[i + 3]);
                file.StareAudio = Enum.Parse<Stare>(lines[i + 4]);

                files.Add(file);
                i += 6; // sare peste "---"
            }

            Console.WriteLine("Library loaded: " + files.Count + " files.");
            return files;
        }
        public MediaFile SearchInFile(string title)
        {
            List<MediaFile> files = Load();

            foreach (MediaFile file in files)
            {
                if (file.Title == title)
                    return file;
            }

            return null;
        }

        public void Update(MediaFile updatedFile)
        {
            List<MediaFile> files = Load();

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Title == updatedFile.Title)
                {
                    files[i] = updatedFile;
                    break;
                }
            }

            Save(files);
            Console.WriteLine("File updated: " + updatedFile.Title);
        }
        private string playlistPath = "playlists.txt";

        public void SavePlaylists(List<Playlist> playlists)
        {
            List<string> lines = new List<string>();

            foreach (Playlist playlist in playlists)
            {
                lines.Add(playlist.Name);
                lines.Add(playlist.Tip.ToString());
                lines.Add(playlist.Optiuni.ToString());
                lines.Add(playlist.Files.Count.ToString());

                foreach (MediaFile file in playlist.Files)
                {
                    lines.Add(file.Title);
                }

                lines.Add("---");
            }

            File.WriteAllLines(playlistPath, lines);
            Console.WriteLine("Playlists saved.");
        }

        public List<Playlist> LoadPlaylists(List<MediaFile> allFiles)
        {
            List<Playlist> playlists = new List<Playlist>();

            if (!File.Exists(playlistPath))
                return playlists;

            string[] lines = File.ReadAllLines(playlistPath);
            int i = 0;

            while (i < lines.Length)
            {
                Playlist playlist = new Playlist();
                playlist.Name = lines[i];
                playlist.Tip = Enum.Parse<TipPlaylist>(lines[i + 1]);
                playlist.Optiuni = Enum.Parse<OptiuniPlaylist>(lines[i + 2]);
                int fileCount = int.Parse(lines[i + 3]);
                i += 4;

                for (int j = 0; j < fileCount; j++)
                {
                    string title = lines[i];
                    MediaFile found = allFiles.FirstOrDefault(f => f.Title == title);
                    if (found != null)
                        playlist.Files.Add(found);
                    i++;
                }

                i++; // sare peste "---"
                playlists.Add(playlist);
            }

            Console.WriteLine("Playlists loaded: " + playlists.Count);
            return playlists;
        }

        public Playlist SearchPlaylist(string name, List<Playlist> playlists)
        {
            foreach (Playlist playlist in playlists)
            {
                if (playlist.Name == name)
                    return playlist;
            }
            return null;
        }

        public void UpdatePlaylist(Playlist updatedPlaylist, List<Playlist> playlists)
        {
            for (int i = 0; i < playlists.Count; i++)
            {
                if (playlists[i].Name == updatedPlaylist.Name)
                {
                    playlists[i] = updatedPlaylist;
                    break;
                }
            }

            SavePlaylists(playlists);
            Console.WriteLine("Playlist updated: " + updatedPlaylist.Name);
        }
    }
}