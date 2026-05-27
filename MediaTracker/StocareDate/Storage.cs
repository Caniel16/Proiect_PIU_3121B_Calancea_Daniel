using Media_file;
using Media_playlist;

namespace StocareDate
{
    // Salveaza si citeste datele in doua fisiere text, langa aplicatie:
    //   library.txt   - toate capitolele (7 linii per capitol, separate cu "---")
    //   playlists.txt - cartile, cu titlurile capitolelor care le contin
    public class Storage
    {
        private string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "library.txt");
        private string playlistPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "playlists.txt");

        // Salveaza toate capitolele in library.txt (rescrie fisierul de la zero).
        public void Save(List<Capitol> files)
        {
            List<string> lines = new List<string>();

            foreach (Capitol file in files)
            {
                lines.Add(file.Title);
                lines.Add(file.Path);
                lines.Add(file.SavedPosition.ToString());
                lines.Add(file.Gen.ToString());
                lines.Add(file.Stare.ToString());
                lines.Add(file.Duration.ToString());
                lines.Add("---");
            }

            File.WriteAllLines(filePath, lines);
        }

        // Citeste capitolele din library.txt. Daca fisierul nu exista, returneaza o lista goala.
        public List<Capitol> Load()
        {
            List<Capitol> files = new List<Capitol>();

            if (!File.Exists(filePath))
                return files;

            string[] lines = File.ReadAllLines(filePath);
            int i = 0;

            while (i + 4 < lines.Length)
            {
                Capitol file = new Capitol();
                file.Title = lines[i];
                file.Path = lines[i + 1];
                file.SavedPosition = double.Parse(lines[i + 2]);
                file.Gen = Enum.Parse<Gen>(lines[i + 3]);
                file.Stare = Enum.Parse<Stare>(lines[i + 4]);

                // suport pentru fisiere vechi (fara Duration)
                if (i + 5 < lines.Length && lines[i + 5] != "---")
                {
                    file.Duration = double.Parse(lines[i + 5]);
                    i += 7;
                }
                else
                {
                    i += 6;
                }

                files.Add(file);
            }

            return files;
        }

        // Modifica un capitol existent (gasit dupa titlu) si rescrie fisierul.
        public void Update(Capitol updatedFile)
        {
            List<Capitol> files = Load();

            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Title == updatedFile.Title)
                {
                    files[i] = updatedFile;
                    break;
                }
            }

            Save(files);
        }

        // Salveaza cartile in playlists.txt. Retine doar titlurile capitolelor,
        // restul datelor lor sunt deja in library.txt.
        public void SavePlaylists(List<Carte> carti)
        {
            List<string> lines = new List<string>();

            foreach (Carte carte in carti)
            {
                lines.Add(carte.Name);
                lines.Add(carte.Tip.ToString());
                lines.Add(carte.Optiuni.ToString());
                lines.Add(carte.Files.Count.ToString());

                foreach (Capitol file in carte.Files)
                {
                    lines.Add(file.Title);
                }

                lines.Add("---");
            }

            File.WriteAllLines(playlistPath, lines);
        }

        // Citeste cartile si le leaga de capitolele primite ca parametru (cautare dupa titlu).
        // Trebuie chemata dupa Load().
        public List<Carte> LoadPlaylists(List<Capitol> allFiles)
        {
            List<Carte> carti = new List<Carte>();

            if (!File.Exists(playlistPath))
                return carti;

            string[] lines = File.ReadAllLines(playlistPath);
            int i = 0;

            while (i < lines.Length)
            {
                Carte carte = new Carte();
                carte.Name = lines[i];
                carte.Tip = Enum.Parse<TipCarte>(lines[i + 1]);
                carte.Optiuni = Enum.Parse<OptiuniCarte>(lines[i + 2]);
                int fileCount = int.Parse(lines[i + 3]);
                i += 4;

                for (int j = 0; j < fileCount; j++)
                {
                    string title = lines[i];
                    Capitol found = allFiles.FirstOrDefault(f => f.Title == title);
                    if (found != null)
                        carte.Files.Add(found);
                    i++;
                }

                i++; // sare peste "---"
                carti.Add(carte);
            }

            return carti;
        }
    }
}
