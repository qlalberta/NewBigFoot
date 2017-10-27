using System;
using WhereIsBigfoot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace WhereIsBigfoot.Tests
{
    [TestClass()]
    public class CommandsTest
    {
        /// <summary>
        /// Test setup
        /// </summary>

		Game testGame = new Game();
        Commands cmd = new Commands();
        /// Create a test player
        Player testPlayer = new Player("Alem");
        /// Create a test asset of type item
        Item testAsset = new Item(
                "lantern", "Item 1", new List<string>(), "You are at first item 1.", "You are at short item 1.",
                "You are at long item 1.", "lantern", new Dictionary<string, string>() { { "put", "You put the lantern" } },
                "LocationOne");

        /// <summary>
        /// Create a location object
        /// </summary>
        Location locationOne = new Location(
                "LocationOne", 
                "Location 1", 
                "You are at first location 1.",
                "You are at long location 1.",
                "You are at short location 1.",
                new Dictionary<string, string>(),
                new Dictionary<string, Character> { },
                new string[] {},
                new Dictionary<string, Item>()
                );

        /// <summary>
        /// Create a new item dictionary
        /// </summary>
            Item itemOne = new Item(
                "ItemOne",
                "Item 1",
                new List<string>(),
                "You are at first item 1.",
                "You are at short item 1.",
                "You are at long item 1.",
                "target",
                new Dictionary<string, string>() { {"drop", "You dropped itemOne" } },
                "LocationOne"
                );

        /// <summary>
        /// Create grease test object
        /// </summary>
        Item grease = new Item(
                "grease", "Item 1", new List<string>(), "You are at first item 1.", "You are at short item 1.",
                "You are at long item 1.", "grease", new Dictionary<string, string>() { { "put", "You put the grease" } },
                "LocationOne");

        /// <summary>
        /// Create emptyCan test object
        /// </summary>
        Item emptyCan = new Item(
           "emptyCan", "Item 1", new List<string>(), "You are at first item 1.", "You are at short item 1.",
           "You are at long item 1.", "emptyCan", new Dictionary<string, string>() { { "put", "You put the emptyCan" } },
           "LocationOne");

        /// <summary>
        /// Create filledLantern test object
        /// </summary>
        Item filledLantern = new Item(
           "filledLantern", "Item 1", new List<string>(), "You are at first item 1.", "You are at short item 1.",
           "You are at long item 1.", "filledLantern", new Dictionary<string, string>() { { "put", "You put the filledLantern" } },
           "LocationOne");

        ///Create a new dictionary for testing
        Dictionary<string, Character> dictionary = new Dictionary<string, Character>();

        ///Create a new book test object
        Item book = new Item(
           "book", "Item 1", new List<string>(), "You are at first item 1.", "You are at short item 1.",
           "You are at long item 1.", "danCooking", new Dictionary<string, string>() { { "give", "You gave the book" } },
           "LocationOne");

        ///Create a new test character test object
        Character danCooking = new Character(
          "danCooking", "Item 1", "You are at first item 1.", "You are at short item 1.",
          "You are at long item 1.", new Dictionary<string, string>(),
          new List<string>(), "danCamp");

        ///Add a second test character test object
        Character danReading = new Character(
          "danReading", "Item 1", "You are at first item 1.", "You are at short item 1.",
          "You are at long item 1.", new Dictionary<string, string>(),
          new List<string>(), "danCamp");

        ///Create a new location test object
        Location danCamp = new Location("danCamp", "Location 1", "You are at first location 1.",
            "You are at long location 1.", "You are at short location 1.", new Dictionary<string, string>(),
            new Dictionary<string, Character> { }, new string[] { }, new Dictionary<string, Item>()
            );

        /// Create locations test object
        Location woods = new Location("woods", "Location 1", "You are at first location 1.",
            "You are at long location 1.", "You are at short location 1.", new Dictionary<string, string>() { { "south", "woods" }, { "text", "woods" } },
            new Dictionary<string, Character> { }, new string[] { }, new Dictionary<string, Item>()
            );

        [TestMethod]
		public void TestCommandDrop()
		{

            testPlayer.Inventory.Add(itemOne.Name, itemOne);
            testPlayer.PlayerLocation = locationOne;

            
            //Check that item is in players inventory and location items is empty
            Assert.AreEqual(itemOne.Name, testPlayer.Inventory[itemOne.Name].Name);
            Assert.AreEqual(0, testPlayer.PlayerLocation.Items.Count);

            //Call drop command
            cmd.Drop(testPlayer, itemOne);

            // Check that item has been removed from player inventory and added to location items
            Assert.AreEqual(false, testPlayer.Inventory.ContainsKey(itemOne.Name));
            Assert.AreEqual(1, testPlayer.PlayerLocation.Items.Count);
        }

        [TestMethod]
        public void TestCommandGet()
        {
            itemOne.Actions = new Dictionary<string, string>() { { "get", "You added itemOne" } };

            List<Item> items = new List<Item>() { itemOne };
            locationOne.Items.Add(itemOne.Name, itemOne);
            testPlayer.PlayerLocation = locationOne;
  
            //check the item in the list before Get method
            Assert.AreEqual(false, testPlayer.Inventory.ContainsKey(itemOne.Name));

            cmd.Get(testPlayer, itemOne, items);

            //check the item in the list after the Get method
            Assert.AreEqual(true, testPlayer.Inventory.ContainsKey(itemOne.Name));

            //check the item is removed from player location
            Assert.AreEqual(false, testPlayer.PlayerLocation.Items.ContainsKey(itemOne.Name));

        }

        [TestMethod()]
        public void GiveTest()
        {

            ///We add the new character to the location object
            danCamp.Characters.Add("danCooking",danCooking);
            /// Add new character to the dictionary
            dictionary.Add("danReading", danReading);

            ///Add new elements to test player instance
            List<Item> items = new List<Item>() { itemOne };
            testPlayer.Inventory.Add(book.Name, book);
            testPlayer.PlayerLocation = danCamp;

            ///Check to see if character and object exist before testing
            Assert.AreEqual(true, testPlayer.PlayerLocation.Characters.ContainsKey(danCooking.Name));
            Assert.AreEqual(true, testPlayer.Inventory.ContainsKey(book.Name));

            ///Test give command
            cmd.Give(testPlayer, book, danCooking, dictionary);
            ///Check to see if elements changed after method test as expected
            Assert.AreEqual(true, testPlayer.PlayerLocation.Characters.ContainsKey(danReading.Name));
            Assert.AreEqual(false, testPlayer.Inventory.ContainsKey(book.Name));
        }

        [TestMethod()]
        public void PutTest()
        {
            /// Create a test asset
            testAsset.Name = "lantern";

            /// Create a new list  of type Item
            List<Item> items = new List<Item>() { grease, filledLantern, grease, grease, grease, grease, emptyCan, grease };
            /// Assign values to player instance 
            testPlayer.PlayerLocation = locationOne;
            testPlayer.Inventory.Add(itemOne.Name, itemOne);
            testPlayer.Inventory.Add(grease.Name, grease);

            /// Check to see if elements are in place before method test
            Assert.AreEqual(true, testPlayer.Inventory.ContainsKey(itemOne.Name));
            Assert.AreEqual(false, testPlayer.Inventory.ContainsKey("filledLantern"));


            cmd.Put(testPlayer, grease, testAsset, items);
            Assert.AreEqual(false, testPlayer.Inventory.ContainsKey("lantern"));
        }

        [TestMethod()]
        public void GoTest()
        {

            /// Create locations test List
            List<Location> locations = new List<Location>();

            ///// Create locations test object
            Location danCamp = new Location("danCamp", "Location 1", "You are at first location 1.",
                "You are at long location 1.", "You are at short location 1.", new Dictionary<string, string>() { { "south", "woods" } },
                new Dictionary<string, Character> { }, new string[] { }, new Dictionary<string, Item>()
                );


            testPlayer.PlayerLocation = danCamp;
            locations.Add(woods);
            /// Sanity check before the test
            Assert.AreEqual("danCamp", testPlayer.PlayerLocation.Name);

            /// Test "Go" method
            cmd.Go(testPlayer, "south", locations);
            Assert.AreEqual("woods", testPlayer.PlayerLocation.Name);
        }

    }
}