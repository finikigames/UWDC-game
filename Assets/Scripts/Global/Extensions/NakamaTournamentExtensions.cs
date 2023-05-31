using System;
using Nakama;

namespace Global.Extensions {
    public static class NakamaTournamentExtensions {
        public static bool IsActive(this IApiTournament tournament) {
            var startTime = DateTimeOffset.Parse(tournament.StartTime).ToUnixTimeSeconds();
            var endTime = startTime + tournament.Duration;
            var nowTime = ((DateTimeOffset) DateTime.Now).ToUnixTimeSeconds();

            var whenEnded = endTime - nowTime;
            if (whenEnded < 0) return false;

            return true;
        }

        public static long GetRemainingTime(this IApiTournament tournament) {
            var startTime = DateTimeOffset.Parse(tournament.StartTime).ToUnixTimeSeconds();
            var endTime = startTime + tournament.Duration;
            var nowTime = ((DateTimeOffset) DateTime.Now).ToUnixTimeSeconds();

            return endTime - nowTime;
        }
    }
}