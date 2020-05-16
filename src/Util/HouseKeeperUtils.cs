using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace OneeChan.Util
{
    class HouseKeeperUtils
    {
        public static IActivity GetCandidateGameFromActivities(IImmutableList<IActivity> activities)
        {
            if (activities.Count <= 1) return activities[0];
            foreach (var activity in activities)
            {
                if (!activity.Name.Equals("Spotify")) // Ignoring spotify if it's not the only activity
                {
                    return activity;
                }
            }

            return activities[0];
        }
    }
}
