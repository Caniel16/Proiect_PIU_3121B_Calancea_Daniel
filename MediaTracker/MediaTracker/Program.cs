using WMPLib;
using Registru_media;
using Media_file;
WindowsMediaPlayer player = new WindowsMediaPlayer();

Lista_file library = new Lista_file();
MediaFile currentFile = null;

Console.WriteLine("Commands: load, search, play, pause, stop, list, quit");


while (true)
{
    string input = Console.ReadLine();

    if (input == "load"){
        library.load();
      
    }
    else if (input == "search")
    {
        Console.WriteLine("Enter title to search:");
        string title = Console.ReadLine();

        
        MediaFile found = library.Search(title);

        if (found == null)
        {
            Console.WriteLine("No file found with that title.");
        }

        else
        {
            currentFile = found;
            player.URL = currentFile.Path;
            Console.WriteLine("Loaded: " + currentFile.Title + " (saved position: " + currentFile.SavedPosition + "s)");
            Console.WriteLine("Type play when ready.");
        }
    }
    else if (input == "play")
    {
        if (currentFile == null)
        {
            Console.WriteLine("No file loaded. Use search first.");
        }
        else
        {
            player.controls.play();
            player.controls.currentPosition = currentFile.SavedPosition;
            Console.WriteLine("Resuming: " + currentFile.Title + " from " + currentFile.SavedPosition + "s");
        }
    }
    else if (input == "pause")
    {
        if (currentFile == null)
        {
            Console.WriteLine("Nothing is playing.");
        }
        else
        {
            currentFile.SavedPosition = player.controls.currentPosition;
            player.controls.pause();
            Console.WriteLine("Paused: " + currentFile.Title + " at " + currentFile.SavedPosition + "s");
        }
    }
    else if (input == "stop")
    {
        if (currentFile == null)
        {
            Console.WriteLine("Nothing is playing.");
        }
        else
        {
            currentFile.SavedPosition = player.controls.currentPosition;
            player.controls.stop();
            Console.WriteLine("Stopped: " + currentFile.Title + " at " + currentFile.SavedPosition + "s");
        }
    }
    else if (input == "list")
    {
        if (library.GetAll().Count == 0)
        {
            Console.WriteLine("Library is empty.");
        }
        else
        {
            library.Afisare();
        }
    }
    else if (input == "quit")
    {
        Console.WriteLine("Goodbye!");
        break;
    }
    else
    {
        Console.WriteLine("Unknown command. Try: load, search, play, pause, stop, list, quit");
    }
}