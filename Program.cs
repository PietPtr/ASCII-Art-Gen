using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ASCII_art
{
    class Program
    {
        static string path = Directory.GetCurrentDirectory();

        static Bitmap asciiImage = (Bitmap)Image.FromFile(path + "/ascii.bmp");

        static List<Icon> iconList = new List<Icon>();

        static Random rnd = new Random();

        static int randint(int low, int high)
        {
            return rnd.Next(low, high + 1);
        }

        static int GetBlackPixels(int x1, int y1, int x2, int y2)
        {
            int blackPixels = 0;

            for (int y = y1; y < y2; y++)
            {
                for (int x = x1; x < x2; x++)
                {
                    if (asciiImage.GetPixel(x, y).B == 0 && asciiImage.GetPixel(x, y).G == 0 && asciiImage.GetPixel(x, y).R == 0)
                    {
                        blackPixels++;
                    }
                }
            }

            return blackPixels;
        }

        static int GetClosestIconIndex(float darknessValue)
        {
            int index = 0;

            float lowestDifference = 100;
            
            for (int i = 0; i < iconList.Count; i++ )
            {
                if (Math.Abs(iconList[i].blackPixels - darknessValue) < lowestDifference)
                {
                    lowestDifference = Math.Abs(iconList[i].blackPixels - darknessValue);

                    index = i;
                }
            }

            return index;
        }
                
        static void DrawASCIIArt(string filename)
        {
            Bitmap sourceImg = (Bitmap)Image.FromFile(path + "/" + filename + ".bmp");

            Console.WriteLine("Loaded image.");

            int[,] darknessValues = new int[sourceImg.Width, sourceImg.Height];
            int[,] indices = new int[sourceImg.Width, sourceImg.Height];

            for (int y = 0; y < sourceImg.Height; y++)
            {
                for (int x = 0; x < sourceImg.Width; x++)
                {
                    darknessValues[x, y] = sourceImg.GetPixel(x, y).R + sourceImg.GetPixel(x, y).G + sourceImg.GetPixel(x, y).B;
                    indices[x, y] = GetClosestIconIndex(36f - (float)darknessValues[x, y] / 21.25f);
                }
            }

            Console.WriteLine("Calculated values.");

            //Write to a file
            using (StreamWriter sw = File.CreateText(path + "\\" + filename + " - ASCII.txt"))
            {
                for (int y = 0; y < sourceImg.Height; y++)
                {
                    sw.Write("    ");
                    for (int x = 0; x < sourceImg.Width; x++)
                    {
                        Icon icon = iconList[indices[x, y]];
                        sw.Write(icon.character);
                        
                    }
                    sw.Write("\n");
                }
            }

            Console.WriteLine("Wrote to text file.");
            
            //Draw and save image:

            //create empty bitmap of the right size
            Bitmap newBmp = new Bitmap(sourceImg.Width * 9, sourceImg.Height * 16);

            //drawing
            using (Graphics g = Graphics.FromImage(newBmp)) //not mine
            {
                g.Clear(Color.White); //not mine

                for (int y = 0; y < sourceImg.Height; y++)
                {
                    for (int x = 0; x < sourceImg.Width; x++)
                    {
                        Icon icon = iconList[indices[x, y]];
                        g.DrawImage(asciiImage, new Rectangle(x * 9, y * 16, 9, 16), new Rectangle((int)icon.position.X, (int)icon.position.Y, 9, 16), GraphicsUnit.Pixel);
                    }
                }
            }

            Console.WriteLine("Drawn image.");

            newBmp.Save(filename + " - ASCII.bmp");

            Console.WriteLine("Saved image.");
        }

        static void Main(string[] args)
        {
            //Get the amount of pixels per icon (more pixels -> darker)
            for (int iconY = 0; iconY < 6; iconY++ )
            {
                for (int iconX = 0; iconX < 16; iconX++)
                {
                    int blacks = GetBlackPixels(iconX * 9, iconY * 16, (iconX + 1) * 9, (iconY + 1) * 16);

                    iconList.Add(new Icon(new Vector2(iconX * 9, iconY * 16), blacks, (char)((iconY - 1) * 16 + iconX + 48)));
                }
            }

            iconList.Sort();

            Console.Write("Filename of the source picture: ");
            string filename = Console.ReadLine();

            if (File.Exists(path + "/" + filename + ".bmp"))
            {
                DrawASCIIArt(filename);
            }
            else
            {
                Console.WriteLine("File not found. Exiting application.");
                Console.ReadKey();
                Environment.Exit(0);
            }
            
            Console.ReadKey();
        }
    }
}
