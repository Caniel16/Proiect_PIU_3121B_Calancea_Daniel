using Media_file;

namespace Media_playlist
{
    public enum TipPlaylist
    {
        Personal,
        Public,
        Descoperire,
        Favorites
    }

    [Flags]
    public enum OptiuniPlaylist
    {
        None = 0,
        Shuffle = 1,
        Repeat = 2,
        Descarcabil = 4
    }

    public class Playlist
    {
        public string Name;
        public List<MediaFile> Files = new List<MediaFile>();
        public TipPlaylist Tip;
        public OptiuniPlaylist Optiuni;

        public void AddFile(MediaFile file)
        {
            Files.Add(file);
            Console.WriteLine(file.Title + " added to " + Name);
        }

        public void ListFiles()
        {
            if (Files.Count == 0)
            {
                Console.WriteLine("Playlist is empty.");
                return;
            }

            foreach (MediaFile file in Files)
            {
                Console.WriteLine("- " + file.Title + " | " + file.GenAudio + " | " + file.StareAudio);
            }
        }
    }
}