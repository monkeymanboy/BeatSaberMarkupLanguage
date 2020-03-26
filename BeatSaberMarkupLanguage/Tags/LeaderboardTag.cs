using IPA.Utilities;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class LeaderboardTag : BSMLTag
    {
        public override string[] Aliases => new[] { "leaderboard" };

        public override GameObject CreateObject(Transform parent)
        {
            LeaderboardTableView table = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<LeaderboardTableView>().First(x => x.name == "LeaderboardTableView"), parent, false);
            table.name = "BSMLLeaderboard";
            table.GetField<LeaderboardTableCell, LeaderboardTableView>("_cellPrefab").GetField<TextMeshProUGUI, LeaderboardTableCell>("_scoreText").enableWordWrapping = false;
            foreach (Transform child in table.transform.GetChild(0).GetChild(0)) //This is to ensure if a leaderboard with scores already on it gets cloned that old scores are cleared off
                GameObject.Destroy(child.gameObject);

            return table.gameObject;
        }
    }
}
