using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject
{
    class Post
    {
        public string myId;
        public int numOfLikes;
        public int numOfReposts;
        public int numOfViews;
        public int date;
        public int priorityModifier;
        public string text;
        public Group group;
        public List<ImageWithHash> imagePathes;
        public bool isPosted;
        public double coefficient;
        

        public Post(int numOfLikes, int numOfReposts, int numOfViews, int date, string text, Group group, List<ImageWithHash> imagePathes)
        {
            this.numOfLikes = numOfLikes;
            this.numOfReposts = numOfReposts;
            this.numOfViews = numOfViews;
            this.date = date;
            this.text = text;
            this.group = group;
            this.imagePathes = imagePathes;
            this.priorityModifier = 1;
            this.isPosted = false;
            this.myId = group.domain + date;
            this.coefficient = GetСoefficient();
        }
        public Post(Post post)
        {
            this.numOfLikes = post.numOfLikes;
            this.numOfReposts = post.numOfReposts;
            this.numOfViews = post.numOfViews;
            this.date = post.date;
            this.text = post.text;
            this.group = post.group;
            this.imagePathes = post.imagePathes;
            this.priorityModifier = post.priorityModifier;
            this.isPosted = post.isPosted;
            this.myId = post.myId;
            this.coefficient = post.coefficient;
        }
        public double GetСoefficient()
        {
            return ((((double)numOfLikes + (double)numOfReposts) * (double)group.priority * (double)priorityModifier) / (double)numOfViews)*10000;
        }
    }
    class ImageWithHash
    {
        public string path;
        public string hash;
    }
}
