using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WhereIsBigfoot
{
    /// <summary>
    /// This is the assert class containing Asset constructor, a couple of properties and one method to overide Tostring
    /// </summary>
    public class Asset
    {/// <summary>
     /// declare members
     /// </summary>
        string name, title, descriptionFirst, descriptionShort, descriptionLong;
        List<string> parseValue;
		public Asset() { }
        /// <summary>
        /// This is the Asset constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="title"></param>
        /// <param name="parseValue"></param>
        /// <param name="descriptionFirst"></param>
        /// <param name="descriptionShort"></param>
        /// <param name="descriptionLong"></param>
        /// 
        public Asset(string name, string title, List<string> parseValue, string descriptionFirst, string descriptionShort, string descriptionLong)
        {
            this.name = name;
            this.title = title;
            this.parseValue = parseValue;
            this.descriptionFirst = descriptionFirst;
            this.descriptionShort = descriptionShort;
            this.descriptionLong = descriptionLong;
        }

        /// <summary>
        /// name property
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// title property
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        public List<string> ParseValue
        {
            get { return this.parseValue; }
            set { this.parseValue = value; }
        }

        public string DescriptionFirst
        {
            get { return this.descriptionFirst; }
            set { this.descriptionFirst = value; }
        }

        public string DescriptionShort
        {
            get { return this.descriptionShort; }
            set { this.descriptionShort = value; }
        }

        public string DescriptionLong
        {
            get { return this.descriptionLong; }
            set { this.descriptionLong = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name;
        }
    }
}
