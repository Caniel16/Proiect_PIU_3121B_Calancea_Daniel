namespace Media_file
{
    public class MediaFile
    {
        public string Title;
        public string Path;
        public double SavedPosition;
    }
    [Flags]
    public enum Stare
    {
        None = 0,
        Ascultat = 1,
        Favorit = 2,
        DeAscultat = 4,
        Arhivat = 8
    }
    public enum Gen
    {
        Fantezie,
        ScientificFiction,
        Comedie,
        Thriller,
        NonFictiune
    }
}
