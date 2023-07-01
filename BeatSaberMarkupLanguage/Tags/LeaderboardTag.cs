using BeatSaberMarkupLanguage.Util;
using UnityEngine;
using Zenject;

namespace BeatSaberMarkupLanguage.Tags
{
    public class LeaderboardTag : BSMLTag
    {
        private LeaderboardTableView leaderboardTemplate;

        public override string[] Aliases => new[] { "leaderboard" };

        public override void Setup()
        {
            base.Setup();
            leaderboardTemplate = DiContainer.Resolve<PlatformLeaderboardViewController>().GetComponentOnChild<LeaderboardTableView>("Container/LeaderboardTableView");
        }

        public override GameObject CreateObject(Transform parent)
        {
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
