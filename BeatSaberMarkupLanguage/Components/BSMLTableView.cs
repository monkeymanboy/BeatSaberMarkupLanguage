using HMUI;

namespace BeatSaberMarkupLanguage.Components
{
    public class BSMLTableView : TableView
    {
        public override void ReloadData()
        {
            base.ReloadData();
            if(tableType == TableType.Horizontal)
            {
                contentTransform.anchorMin = new UnityEngine.Vector2(0, 0);
                contentTransform.anchorMax = new UnityEngine.Vector2(1, 1);
                contentTransform.sizeDelta = new UnityEngine.Vector2(0, 0);
            }
        }
    }
}
