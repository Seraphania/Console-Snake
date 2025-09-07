using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Media;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

class Program
{
    //current score
    public static int score = 0;
    public static string scoreTXT = "Score";
    public static bool newScore = false;
    public static bool isFastMode = false;

    // Define the path to the storage file
    private static readonly string FilePath = "Storage.txt";
    // A dictionary to hold all our settings in memory
    private static readonly Dictionary<string, string> Settings = new Dictionary<string, string>();
    // Load all settings from the file into the Dictionary
    private static void LoadSettings()
    {
        // Clear the dictionary before loading
        Settings.Clear();

        // Check if the file exists before trying to read it
        if (!File.Exists(FilePath))
        {
            // Create a file with default values if it doesn't exist
            SetDefaultSettings();
            SaveSettings();
            return;
        }

        try
        {
            // Read all lines from the file
            var lines = File.ReadAllLines(FilePath);
            foreach (var line in lines)
            {
                // Skip empty lines and lines without an '=' sign
                if (string.IsNullOrWhiteSpace(line) || !line.Contains('='))
                    continue;

                // Split the line into key and value parts
                var parts = line.Split('=', 2);
                var key = parts[0].Trim();
                var value = parts[1].Trim();

                // Add the key-value pair to the dictionary
                Settings[key] = value;
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error reading settings file: {ex.Message}");
            // Optionally, load default settings if the file is corrupted
            SetDefaultSettings();
        }
    }

    // Save all settings from the Dictionary back to the file
    private static void SaveSettings()
    {
        try
        {
            // Create lines in the "key=value" format
            var lines = Settings.Select(kvp => $"{kvp.Key}={kvp.Value}");
            // Write all lines to the file, overwriting the previous content
            File.WriteAllLines(FilePath, lines);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error writing settings file: {ex.Message}");
        }
    }

    // Set default values for all settings (used if file doesn't exist)
    private static void SetDefaultSettings()
    {
        Settings["highscore1"] = "1500";
        Settings["highscore2"] = "1000";
        Settings["highscore3"] = "500";
        Settings["left"] = "a";
        Settings["right"] = "d";
        Settings["up"] = "w";
        Settings["down"] = "s";
        Settings["select"] = "l";
    }
    public static void End()
    {
        Console.CursorVisible = true;
        Console.Clear();
        SaveSettings();
        Environment.Exit(0);
    }
    static void Print(string sentence, int partX, int partY)
    {
        int sentenceLength = 0;
        var worldSize = (X: 122, Y: 32);
        foreach (char c in sentence)
        {
            sentenceLength++;
        }
        if (sentenceLength <= worldSize.X && worldSize.Y / partY >= 1 && partX != 0 && partY != 0)
        {
            Console.SetCursorPosition((int)((worldSize.X - sentenceLength) / partX), (int)(worldSize.Y / partY - 1));
        }
        else
        {
            try
            {
                Console.SetCursorPosition((int)((worldSize.X - sentenceLength) / partX), (int)(worldSize.Y / partY - 1));
            }
            catch (DivideByZeroException ex) //Make just there is no bugs in the code
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Error: Divice by Zero!! ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\"" + ex.Message + "\"");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Please Fix The Proglem..");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.ReadKey();
            }
        }

