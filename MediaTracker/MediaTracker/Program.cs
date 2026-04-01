using System;
using Media_file;
using Registru_media;
using WMPLib;
using StocareDate;
using Media_playlist;
using static System.Net.Mime.MediaTypeNames;


WindowsMediaPlayer player = new WindowsMediaPlayer();
Storage storage = new Storage();
Lista_file library = new Lista_file();
MediaFile currentFile = null;
List<Playlist> playlists = storage.LoadPlaylists(library.GetAll());

Console.WriteLine("Commands: stocare, search, play, pause, list, stare, save, newplaylist, addtoplaylist, listplaylists, quit");
List<MediaFile> loadedFiles = storage.Load();
foreach (MediaFile f in loadedFiles)
    library.Add(f);

while (true)
{
    string input = Console.ReadLine();

    if (input == "stocare"){
        Console.WriteLine("Enter the path to your MP3 file:");
        string path = Console.ReadLine();

        Console.WriteLine("Enter a title for this file:");
        string title = Console.ReadLine();
        Console.WriteLine("Enter book genre");
        string gen = Console.ReadLine();
        library.stocare(path, title, gen);
        Console.WriteLine("Added: " + title);

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
            if (currentFile != null)
                currentFile.SavedPosition = player.controls.currentPosition;
            currentFile = found;
            player.URL = currentFile.Path;
            Console.WriteLine("Loaded: " + currentFile.Title + " (saved position: " + currentFile.SavedPosition + ") s");
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
            storage.Update(currentFile);
            Console.WriteLine("Paused: " + currentFile.Title + " at " + currentFile.SavedPosition + "s");
            
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
            Console.WriteLine("Your library:");
            foreach (MediaFile file in library.GetAll())
            {
                Console.WriteLine("- " + file.Title + " (" + file.SavedPosition + "s)");
            }
        }
    }
    else if (input == "stare")
    {
        Console.WriteLine("Enter the title to be changed");
        string title = Console.ReadLine();
        MediaFile found = library.Search(title);
        if (found == null)
        {
            Console.WriteLine("File not found.");
        }
        else
        {
            Console.WriteLine("Current state: " + found.StareAudio);
            Console.WriteLine("Add state (Ascultat, Favorit, DeAscultat, Arhivat):");
            string stareInput = Console.ReadLine();

            Stare novaStare = Enum.Parse<Stare>(stareInput);
            found.StareAudio = found.StareAudio | novaStare;

            Console.WriteLine("New state: " + found.StareAudio);
        }
    }
    else if (input == "save")
    {
        storage.Save(library.GetAll());
    }
    else if (input == "quit")
    {
        Console.WriteLine("Goodbye!");
        break;
    }
    else if (input == "newplaylist")
    {
        Console.WriteLine("Enter playlist name:");
        string name = Console.ReadLine();

        Console.WriteLine("Enter type (Personal, Public, Descoperire, Favorites):");
        string tip = Console.ReadLine();

        Console.WriteLine("Enter options (None, Shuffle, Repeat, Descarcabil):");
        string optiuni = Console.ReadLine();

        Playlist playlist = new Playlist();
        playlist.Name = name;
        playlist.Tip = Enum.Parse<TipPlaylist>(tip);
        playlist.Optiuni = Enum.Parse<OptiuniPlaylist>(optiuni);

        playlists.Add(playlist);
        storage.SavePlaylists(playlists);
        Console.WriteLine("Playlist created: " + name);
    }
    else if (input == "addtoplaylist")
    {
        Console.WriteLine("Enter playlist name:");
        string playlistName = Console.ReadLine();

        Playlist found = storage.SearchPlaylist(playlistName, playlists);
        if (found == null)
        {
            Console.WriteLine("Playlist not found.");
        }
        else
        {
            Console.WriteLine("Enter file title to add:");
            string title = Console.ReadLine();

            MediaFile file = library.Search(title);
            if (file == null)
            {
                Console.WriteLine("File not found.");
            }
            else
            {
                found.AddFile(file);
                storage.UpdatePlaylist(found, playlists);
            }
        }
    }
    else if (input == "listplaylists")
    {
        if (playlists.Count == 0)
        {
            Console.WriteLine("No playlists.");
        }
        else
        {
            foreach (Playlist p in playlists)
            {
                Console.WriteLine("- " + p.Name + " | " + p.Tip + " | " + p.Optiuni + " | " + p.Files.Count + " files");
            }
        }
    }
    else
    {
        Console.WriteLine("Unknown command. Try: load, search, play, pause, stop, list, quit");
    }
}