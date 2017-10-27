using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Timers;
using System.Data;
using System.IO;
using System.Linq;
using System.Media;


namespace WhereIsBigfoot
{
    // commands are in alpha order
    // helper functions at bottom 

    // TODO: null check for all 
    /// <summary>
    /// Command class
    /// </summary>
    public class Commands
    {
        private int textLoadSpeed;
        public int userSpeed;
        // Used in Console.WriteLine. Sleep for 15 milliseconds between characters.
        // no checks needed 


        // DONE
        // passing player, asset 
        // TESTME: takes new player, takes an asset, test "RESULT"
        /// <summary>
        /// Drop method
        /// </summary>
        /// <param name="p"></param>
        /// <param name="a"></param>
        public void Drop(Player p, Asset a)
        {
            // check if passed null 
            if (a != null)
            {
                Item item = (Item)a;
                p.PlayerLocation.Items.Add(item.Name, item);
                p.Inventory.Remove(item.Name);
                // RESULT
                Console.WriteLine(item.Actions["drop"]);
            }
            else
            {
                // handle item doesn't exist
                // RESULT
                Console.WriteLine($"You do not have {a.Name} in your inventory.");
            }
        }

        // DONE
        // TESTME: accepts player, item, dictionary
        /// <summary>
        /// Get method
        /// </summary>
        /// <param name="p">player</param>
        /// <param name="item">item</param>
        /// <param name="items">item list</param>
        public void Get(Player p, Item item, List<Item> items)
        {
            // if asset is an item
            if (p.PlayerLocation.Items.ContainsKey(item.Name))
            {
                if (p.PlayerLocation.Items[item.Name].Actions.ContainsKey("get"))
                {
                    if (item.Name == "lantern" && p.PlayerLocation.Characters.ContainsKey("octopus"))
                    {
                        WrapText($"\n{item.Actions["blocked"]}");
                    }
                    else if (item.Name == "grease")
                    {
                        DanCheck(p, item);
                    }
                    else if (item.Name == "blackberries")
                    {
                        BlackberryCheck(p, item, items);
                    }
                    else
                    {
                        TransferItem(p, item);
                    }
                }
            }

            // if asset is a character
            // go back to later
            //else if (p.PlayerLocation.Characters.ContainsKey(a.Name))
            //{
            //    Character character = (Character)a;
            //    if (p.PlayerLocation.Characters[character.Name].Actions.ContainsKey("get"))
            //    {
            //        Console.WriteLine(p.PlayerLocation.Characters[character.Name].Actions["get"]);
            //    }
            //    else
            //    {
            //        CannotVerbNoun("get", a.Name);
            //        Console.WriteLine($"Getting {a.Name} would be rude and they do not fit in your backpack.");
            //    }
            //}
            // 

            else
            {
                CannotVerbNoun("get", item.Name);
                Console.WriteLine("Let's face it, you just have to let go and move on.");
            }
        }

        // DONE takes player, item, two characters
        // see RESULT in SwitchChar 
        // player, item as an item, character as a character
        // character dictionary passed 
        /// <summary>
        /// Give method
        /// </summary>
        /// <param name="p"></param>
        /// <param name="item"></param>
        /// <param name="character"></param>
        /// <param name="characters"></param>
        public void Give(Player p, Item item, Character character, Dictionary<string, Character> characters)
        {
            // check to see if character is target
            if (item.Target == "danCooking" || item.Target == "bigfootHostile" || item.Target == "octopus")
            {
                // give Dan book
                if (item.Name == "book" && character.Name == "danCooking")
                {
                    SwitchChar(p, item, character, characters, "danReading");
                    Console.WriteLine(item.Actions["give"]);
                }
                // RESULT check if bigfootHostile is removed from characters dict in player location
                // RESULT check if bigfootFriendly is in characters dict in player location
                else if (item.Name == "canOfBerries" && character.Name == "bigfootHostile")
                {
                    SwitchChar(p, item, character, characters, "bigfootFriendly");
                    GameOverMan(p, characters["bigfootFriendly"].DescriptionLong);
                    MusicPlayer2("wingame.wav");
                }
                else if (item.Name == "bacon" && character.Name == "octopus")
                {
                    WrapText(item.Actions["give"]);
                    TransferItem(p, p.PlayerLocation.Items["lantern"]);
                    p.Inventory.Remove(item.Name);

                }
            }
            // RESULT
            // handle mismatch
            else
            {
                CannotVerbNoun("give", item.Name);
                Console.WriteLine($"Maybe {character.Title} doesn't want the {item.Name}.");
            }
        }

