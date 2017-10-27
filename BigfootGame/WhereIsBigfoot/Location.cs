using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace WhereIsBigfoot
{/// <summary>
/// class for location
/// </summary>
    public class Location : Asset
    {
        string name, title, descriptionFirst, descriptionLong, descriptionShort;
        Dictionary<string, string> exits;
        Dictionary<string, Character> characters;
        bool hasElixr = false;
        string[] objects;
        Dictionary<string, Item> items;
        bool visited = false;

        // create exits dictionary in location
        /// <summary>
        /// location constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="descriptionFirst"></param>
        /// <param name="descriptionLong"></param>
        /// <param name="descriptionShort"></param>
        /// <param name="exits"></param>
        /// <param name="characters"></param>
        /// <param name="objects"></param>
        /// <param name="items"></param>
        public Location(                       
                        string name,
                        string title,
                        string descriptionFirst,
                        string descriptionLong,
                        string descriptionShort,
                        Dictionary<string, string> exits,
                        Dictionary<string, Character> characters,
                        string[] objects,
                        Dictionary<string, Item> items) : base(name,
                                                               title,
                                                               null,
                                                               descriptionFirst,
                                                               descriptionShort,
                                                               descriptionLong)
        {          
            this.name = name;
            this.title = title;
            this.descriptionFirst = descriptionFirst;
            this.descriptionLong = descriptionLong;
            this.descriptionShort = descriptionShort;
            this.objects = objects;
            this.exits = exits;
            this.items = new Dictionary<string, Item>();
            this.characters = new Dictionary<string, Character>();
        }

        //[JsonConverter(typeof(Dictionary<string, string>))]
        /// <summary>
        /// dictionary exits
        /// </summary>
        public Dictionary<string, string> Exits
        {
            get { return this.exits; }
            set { this.exits = value; }
        }

        /// <summary>
        /// dictionary characters
        /// </summary>
        public Dictionary<string, Character> Characters
        {
            get { return this.characters; }
            set { this.characters = value; }
        }

        /// <summary>
        /// object array
        /// </summary>
        public string[] Objects
        {
            get { return this.objects; }
            set { this.objects = value; }
        }

        public bool HasExlir
        {
            get { return this.hasElixr; }
            set { this.hasElixr = value; }
        }

        /// <summary>
        /// item dictionary property
        /// </summary>
        public Dictionary<string, Item> Items
        {
            get { return this.items; }
            set { this.items = value; }
        }
        /// <summary>
        /// property of visited
        /// </summary>
        public bool Visited
        {
            get { return this.visited; }
            set { this.visited = value; }
        }
    }
}

