using LiteDB;
using NNostr.Client;
using System.Collections.Concurrent;

namespace Nostrid.Model;

public class Event : NostrEvent
{
    public bool Processed { get; set; }

    public DateTimeOffset CreatedAtCurated { get; set; }

    public NoteMetadata NoteMetadata { get; set; } = new();

    public Event()
    {
    }

    public Event(NostrEvent ev)
    {
        Content = ev.Content;
        CreatedAt = ev.CreatedAt;
        Deleted = ev.Deleted;
        Id = ev.Id;
        Kind = ev.Kind;
        PublicKey = ev.PublicKey;
        Signature = ev.Signature;
        Tags = ev.Tags;
    }

    public override bool Equals(object obj)
    {
        return obj is Event @event &&
               Id == @event.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }

    public static void MergeAndClear(ConcurrentDictionary<string, Event> destination, ConcurrentDictionary<string, Event> source)
    {
        string id;
        while ((id = source.Keys.FirstOrDefault()) != null)
        {
            if (source.TryRemove(id, out var ev))
            {
                Merge(destination, ev);
            }
        }
    }

    public static bool Merge(ConcurrentDictionary<string, Event> destination, IEnumerable<Event> source, Func<Event, bool> copyIf = null, Action<Event> onCopy = null)
    {
        var ret = false;
        foreach (var ev in source)
        {
            if (copyIf == null || copyIf(ev))
            {
                ret = true;
                Merge(destination, ev, onCopy);
            }
        }
        return ret;
    }

    public static void Merge(ConcurrentDictionary<string, Event> destination, Event ev, Action<Event> onCopy = null)
    {
        destination.AddOrUpdate(
            ev.Id,
            addValueFactory: _ =>
            {
                onCopy?.Invoke(ev);
                return ev;
            },
            updateValueFactory: (_, old) =>
            {
                var copy = !old.Processed && ev.Processed;
                if (copy)
                    onCopy?.Invoke(ev);
                return copy ? ev : old;
            });
    }
}
