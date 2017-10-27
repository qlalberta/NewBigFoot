using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using static System.Console;
using System.Media;
using System.Timers;

namespace WhereIsBigfoot
{
    /// <summary>
    /// game class
    /// </summary>
    public class Game
    {
        private List<Location> locations;
        private int level;
        private List<Item> items;
        private List<Character> characters;
        //help to switch characters
        private Dictionary<string, Character> charactersDictionary;
        private Dictionary<string, string> parseDict;
        List<string> allowedVerbs = new List<string>() { "save", "drop", "get", "go", "give", "look", "use", "talk", "put", "help", "quit", "inventory" };

        Commands commands = new Commands();

        private Player player;

        /// <summary>
        /// property Player
        /// </summary>
        public Player Player
        {
            get { return this.player; }
            set { this.player = value; }
        }

        /// <summary>
        /// location list
        /// </summary>
        public List<Location> Locations
        {
            get { return this.locations; }
            set { this.locations = value; }
        }

        /// <summary>
        /// item list
        /// </summary>
        public List<Item> Items
        {
            get { return this.items; }
            set { this.items = value; }
        }

        /// <summary>
        /// character list
        /// </summary>
        public List<Character> Character
        {
            get { return this.characters; }
            set { this.characters = value; }
        }

        /// <summary>
        /// parse dictionary
        /// </summary>
        public Dictionary<string, string> ParseDict
        {
            get { return this.parseDict; }
            set { this.parseDict = value; }
        }

        // Deserialize JSON from a file. 
        /// <summary>
        /// load data
        /// </summary>
        /// <param name="game"></param>
        public void LoadData(Game game)
        {
            string jsonLocationFile = @"../../Data/locations.json";
            string jsonItemFile = @"../../Data/items.json";
            string jsonCharacterFile = @"../../Data/characters.json";
            string parseDictFile = @"../../Data/parseDictionary.json";

            game.items = JsonConvert.DeserializeObject<List<Item>>(File.ReadAllText(jsonItemFile));
            game.characters = JsonConvert.DeserializeObject<List<Character>>(File.ReadAllText(jsonCharacterFile));
            game.locations = JsonConvert.DeserializeObject<List<Location>>(File.ReadAllText(jsonLocationFile));
            game.parseDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(parseDictFile));
            game.charactersDictionary = new Dictionary<string, Character>();
            // Go through each item and assign the item to the items dict in each location based on the location property of the item.
            foreach (Item item in game.items)
            {
                string name = item.Location;
                string key = item.Name;

                foreach (Location location in game.locations)
                {
                    if (location.Name == name)
                    {
                        location.Items.Add(key, item);
                    }
                }
            }

            // Go through each character and assign the character to the character dict in each location, based on the location property of the character.
            foreach (Character character in game.characters)
            {
                string name = character.Location;
                string key = character.Name;
                game.charactersDictionary.Add(key, character);

                foreach (Location location in game.locations)
                {
                    if (location.Name == name)
                    {
                        location.Characters.Add(key, character);
                    }
                }
            }
        }

        // HELPER METHODS

        // Check if an item is in a given dictionary of items and return that item or null
        private Item ItemExistsIn(Dictionary<string, Item> itemsDict, string itemName)
        {
            foreach (Item item in itemsDict.Values)
            {
                if (item.ParseValue.Contains(itemName))
                    return item;

            }
            return null;
        }

        // Check if a character is in a location and return that character or null
        private Character CharacterExistsIn(Location location, string characterName)
        {
            foreach (Character character in location.Characters.Values)
            {
                if (character.ParseValue.Contains(characterName))
                    return character;
            }
            return null;
        }

        /// <summary>
        /// After player completes an action their turn is decremented
        /// </summary>
        /// <param name="p"></param>
        private void CheckTurnsLeftForPlayer(Player player)
        {
            if (player.PlayerLocation.Name == "valley" && player.isAlive)
            {
                if (player.Turns == 4)
                {
                    Console.WriteLine($"You have {player.Turns - 1} turns to make the right decision!");
                    player.DecreaseTurnsLeft();
                }
                else
                {
                    if (player.Turns == 0)
                    {
                        commands.GameOverMan(player, player.PlayerLocation.Characters["bigfootHostile"].DescriptionLong);
                        // return;
                    }
                    else
                    {

                        player.DecreaseTurnsLeft();
                        Console.WriteLine($"--You have {player.Turns} turns left--");


                    }

                }




            }

        }

