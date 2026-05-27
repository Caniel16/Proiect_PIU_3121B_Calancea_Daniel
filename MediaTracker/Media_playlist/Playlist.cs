using Media_file;

namespace Media_playlist
{
    // Tipul cartii.
    public enum TipCarte
    {
        Personal,
        Public,
        Descoperire,
        Favorites
    }

    // Setari care se pot combina (ex: amestecare + repetare).
    [Flags]
    public enum OptiuniCarte
    {
        None = 0,
        Shuffle = 1,
        Repeat = 2,
        Descarcabil = 4
    }

    // O carte audio - o lista ordonata de capitole.
    public class Carte
    {
        public string Name { get; set; }
        public List<Capitol> Files { get; set; } = new List<Capitol>();
        public TipCarte Tip { get; set; }
        public OptiuniCarte Optiuni { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
