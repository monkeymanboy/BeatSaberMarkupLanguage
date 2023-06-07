using HMUI;

namespace BeatSaberMarkupLanguage.Components
{
    /// <summary>
    /// This class is used in <see cref="KEYBOARD"/> so sizing works even with leading/trailing spaces. It does nothing on its own.
    /// Functionality is provided by the <see cref="Harmony_Patches.TMP_Text_CalculatePreferredValues"/> and <see cref="Harmony_Patches.TextMeshProUGUI_GenerateTextMesh"/> patches.
    /// </summary>
    internal class WhitespaceIncludingCurvedTextMeshPro : CurvedTextMeshPro
    {
    }
}
