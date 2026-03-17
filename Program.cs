using WMPLib;

WindowsMediaPlayer player = new WindowsMediaPlayer();
//player.URL = @"C:\Users\user\Downloads\William King - Gotrek & Felix, Book 3 - Daemonslayer.mp3";

Console.WriteLine("Commands: load, play, pause, stop, quit");
double savedPosition = 0;
while (true)
{
    string input = Console.ReadLine();
    if (input == "load"){
        Console.WriteLine("Introdu pathul pentru fisierul tău");
        player.URL = Console.ReadLine();
        player.controls.stop();
        savedPosition = 0;
        Console.WriteLine("Loaded" + player.URL);
    }
    else if (input == "play")
    {
        player.controls.play();
        player.controls.currentPosition = savedPosition;
        Console.WriteLine("Resuming from " + savedPosition + " seconds.");

    }
    else if (input == "pause")
    {
        player.controls.pause();
        savedPosition = player.controls.currentPosition;
        Console.WriteLine("Paused at " + savedPosition + " seconds.");
    }
    else if (input == "stop")
    {
        savedPosition = player.controls.currentPosition;
        player.controls.stop();
        Console.WriteLine("Stopped at " + savedPosition + " seconds.");
    }
    else if (input == "quit")
    {
        break;
        Console.WriteLine("Goodbye!");
    }
    else
        Console.WriteLine("Unknown command.");
}
