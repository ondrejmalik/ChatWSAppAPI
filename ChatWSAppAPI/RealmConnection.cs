using System.Security.Cryptography;
using System.Text;
using BlazorApp2.Shared;
using Realms;

namespace ChatWSAppAPI;

public class RealmConnection
{
    public RealmConfiguration config { get; set; }

    public RealmConnection()
    {
        config = new RealmConfiguration(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChatWSApp\Data\Database.realm"));
        if (!File.Exists(config.DatabasePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(config.DatabasePath));
            File.Create(config.DatabasePath);
        }
    }

    public List<RealmMessage> SelectAllMessages()
    {
        Realm realm = Realm.GetInstance(config);
        return realm.All<RealmMessage>().ToList();
    }

    public List<RealmMessage> MessagesByUser(string user)
    {
        Realm realm = Realm.GetInstance(config);
            var result = realm.All<RealmMessage>().Where(x => x.User == user);
            return result.ToList();
    }
    public List<RealmMessage> MessagesByGroup(string group, string? key)
    {
        Realm realm = Realm.GetInstance(config);
        key ??= "";
        string v = GetFullGroupName(group, key);
        var result = realm.All<RealmMessage>().Where(x => x.GroupName == v);
        return result.ToList();
    }
    public List<RealmMessage> MessagesByWord(string word)
    {
        word = word.ToLower();
        Realm realm = Realm.GetInstance(config);
        var result = realm.All<RealmMessage>().Where(x => x.Text.Contains(word));
        return result.ToList();
    }
    private string GetFullGroupName(string groupInput, string keyInput)
    {
        return $"{groupInput},{string.Concat(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(keyInput)).Select(b => b.ToString("x2")))}";
    }
}