        // Handle tunnel = checking location 
        // check inventory and check if lantern is lit 
        // tunnel 1 or tunnel 4 (check against map) 
        // - lantern is lit - different tunnel
        // - tunnel1Lit (naming convention) 
        // - lantern is dark - has three moves 
        // - if leave counter reset 
        // - special case, handle tunnel 
        // - handle counter for player 
        // Handle walking stick = checking inventory 
        // If in mountain and try to go to the cave
        // - without walking stick 
        // if chasm game over 
        /// <summary>
        /// go method
        /// </summary>
        /// <param name="p">player</param>
        /// <param name="direction">direction description</param>
        /// <param name="locations">location list</param>
        public void Go(Player p, string direction, List<Location> locations)
        {
            Location currentLocation = p.PlayerLocation;
            string newLocation;
            if (currentLocation.Exits.ContainsKey(direction))
            {
                // Console.Title = Console.Title.Remove(16);
                newLocation = currentLocation.Exits[direction];
                if (currentLocation.Name == "woods5" & direction == "north")
                {
                    Mountain(p, locations, currentLocation);
                }
                else if (currentLocation.Exits[direction].StartsWith("tunnel") || currentLocation.Name == "mountain" || currentLocation.Name == "valley")
                {
                    if (newLocation == "mountain" || newLocation == "valley")
                    {
                        p.GrueCounter = 0;
                    }

                    // Checks if the lantern is lit if so changes tunnel track from not lit to lit
                    if (p.Inventory.ContainsKey("glowingLantern") && newLocation.StartsWith("tunnel") && !newLocation.EndsWith("Lit"))
                    {
                        currentLocation.Exits[direction] = newLocation + "Lit";
                    }

                    Tunnel(p, locations, currentLocation, direction);
                }
                else
                {
                    foreach (Location location in locations)
                    {
                        if (location.Name == newLocation)
                        {
                            GoToLocation(p, location);
                        }
                    }
                }
            }
            else
            {
                CannotVerbNoun("go", direction);
                WrapText("Try a different direction. Up is also an option.");
            }
        }

        /// <summary>
        /// help method
        /// </summary>
        /// <param name="p">player</param>
        /// <param name="allowedVerbs">verb list </param>
        public void Help(Player p, List<string> allowedVerbs)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("To go different directions. Use: ");
            Console.WriteLine("   Go north\n   Go south\n   Go east\n   Go west\n");
            Console.WriteLine();
            WrapText($"Some possible verbs for {p.PlayerName} are: ");
            foreach (string verb in allowedVerbs)
            {
                Console.WriteLine($"   {verb}");
            }
            Console.WriteLine();
            Console.WriteLine("To get random extra lives, type Get Exlir (game level 1 only)");
            Console.WriteLine();
            WrapText($"Time limit is shown in Console bar (game level 3 only)");
            Console.WriteLine();
            WrapText($"To load scrolling text instantly, press the spacebar.");
        }

        // DONE
        /// <summary>
        /// inventory method
        /// </summary>
        /// <param name="p"></param>
        public void Inventory(Player p)
        {
            Console.WriteLine("You have the following items in your inventory: ");
            foreach (var item in p.Inventory.Values)
            {
                Console.WriteLine($"{item.Title}");
            }
        }

        /// <summary>
        /// look method
        /// </summary>
        /// <param name="p"></param>
        /// <param name="entry"></param>
        public void Look(Player p, string entry)
        {
            foreach (Item item in p.Inventory.Values)
            {
                if (item.ParseValue.Contains(entry))
                {
                    WrapText($"{item.DescriptionLong} \n");
                    return;
                }
            }
            foreach (Item item in p.PlayerLocation.Items.Values)
            {
                if (item.ParseValue.Contains(entry))
                {
                    WrapText($"{item.DescriptionLong} \n");
                    return;
                }
            }
            foreach (Character character in p.PlayerLocation.Characters.Values)
            {
                if (character.ParseValue.Contains(entry))
                {
                    WrapText($"{character.DescriptionLong} \n");
                    return;
                }
            }

            if (entry == "none")
            {
                WrapText($"{p.PlayerLocation.DescriptionLong}\n");
                string descriptions = "";
                foreach (Character character in p.PlayerLocation.Characters.Values)
                    descriptions += character.DescriptionShort;
                foreach (Item item in p.PlayerLocation.Items.Values)
                    descriptions += item.DescriptionShort;

                if (descriptions != "")
                    WrapText($"{descriptions}");

                WrapText($"{p.PlayerLocation.Exits["text"]}");
                return;
            }
        }

