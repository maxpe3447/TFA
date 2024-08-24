using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace TFA.Forum.Domain.Monitoring;

internal class DomainMetrics
{
    private readonly Meter meter;
    //1private readonly Counter<int> forumfetch;
    private readonly ConcurrentDictionary<string, Counter<int>> counters = new();
    internal static readonly ActivitySource ActivitySource = new ActivitySource("TFA.Domain");

    public DomainMetrics(IMeterFactory meterFactory)
    {
        meter = meterFactory.Create("TFA.DOMAIN");
        //1 forumfetch = meter.CreateCounter<int>("forum_fetch");
    }

    //public void ForumFetched(bool success) =>
    //    Increment("forum.fetch", 1, new Dictionary<string, object?>
    //    {
    //        ["success"] = success
    //    });
    //1forumfetch.Add(1, new KeyValuePair<string, object?>("success", success));

    //2public void ForumCreate(bool success) =>
    //    Increment("forum.created", 1, new Dictionary<string, object?>
    //    {
    //        ["success"] = success
    //    });
    //2public void TopicCreate(bool success) =>
    //    Increment("topics.created", 1, new Dictionary<string, object?>
    //    {
    //        ["success"] = success
    //    });

    public void Increment(string name, int value, IDictionary<string, object?>? additionalTags = null)
    {
        var counter = counters.GetOrAdd(name, _ => meter.CreateCounter<int>(name));
        counter.Add(value,
            additionalTags?.ToArray() ??
            ReadOnlySpan<KeyValuePair<string, object?>>.Empty);
    }

    public static IDictionary<string, object?> ResultTags(bool success) => new Dictionary<string, object?>
    {
        ["success"] = success
    };
}