        // Handles the parsing of input from the user.
        /// <summary>
        /// parseinput
        /// </summary>
        /// <param name="prompt"></param>
        public void ParseInput(string prompt)
        {
            string input = GetInput(prompt).ToLower();

            if (input == "cheat")
            {
                Console.WriteLine("What location would you like?");
                string locationInput = ReadLine();
                Console.WriteLine("Do you want some items?");
                string itemsInput = ReadLine();
                string[] parsedItems = itemsInput.Split(' ');
                foreach (Location location in this.Locations)
                {
                    if (location.Name == locationInput)
                    {
                        this.Player.PlayerLocation = location;
                    }
                }

                foreach (Item item in this.Items)
                {
                    foreach (string itInput in parsedItems)
                        if (item.Name == itInput)
                        {
                            this.Player.Inventory.Add(item.Name, item);
                        }
                }

            }

            if (IsValidCommandInput(input) && (input != "") && input != null)
            {
                string verb = "";
                string noun = "";

                // split the incoming string and check it against the possible verbs in the parseDict.
                string[] parsed = input.Split(default(string[]), 2, StringSplitOptions.RemoveEmptyEntries);
                Console.ForegroundColor = ConsoleColor.Cyan;

                if (parsed.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    commands.WrapText($"Sorry, {this.Player.PlayerName} I didn't catch that.");
                    Console.ResetColor();
                    return;
                }
                else if (parsed.Length == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    parsed = new string[2] { parsed[0], "none" };
                    Console.ResetColor();
                }

                // create list of swear words and check to make sure none were entered.
                List<string> blueWords = new List<string>() { "shit", "fuck", "bitch", "asshole", "ass", "fag", "pussy", "dick", "cock", "damn", "bugger", "bollocks", "arsehole", "cunt" };

                foreach (string word in blueWords)
                {
                    if (parsed[0] == word || parsed[1] == word)
                    {
                        commands.WrapText($"\nWay to stay classy there, {this.Player.PlayerName}. I'm sure the trees are very impressed by your masterful command of the English language.");
                        return;
                    }
                }

                if (this.parseDict.ContainsKey(parsed[0]))
                {
                    verb = this.parseDict[parsed[0]];
                    noun = parsed[1];
                }

                if (allowedVerbs.Contains(verb))
                {
                    switch (verb)
                    {
                        case "save":
                            commands.SaveGame(this.Player);

                            break;
                        case "drop":
                            Item itemToDrop = ItemExistsIn(this.Player.Inventory, noun);
                            commands.Drop(this.Player, itemToDrop);
                            break;

                        case "get":
                            //Checks against the parseValues of items in current location.
                            Item itemToGet = ItemExistsIn(this.Player.PlayerLocation.Items, noun);


                            if (noun == "exlir" && this.Player.Level == 1)
                            {
                                if (this.Player.PlayerLocation.HasExlir)
                                {
                                    this.Player.IncreaseLife();
                                    this.Player.PlayerLocation.HasExlir = false;
                                    Console.WriteLine("You recived an Exlir!!");
                                    break;

                                }
                                else
                                {
                                    this.Player.DecreaseLife();
                                    if (this.Player.Life < 1)
                                    {
                                        commands.GameOverMan(this.Player, "You gambled too many times, GAME OVER!!");
                                        if (aTimer != null)
                                        {
                                            aTimer.Stop();

                                        }
                                    }

                                    Console.WriteLine("There is no exlir here. You lost a life, be careful.");
                                    break;

                                }
                            }
                            else if (itemToGet == null)
                            {
                                commands.WrapText($"You're not able to get the {noun}.");
                                break;
                            }
                            commands.Get(this.Player, itemToGet, this.Items);
                            break;

                        case "give":
                            // check that item is in player inventory
                            Item itemToGive = ItemExistsIn(this.Player.Inventory, noun);
                            if (itemToGive == null)
                            {
                                Console.WriteLine($"\nYou do not have a {noun} in your inventory.\n");
                                break;
                            }
                            // Get the target character for it item being given.
                            string giveResponse = GetInput($"To who do you want to give the {noun}? ").ToLower();

                            // check characters.parsevalue is in location (write method to check location) IsInLocation
                            Character targetCharacter = CharacterExistsIn(this.Player.PlayerLocation, giveResponse);
                            if (targetCharacter == null)
                            {
                                Console.WriteLine($"\n{giveResponse} is not at this location.\n");
                                break;
                            }

                            // pass player item being gotten (Item), character(Character), character dictionary
                            commands.Give(this.Player, itemToGive, targetCharacter, this.charactersDictionary);
                            break;

                        case "go":
                            commands.Go(Player, noun, this.Locations);
                            break;

                        case "inventory":
                            commands.Inventory(Player);
                            break;

                        case "look":
                            commands.Look(Player, noun);
                            break;

                        case "put":
                            Item itemToPut = ItemExistsIn(this.Player.Inventory, noun);
                            if (itemToPut == null)
                            {
                                commands.WrapText($"You don't have {noun} in your inventory.");
                                break;
                            }
                            else
                            {
                                // Get the target for the use command
                                string putTarget = GetInput($"What do you want to put the {noun} on? ").ToLower();

                                //check player inventory items & list of character in location vs. target on each asset.
                                string itemTarget = itemToPut.Target;

                                if (itemTarget != null)
                                {
                                    foreach (Item item in this.Items)
                                    {
                                        if (item.Name == itemTarget)
                                        {
                                            commands.Put(this.Player, itemToPut, item, items);
                                            break;
                                        }
                                    }
                                    foreach (Character character in this.characters)
                                    {
                                        if (character.Name == itemTarget)
                                        {
                                            commands.Put(this.Player, itemToPut, character, items);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    commands.WrapText($"You can't put the {noun} there.");
                                }
                            }
                            break;

                        case "talk":
                            // check player.location.characters.parseValues for match, pass character.
                            Character talkTarget = CharacterExistsIn(this.Player.PlayerLocation, noun);

                            if (talkTarget != null)
                            {
                                commands.Talk(Player, talkTarget);
                                break;
                            }
                            else
                            {
                                commands.WrapText($"Fond of your own voice, are you? {UppercaseFirst(noun)} isn't here to talk with you.");
                                break;
                            }

                        case "use":
                            Item itemToUse = ItemExistsIn(this.Player.Inventory, noun);
                            if (itemToUse == null)
                            {
                                commands.WrapText($"You don't have {noun} in your inventory.");
                                break;
                            }
                            else
                            {
                                // Get the target for the use commandcommands.WrapText(commands.WrapText
                                if (itemToUse.Name != "bacon")
                                {
                                    string useTarget = GetInput($"What do you want to use the {noun} on?").ToLower();

                                }

                                //check player inventory items & list of character in location vs. target on each asset.
                                string itemTarget = itemToUse.Target;

                                if (itemTarget != null)
                                {
                                    foreach (Item item in this.Items)
                                    {
                                        if (item.Name == itemTarget)
                                        {
                                            commands.Use(this.Player, itemToUse, item);
                                            break;
                                        }
                                    }
                                    foreach (Character character in this.characters)
                                    {
                                        if (character.Name == itemTarget)
                                        {
                                            commands.Use(this.Player, itemToUse, character);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    commands.WrapText($"You can't use the {noun} that way.");
                                }
                            }
                            break;

                        case "help":
                            commands.Help(Player, this.allowedVerbs);
                            break;

                        case "quit":
                            string verify = GetInput("Are you sure you want to quit? y/n: ").ToLower();
                            if (verify == "y" || verify == "yes")
                            {
                                commands.Quit(this.Player);
                                break;
                            }
                            else
                            {
                                WriteLine("Quit game cancelled.");
                                break;
                            }

                        default:
                            commands.Help(Player, this.allowedVerbs);
                            break;
                    }

                    
                    CheckTurnsLeftForPlayer(this.Player);
                   


                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    MusicPlayer("invalidCommand.wav");
                    commands.WrapText("I'm sorry, I didn't understand that. For information about what kinds of commands are available, type \"help\".");
                    //System.Windows.Forms.MessageBox.Show("Please input a valid command. Type HELP for the command list", "Error");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                MusicPlayer("invalidCommand.wav");
                //commands.WrapText("I'm sorry, I didn't understand that. For information about what kinds of commands are available, type \"help\".");
                //System.Windows.Forms.MessageBox.Show("Please input a valid command. Type HELP for the command list", "Error");
            }
        }

        // Console formatting
        /// <summary>
        /// format console setting
        /// </summary>
        public void FormatConsole()
        {
            Console.Title = "Where Is Bigfoot?";
            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// This method will insert random extra lives throughout the game
        /// </summary>
        /// <param name="locations">All locations in the game</param>
        public void AddRandomExtraLifeToLocations(List<Location> locations)
        {
            Random random = new Random();
            int randomIndex;
            for (int i = 0; i < 13; i++)
            {
                randomIndex = random.Next(0, locations.Count);
                locations[randomIndex].HasExlir = true;
            }

        }

        /// <summary>
        /// add padding to center text
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// based on code from stackflow
        public string PadBoth(string str, int length)
        {
            int spaces = length - str.Length;
            int padLeft = spaces / 2 + str.Length;
            return str.PadLeft(padLeft).PadRight(length);
        }

        /// <summary>
        /// start game method
        /// </summary>
        public void StartGame()
        {
            Console.Clear();
            Console.WindowHeight = Console.LargestWindowHeight - 10;
            Console.WindowWidth = Console.LargestWindowWidth - 10;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(PadBoth("TEAM Bug Catchers DEVELOPMENT PRESENTS", 70));
            Console.WriteLine();
            FormatConsole();
            Console.WriteLine(@"
             __          ___                     _     
             \ \        / / |                   (_)    
              \ \  /\  / /| |__   ___ _ __ ___   _ ___ 
               \ \/  \/ / | '_ \ / _ \ '__/ _ \ | / __|
                \  /\  /  | | | |  __/ | |  __/ | \__ \
              ___\/ _\/   |_|_|_|\___|_|  \___|_|_|___/
             |  _ \(_)      / _|          | ||__ \     
             | |_) |_  __ _| |_ ___   ___ | |_  ) |    
             |  _ <| |/ _` |  _/ _ \ / _ \| __|/ /     
             | |_) | | (_| | || (_) | (_) | |_|_|      
             |____/|_|\__, |_| \___/ \___/ \__(_)      
                       __/ |                           
                      |___/                            
            ");


            Console.ResetColor();

            MusicPlayer("start.wav");
            Console.ForegroundColor = ConsoleColor.Cyan;

            string input;
            do
            {
                WriteLine("What would you like to do?");
                WriteLine();
                WriteLine("Press 1 to Play New Game.");
                WriteLine("Press 2 to Play Saved Game.");
                Write("Press");
                Console.ForegroundColor = ConsoleColor.Red;
                Write(" 3");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Write(" to quit the game.\n>");
                input = ReadLine().Trim().ToLower();

            } while (input != "1" && input != "2" && input != "3");

            if (input == "3")
            {
                Console.WriteLine("See you later!");
                Environment.Exit(0);

            }
            if (input == "2")
            {
                WriteLine("What was the name of your player?");
                string playerName = ReadLine().Trim();

                this.Player = new Player(playerName);

                commands.LoadGame(this.Player, this.Locations, this.Items);
            }
            if (input == "1")
            {
                this.Player = null;
                WriteLine();
                Clear();
                Console.Write(PadBoth
          ("WHERE IS BIGFOOT is a text-based adventure game where you take on the role of a camper\n" +
          "who is trying to find that most elusive of cryptids: ", 60));
                MusicPlayer("SpiritShout.wav");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(" BIGFOOT!");
                Console.WriteLine();
                Console.WriteLine(@"
                                                  ``.------------.``                                         
                                               `.-----.`  ````````  `.-----.`                                   
                                           `---.` .-::/ ++oooooooooo++ /::-. `----`                               
                                        `-:-``-:+ooooo++++++++++++++++ooooo +:-``-:-`                            
                                     `-:. .:+ooo++ +:+syo:++/////////++++++++ooo+:. .:-`                         
                                   `:-``-+ooo++++:/ mNNNNNo:://::::////////+++++ooo+-``-:`                       
                                 `:-``:ooo++++//:/NNNNNNNNNdsooo+/::::::://///++++ooo:``-:`                     
                                -:``:ooo++ +////:-mNNNNNNNNNNNNNNNNNh/::::::::////++++oo:``:-                    
                              `:. -+oo++ +////::-/NNNNNNNNNNNNNNNNNNNN+::--:::::////++++o+- .:`                  
                             .:``/ o++++///:::::.yNNNNNNNNNNNNNNNNNNNNNd/::---:::::///++++o/``:.                 
                            -: .+oo++ +///:::-/o.+NNNNNNNNNNNNNNNNNNNNNNNo-:-----/::///++++o+. :-                
                           -: -osss +///:://+:+s+/:yNNNNNNNNNNNNNNNNNNNNNN/:-.-:os+:::///+osos- :.               
                          ./ .oosys +//+::/sys+y++-:NmNNNNNNNNNNNNNNNNNNNNm:--::oyo/:/+//osysss. /.              
                          /``+sosyso//o::+syssyss::NyoNNNNNNNNNNmdmNNNNNNNNo-o/+ys+:/o:/osyysys``/              
                         :- / syoosso//ysyhhhyoysy+.mNsomNNNNNNNNNmhoNNNNNNNNoo//sso+oyo+oyyssyy+ --             
                         + .ssysoso + osyoydhysoyyyoo + NNdsdNNNNNNNNNNsNNNNNNNNh / s + o / oyhdyosyyssyyy. +
                        .: / yyyyyhhyyyyoohdhyyyhhyy + hNNNmhNNNNNNNNNhmNNNNNNNyodyyyyyhdy + syyyyyyh / :.            
                        :. syyyyyhdhddhyyhdhyyhddhs + dNNNNoNNNNNNNNNNmNNNNNNNyomhdhhyhddyyyydhhhdy.:            
                        + `yyyyyyydddddhdddhhyhdy + smNNNNNosmmNNNNNNNNNNNNNNNm:mddddhdddhhhhdhhddd` /
                          + `yysyyyydddddhddddhhhs + dNNNNNNh / -yNNNNNNNNNmdNNNNNN + hddddhddddyyhdddddd` +
                                + `yyyyyhhdddddddddddds + NNNNNNds +/ hNNNNNNNNNNNhdNNNNNm / ddddhddddhhddhdddd` /
                        :. yyyyyyhddddddddddd + sNNNNNmoss + mNNNNNNNNNNNNNdmNNNNNdsooddddddddddddddh.:            
                        .: oyyyhddddddddddddsoNNNmmm / dy + NNNNNNNNNNNNNNNNy + dmNNmh + ydddddddhddhdhdo :.            
                        `+-hyyhddddddddddddy / NNm:sohh + NNNNNNNNNNNNNNNNNy / ssssyydddddddhhddddddd - +`            
                         :-oyyhdddddddddddddsos + yhdh:mNNNNNNNNNhhNNNNNNy + dhddddddddddddddhdddds--
                         `/`.shddddddddddddddddhdddsodNNNNNNNmho: dNNNNNNsoyhddddddddddddddddddh.`/`             
                          ./ -hdddddddddddddddddddd:mNNNNNNNd + ydy / mNNNNNNmdsoddddddddddddddddd: /.
                          `-: / dddddddddddddddddddh:NNNNNNNNm: mddy + mNNNNNNNNh / mdddddddddddddh: :-`              
                           `-: :hddddddhdddddddddd / hNNNNNNNNN:dhhhy + yNNNNNNNN:mddddhdddddddh: :-`               
                            `-:`-ydddddddddddhhyys.NNNNNNNNNh`-.....`oNNNNNNNoymddddddddddy.`:.`     `          
                         oyo / --/``+hddddhyo /:-.`  .NNNNNNNm +.`        oNNNNNNd//+oyhddddh+`.:.```.:+yy`         

            ");
                Console.ForegroundColor = ConsoleColor.Cyan;

                WriteLine("Press ANY KEY to continue......");
            }
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                Console.WriteLine("See you later!");
                Environment.Exit(0);
            }

         }
        /// <summary>
        /// Method to play different music files
        /// </summary>
        /// <param name="FileName">music file name</param>
        /// 
        public void MusicPlayer(string FileName)
        {
            string path = Directory.GetCurrentDirectory();
            SoundPlayer player = new SoundPlayer(

            $@"{path}\..\..\Music\" + FileName);
            player.Play();
        }

        private string[] GetPlayerDetails()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            commands.WrapText("\nNow you need to decide who you are: ");

            string name = "";

            do
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                name = GetInput("What is your name?\n> ");
                if (!IsValidStartingInput(name) && name != null)
                {
                    MusicPlayer("invalidCommand.wav");
                    Console.ForegroundColor = ConsoleColor.Red;
                    commands.WrapText($"\nHm...I didn't quite get that. Names usually contain just letters (and maybe the occasional hyphen).");
                    System.Windows.Forms.MessageBox.Show("Invalid input", "Error");
                }
            } while (!IsValidStartingInput(name) || name == null);


            string[] deets = { name };
            commands.WrapText("\nNow that that's done with:");
            return deets;
        }

        // Check against a regex string that allows all letters, spaces, apostrophes, dashes.
        private static bool IsValidStartingInput(string str)
        {
            Regex regexString = new Regex(@"^[a-zA-Z\s-'\.][^\n\r]+\z");
            bool result = regexString.IsMatch(str);
            return result;
        }

        // check against a regex string that takes one word of just letters, or two words separated by a space.
        // Format is beginning of line, one or more letters of any length, one or no spaces, zero or more letters of any length, end of line.
        private static bool IsValidCommandInput(string str)
        {
            Regex regexString = new Regex(@"^[a-zA-Z]+\s?[a-zA-Z]*\z");
            bool result = regexString.IsMatch(str);
            return result;
        }

        // Take in string input from the user. Corrects null input.
        /// <summary>
        /// get  input method
        /// </summary>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public static string GetInput(string prompt)
        {
            Console.Write(prompt);
            string input = ReadLine();
            while (input == null)
            {
                Console.Write(prompt);
                input = ReadLine();
            }
            return input.Trim();
        }

        // Capitalize first letter of a string. From https://www.dotnetperls.com/uppercase-first-letter;
        private static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        /// <summary>
        /// refactored part of code from Main method
        /// </summary>
        /// <param name="game"></param>
        public void NewGame(Game game, int level)
        {
            int settings = GameSettings.GetGameSettings();
            GameSettings gameSettings = new GameSettings(settings);
            game.MusicPlayer("bomb.wav");
            game.commands.userSpeed = gameSettings.TypeSpeedConverter();


            // create Player instance
            string[] playerDetails = game.GetPlayerDetails();
            Player newPlayer = new Player(playerDetails[0]);
            newPlayer.Level = level;
            Console.ForegroundColor = ConsoleColor.Cyan;
            game.MusicPlayer("loadgame.wav");
            //game.commands.WrapText($"Press ANY KEY to start your game...");
            //ReadKey();

            //set the game settings
            foreach (Location location in game.Locations)
            {
                if (location.Name == "tent")
                    newPlayer.PlayerLocation = location;
            }
            foreach (Item item in game.Items)
            {
                if (item.Name == "cellPhone")
                    newPlayer.Inventory.Add("cellPhone", item);
            }

            // Assign Player instance to game
            game.Player = newPlayer;

            // Show starting room
            Console.WriteLine();

            //Add status bar
            Console.Clear();
            commands.DisplayStatus(newPlayer.PlayerLocation.Title, newPlayer.PlayerLocation.Items.Count, newPlayer.Inventory.Count, newPlayer.Life, newPlayer.Level);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Press SPACEBAR to skip the description...");
            Console.ForegroundColor = ConsoleColor.Cyan;
            game.commands.ShowLocation(game.Player.PlayerLocation);
            //added type writer sound
            game.MusicPlayer("typewriter.wav");
            SoundPlayer StopPlayer = new SoundPlayer();
            StopPlayer.Stop();
        }

        public static System.Timers.Timer aTimer;
        public static void SetTimer(Player player)
        {
            // Create a timer with a two second interval.

            Timer12 ti = new Timer12(0, 05);
            Commands command = new Commands();
            aTimer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e, ti, command, player); ;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e, Timer12 ti, Commands cmd, Player player)
        {
            ti.CountDown();
            if (ti.TimeUp)
            {
                aTimer.Stop();
                cmd.GameOverMan(player, "\n**********************************\n\nYou are out of Time\n GAME OVER\n\n**********************************");
                
            }
            else
            {
                Console.Title = Console.Title.Remove(16);
                Console.Title += $"? -- Time {ti.GetTime()}";

            }
        }

        static void Main(string[] args)
        {
            Game game = new Game();
            game.LoadData(game);
            game.StartGame();

            if (game.Player == null)
            {
                string input;
                do
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine();
                    Write("GAME SETTINGS:\n\nThere are THREE secret levels.\nChoose 1, 2 or 3 to have different secret experience.\n>");
                    input = ReadLine().Trim().ToLower();
                    WriteLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                } while (input != "1" && input != "2" && input != "3");
                int level;
                if (input == "1")
                {
                    Console.WriteLine("You will receive Random extra lives at different locations!");
                    Console.WriteLine();
                    level = 1;
                    game.AddRandomExtraLifeToLocations(game.Locations);


                    game.NewGame(game, level);
                }
                if (input == "2")
                {
                    Console.WriteLine("You won't have Random extra lives but there is no time limit to find big foot!");
                    Console.WriteLine();
                    level = 2;
                    game.NewGame(game, level);
                    //Console.ForegroundColor = ConsoleColor.Cyan;
                }
                if (input == "3")
                {
                    WriteLine("Now you will have to find big foot in certain time!");
                    level = 3;
                    game.NewGame(game, level);
                    SetTimer(game.Player);
                }
            }

            do
            {
                if (game.Player.isAlive == false)
                {
                    if (aTimer != null)
                    {
                        aTimer.Stop();
                    }
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(" Do you want to play again? y/n ");
                    string playAgainInput = Console.ReadLine();
                    if (playAgainInput.ToLower() == "y")
                    {
                        game.Player.isAlive = true;
                        game.LoadData(game);
                        game.StartGame();

                        if (game.Player == null)
                        {
                            string levelInput;
                            do
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine();
                                Write("GAME SETTINGS:\n\nThere are THREE secret levels.\nChoose 1, 2 or 3 to have different secret experience.\n>");
                                levelInput = ReadLine().Trim().ToLower();
                                WriteLine();
                                Console.ForegroundColor = ConsoleColor.Cyan;
                            } while (levelInput != "1" && levelInput != "2" && levelInput != "3");
                            int level;
                            if (levelInput == "1")
                            {
                                Console.WriteLine("You will receive Random extra lives at different locations!");
                                Console.WriteLine();
                                level = 1;
                                game.AddRandomExtraLifeToLocations(game.Locations);
                                game.NewGame(game, level);
                            }
                            if (levelInput == "2")
                            {
                                Console.WriteLine("You won't have Random extra lives but there is no time limit to find big foot!");
                                Console.WriteLine();
                                level = 2;
                                game.NewGame(game, level);
                                Console.ForegroundColor = ConsoleColor.Cyan;

                            }
                            if (levelInput == "3")
                            {
                                WriteLine("Now you will have to find big foot in certain time!");
                                level = 3;
                                game.NewGame(game, level);
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                SetTimer(game.Player);
                            }
                        }


                    }

                    else
                    {
                        game.Player.GameIsRunning = false;
                    }

                }
                else

                {

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    game.ParseInput("\nWhat would you like to do? Type HELP for a list of commands\n> ");

                }



            } while (game.Player.GameIsRunning == true);



        }
    }
}