        // DONE
        // write like use 
        // DONE    
        /// <summary>
        /// put method
        /// </summary>
        /// <param name="p">player</param>
        /// <param name="item">item</param>
        /// <param name="asset">asset</param>
        /// <param name="items">item list</param>
        public void Put(Player p, Item item, Asset asset, List<Item> items)
        {
            if (item.Name == "grease" & asset.Name == "lantern")
            {
                Lantern(p, item, asset, items, "filledLantern");
            }
            else if (item.Name == "matches" & asset.Name == "filledLantern")
            {
                Lantern(p, item, asset, items, "glowingLantern");
            }
            else if (item.Target == asset.Name)
            {
                WrapText(item.Actions["put"]);
            }
            else
            {
                WrapText($"You can't put {item.Name} in {asset.Name}.");
                WrapText($"Are you sure you're using {item.Name} correctly?");
            }
        }

        // DONE
        /// <summary>
        /// quit method
        /// </summary>
        /// <param name="p"></param>
        public void Quit(Player p)
        {
            p.GameIsRunning = false;
            Console.WriteLine();
            WrapText("Thank you for playing Where is Bigfoot!");
            Console.WriteLine();
        }

        // DONE
        /// <summary>
        /// talk method
        /// </summary>
        /// <param name="p">player</param>
        /// <param name="c"></param>
        public void Talk(Player p, Character c)
        {
            WrapText(c.Actions["talk"]);
        }

        // DONE
        // player, item, asset 
        // check if asset is target
        /// <summary>
        /// use method
        /// </summary>
        /// <param name="p"></param>
        /// <param name="item"></param>
        /// <param name="asset"></param>
        public void Use(Player p, Item item, Asset asset)
        {
            if (item.Name == "book" | item.Target == asset.Name)
            {
                WrapText(item.Actions["use"]);
            }
            else
            {
                WrapText($"You can't use {item.Name} on {asset.Name}.");
                WrapText($"Are you using {item.Name} correctly?");
            }
        }

        /// <summary>
        /// Show location method
        /// </summary>
        /// <param name="location"></param>
        public void ShowLocation(Location location)
        {
            string descriptions = "";
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (location.Visited == false)
            {
                //WrapText($"{location.DescriptionFirst}\n");
                descriptions += location.DescriptionFirst;
                foreach (Character character in location.Characters.Values)
                    descriptions += character.DescriptionFirst;
                foreach (Item item in location.Items.Values)
                    descriptions += item.DescriptionFirst;

                if (descriptions != "")
                    WrapText($"{descriptions}");

                location.Visited = true;
                //helper function to print out AscII art
                PrintAscII(location);
            }
            else
            {
                WrapText($"{location.DescriptionShort}\n");
                foreach (Character character in location.Characters.Values)
                    descriptions += character.DescriptionShort;
                foreach (Item item in location.Items.Values)
                    descriptions += item.DescriptionShort;

                if (descriptions != "")
                    WrapText($"{descriptions}");
            }
            Console.WriteLine();
            WrapText($"{location.Exits["text"]}");
        }

        public void ShowLocationQuick(Location location)
        {
            string descriptions = "";

            if (location.Visited == false)
            {
                Console.WriteLine($"{location.DescriptionFirst}\n");
                foreach (Character character in location.Characters.Values)
                    descriptions += character.DescriptionFirst;
                foreach (Item item in location.Items.Values)
                    descriptions += item.DescriptionFirst;

                if (descriptions != "")
                    Console.WriteLine($"{descriptions}");

                location.Visited = true;
            }
            else
            {
                Console.WriteLine($"{location.DescriptionShort}\n");
                foreach (Character character in location.Characters.Values)
                    descriptions += character.DescriptionShort;
                foreach (Item item in location.Items.Values)
                    descriptions += item.DescriptionShort;

                if (descriptions != "")
                    Console.WriteLine($"{descriptions}");
            }
            Console.WriteLine($"{location.Exits["text"]}");
        }

