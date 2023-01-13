using Nostrid.Misc;
using NNostr.Client;

namespace Nostrid.Data.Relays;

public class AllSubscriptionFilter : SubscriptionFilter
{
	public AllSubscriptionFilter()
    {
        ParamsId = Utils.HashWithSHA256($"aasf");
    }

    public override NostrSubscriptionFilter[] GetFilters()
    {
        return new[] { new NostrSubscriptionFilter() { Kinds = new[] { NostrKind.Text, NostrKind.Relay, NostrKind.Deletion, NostrKind.Repost, NostrKind.Reaction }, Limit = limitFilterData?.Limit, Since = limitFilterData?.Since, Until = limitFilterData?.Until } };
    }

    public override SubscriptionFilter Clone()
    {
        return new AllSubscriptionFilter();
    }
}
