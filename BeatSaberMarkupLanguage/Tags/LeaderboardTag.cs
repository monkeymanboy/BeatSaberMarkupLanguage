using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class LeaderboardTag : PrefabBSMLTag
    {
        public override string[] Aliases => new[] { "leaderboard" };

        protected override PrefabParams CreatePrefab()
        {
            LeaderboardTableView leaderboardTemplate = BeatSaberUI.DiContainer.Resolve<PlatformLeaderboardViewController>()._leaderboardTableView;
            LeaderboardTableView table = BeatSaberUI.DiContainer.InstantiatePrefabForComponent<LeaderboardTableView>(leaderboardTemplate);
            table.name = "BSMLLeaderboard";

            // This is to ensure if a leaderboard with scores already on it gets cloned that old scores are cleared off
            foreach (LeaderboardTableCell tableCell in table.GetComponentsInChildren<LeaderboardTableCell>())
            {
                Object.Destroy(tableCell.gameObject);
            }

            return new PrefabParams(table.gameObject);
        }
    }
}
