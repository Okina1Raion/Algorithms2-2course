using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model.RequestParams;

namespace MyProject
{
    class Program
    {
        static string setName = "";
        static void Main(string[] args)
        {
            string command = "";
            Console.WriteLine("Choose existing set, or create your own one.");
            Console.WriteLine("To see command list write 'list'");
            Controller c = new Controller();
            bool exit = true;

            while (exit)
            {
                command = Console.ReadLine();
                switch (command)
                {
                    case "list":
                        Console.WriteLine("1. View existing sets -- 'show' \n2. Create new set -- 'create' \n3. Start posting -- 'start' \n4. Parse forcibly -- 'parse' \n5. To exit program -- 'exit'");
                        break;
                    case "show":
                        list();
                        break;
                    case "create":
                        create();
                        Console.WriteLine("Ok!");
                        break;
                    case "start":
                        start();
                        break;
                    case "stop":
                        stop();
                        break;
                    case "changeSet":

                        break;
                    case "parse":
                        Console.WriteLine("How many days?");
                        try
                        {
                            parse(Int32.Parse(Console.ReadLine()));
                        }
                        catch
                        {
                            Console.WriteLine("Input is incorect");
                        }
                        break;
                    case "exit":
                        exit = false;
                        break;
                    default:
                        Console.WriteLine("Command: '{0}' is not supported", command);
                        break;
                }
            }
        }
        static void stop()
        {
            asyncPosting.stop();
        }
        static void changeSet()
        {
            asyncPosting.stop();
            Controller c = new Controller();
            Console.WriteLine("Enter set name");
            setName = Console.ReadLine();
            Console.WriteLine("Parsing started");
            parse(1);
            Filter f = new Filter(c.GetAllPostsFromSet(c.ViewSetInfo(setName).ToArray()));
            Sorter sorter = new Sorter(f.posts);
            asyncPosting a = new asyncPosting(sorter.sortedPosts, setName);
        }
        static void start()
        {
            asyncPosting.stop();
            Console.WriteLine("Started!");
            Controller c = new Controller();
            Console.WriteLine("Enter set name");
            setName = Console.ReadLine();
            Console.WriteLine("Parsing started");
            parse(1);
            Filter f = new Filter(c.GetAllPostsFromSet(c.ViewSetInfo(setName).ToArray()));
            Sorter sorter = new Sorter(f.posts);
            asyncPosting a = new asyncPosting(sorter.sortedPosts, setName);
        }
        static void parse(int days)
        {
            Controller c = new Controller();
            MyVkApi vk = new MyVkApi(c.ViewSetInfo(setName).ToArray());
            Filter f = new Filter(vk.posts, days);
            foreach (var i in f.posts) c.InsertPost(i);
        }
        static void create()
        {
            List<string> s = new List<string>();
            Controller c = new Controller();
            string temp;
            Console.WriteLine("Enter new original set name");
            string name = Console.ReadLine();
            Console.WriteLine("Enter group pathes in one line each. To stop enter stop");
            while ((temp = Console.ReadLine()) != "stop")
            {
                s.Add(temp);
            }
            c.NewSet(new Set(name, s.ToArray()));
        }
        static void list()
        {
            Controller c = new Controller();
            bool i = true;
            string command;
            Console.WriteLine("1. View set list -- 'show' \n2. View set info -- 'info' \n3. Back to main -- 'back'");
            while (i)
            {
                switch (command = Console.ReadLine())
                {
                    case "list":
                        Console.WriteLine("1. View set list -- 'show' \n2. View set info -- 'info' \n3. Back to main -- 'back'");
                        break;
                    case "show":
                        List<string> a = c.ViewSetList();
                        foreach (string s in a)
                        {
                            Console.WriteLine(s);
                        }
                        if (a.Count == 0) Console.WriteLine("It's nothing to show");
                        break;
                    case "info":
                        Console.WriteLine("Enter set name");
                        foreach (string s in c.ViewSetInfo(Console.ReadLine()))
                        {
                            Console.WriteLine(s);
                        }
                        break;
                    case "back":
                        i = false;
                        break;
                    default:
                        Console.WriteLine("Command: \'" + command + "\' is not supported");
                        break;

                }
            }
        }
    }
}
