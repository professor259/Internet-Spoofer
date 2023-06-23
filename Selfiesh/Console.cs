using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfiesh
{
    public static class Console
    {
        private static DateTime NOW = DateTime.Now;
        private static DateTime NOW32 = DateTime.Now;
        public static string Title
        {
            get
            {
                return System.Console.Title;
            }
            set
            {
                System.Console.Title = value;
            }
        }

        public static int WindowWidth
        {
            get
            {
                return System.Console.WindowWidth;
            }
            set
            {
                System.Console.WindowWidth = value; ;
            }
        }

        public static int WindowHeight
        {
            get
            {
                return System.Console.WindowHeight;
            }
            set
            {
                System.Console.WindowHeight = value; ;
            }
        }

        public static void WriteLine(object line, ConsoleColor color = ConsoleColor.Cyan)
        {
            if (line.ToString() == "" || line.ToString() == " ")
                System.Console.WriteLine();
            else
            {
               var lines= DateTime.Now.ToString("[hh:mm:ss tt]:");
                ForegroundColor = ConsoleColor.Yellow;              
               // ForegroundColor = color;
                System.Console.Write(lines);
                ForegroundColor = color;
                System.Console.Write(line);
               // System.Console.Write(line + "\n");
            }
        }

        public static void WriteLine()
        {
            System.Console.WriteLine();
        }

        public static string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public static ConsoleColor BackgroundColor
        {
            get
            {
                return System.Console.BackgroundColor;
            }
            set
            {
                System.Console.BackgroundColor = value;
            }
        }

        public static void Clear()
        {
            System.Console.Clear();
        }

        public static ConsoleColor ForegroundColor
        {
            get
            {
                return System.Console.ForegroundColor;
            }
            set
            {
                System.Console.ForegroundColor = value;
            }
        }

        public static string TimeStamp()
        {
            return "[" + NOW.AddMilliseconds((DateTime.Now - NOW32).GetHashCode()).ToString("hh:mm:ss") + "]";
        }
    }
}
