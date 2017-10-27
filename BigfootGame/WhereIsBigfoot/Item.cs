using System;
using System.Collections.Generic;
using System.Text;

namespace WhereIsBigfoot
{/// <summary>
/// class for all the items avaialbe for the players
/// </summary>
    public class Item : Asset
    {
        string name, title, descriptionFirst, descriptionShort, descriptionLong;
		List<string> parseValue;
		Dictionary<string, string> actions;
        string location;
        string target;

        /// <summary>
        /// Item constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="parseValue"></param>
        /// <param name="descriptionFirst"></param>
        /// <param name="descriptionShort"></param>
        /// <param name="descriptionLong"></param>
        /// <param name="target"></param>
        /// <param name="actions"></param>
        /// <param name="location"></param>
        public Item(string name, string title, List<string> parseValue, string descriptionFirst, 
                    string descriptionShort, string descriptionLong,string target, Dictionary<string, string> actions,string location)
                    : base(name,title,parseValue,descriptionFirst, descriptionShort, descriptionLong)

        {
            this.name = name;
            this.title = title;
			this.parseValue = parseValue;
            this.descriptionFirst = descriptionFirst;
            this.descriptionShort = descriptionShort;
            this.descriptionLong = descriptionLong;
            this.actions = actions;
            this.target = target;
            this.location = location;
        }

        /// <summary>
        /// action dictionary
        /// </summary>
        public Dictionary<string, string> Actions
        {
            get { return this.actions; }
            set { this.actions = value; }
        }

        /// <summary>
        /// location property
        /// </summary>
        public string Location {
            get { return this.location; }
            set { this.location = value; }
        }

        /// <summary>
        /// target property
        /// </summary>
        public string Target
        {
            get { return this.target; }
            set { this.target = value; }
        }
    }
}
