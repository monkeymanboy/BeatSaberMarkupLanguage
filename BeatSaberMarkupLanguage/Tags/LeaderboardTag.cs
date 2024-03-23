using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class LeaderboardTag : BSMLTag
    {
        private LeaderboardTableView leaderboardTemplate;

        public override string[] Aliases => new[] { "leaderboard" };

        public override GameObject CreateObject(Transform parent)
        {
            if (leaderboardTemplate == null)
            {
                leaderboardTemplate = DiContainer.Resolve<PlatformLeaderboardViewController>()._leaderboardTableView;
            }

            LeaderboardTableView table = DiContainer.InstantiatePrefabForComponent<LeaderboardTableView>(leaderboardTemplate, parent);
            table.name = "BSMLLeaderboard";

            // This is to ensure if a leaderboard with scores already on it gets cloned that old scores are cleared off
            foreach (LeaderboardTableCell tableCell in table.GetComponentsInChildren<LeaderboardTableCell>())
            {
                Object.Destroy(tableCell.gameObject);
            }

            return table.gameObject;
        }
    }
}