        Console.Write(sentence);
    }
    static void Main(string[] args)
    {
        LoadSettings();
        Console.CursorVisible = false;
    
        // If loading failed or no file exists, set defaults
        if (Settings.Count == 0)
        {
            SetDefaultSettings();
            SaveSettings();
        }
        //Screen Size (122, 32);
        //Load scene
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        for (int i = 1; i <= 3; i++)
        {
            for (int j = 1; j <= 3; j++)
            {
                Console.Clear();
                Print("Loading" + new string('.', j), 2, 2);
                Thread.Sleep(500);
            }
        }
        LoadSettings(); //Load the settings
        Console.Clear();
        Print("~ Start ~", 2, 2);
        Thread.Sleep(1000);

        MainMenu();
    }
    static void MainMenu()
    {
        LoadSettings();

        string[] consoleArt = {
            "  ____                      _        ____              _  __",
            " / ___|___  _ __  ___  ___ | | ___  / ___| _ __   ___ | |/ / ___",
            "| |   / _ \\| '_ \\/ __|/ _ \\| |/ _ \\ \\___ \\| '_ \\ / _ '| ' / / _ \\",
            "| |__| (_) | | | \\__ \\ (_) | |  __/  ___) | | | | (_| | . \\|  __/",
            " \\____\\___/|_| |_|___/\\___/|_|\\___| |____/|_| |_|\\__,_|_|\\_\\\\___|"
        };
        string start = "~    Start    ~";
        string heightScores = "~Height Scores~";
        string settings = "~   Setting   ~";
        string exit = "~     Exit    ~";
        string arrow = "=>";
        string empty = "  ";
        int choose = -4;
        bool select = false;
        
        Console.Clear();
        
        for (int i = 0; i < consoleArt.Length; i++)
        {
            Console.SetCursorPosition((122 - consoleArt[2].Length) / 2, i + 4);
            Console.WriteLine(consoleArt[i]);
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition((122 - start.Length) / 2, 32 / 2 - 4);
        Console.WriteLine(start);
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetCursorPosition((122 - heightScores.Length) / 2, 32 / 2 - 2);
        Console.WriteLine(heightScores);
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.SetCursorPosition((122 - settings.Length) / 2, 32 / 2);
        Console.WriteLine(settings);
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition((122 - exit.Length) / 2, 32 / 2 + 2);
        Console.WriteLine(exit);

        while (true)
        {
            ConsoleKeyInfo? key = null;
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey(true);
            }

            if (key.HasValue)
            {
                if (key.Value.KeyChar == Settings["up"][0])
                {
                    choose -= 2;
                    if (choose < -4) { choose = 2; }
                }
                else if (key.Value.KeyChar == Settings["down"][0])
                {
                    choose += 2;
                    if (choose > 2) { choose = -4; }
                }
                else if (key.Value.KeyChar == Settings["select"][0]) {
                    select = true;
                }
            }

            //Display arrow

            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < 8; i += 2)
            {
                if (i - 4 != choose)
                {                    
                    Console.SetCursorPosition((122 - 22) / 2, 32 / 2 + i - 4);
                    Console.WriteLine(empty);
                }
            }
            Console.SetCursorPosition((122 - 22) / 2, 32 / 2 + choose);
            Console.WriteLine(arrow);

            Console.ForegroundColor = ConsoleColor.White;

            if (select)
            {
                switch (choose)
                {
                    case -4:
                        GameStart();
                        break;
                    case -2:
                        SaveSettings();
                        LoadSettings();
                        HighscoreMenu();
                        break;

                    case 0:
                        SaveSettings();
                        LoadSettings();
                        Setting();
                        break;

                    case 2:
                        End();
                        break;
                }
            }

            select = false;

            Thread.Sleep(100);
        }
    }
    static void HighscoreMenu()
    {
        
        LoadSettings(); //Load the settings
        string first = Settings["highscore1"];
        string second = Settings["highscore2"];
        string third = Settings["highscore3"];
        string title = "~ H I G H  S C O R E S ~";
        string exit = "~ Exit ~";
        string arrow = "=>";
        int choose = 6;
        bool select = false;

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetCursorPosition((122 - title.Length) / 2, 32 / 2 - 4);
        Console.WriteLine(title);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition((122 - first.Length) / 2, 32 / 2 - 2);
        Console.WriteLine(first);
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.SetCursorPosition((122 - second.Length) / 2, 32 / 2);
        Console.WriteLine(second);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition((122 - third.Length) / 2, 32 / 2 + 2);
        Console.WriteLine(third);
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition((122 - exit.Length) / 2, 32 / 2 + 6);
        Console.WriteLine(exit);

        while (true)
        {
            ConsoleKeyInfo? key = null;
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey(true);
            }

            if (key.HasValue)
            {
                if (key.Value.KeyChar == Settings["select"][0])
                {
                    select = true;   
                }
            }

            if (select)
            {
                switch (choose)
                {
                    case 6:
                        MainMenu();
                        break;
                }
            }

            select = false;

            //Display arrow
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition((122 - 22) / 2, 32 / 2 + choose);
            Console.WriteLine(arrow);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    static void SettingDisplay()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
        for (int i = 0; i < 25; i++)
        {
            Console.SetCursorPosition(122 * 1 / 16, 5 + i);
            Console.WriteLine("|");
            Console.SetCursorPosition(122 * 7 / 16, 5 + i);
            Console.WriteLine("|");
            Console.SetCursorPosition(122 * 9 / 16, 5 + i);
            Console.WriteLine("|");
            Console.SetCursorPosition(122 * 15 / 16, 5 + i);
            Console.WriteLine("|");
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(122 * 7 / 32, 5);
        Console.WriteLine("CONTROL");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetCursorPosition(122 * 1 / 8, 7);
        Console.WriteLine("Inputs: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(122 * 1 / 8, 9);
        Console.WriteLine("Left:");
        Console.SetCursorPosition(122 * 1 / 8, 11);
        Console.WriteLine("Right:");
        Console.SetCursorPosition(122 * 1 / 8, 13);
        Console.WriteLine("Up:");
        Console.SetCursorPosition(122 * 1 / 8, 15);
        Console.WriteLine("Down:");
        Console.SetCursorPosition(122 * 1 / 8, 17);
        Console.WriteLine("Select:");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.SetCursorPosition(122 * 3 / 8, 9);
        Console.WriteLine(Settings["left"]);
        Console.SetCursorPosition(122 * 3 / 8, 11);
        Console.WriteLine(Settings["right"]);
        Console.SetCursorPosition(122 * 3 / 8, 13);
        Console.WriteLine(Settings["up"]);
        Console.SetCursorPosition(122 * 3 / 8, 15);
        Console.WriteLine(Settings["down"]);
        Console.SetCursorPosition(122 * 3 / 8, 17);
        Console.WriteLine(Settings["select"]);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(122 * 23 / 32, 5);
        Console.WriteLine("STORAGE");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetCursorPosition(122 * 9 / 16 + 2, 7);
        Console.WriteLine("Reset the settings = Lose height score data");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(122 * 10 / 16 - 4, 9);
        Console.WriteLine("Reset The Settings: Are you sure Sure?!");
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(122 * 23 / 32, 11);
        Console.WriteLine("Fast Mode:");
        if (isFastMode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        Console.SetCursorPosition(122 * 14 / 16 - 2, 11);
        Console.WriteLine("" + isFastMode + "");
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(122 * 12 / 16 - 2, 13);
        Console.WriteLine("Exit");
    }
    static void Setting()
    {
        SettingDisplay();
        string arrowLeft = "<=";
        string arrowRight = "=>";
        string empty = "  ";
        int choose = 0;
        bool select = false;
        while (true)
        {
            ConsoleKeyInfo? key = null;
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey(true);
            }

            if (key.HasValue)
            {
                if (key.Value.KeyChar == Settings["up"][0])
                {
                    choose -= 2;
                    if (choose < 0) choose = 18;
                }
                else if (key.Value.KeyChar == Settings["down"][0])
                {
                    choose += 2;
                    if (choose > 18) choose = 0;
                }
                else if (key.Value.KeyChar == Settings["select"][0])
                {
                    select = true;
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < 20; i += 2)
            {
                if (i < 10)
                {
                    if (i != choose)
                    {
                        Console.SetCursorPosition(122 * 7 / 16 + 1, 9 + i);
                        Console.WriteLine(empty);
                    }
                }
                else if (i >= 10)
                {
                    if (i != choose)
                    {
                        Console.SetCursorPosition(122 * 9 / 16 - 2, 9 - 10 + i);
                        Console.WriteLine(empty);
                    }
                }
            }
            if (choose < 10)
            {

                Console.SetCursorPosition(122 * 7 / 16 + 1, 9 + choose);
                Console.WriteLine(arrowLeft);
            }
            else if (choose >= 10)
            {


                Console.SetCursorPosition(122 * 9 / 16 - 2, 9 - 10 + choose);
                Console.WriteLine(arrowRight);
            }
            if (select)
            {
                switch (choose)
                {
                    case 0:
                        ChangeKeys("left", choose);
                        break;
                    case 2:
                        ChangeKeys("right", choose);
                        break;
                    case 4:
                        ChangeKeys("up", choose);
                        break;
                    case 6:
                        ChangeKeys("down", choose);
                        break;
                    case 8:
                        ChangeKeys("select", choose);
                        break;
                    case 10:
                        SetDefaultSettings();
                        SaveSettings();
                        break;
                    case 12:
                        isFastMode = !isFastMode;
                        break;
                    case 14:
                        SaveSettings();
                        MainMenu();
                        break;
                    case 16:
                        break;
                }
            }

            if (select)
            {
                SettingDisplay();
                select = false;
            }
        }
    }
    static void ChangeKeys(string Key, int num)
    {
        string[] keys = [
            "left",
            "right",
            "up",
            "down",
        ];
        bool usedKey = true;
        while (usedKey)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(122 * 3 / 8, 9 + num);
            Console.Write("");
            Console.CursorVisible = true;
            Console.SetCursorPosition(122 * 3 / 8, 9 + num);
            Console.WriteLine(Settings[Key] = Console.ReadKey(true).KeyChar.ToString());
            Console.CursorVisible = false;
            foreach (string used in keys)
            {
                if (used != Key)
                {
                    if (Settings[used] == Settings[Key])
                    {
                        usedKey = true;
                        break;
                    }
                    usedKey = false;
                }
            }
        }
    }
    static void GameStart()
    {
        Console.Clear();
        //game
        bool gameRun = true;
        Random rnd = new Random();
        DateTime lastFrameTime = DateTime.Now;
        TimeSpan frameDelay = TimeSpan.FromMilliseconds(250); // 4 FPS - normal speed
        TimeSpan fastFrameDelay = TimeSpan.FromMilliseconds(100); // 10 FPS - fast speed 
        int frames = 0; //Count the frames
        int screenSpace = 5;

        //wall
        int leftWall = 28 + screenSpace * 2 - 2;
        int rightWall = 96 - 2 - screenSpace * 2 + 2;
        int topWall = 0 + screenSpace - 1;
        int bottomWall = 32 - screenSpace + 1;
        char wallBlock = '▯';

        //inputs 
        ConsoleKeyInfo? lastKey = new ConsoleKeyInfo(
            Settings["right"][0],          // The character
            (ConsoleKey)char.ToUpper(Settings["right"][0]), // The ConsoleKey enum value
            false,        // shift modifier
            false,        // alt modifier
            false         // control modifier
        );

        //Snake 
        string snakeBody = "⨴⨵";
        var snake = new List<(int X, int Y)> //snake body part positions
        {
            (X: 60, Y: 15),
            (X: 58, Y: 15),
            (X: 56, Y: 15),
        };
        bool keyPressed = false;

        //candy
        string candy = "⨭⨮";
        var candyPos = (X: 68, Y: 15); //First positon
        bool candyEaten = false;
        score = 0;

        //Display map
        Console.ForegroundColor = ConsoleColor.White;
        for (int x = leftWall; x <= rightWall; x++) // Draw top horizontal wall
        {
            Console.SetCursorPosition(x, topWall);
            Console.Write(wallBlock);
        }

        for (int x = leftWall; x <= rightWall; x++) // Draw bottom horizontal wall
        {
            Console.SetCursorPosition(x, bottomWall);
            Console.Write(wallBlock);
        }

        for (int y = topWall; y <= bottomWall; y++) // Draw left vertical wall
        {
            Console.SetCursorPosition(leftWall, y);
            Console.Write(new string(wallBlock, 2));
        }

        for (int y = topWall; y <= bottomWall; y++) // Draw right vertical wall
        {
            Console.SetCursorPosition(rightWall, y);
            Console.Write(new string(wallBlock, 2));
        }

        while (gameRun)
        {
            //Game Logic
            DateTime currentTime = DateTime.Now;
            TimeSpan elapsed = currentTime - lastFrameTime;

            // Process input - only accept valid movement keys
            if (Console.KeyAvailable && !keyPressed)
            {
                ConsoleKeyInfo Key = Console.ReadKey(true);
                // Only process valid movement keys
                if (Key.KeyChar == Settings["up"][0] ||
                    Key.KeyChar == Settings["left"][0] ||
                    Key.KeyChar == Settings["down"][0] ||
                    Key.KeyChar == Settings["right"][0])
                {
                    //Make the snake can't move backwards
                    if (lastKey.Value.KeyChar == Settings["up"][0])
                    {
                        if (Key.KeyChar != Settings["down"][0]) lastKey = Key;
                    }
                    else if (lastKey.Value.KeyChar == Settings["left"][0])
                    {
                        if (Key.KeyChar != Settings["right"][0]) lastKey = Key;
                    }
                    else if (lastKey.Value.KeyChar == Settings["down"][0])
                    {
                        if (Key.KeyChar != Settings["up"][0]) lastKey = Key;
                    }
                    else if (lastKey.Value.KeyChar == Settings["right"][0])
                    {
                        if (Key.KeyChar != Settings["left"][0]) lastKey = Key;
                    }
                    keyPressed = true;
                }
            }

            TimeSpan currentFrameDelay = isFastMode ? fastFrameDelay : frameDelay;

            // Only update at fixed intervals (every 250ms)
            if (elapsed >= currentFrameDelay)
            {
                // Process movement if a key was pressed
                if (lastKey.HasValue)
                {
                    //Candy eaten
                    if (snake[0] == candyPos)
                    {
                        candyEaten = true;
                        snake.Add(snake[^1]);
                        bool correctPos = false;
                        do
                        {
                            candyPos = (rnd.Next((leftWall + 2) / 2, (rightWall - 1) / 2) * 2, rnd.Next(topWall + 1, bottomWall - 1));
                            for (int i = 0; i < snake.Count; i++)
                            {
                                if (candyPos == snake[i])
                                {
                                    correctPos = false;
                                    break;
                                }
                                correctPos = true;
                            }
                        }
                        while (!correctPos);
                        score += 100;
                    }

                    for (int i = snake.Count - 1; i > 0; i--)
                    {
                        snake[i] = snake[i - 1];
                    }

                    if (lastKey.Value.KeyChar == Settings["up"][0])
                    {
                        if (snake[0].Y - 1 > topWall) snake[0] = (snake[0].X, snake[0].Y - 1);
                    }
                    else if (lastKey.Value.KeyChar == Settings["left"][0])
                    {
                        if (snake[0].X - 2 > leftWall) snake[0] = (snake[0].X - 2, snake[0].Y);
                    }
                    else if (lastKey.Value.KeyChar == Settings["down"][0])
                    {
                        if (snake[0].Y + 1 < bottomWall) snake[0] = (snake[0].X, snake[0].Y + 1);
                    }
                    else if (lastKey.Value.KeyChar == Settings["right"][0])
                    {
                        if (snake[0].X + 2 < rightWall) snake[0] = (snake[0].X + 2, snake[0].Y);
                    }
                    keyPressed = false;
                }

                //Game over condition
                for (int i = 1; i < snake.Count; i++)
                {
                    if (snake[0] == snake[i])
                    {
                        gameRun = false;
                    }
                }

                //Game Display
                if (gameRun)
                {
                    for (int y = topWall + 1; y < bottomWall; y++)
                    {
                        // Create a string of spaces for the entire width of the play area
                        string clearLine = new string(' ', rightWall - leftWall - 2);

                        // Position cursor and write the spaces
                        Console.SetCursorPosition(leftWall + 2, y);
                        Console.Write(clearLine);
                    }

                    //Diaplay candy
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(candyPos.X, candyPos.Y);
                    Console.WriteLine(candy);
                    Console.ForegroundColor = ConsoleColor.White;

                    //Display the snake
                    for (int i = 0; i < snake.Count; i++)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        if (i == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }

                        Console.SetCursorPosition(snake[i].X, snake[i].Y);
                        Console.WriteLine(snakeBody);

                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (candyEaten)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.SetCursorPosition(snake[^1].X, snake[^1].Y);
                        Console.WriteLine(snakeBody);
                        Console.ForegroundColor = ConsoleColor.White;
                        candyEaten = false;
                    }
                    //update the height score
                    if (score > int.Parse(Settings["highscore3"]))
                    {
                        newScore = true;
                    }

                    //Display informations
                    if (newScore)
                    {
                        if (score > int.Parse(Settings["highscore1"]))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.SetCursorPosition((122 - ("!! " + scoreTXT + ": " + score + " !!").Length) / 2, 2);
                            Console.WriteLine("!! " + scoreTXT + ": " + score + " !!");
                        }
                        else if (score > int.Parse(Settings["highscore2"]))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.SetCursorPosition((122 - ("! " + scoreTXT + ": " + score + " !").Length) / 2, 2);
                            Console.WriteLine("! " + scoreTXT + ": " + score + " !");
                        }
                        else if (score > int.Parse(Settings["highscore3"]))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.SetCursorPosition((122 - ("~ " + scoreTXT + ": " + score + " ~").Length) / 2, 2);
                            Console.WriteLine("~ " + scoreTXT + ": " + score + " ~");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.SetCursorPosition((122 - ("~ " + scoreTXT + ": " + score + " ~").Length) / 2, 2);
                        Console.WriteLine("~ " + scoreTXT + ": " + score + " ~");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    // Console.WriteLine("Snake length: " + snake.Count);
                    // Console.WriteLine("Candy eaten times: " + candyEatenTimes);
                    // Console.WriteLine("Frames Passed: " + frames);
                    frames++;

                    lastFrameTime = currentTime;
                }
            }
            // Small sleep to prevent CPU overuse
            Thread.Sleep(10);
        }
        GameOver();
    }

    static void GameOver()
    {
        string gameOver = "!!  G A M E  O V E R  !!", rest = "~ Rest ~", exit = "~ Exit ~", menu = "~ Menu ~";
        string arrow = "=>";
        string empty = "  ";
        int choose = -2;
        bool select = false;

        Console.Clear();

        if (newScore)
        {
            if (score > int.Parse(Settings["highscore1"]))
            {
                Settings["highscore3"] = Settings["highscore2"];
                Settings["highscore2"] = Settings["highscore1"];
                Settings["highscore1"] = score.ToString();
            }
            else if (score > int.Parse(Settings["highscore2"]))
            {
                Settings["highscore3"] = Settings["highscore2"];
                Settings["highscore2"] =  score.ToString();
            }
            else if (score > int.Parse(Settings["highscore3"]))
            {
                Settings["highscore3"] = score.ToString();
            }
            SaveSettings();
        }

        while (true)
        {
            ConsoleKeyInfo? key = null;
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey(true);
            }

            if (key.HasValue)
            {
                if (key.Value.KeyChar == Settings["up"][0])
                {
                    choose -= 2;
                    if (choose < -2) { choose = 2; }
                }
                else if (key.Value.KeyChar == Settings["down"][0])
                {
                    choose += 2;
                    if (choose > 2) { choose = -2; }
                }
                else if (key.Value.KeyChar == Settings["select"][0]) {
                    select = true;
                }
            }

            //Display chooses
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition((122 - gameOver.Length) / 2, 32 / 2 - 6);
            Console.WriteLine(gameOver);

            if (newScore)
            {
                if (score == int.Parse(Settings["highscore1"]))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition((122 - ("!!! " + scoreTXT + ": " + score + " !!!").Length) / 2, 32 / 2 - 4);
                    Console.WriteLine("!!! " + scoreTXT + ": " + score + " !!!");      
                }
                else if (score == int.Parse(Settings["highscore2"]))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.SetCursorPosition((122 - ("! " + scoreTXT + ": " + score + " !").Length) / 2, 32 / 2 - 4);
                    Console.WriteLine("! " + scoreTXT + ": " + score + " !");      
                }
                else if (score == int.Parse(Settings["highscore3"]))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition((122 - ("~ " + scoreTXT + ": " + score + " ~").Length) / 2, 32 / 2 - 4);
                    Console.WriteLine("~ " + scoreTXT + ": " + score + " ~");                    
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.SetCursorPosition((122 - ("~ " + scoreTXT + ": " + score + " ~").Length) / 2, 32 / 2 - 4);
                Console.WriteLine("~ " + scoreTXT + ": " + score + " ~");
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition((122 - rest.Length) / 2, 32 / 2 - 2);
            Console.WriteLine(rest);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.SetCursorPosition((122 - menu.Length) / 2, 32 / 2);
            Console.WriteLine(menu);
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition((122 - exit.Length) / 2, 32 / 2 + 2);
            Console.WriteLine(exit);

            //Display arrow
            Console.ForegroundColor = ConsoleColor.Green;

            for (int i = 0; i < 6; i += 2)
            {
                if (i - 2 != choose)
                {   
                    Console.SetCursorPosition((122 - 14) / 2, 32 / 2 + i - 2);
                    Console.WriteLine(empty);
                }
            }

            Console.SetCursorPosition((122 - 14) / 2, 32 / 2 + choose);
            Console.WriteLine(arrow);

            Console.ForegroundColor = ConsoleColor.White;

            if (select)
            {
                newScore = false;
                SaveSettings();

                switch (choose)
                {
                    case -2:
                        GameStart();
                        break;

                    case 0:
                        MainMenu();
                        break;

                    case 2:
                        End();
                        break;
                }
            }

            select = false;

            Thread.Sleep(100);
        }
    }
}
