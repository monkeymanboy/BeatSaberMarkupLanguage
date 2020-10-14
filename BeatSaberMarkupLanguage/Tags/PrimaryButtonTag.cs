
namespace BeatSaberMarkupLanguage.Tags
{
    class PrimaryButtonTag : ButtonTag
    {
        public override string[] Aliases => new[] { "primary-button", "action-button" };
        public override string PrefabButton => "PlayButton";
    }
}
