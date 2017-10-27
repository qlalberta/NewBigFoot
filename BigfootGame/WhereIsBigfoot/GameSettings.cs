using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using static System.Console;


namespace WhereIsBigfoot
{
    /// <summary>
    /// Game setting class
    /// </summary>
    public class GameSettings
    {
        int typeSpeed;
        /// <summary>
        /// constructor for game setting
        /// </summary>
        /// <param name="typeSpeed"></param>
        public GameSettings(int typeSpeed)
        {
            this.typeSpeed = typeSpeed;
        }

        /// <summary>
        /// property for typespeed
        /// </summary>
        public int TypeSpeed
        {
            get { return this.typeSpeed; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int GetGameSettings()
        {
            string speedChosen;
            int typeSpeed;
            bool isNunber = false;
            Console.ResetColor();
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;


            //Console.WriteLine($"GAME SETTINGS:\n(Hit enter for default values.)\n");
            

            do
            {
                speedChosen = Game.GetInput("Choose a gamespeed from 1 (slow) to 10 (fast).\nHit enter for default values 10.\n> ");
                if (speedChosen == "")
                    speedChosen = "10";
                //typeSpeed = Convert.ToInt16(speedChosen);
                isNunber = Int32.TryParse(speedChosen, out typeSpeed);
            } while (typeSpeed < 1 || typeSpeed > 10 || !isNunber);

            //int settings = typeSpeed;
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nYour settings have been implemented. Boom!");
            
            return typeSpeed;
        }
        public int TypeSpeedConverter()
        {
            int milliseconds;

            switch (typeSpeed)
            {
                case 1:
                    milliseconds = 60;
                    break;
                case 2:
                    milliseconds = 55;
                    break;
                case 3:
                    milliseconds = 50;
                    break;
                case 4:
                    milliseconds = 45;
                    break;
                case 5:
                    milliseconds = 40;
                    break;
                case 6:
                    milliseconds = 35;
                    break;
                case 7:
                    milliseconds = 30;
                    break;
                case 8:
                    milliseconds = 25;
                    break;
                case 9:
                    milliseconds = 20;
                    break;
                case 10:
                    milliseconds = 15;
                    break;
                default:
                    milliseconds = 15;
                    break;
            }

            return milliseconds;
        }
    }
}
