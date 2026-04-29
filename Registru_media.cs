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
        public void stocare(string path, string title, string gen )
        {
            
            MediaFile file = new MediaFile();
            file.Path = path;
            file.Title = title;
            file.GenAudio = Enum.Parse<Gen>(gen);
            file.SavedPosition = 0;
            files.Add(file);
            
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
       


    }
}