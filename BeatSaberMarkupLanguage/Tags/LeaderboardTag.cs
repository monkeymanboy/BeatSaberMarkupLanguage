using IPA.Utilities;
using System.Linq;
using TMPro;
using UnityEngine;
using VRUIControls;

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
            LeaderboardTableView table = Object.Instantiate(leaderboardTemplate, parent, false);
            table.name = "BSMLLeaderboard";
            table.GetField<LeaderboardTableCell, LeaderboardTableView>("_cellPrefab").GetField<TextMeshProUGUI, LeaderboardTableCell>("_scoreText").enableWordWrapping = false;
            table.GetComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", BeatSaberUI.PhysicsRaycasterWithCache);
            foreach (LeaderboardTableCell tableCell in table.GetComponentsInChildren<LeaderboardTableCell>()) //This is to ensure if a leaderboard with scores already on it gets cloned that old scores are cleared off
                Object.Destroy(tableCell.gameObject);

            return table.gameObject;
        }
    }
}
