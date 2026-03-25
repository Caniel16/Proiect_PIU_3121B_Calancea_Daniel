using System.Runtime.InteropServices;
using Media_file;
namespace Registru_media
{
    public class Lista_file
    {
        List<MediaFile> files = new List<MediaFile>();
        
        public void Add(MediaFile file) {
            files.Add(file);
        }
        public List<MediaFile> GetAll()
        {
            return files;
        }
        public void load()
        {
            Console.WriteLine("Enter the path to your MP3 file:");
            string path = Console.ReadLine();

            Console.WriteLine("Enter a title for this file:");
            string title = Console.ReadLine();
            MediaFile file = new MediaFile();
            file.Title = title;
            file.Path = path;
            file.SavedPosition = 0;
            files.Add(file);
            Console.WriteLine("Added: " + title);
        }
        public MediaFile Search(string title)
        {
            foreach (MediaFile file in files)
            {
                if (file.Title == title)
                    return file;
            }
            return null;
        }
        public void Afisare()
        {
            Console.WriteLine("Your library:");
            foreach (MediaFile file in files)
            {
                Console.WriteLine("- " + file.Title + " (" + file.SavedPosition + "s)");
            }
        }


    }
}