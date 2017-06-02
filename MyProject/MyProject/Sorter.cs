using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject
{
    class Sorter
    {
        public List<Post> sortedPosts;
        public Sorter(List<Post> posts)
        {
            sortedPosts = new List<Post>(posts);
            Sort(sortedPosts);
            sortedPosts.Reverse();
        }
        public void Sort(List<Post> posts)
        {
            Sort(posts, 0, posts.Count-1);
        }
        public void Sort(List<Post> posts, double l, double r)
        {
            Post temp;
            double x = posts[(int)(l + (r - l) / 2)].coefficient;
            double i = l;
            double j = r;

            while (i <= j)
            {
                while (posts[(int)i].coefficient < x) i++;
                while (posts[(int)j].coefficient > x) j--;
                if (i <= j)
                {
                    temp = new Post(posts[(int)i]);
                    posts[(int)i] = posts[(int)j];
                    posts[(int)j] = temp;
                    i++;
                    j--;
                }
            }
            if (i < r)
                Sort(posts, i, r);

            if (l < j)
                Sort(posts, l, j);

        }
    }
}
