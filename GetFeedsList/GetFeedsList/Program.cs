using System;
using System.IO;

namespace GetFeedsList
{
    class Program
    {
        static void Main(string[] args)
        {
            string data_path = Directory.GetCurrentDirectory() + "/../../../../Data";
            string result_file_path = Directory.GetCurrentDirectory() + "/../../../../list.txt";
            int count = 0;
            using StreamWriter result_file = new(result_file_path);
            Console.WriteLine(data_path);
            string[] files = Directory.GetFiles(data_path, "*.html", SearchOption.TopDirectoryOnly);
            foreach(var file in files)
            {
                string text = File.ReadAllText(file);
                string[] text_splits = text.Split("<a class=\"link DiscoverFeed__metadata-item\" href=\"");
                Console.WriteLine(file);
                foreach (var part_text in text_splits)
                {
                    string[] links = part_text.Split("\" target=\"_blank\"");
                    if (links.Length > 1)
                    {
                        Console.WriteLine("    " + links[0]);
                        result_file.WriteLine(links[0]);
                        count++;
                    }
                }
            }
            

            Console.WriteLine($"found {count}");
        }
    }
}
