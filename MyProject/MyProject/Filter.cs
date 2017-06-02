using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyProject
{
    class Filter
    {
        public List<Post> posts;
        public Filter(List<Post> unfiltredPosts, int days)
        {
            posts = new List<Post>();
            Controller c = new Controller();
            foreach (Post p in unfiltredPosts)
            {
                if (c.isExist(p))
                {
                    Post changedPost = c.getPost(p);
                    posts.Add(changedPost);
                }
                else PictureDownloader(p);
            }
            CheckOnDuplicatesDry();
        }
        public Filter(List<Post> maybeDuplicatePosts)
        {
            posts = maybeDuplicatePosts;
            CheckOnDuplicatesDB();

        }
        private void PictureDownloader(Post post)
        {
            WebClient client = new WebClient();
            string newPath;
            for (int i = 0; i < post.imagePathes.Count; i++)
            {
                newPath = @"A:\pictures\" + post.imagePathes[i].path.Split('/')[post.imagePathes[i].path.Split('/').Length - 1];
                client.DownloadFile(post.imagePathes[i].path, newPath);
                post.imagePathes[i].path = newPath;
                post.imagePathes[i].hash = Picture.GetHash(newPath);
            }
            posts.Add(post);
        }
        public void CheckOnDuplicatesDry()
        {
            bool skip = false;
            for (int i = 0; i < posts.Count; i++)
            {
                for (int k = i; k < posts.Count; k++)
                {
                    skip = false;
                    if (i == k) continue;
                    for (int l = 0; l < posts[k].imagePathes.Count; l++)
                    {
                        if (skip) break;
                        for (int j = 0; j < posts[i].imagePathes.Count; j++)
                        {
                            if (skip) break;
                            if (posts[k].imagePathes[l].hash == posts[i].imagePathes[j].hash)
                            {
                                if (posts[k].date < posts[i].date)
                                {
                                    posts[i].priorityModifier += posts[k].priorityModifier;
                                    posts.RemoveAt(k);
                                }
                                else
                                {
                                    posts[k].priorityModifier += posts[i].priorityModifier;
                                    posts.RemoveAt(i);
                                }
                                skip = true;
                            }
                        }
                    }
                }
            }
        }
        public void CheckOnDuplicatesDB()
        {
            Controller c = new Controller();
            bool skip = false;
            for (int i = 0; i < posts.Count; i++)
            {
                for (int k = i; k < posts.Count || skip; k++)
                {
                    skip = false;
                    if (i == k) continue;
                    for (int l = 0; l < posts[k].imagePathes.Count; l++)
                    {
                        for (int j = 0; j < posts[i].imagePathes.Count; j++)
                        {
                            if (skip) break;
                            if (posts[k].imagePathes[l].hash == posts[i].imagePathes[j].hash)
                            {
                                if (posts[k].date < posts[i].date)
                                {
                                    posts[i].priorityModifier += posts[k].priorityModifier;
                                    posts.RemoveAt(k);
                                    c.Delete(posts[k].myId);
                                    c.SetBetterCoefficient(posts[i]);
                                }
                                else
                                {
                                    posts[k].priorityModifier += posts[i].priorityModifier;
                                    posts.RemoveAt(i);
                                    c.Delete(posts[i].myId);
                                    c.SetBetterCoefficient(posts[k]);
                                }
                                skip = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
