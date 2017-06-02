using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VkNet;
using VkNet.Enums.Filters;
using System.Net;
using VkNet.Model.RequestParams;
using VkNet.Model.Attachments;

namespace MyProject
{
    class MyVkApi
    {
        private const string WALL_PATH = "https://api.vk.com/method/wall.get";
        private const string GROUP_PATH = "https://api.vk.com/method/groups.getById";
        private const int COUNT = 10;
        private const string VERSION_NUM = "5.64";
        private const int GROUP_NAME_SEPARATOR = 3;

        private string domain;
        private Group group;
        public List<Post> posts;


        private static long appID = 6056753;
        private static string phoneNumber = "+380505427595";
        private static string pass = "okina_raion";
        private static Settings scope = Settings.All;

        public MyVkApi(string[] domains)
        {
            this.posts = new List<Post>();
            foreach (string domain in domains)
            {
                this.domain = domain;
                getGroupInfo(this.domain);
                getWallInfo(group);
            }
        }
        public static void postIntoGroup(Post post)
        {
            var vk = new VkApi();
            vk.Authorize(new ApiAuthParams
            {
                ApplicationId = (ulong)appID,
                Login = phoneNumber,
                Password = pass,
                Settings = scope
            });

            var attachments = new List<MediaAttachment>();

            foreach (ImageWithHash image in post.imagePathes)
            {
                var uploadServer = vk.Photo.GetUploadServer(244980023, -41097309).UploadUrl;
                var wc = new WebClient();
                var responseFile = Encoding.ASCII.GetString(wc.UploadFile(uploadServer, image.path));
                var photo = vk.Photo.Save(new PhotoSaveParams
                {
                    SaveFileResponse = responseFile,
                    AlbumId = 244980023
                });

                attachments.Add(photo[0]);
            }

            string text = post.group.name + "\n\n" + post.text;

            if (attachments.Count != 0)
            {
                vk.Wall.Post(new VkNet.Model.RequestParams.WallPostParams()
                {
                    OwnerId = -147970369,
                    FromGroup = true,
                    Message = text,
                    Attachments = attachments
                });
            }
            else
            {
                vk.Wall.Post(new VkNet.Model.RequestParams.WallPostParams()
                {
                    OwnerId = -147970369,
                    FromGroup = true,
                    Message = text
                });
            }
            post.isPosted = true;
            Controller c = new Controller();
            c.setPosted(post.myId);
        }
        private void getGroupInfo(string domain)
        {
            HttpRequest info = new HttpRequest();
            info.AddUrlParam("group_id", domain);
            info.AddUrlParam("v", VERSION_NUM);
            string result = info.Get(GROUP_PATH).ToString();
            groupAnaliser(JObject.Parse(result));
        }
        private void getWallInfo(Group group)
        {
            HttpRequest info = new HttpRequest();
            info.AddUrlParam("domain", group.domain);
            info.AddUrlParam("count", COUNT);
            info.AddUrlParam("v", VERSION_NUM);
            info.AddUrlParam("fields", "members_count");
            string result = info.Get(WALL_PATH).ToString();
            wallAnaliser(JObject.Parse(result));
        }
        private void groupAnaliser(JObject groupJSON)
        {
            string name = groupJSON["response"][0]["name"].ToString();
            this.group = new Group(name, domain);

        }
        private void wallAnaliser(JObject stuff)
        {
            foreach (var i in stuff["response"]["items"].Select(t => t).ToList())
            {
                if (Int32.Parse(i["marked_as_ads"].ToString()) == 1) continue;
                int date = Int32.Parse(i["date"].ToString());
                string text = i["text"].ToString();
                int numOfLikes = Int32.Parse(i["likes"]["count"].ToString());
                int numOfReposts = Int32.Parse(i["reposts"]["count"].ToString());
                int numOfViews = Int32.Parse(i["views"]["count"].ToString());
                List<ImageWithHash> imagePathes = new List<ImageWithHash>();
                if (i["attachments"] != null)
                {
                    foreach (var k in i["attachments"].Select(t => t).ToList())
                    {
                        if (k["type"].ToString() == "photo")
                        {
                            if (k["photo"]["photo_807"] != null)
                            {
                                ImageWithHash img = new ImageWithHash();
                                img.path = k["photo"]["photo_807"].ToString();
                                imagePathes.Add(img);
                            }
                            else if (k["photo"]["photo_604"] != null)
                            {
                                ImageWithHash img = new ImageWithHash();
                                img.path = k["photo"]["photo_604"].ToString();
                                imagePathes.Add(img);
                            }
                            else if (k["photo"]["photo_130"] != null)
                            {
                                ImageWithHash img = new ImageWithHash();
                                img.path = k["photo"]["photo_130"].ToString();
                                imagePathes.Add(img);
                            }
                            else if (k["photo"]["photo_75"] != null)
                            {
                                ImageWithHash img = new ImageWithHash();
                                img.path = k["photo"]["photo_75"].ToString();
                                imagePathes.Add(img);
                            }
                        }
                    }
                }
                Post post = new Post(numOfLikes, numOfReposts, numOfViews, date, text, group, imagePathes);
                posts.Add(post);
            }
        }

    }
}