        // >>> AUXILIARY METHODS <<< 
        private void Lantern(Player p, Item item, Asset asset, List<Item> items, string newState)
        {
            Item emptyCan = items[6];
            Item target = (Item)asset;
            foreach (Item lantern in items)
            {
                if (lantern.Name == newState)
                {
                    if (!p.Inventory.ContainsKey("emptyCan"))
                    {
                        p.Inventory.Add("emptyCan", emptyCan);
                    }
                    WrapText(item.Actions["put"]);
                    p.Inventory.Add(newState, lantern);
                    p.Inventory.Remove(item.Name);
                    p.Inventory.Remove(target.Name);
                }
            }
        }

        // Method adapted from https://rianjs.net/2016/03/line-wrapping-at-word-boundaries-for-console-applications-in-csharp
        public void WrapText(string paragraph)
        {
            if (paragraph.Length > 0)
            {

                if (string.IsNullOrWhiteSpace(paragraph))
                {
                    return;
                }

                string[] splitOn = { $"\n\n" };
                string[] splitParagraph = paragraph.Split(splitOn, StringSplitOptions.RemoveEmptyEntries);
                int windowWidth;
                try
                {
                    windowWidth = Console.WindowWidth;
                }
                catch (Exception)
                {
                    windowWidth = 5;
                }
                foreach (string para in splitParagraph)
                {
                    int approxLineCount = para.Length / windowWidth;
                    StringBuilder lines = new StringBuilder(para.Length + (approxLineCount * 4));

                    for (var i = 0; i < para.Length;)
                    {
                        int grabLimit = Math.Min(windowWidth, para.Length - i);
                        string line = para.Substring(i, grabLimit);

                        var isLastChunk = grabLimit + i == para.Length;

                        if (isLastChunk)
                        {
                            i = i + grabLimit;
                            lines.Append(line);
                        }
                        else
                        {
                            var lastSpace = line.LastIndexOf(" ", StringComparison.Ordinal);
                            try
                            {
                                lines.AppendLine(line.Substring(0, lastSpace));
                            }
                            catch (Exception)
                            {
                                return;
                                // lines.AppendLine("Testing game falied dummy texts");
                            }
                            //lines.AppendLine(line.Substring(0, lastSpace));


                            //Trailing spaces needn't be displayed as the first character on the new line
                            i = i + lastSpace + 1;
                        }
                    }
                    Console.WriteLine(lines.ToString());
                    Console.WriteLine();
                }
            } else
            {
                return;
            }
        }

        private void GoToLocation(Player p, Location location)
        {
            try 
            {
            Console.Clear();
            }
            catch(Exception) 
            {
                
            }
            p.PlayerLocation = location;
            DisplayStatus(location.Title, location.Items.Count, p.Inventory.Count, p.Life, p.Level);
            Console.WriteLine();
            ShowLocation(location);
        }

        // tunnel 1 or tunnel 5 (check against map) 
        // - lantern is dark - has three moves 
        // - if leave counter reset 
        // - special case, handle tunnel 
        // - handle counter for player 
        private void GoToTunnel(Player p, Location location, Location currentLocation)
        {
            if (currentLocation.Exits.ContainsValue(location.Name))
            {
                GoToLocation(p, location);
            }
        }

        private void GoToGrueDeath(Player p, Location location, Location currentLocation)
        {
            if (p.GrueCounter < 4)
            {
                if (currentLocation.Exits.ContainsValue(location.Name))
                {
                    GoToLocation(p, location);
                    WrapText(p.GrueCountdown[p.GrueCounter]);
                    p.GrueCounter++;
                }
            }
            else
            {
                // Gruedeath flavor text needed from writer
                GameOverMan(p, "The rest of your very short life was spent nourishing a rather hungry grue.");
            }
        }

