using System;
using System.Collections.Generic;
using System.Text;

namespace WhereIsBigfoot
{/// <summary>
/// charcter class
/// </summary>
    public class Character : Asset
    {/// <summary>
    /// 
    /// </summary>
        string name, title, descriptionFirst, descriptionShort, descriptionLong;
        List<string> parseValue;
        Dictionary<string, string> actions;
        string location;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="descriptionFirst"></param>
        /// <param name="descriptionShort"></param>
        /// <param name="descriptionLong"></param>
        /// <param name="actions"></param>
        /// <param name="parseValue"></param>
        /// <param name="location"></param>
         

        public Character(string name, string title, string descriptionFirst, string descriptionShort, 
            string descriptionLong, Dictionary<string, string> actions,List<string> parseValue,string location) 
            : base(name, title, parseValue, descriptionFirst, descriptionShort, descriptionLong)
        {
            this.name = name;
            this.title = title;
            this.parseValue = parseValue;
            this.descriptionFirst = descriptionFirst;
            this.descriptionShort = descriptionShort;
            this.descriptionLong = descriptionLong;
            this.actions = actions;
            this.location = location;
        }

        public Dictionary<string, string> Actions
        {
            get { return this.actions; }
            set { this.actions = value; }
        }

        public string CharacterName
        {
            get { return this.name; }
        }

        public string Location {
            get { return this.location; }
            set { this.location = value; }
        }
    }
}
