using Realms;

namespace BlazorApp2.Shared;

public class RealmMessage : RealmObject
{
    [PrimaryKey] public string Id { get; set; } = Guid.NewGuid().ToString();
    public string GroupName { get; set; }
    public string User { get; set; }
    public string Text { get; set; }
    public DateTimeOffset Date { get; set; } = DateTime.Now;
}