        private void Tunnel(Player p, List<Location> locations, Location currentLocation, string direction)
        {
            Dictionary<string, Location> tunnels = new Dictionary<string, Location>();

            foreach (Location l in locations)
            {
                if (l.Name.StartsWith("tunnel"))
                {
                    tunnels.Add(l.Name, l);
                }
            }
            if (p.Inventory.ContainsKey("glowingLantern"))
            {
                foreach (string key in tunnels.Keys)
                {
                    switch (key)
                    {
                        case "tunnel1Lit":
                            GoToTunnel(p, tunnels[key], currentLocation);
                            break;
                        case "tunnel2Lit":
                            GoToTunnel(p, tunnels[key], currentLocation);
                            break;
                        case "tunnel3Lit":
                            GoToTunnel(p, tunnels[key], currentLocation);
                            break;
                        case "tunnel4Lit":
                            GoToTunnel(p, tunnels[key], currentLocation);
                            break;
                        case "tunnel5Lit":
                            GoToTunnel(p, tunnels[key], currentLocation);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                foreach (string key in tunnels.Keys)
                {
                    switch (key)
                    {
                        case "tunnel1":
                            GoToGrueDeath(p, tunnels[key], currentLocation);
                            break;
                        case "tunnel2":
                            GoToGrueDeath(p, tunnels[key], currentLocation);
                            break;
                        case "tunnel3":
                            GoToGrueDeath(p, tunnels[key], currentLocation);
                            break;
                        case "tunnel4":
                            GoToGrueDeath(p, tunnels[key], currentLocation);
                            break;
                        case "tunnel5":
                            GoToGrueDeath(p, tunnels[key], currentLocation);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void Mountain(Player p, List<Location> locations, Location currentLocation)
        {
            if (p.Inventory.ContainsKey("stick"))
            {
                foreach (Location location in locations)
                {
                    if (location.Name == "mountain")
                    {
                        GoToLocation(p, location);
                        break;
                    }
                }
            }
            else
            {
                WrapText($"That path is way too steep to climb without something to help you keep your balance.");
                GoToLocation(p, currentLocation);
            }
        }

        private void DanCheck(Player p, Item item)
        {
            if (p.PlayerLocation.Characters.ContainsKey("danReading"))
            {
                TransferItem(p, item);
            }
            else
            {
                WrapText(p.PlayerLocation.Items["grease"].Actions["blocked"]);
            }
        }

        private void BlackberryCheck(Player p, Item item, List<Item> items)
        {
            if (p.Inventory.ContainsKey("emptyCan"))
            {
                foreach (Item i in items)
                {
                    if (i.Name == "blackberries")
                    {
                        p.Inventory.Add(items[4].Name, items[4]);
                        p.Inventory.Remove(item.Target);
                        WrapText(i.Actions["get"]);
                    }
                }
            }
            else
            {
                WrapText(p.PlayerLocation.Items["blackberries"].Actions["blocked"]);
            }
        }

        private void TransferItem(Player p, Item item)
        {
            p.Inventory.Add(item.Name, item);
            p.PlayerLocation.Items.Remove(item.Name);
            Console.WriteLine(item.Actions["get"]);
        }

        private void SwitchChar(Player p, Item item, Character character, Dictionary<string, Character> characters, string switchTo)
        {
            foreach (Character c in characters.Values)
            {
                if (c.Name == switchTo)
                {
                    p.Inventory.Remove(item.Name);
                    p.PlayerLocation.Characters.Remove(character.Name);
                    p.PlayerLocation.Characters.Add(c.Name, c);
                }
            }

        }

        private void CannotVerbNoun(string verb, string noun)
        {
            WrapText($"You can't {verb} {noun} ");
        }

        private void TypeLine(string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                Console.Write(line[i]);
                Thread.Sleep(textLoadSpeed);
                try
                {
                    if (Console.KeyAvailable)
                    {
                        if (Console.ReadKey(true).Key == ConsoleKey.Spacebar)
                        {

                            textLoadSpeed = 0;
                        }
                    }
                }
                catch (Exception)
                {

                }

            }
            textLoadSpeed = userSpeed;
            Console.WriteLine();
        }

        /// <summary>
        /// Allows a way to persist state of game
        /// </summary>
        /// <param name="player">Player object</param>
        public void SaveGame(Player player)
        {
            GameState playerState = new GameState();
            playerState.Name = player.PlayerName;
            playerState.Location = player.PlayerLocation.Name;
            playerState.level = player.Level;

            playerState.History.Add("south");
            playerState.History.Add("east");
            playerState.History.Add("north");
            playerState.History.Add("south");
            foreach (string item in player.Inventory.Keys)
            {
                playerState.Items.Add(item);
            }

            if (!Directory.Exists(@"../../SavedGames/"))
            {
                Directory.CreateDirectory(@"../../SavedGames/");
            }

            File.WriteAllText($@"../../SavedGames/{player.PlayerName}.json", JsonConvert.SerializeObject(playerState));
            Console.WriteLine($"\n\nGame was saved for {player.PlayerName}.");
        }


        /// <summary>
        /// This method will load the last saved game state for a player
        /// </summary>
        /// <param name="player">Current player</param>
        /// <param name="locations">All locations</param>
        /// <param name="items">All items</param>
        public void LoadGame(Player player, List<Location> locations, List<Item> items)
        {
            GameState gameState = new GameState();
            string playerState;
            List<string> itemNames;
            bool fileExist = false;
            string playerPlayer = player.PlayerName;

            if (!Directory.Exists(@"../../SavedGames/"))
            {
                Directory.CreateDirectory(@"../../SavedGames/");
            }


            while (!fileExist)
            {
                try
                {
                    playerState = File.ReadAllText($@"../../SavedGames/{playerPlayer}.json");
                    gameState = JsonConvert.DeserializeObject<GameState>(playerState);
                    fileExist = true;

                }
                catch (Exception)
                {

                    Console.Write(" This user does not have a saved game.  Please try a different user. ");
                    playerPlayer = Console.ReadLine();


                }

            }

            // puts players last level
            player.Level = gameState.level;

            // load save game state json
            //Assign location instance to player instance
            for (int i = 0; i < locations.Count; i++)
            {
                if (gameState.Location == locations[i].Name)
                {
                    player.PlayerLocation = locations[i];
                }
                itemNames = new List<string>();
                foreach (var key in locations[i].Items.Keys)
                    itemNames.Add(key);

                for (int j = 0; j < itemNames.Count; j++)
                {
                    if (gameState.Items.Contains(itemNames[j]))
                    {
                        locations[i].Items.Remove(itemNames[j]);
                    }

                }
            }

            //Add any items to play inventory
            foreach (Item item in items)
            {
                if (gameState.Items.Contains(item.Name))
                {
                    player.Inventory.Add(item.Name, item);
                }
            }

            GoToLocation(player, player.PlayerLocation);
        }


        public void GameOverMan(Player player, string description)
        {
            WrapText(description);
            player.isAlive = false;
            Console.WriteLine("Press ANY KEY to continue....");
            Console.ReadKey(true);



        }

        public void MusicPlayer2(string FileName)
        {
            string path = Directory.GetCurrentDirectory();
            SoundPlayer player = new SoundPlayer(

            $@"{path}\..\..\Music\" + FileName);
            player.Play();
        }


        //display textbox for user status
        public void DisplayStatus(string title, int ItemCount, int InventoryCount, int ExtraLives, int level)
        {
            string ItCount = Convert.ToString(ItemCount);
            string InCount = Convert.ToString(InventoryCount);
            string extraLife = level == 1 ? "Number of Lives: ".PadLeft(25) + $"{ExtraLives}".PadRight(10) : " ";
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Location: ".PadLeft(20) + $"{title}" + "Number of Item: ".PadLeft(30) + $"{ItCount}"
                + $"Number of Inventory: ".PadLeft(30) + $"{InCount}" + $"{extraLife}");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        //display AscII art for differnt locations and start page
        public void PrintAscII(Location location)
        {
            if (location.Name == "tent")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
                        ..e=******=e..                         
              .r      .F            ^4.
           .@        $           .d$b.                  
         z$b.        J''.d$$$$$$$c
       .$$$$$$bc.    $    .e$$$$$$$$$$$$$$c
      d$$$$$$$$$$$$bdbe$$$$$$$$$$$$$$$$$$$$$.            
     d$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$b           
    $$$$$$$$$$$$$$$$$$$$$$$$$$$F *$$$$$$$$$$$$$$          
   $$$$$$$$$$$$$$$$$$$$$$$$$$$% 3 ^  **$$$$$$$$$.        
  d$$$$$$$$$$$$$$$$$$$$$$$$$$'  L         .^$$$$$        
 4$$$$$$$$$$$$$$$$$$$$$$$$$'    4           ^$$$$$       
 $$$$$$$$$$$$$$$$$$$$$$$*''      b           ^$$$$L      
4$$$$$$$$$$$$$$$$$$$$'           'r             $*      
$$$$$$$$$$$$$$$$$$$$$             $        .=''          
  *$$$$$$$$$$$$$$$$$$             ^F  .r^''               
    ^*$$$$$$$$$$$$$$$            ..*''                    
        '$$$$$$$$$$$P        .=''                       
           '*$$$$$$$    ./^'                             
              '*$$$L.=' 

                ");
                Console.ResetColor();
            }

            if (location.Name == "danCamp")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
            (                 ,&&&.
            )                .,.&&
           (  (              \=__/
               )             ,'-'.
         (    (  ,,      _.__|/ /|
          ) /\ -((------((_|___/ |
        (  // | (`'      ((  `'--|
      _ -.;_/ \\--._      \\ \-._/.
     (_;-// | \ \-'.\    <_,\_\`--'|
     ( `.__ _  ___,')      <_,-'__,'
     `'(_ )_)(_)_)'"

                );
                Console.ResetColor();
            }

            if (location.Name == "lakeshore")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
           ,%&& %&& %
       ,%&%& %&%& %&
      %& %&% &%&% % &%
     % &%% %&% &% %&%&,
     &%&% %&%& %& &%& %
    %%& %&%& %&%&% %&%%&
    &%&% %&% % %& &% %%&
    && %&% %&%& %&% %&%'
     '%&% %&% %&&%&%%'%
      % %& %& %&% &%%
        `\%%.'  /`%&'
          |    |            /`-._           _\\/
          |,   |_          /     `-._ ..--~`_
          |;   |_`\_      /  ,\\.~`  `-._ -  ^
          |;:  |/^}__..-,@   .~`    ~    `o ~
          |;:  |(____.-'     '.   ~   -    `    ~
          |;:  |  \ / `\       //.  -    ^   ~
          |;:  |\ /' /\_\_        ~. _ ~   -   //-
        \\/;:   \'--' `---`           `\\//-\\///
       ");
                Console.ResetColor();
            }

            if (location.Name == "mountain")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
                                        /^ L_,. \
           / ~\       __ / ~    \   ./    \
          / _\   _ /  \     / T~\| ~\_\ / \_ / ~| _ ^
        / \ / W  \ / V ^\/ X / ~T. \/   \    , v -./
 , '`-. /~   ^     H  ,  . \/    ;   .   \      `. \-' /
     M      ~     | . ;  /         ,  _:  .    ~\_,-'
 / ~    .    \    /   :                   '   \   ,/`
   I o. ^ oP     '98b         -      _  9.`       `\9b.
 8oO888.oO888P d888b9bo. .8o 888o.       8bo.o     988o.
 88888888888888888888888888bo.98888888bo.    98888bo. .d888P
 88888888888888888888888888888888888888888888888888888888888
 88888888888888P'   '' '   '''9888P' P' '8P'   ''*9888888888
       ");
                Console.ResetColor();
            }

            if (location.Name == "campsite")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
                 
                z$$$$$$.   *$$$$$$$$$.                    
            .z$$$$$$$$$$$e. '$$$$$$$$$$c.                 
         .e$$P''  $$  '' *$$$bc.'$$$$$$$$$$$e.             
     .e$*''      $$         '**be$$$***$   3             
     $            $F              $    4$r  'F            
     $           4$F              $    4$F   $            
    4P   \       4$F              $     $$   3r
    $     r      4$F              3     $$r   $           
    $     '.     $$F              4F    4$$   'b
   dF      3     $$    ^           b     $$L    L         
   $        .    $$   %            $     ^$$r    c        
  JF             $$  %             4r     '$$.   3L       
 .$              $$                 $      ^$$r ''       
 $%              $$P                3r   .e *               
'*=*********************************$$P     
       ");
                Console.ResetColor();
            }

            if (location.Name == "tunnel1" || location.Name == "tunnel2" || location.Name == "tunnel3" || location.Name == "tunnel4" || location.Name == "tunnel5")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
           .-'''L   |   J'''-.
                   .'\     |   |   |     /'.
                 .'   \    J   |   F    /   '.
               .'.     \    L  |  J    /     .'.
              /   '.    \   |  |  |   /    .'   \
             +-.    '.   \  J  |  F  /   .'    .-+
            J   '-.   '.  \ .L.|.J. /  .'   .-'   L
            L__    '-.  '.JHHHHHHHHHh.'  .-'    __J
           J   ""-__  '-.HHHHHHHHHHHHH.-'  __-""   L
           |        ""--|HHHHHHHHHHHHH|--""        |
           |------------HHHHHHHHHHHHHHH------------|
           |        __--|HHHHHHHHHHHHH|--__        |
           J   __-""  .-'HHHHHHHHHHHHH'-.  ""-__   F
            L""    .-'  .TYHHHHHHHHHP'.  '-.    ""J
            J   .-'   .'  / 'L'T'J' \  '.   '-.   F
             +-'    .'   /  J  |  L  \   '.    '-+
              \   .'    /   |  |  |   \    '.   /
               '.'     /    L  |  J    \     '.'
                 '.   /    J   |   L    \   .'
                   './     |   |   |     \.'
                      '-...L   |   J...-'

       ");
                Console.ResetColor();
            }
            if (location.Name == "woods1" || location.Name == "woods2" || location.Name == "woods3" || location.Name == "woods4" || location.Name == "woods5")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
          ......  ....................%.... .... ..... .........%............
 .@@@ ........ @@.... @@@@  . ............................  *  .....
 ....@@ ..... @ .... @ .............   ....... .....; .... *** .....
 .....\@\....@ .... @ ............................. #  .. *****  ...
  @@@.. @@@@@  @@@@@@___.. ....... ...%..... ...  {###}  *******
 ....@-@..@ ..@......@@@\...... %...... ....... <## ####>********
   @@@@\...@ @ ........\@@@@ ..... ...... ....... {###}***********
 ....%..@  @@ /@@@@@ . ....... ...............<###########> *******
 ...... .@-@@@@ ...V......     .... %.......... {#######}******* ***
 ...... .  @@ .. ..v.. .. . { } ............<###############>*******
 ......... @@ .... ........ {^^,     .......   {## ######}***** ****
 ..%..... @@ .. .%.... . .. (   `-;   ... <###################> ****
 . .... . @@ . .... .. _  .. `;;~~ ......... {#############}********
 .... ... @@ ... ..   /(______); .. ....<################  #####>***
 . .... ..@@@ ...... (         (  .........{##################}*****
 ......... @@@  ....  |:------( )  .. <##########################>**
  @@@@ ....@@@  ... _// ...... \\ ...... {###   ##############}*****
 @@@@@@@  @@@@@ .. / /@@@@@@@@@ vv  <##############################>
 @@@@@@@ @@@@@@@ @@@@@@@@@@@@@@@@@@@ ..... @@@@@@  @@@@@@@  @@@@
 @@@@@@###@@@@@### @@@@@@@@@@@@@@@@@@ @@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 @@@@@@@@###@##@@ @@@@@@@@@@@@@@@@@@@@@ @@@@@   @@@@@@@@@@@@@@@@@@@
 @@@@@@@@@@@### @@@@@@@@@@@@@@@@@@@@@@@@@@ @@@@@@@@@@@@@@@@@@@@@@@@
 -@@@@@@@@@#####@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
       ");
                Console.ResetColor();
            }


            if (location.Name == "valley")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
       
                      ,d$$$$$$$$b.
                     d$P'`Y'`Y'`?$b
                    d'    `  '  \ `b
                   /    |        \  \
                  /    / \       |   \
             _,--'        |      \    |
           /' _/          \   |        \
        _/' /'             |   \        `-.__
    __/'       ,-'    /    |    |     \      `--...__
  /'          /      |    / \     \     `-.           `\
 /    /;;,,__-'      /   /    \            \            `-.
/    |;;;;;;;\                                             \

       ");
                Console.ResetColor();
            }

            if (location.Name == "tree")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
          ......  ....................%.... .... ..... .........%............
                                 .         ;  
                 .              .              ;%     ;;   
                   ,           ,                :;%  %;   
                    :         ;                   :;%;'     .,   
           ,.        %;     %;            ;        %;'    ,;
             ;       ;%;  %%;        ,     %;    ;%;    ,%'
              %;       %;%;      ,  ;       %;  ;%;   ,%;' 
               ;%;      %;        ;%;        % ;%;  ,%;'
                `%;.     ;%;     %;'         `;%%;.%;'
                 `:;%.    ;%%. %@;        %; ;@%;%'
                    `:%;.  :;bd%;          %;@%;'
                      `@%:.  :;%.         ;@@%;'   
                        `@%.  `;@%.      ;@@%;         
                          `@%%. `@%%    ;@@%;        
                            ;@%. :@%%  %@@%;       
                              %@bd%%%bd%%:;     
                                #@%%%%%:;;
                                %@@%%%::;
                                %@@@%(o);  . '         
                                %@@@o%;:(.,'         
                            `.. %@@@o%::;         
                               `)@@@o%::;         
                                %@@(o)::;        
                               .%@@@@%::;         
                               ;%@@@@%::;.          
                              ;%@@@@%%:;;;. 
                          ...;%@@@@@%%:;;;;,..    

       ");
                Console.ResetColor();
            }

        }


    }

}




