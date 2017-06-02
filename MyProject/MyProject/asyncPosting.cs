using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MyProject
{
    class asyncPosting
    {
        private const int treadTimeout = 600000;
        public static List<Post> posts;
        public static bool haveItWork;
        private static DateTime timeOfStart;
        private static string setName;
        public asyncPosting(List<Post> posts, string setName)
        {
            haveItWork = true;
            timeOfStart = DateTime.Now;
            asyncPosting.setName = setName;
            asyncPosting.posts = new List<Post>(posts);
            Thread t = new Thread(threading);
            t.Start();
        }
        public static void stop()
        {
            haveItWork = false;
        }
        public static void threading()
        {
            while (haveItWork)
            {
                for(int i = 0; i< posts.Count; i++)
                {
                    if (!posts[i].isPosted)
                    {
                        MyVkApi.postIntoGroup(posts[i]);
                        Thread.Sleep(treadTimeout);
                    }
                    if(DateTime.Now.Day - timeOfStart.Day > 1)
                    {
                        Controller c = new Controller();
                        MyVkApi vk = new MyVkApi(c.ViewSetInfo(setName).ToArray());
                        Filter f = new Filter(vk.posts, 1);
                        foreach (var k in f.posts) c.InsertPost(k);
                        Sorter sorter = new Sorter(f.posts);
                        posts = sorter.sortedPosts;
                        i = 0;

                    }
                }
            }
        }
    }
}
