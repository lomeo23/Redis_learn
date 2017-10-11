using System;
using System.Collections.Generic;
using System.Configuration;
using StackExchange.Redis;
using Newtonsoft.Json;

public class StackExchangeRedisHelper
{
    private Lazy<ConnectionMultiplexer> conn = null;
    private IServer redisserver = null;
    public StackExchangeRedisHelper()
    {
        try
        {
            conn = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configOptions.Value));
        }
        catch
        {
            throw;
        }
    }

    public ConnectionMultiplexer SafeConn
    {
        get
        {
            return conn.Value;
        }
    }

    //public IServer RedisServer
    //{
    //    get
    //    {
    //        string _RedisServer = ConfigurationManager.AppSettings["RedisServer"];
    //        int _RedisPort = int.Parse(ConfigurationManager.AppSettings["RedisPort"]);

    //        redisserver = conn.Value.GetServer(_RedisServer, _RedisPort);
    //        return redisserver;
    //    }
    //}


    private readonly Lazy<ConfigurationOptions> configOptions = new Lazy<ConfigurationOptions>(() =>
    {
        var configOptions = new ConfigurationOptions();
        configOptions.EndPoints.Add("localhost", 6379);
        configOptions.ClientName = "lomeobb";
        configOptions.Password = "sa";
        configOptions.AbortOnConnectFail = false;
        return configOptions;
    });

    public bool Add<T>(IDatabase database, string key, T value, DateTimeOffset? expiresAt = null) where T : class
    {
        //serialize
        var serializedObject = JsonConvert.SerializeObject(value);
        if (expiresAt.HasValue)
        {
            var expiration = expiresAt.Value.Subtract(DateTimeOffset.Now);
            return database.StringSet(key, serializedObject, expiration);
        }
        return database.StringSet(key, serializedObject);
    }

    public bool DeleteKey(IDatabase database, string key)
    {
        return database.KeyDelete(key);
    }

    public T Get<T>(IDatabase database, string key) where T : class
    {
        var serializedObject = database.StringGet(key);
        //deserialize
        return JsonConvert.DeserializeObject<T>(serializedObject);
    }

    public IEnumerable<T> Get<T>(IDatabase database, RedisKey[] keys) where T : class
    {
        var allserializedObject = database.StringGet(keys);
        //deserialize
        List<T> allvalues = new List<T>();
        foreach (RedisValue data in allserializedObject)
        {
            if (data.HasValue)
                allvalues.Add(JsonConvert.DeserializeObject<T>(data));
        }
        return allvalues;
    }

}