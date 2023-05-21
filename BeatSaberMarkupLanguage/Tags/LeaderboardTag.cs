using System.Linq;
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
                leaderboardTemplate = Resources.FindObjectsOfTypeAll<LeaderboardTableView>().First(x => x.name == "LeaderboardTableView");
            LeaderboardTableView table = diContainer.InstantiatePrefabForComponent<LeaderboardTableView>(leaderboardTemplate, parent);
            table.name = "BSMLLeaderboard";
            foreach (LeaderboardTableCell tableCell in table.GetComponentsInChildren<LeaderboardTableCell>()) //This is to ensure if a leaderboard with scores already on it gets cloned that old scores are cleared off
                Object.Destroy(tableCell.gameObject);

            return table.gameObject;
        }
    }
}
