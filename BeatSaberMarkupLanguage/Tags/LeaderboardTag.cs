using BS_Utils.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BeatSaberMarkupLanguage.Tags
{
    public class LeaderboardTag : BSMLTag
    {
        public override string[] Aliases => new[] { "leaderboard" };

        public override GameObject CreateObject(Transform parent)
        {
            LeaderboardTableView table = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<LeaderboardTableView>().First(), parent, false);
            table.name = "BSMLLeaderboard";
            table.GetPrivateField<LeaderboardTableCell>("_cellPrefab").GetPrivateField<TextMeshProUGUI>("_scoreText").enableWordWrapping = false;
            foreach (Transform child in table.transform.GetChild(0).GetChild(0)) //This is to ensure if a leaderboard with scores already on it gets cloned that old scores are cleared off
            {
                GameObject.Destroy(child.gameObject);
            }
            return table.gameObject;
        }
    }
}
