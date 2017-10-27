using System;
using System.Collections.Generic;
using System.Text;

namespace WhereIsBigfoot
{
    /// <summary>
    /// class for player
    /// </summary>

    public class Player
    {
        private string playerName;
        private Location playerLocation;
        private Dictionary<string, Item> inventory;
        private int grueCounter = 0;
        private int bigfootCounter = 0;
        private int life = 3 ;
        private int level;
        public bool isAlive = true;
        private bool gameIsRunning = true;
        private int turns = 4;
        private List<string> grueCountdown = new List<string>()
        {
            "\nThe dark is disorienting.",
            "\nYou think you hear something moving deeper in the cave.",
            "\nYou definitely hear something. A shuffling noise, that's getting louder, like something is moving towards you.",
            "\nWhatever it is that moving towards you is getting closer. You think you can hear breathing. Maybe panting. And there's a smell, like rotten meat, that is getting stronger and stronger...",
        };
        private List<string> bigfootCountdown = new List<string>()
        {
            "\nThe valley is pretty wide, and Bigfoot is pretty far away, but it turns out that with those big feet, he can REALLY move. He'll be on you soon!",
            "\nNot to to alarm you, but Bigfoot is closer than ever, and he still looks pretty pissed off. You figure you've got time to do one, or maybe two more things before he reaches you.",
            "\nYou have only seconds left to figure out something brilliant to do to keep Bigfoot from carrying out the imminent violence you can see in his eyes! Things are going to get really unpleasant for you otherwise...",
            "\nToo late! Bigfoot's almost here, and you’re out of ideas.\n\nAnd when you’re out of ideas, there’s only one idea left: RUN!\n\nBigfoot is practically on top of you, and y’know, there’s those big feet to contend with, but you run. You run as fast as terror and regret for going in on Dan’s ideas can take you. Heck, you run so fast, you even lose your backpack. Who cares? It’s slowing you down anyway!\n\nYou look back, though, just once to see if Bigfoot is still chasing you. And it turns out looking backward while in the middle of Run For Your Life Speed is a little disorienting. You don’t see Bigfoot right away...but when you look back around, you definitely see the big tree trunk.\n\nAlthough you don’t quite see it in ti—\n\n\n\nYou awaken on the ground. Your nose hurts. It really hurts. The world is darker now, but as it comes into focus you realize that might be more about the clouds gathering overhead. However, you’re alive, and right where you left off, so that’s good.\n\nCrawling to your feet, you look around. There’s no Bigfoot anymore. Just lots of big footprints, and your backpack, torn open with its contents strewn around. Fortunately, just about everything is still here, laying in plain sight.\n\nAh well. You’re alive. You saw Bigfoot. And you’ve got a story to tell, and lots of photos to...sigh. No, no photos. Your phone is still dead. Also, those clouds look like they’re full of rain. Thankfully, you see the lantern nearby, and it still has enough oil to get you back to camp. Hopefully you’ll get there before the rain starts. But it’s almost certainly going to wash away these tracks.\n\nAs you gather your things and head out, you wonder if maybe this is why nobody ever produces proof of Bigfoot. It’s not as easy as it sounds.\n\n*** GAME OVER ***"
        };

        /// <summary>
        /// constructor for player name
        /// </summary>
        /// <param name="playerName"></param>

        public Player(string playerName)
        {
            this.playerName = playerName;
            this.inventory = new Dictionary<string, Item>();
        }

        public Player()
        {
        }

        /// <summary>
        /// Level property to keep track of game difficulty
        /// </summary>
        public int Level
        {
            get { return this.level; }
            set { this.level = value; }
        }

        /// <summary>
        /// property to get player's name
        /// </summary>
        public string PlayerName
        {
            get { return this.playerName; }
        }

        /// <summary>
        /// location of where player is
        /// </summary>
        public Location PlayerLocation
        {
            get { return this.playerLocation; }
            set { this.playerLocation = value; }
        }

        /// <summary>
        /// dictionary for inventory
        /// </summary>
        public Dictionary<string, Item> Inventory
        {
            get { return this.inventory; }
            set { this.inventory = value; }
        }

        /// <summary>
        /// property for grue counter
        /// </summary>
        public int GrueCounter
        {
            get { return this.grueCounter; }
            set { this.grueCounter = value; }
        }

        /// <summary>
        /// property for big foot counter
        /// </summary>
        public int BigFootCounter
        {
            get { return this.bigfootCounter; }
            set { this.bigfootCounter = value; }
        }

        /// <summary>
        /// property for game status
        /// </summary>
        public bool GameIsRunning
        {
            get { return this.gameIsRunning; }
            set { this.gameIsRunning = value; }
        }

        /// <summary>
        /// List for grue count down
        /// </summary>
        public List<string> GrueCountdown
        {
            get { return this.grueCountdown; }
            set { this.grueCountdown = value; }
        }

        /// <summary>
        /// List for big foot Count down
        /// </summary>
        public List<string> BigfootCountdown
        {
            get { return this.bigfootCountdown; }
            set { this.bigfootCountdown = value; }
        }

        /// <summary>
        /// field to keep track of turns
        /// </summary>
        public int Turns
        {
            get { return this.turns; }
        }

        /// <summary>
        /// Decreases player's turns by one
        /// </summary>
        public void DecreaseTurnsLeft()
        {
            this.turns--;
        }

        /// <summary>
        /// Increases player's turns by one
        /// </summary>
        public void IncreaseTurnsLeft()
        {
            this.turns++;
        }

        /// <summary>
        /// Decreases player's life by 1
        /// </summary>
        public void DecreaseLife()
        {
            this.life--;
        }


        /// <summary>
        /// Increases player's life by 1
        /// </summary>
        public void IncreaseLife()
        {
            this.life++;
        }

        /// <summary>
        /// Field for player life
        /// </summary>
        public int Life
        {
            get { return this.life; }
            set { this.life = value; }
        }
    }
}
