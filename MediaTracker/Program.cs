using WMPLib;


WindowsMediaPlayer player = new WindowsMediaPlayer();
List<MediaFile> library = new List<MediaFile>();
MediaFile currentFile = null;

Console.WriteLine("Commands: load, search, play, pause, stop, list, quit");


while (true)
{
    string input = Console.ReadLine();

    if (input == "load"){
        Console.WriteLine("Enter the path to your MP3 file:");
        string path = Console.ReadLine();

        Console.WriteLine("Enter a title for this file:");
        string title = Console.ReadLine();

        MediaFile file = new MediaFile();
        file.Title = title;
        file.Path = path;
        file.SavedPosition = 0;

        library.Add(file);
        Console.WriteLine("Added: " + title);
    }
    else if (input == "search")
    {
        Console.WriteLine("Enter title to search:");
        string title = Console.ReadLine();

        MediaFile found = null;
        foreach (MediaFile file in library)
        {
            if (file.Title == title)
            {
                found = file;
                break;
            }
        }
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
        if (library.Count == 0)
        {
            Console.WriteLine("Library is empty.");
        }
        else
        {
            Console.WriteLine("Your library:");
            foreach (MediaFile file in library)
            {
                Console.WriteLine("- " + file.Title + " (" + file.SavedPosition + "s)");
            }
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