using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace MyProject
{
    class Controller
    {
        string connectionString;
        MongoClient client;
        IMongoDatabase database;

        public Controller()
        {
            connectionString = "mongodb://localhost:27017";
            client = new MongoClient(connectionString);
            database = client.GetDatabase("projectDB");
        }

        public void InsertPost(Post post)
        {
            var collection = database.GetCollection<BsonDocument>("Posts");
            if (collection.Find(new BsonDocument() { { "myId", post.myId } }).Count() == 0)
                collection.InsertOne(post.ToBsonDocument());
            else
            {
                var filter = Builders<BsonDocument>.Filter.Eq("myId", post.myId);
                var update = Builders<BsonDocument>.Update
                    .Set("numOfLikes", post.numOfLikes)
                    .Set("numOfReposts", post.numOfReposts)
                    .Set("numOfViews", post.numOfViews)
                    .Set("coefficient", post.coefficient);
                collection.UpdateOne(filter, update);
            }
        }
        public Post getPost(Post oldPost)
        {
            var collection = database.GetCollection<BsonDocument>("Posts");
            var cursor = collection.Find(new BsonDocument() { { "myId", oldPost.myId } }).ToCursor();
            while (cursor.MoveNext())
            {
                foreach (var cur in cursor.Current)
                {
                    int numOfLikes = Int32.Parse(cur["numOfLikes"].ToString());
                    int numOfReposts = Int32.Parse(cur["numOfReposts"].ToString());
                    int numOfViews = Int32.Parse(cur["numOfViews"].ToString());
                    int date = Int32.Parse(cur["date"].ToString());
                    string text = cur["text"].ToString();
                    string groupName = cur["group"]["name"].ToString();
                    string groupDomain = cur["group"]["domain"].ToString();
                    Group group = new Group(groupName, groupDomain);
                    List<ImageWithHash> images = new List<ImageWithHash>();
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            ImageWithHash img = new ImageWithHash();
                            img.path = cur["imagePathes"][i]["path"].ToString();
                            img.hash = cur["imagePathes"][i]["hash"].ToString();
                            images.Add(img);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    return new Post(numOfLikes, numOfReposts, numOfViews, date, text, group, images);
                }
            }
            return null;
        }
    public bool isExist(Post post)
    {
        var collection = database.GetCollection<BsonDocument>("Posts");
        return (collection.Find(new BsonDocument() { { "myId", post.myId } }).Count() != 0);
    }
    public List<Post> GetAllPostsFromSet(string[] set)
    {
        List<Post> posts = new List<Post>();
        var collection = database.GetCollection<BsonDocument>("Posts");
        foreach (string groupNameFromSet in set)
        {
            var cursor = collection.Find(new BsonDocument() { { "group.domain", groupNameFromSet } }).ToCursor();
            while (cursor.MoveNext())
            {
                foreach (var cur in cursor.Current)
                {
                    int numOfLikes = Int32.Parse(cur["numOfLikes"].ToString());
                    int numOfReposts = Int32.Parse(cur["numOfReposts"].ToString());
                    int numOfViews = Int32.Parse(cur["numOfViews"].ToString());
                    int date = Int32.Parse(cur["date"].ToString());
                    string text = cur["text"].ToString();
                    string groupName = cur["group"]["name"].ToString();
                    string groupDomain = cur["group"]["domain"].ToString();
                    Group group = new Group(groupName, groupDomain);
                    List<ImageWithHash> images = new List<ImageWithHash>();
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            ImageWithHash img = new ImageWithHash();
                            img.path = cur["imagePathes"][i]["path"].ToString();
                            img.hash = cur["imagePathes"][i]["hash"].ToString();
                            images.Add(img);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    Post post = new Post(numOfLikes, numOfReposts, numOfViews, date, text, group, images);
                    posts.Add(post);
                }
            }
        }
        return posts;
    }
    public void setPosted(string id)
    {
        var collection = database.GetCollection<BsonDocument>("Posts");
        var filter = Builders<BsonDocument>.Filter.Eq("myId", id);
        var update = Builders<BsonDocument>.Update
            .Set("isPosted", true);
        collection.UpdateOne(filter, update);
    }
    public List<Post> GetAllPostsFromDB()
    {
        List<Post> posts = new List<Post>();
        var collection = database.GetCollection<BsonDocument>("Posts");
        var cursor = collection.Find(new BsonDocument()).ToCursor();
        while (cursor.MoveNext())
        {
            foreach (var cur in cursor.Current)
            {
                int numOfLikes = Int32.Parse(cur["numOfLikes"].ToString());
                int numOfReposts = Int32.Parse(cur["numOfReposts"].ToString());
                int numOfViews = Int32.Parse(cur["numOfViews"].ToString());
                int date = Int32.Parse(cur["date"].ToString());
                string text = cur["text"].ToString();
                string groupName = cur["group"]["name"].ToString();
                string groupDomain = cur["group"]["domain"].ToString();
                Group group = new Group(groupName, groupDomain);
                List<ImageWithHash> images = new List<ImageWithHash>();
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        ImageWithHash img = new ImageWithHash();
                        img.path = cur["imagePathes"][i]["path"].ToString();
                        img.hash = cur["imagePathes"][i]["hash"].ToString();
                        images.Add(img);
                    }
                    catch
                    {
                        break;
                    }
                }
                Post post = new Post(numOfLikes, numOfReposts, numOfViews, date, text, group, images);
                posts.Add(post);
            }
        }

        return posts;
    }
    public void Delete(string id)
    {
        var collection = database.GetCollection<BsonDocument>("Posts");
        collection.DeleteOne(new BsonDocument() { { "myId", id } });
    }
    public void SetBetterCoefficient(Post post)
    {
        var collection = database.GetCollection<BsonDocument>("Posts");
        var filter = Builders<BsonDocument>.Filter.Eq("myId", post.myId);
        var update = Builders<BsonDocument>.Update
            .Set("priorityModifier", (post.priorityModifier + 1))
            .Set("coefficient", post.GetСoefficient());
        collection.UpdateOne(filter, update);
    }
    public void NewSet(Set s)
    {
        var collection = database.GetCollection<BsonDocument>("Sets");
        collection.InsertOne(s.ToBsonDocument());
    }
    public BsonDocument GetSet(string setName)
    {
        var collection = database.GetCollection<BsonDocument>("Sets");
        return collection.Find(new BsonDocument() { { "setName", setName } }).ToBsonDocument();
    }
    public List<string> ViewSetList()
    {
        List<string> a = new List<string>();
        var collection = database.GetCollection<BsonDocument>("Sets");
        var documents = collection.Find(_ => true).ToList();
        foreach (var i in documents) a.Add(((i["setName"])).ToString());
        return a;
    }
    public List<string> ViewSetInfo(string setName)
    {
        var collection = database.GetCollection<BsonDocument>("Sets");
        var documents = collection.Find(_ => true).ToList();
        List<string> s = new List<string>();
        foreach (var i in documents.ToList())
        {
            if (i["setName"] == setName)
            {
                foreach (var k in Regex.Matches(i["groups"].ToJson(), "(?i)[a-z1-9\\.]+"))
                {
                    s.Add(k.ToString());
                }
            }
        }
        return s;
    }
}
}
