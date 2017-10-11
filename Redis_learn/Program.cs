using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange.Redis;
namespace Redis_learn
{
    class Program
    {
        static void Main(string[] args)
        {
            //var client = ConnectionMultiplexer.Connect(new ConfigurationOptions
            //{
            //    EndPoints =
            //    {
            //        "localhost","133.33.214.20"
            //    },
            //    Password = "sa"
            //});
            //var db = client.GetDatabase();
            var op = new helper();
            //_Program.Flushdb();

            //insert
            op.Insert();
            //Select
            RedisKey[] mykeys = { "User1", "User2", "User3" };//注意大小寫
            op.GetUser(mykeys);
            //update
            op.Update("User1", "ricoisme", DateTime.Parse("1981-12-13"));
            op.GetUser(mykeys);

            //delete
            op.DeleteUser("User1");
            op.GetUser(mykeys);
            //Console.WriteLine(client.ToString());
            Console.ReadKey();
        }

        class helper
        {
            ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints =
                {
                    "localhost","133.33.214.20"
                },
                Password = "sa"
            });
            public class User
            {
                public int ID { get; set; }
                public string Name { get; set; }
                public DateTime Birthday { get; set; }
                public bool Sex { get; set; }
            }
            public void Insert()
            {
                var seRedisHelper = new StackExchangeRedisHelper();
                conn = seRedisHelper.SafeConn;//create safe connection

                if (conn == null || !conn.IsConnected)
                {
                    Console.WriteLine("No connection is available");
                    return;
                }
                var database = conn.GetDatabase();

                var user1 = new User
                {
                    ID = 1,
                    Name = "rico",
                    Birthday = DateTime.Parse("1982-01-01 12:12:12"),
                    Sex = true
                };

                var user2 = new User
                {
                    ID = 2,
                    Name = "sherry",
                    Birthday = DateTime.Parse("1982-02-01 12:12:12"),
                    Sex = false
                };

                var user3 = new User
                {
                    ID = 3,
                    Name = "fifi",
                    Birthday = DateTime.Parse("1982-03-01 12:12:12"),
                    Sex = false
                };

                //key要注意大小寫
                if (seRedisHelper.Add(database, "User" + user1.ID, user1))
                {
                    Console.WriteLine("User" + user1.ID + " has inserted");
                }
                else
                {
                    Console.WriteLine("User" + user1.ID + " insert failed");
                }
                if (seRedisHelper.Add(database, "User" + user2.ID, user2))
                {
                    Console.WriteLine("User" + user2.ID + " has inserted");
                }
                else
                {
                    Console.WriteLine("User" + user2.ID + " insert failed");
                }
                if (seRedisHelper.Add(database, "User" + user3.ID, user3))
                {
                    Console.WriteLine("User" + user3.ID + " has inserted");
                }
                else
                {
                    Console.WriteLine("User" + user3.ID + " insert failed");
                }
            }

            public void GetUser(RedisKey[] keys)
            {
                var redisHelper = new StackExchangeRedisHelper();
                conn = redisHelper.SafeConn;//create safe connection
                var database = conn.GetDatabase();

                var datas = redisHelper.Get<User>(database, keys);
                if (datas == null)
                    Console.WriteLine("No Data");
                else
                {
                    foreach (var data in datas)
                    {
                        Console.WriteLine("ID:{0} ,Name:{1}, Birthday:{2}, Sex:{3}",
                       data.ID, data.Name, data.Birthday.ToShortDateString(), data.Sex);
                    }
                }
            }

            public void DeleteUser(string key)
            {
                var redisHelper = new StackExchangeRedisHelper();
                conn = redisHelper.SafeConn;//create safe connection
                var database = conn.GetDatabase();
                if (redisHelper.DeleteKey(database, key))
                    Console.WriteLine("Delete OK");
                else
                    Console.WriteLine("Delete failed");
            }

            public void Update(string key, string name, DateTime birthday)
            {
                var redisHelper = new StackExchangeRedisHelper();
                conn = redisHelper.SafeConn;//create safe connection
                IDatabase database = conn.GetDatabase();

                var data = redisHelper.Get<User>(database, key);
                if (data == null)
                    Console.WriteLine("No Data");
                else
                {
                    data.Name = name;
                    data.Birthday = birthday;
                    redisHelper.Add<User>(database, key, data);
                }
            }
        }

    }
}
