using System;
using BGLib.Polyglot;
using IPA.Utilities;

namespace BeatSaberMarkupLanguage.Util
{
    internal static class LocalizationModelExtensions
    {
        private static readonly FieldAccessor<LocalizationModel, Action<LocalizationModel>>.Accessor OnChangeLanguageAccessor = FieldAccessor<LocalizationModel, Action<LocalizationModel>>.GetAccessor("_onChangeLanguage");

        internal static void RemoveOnLocalizeEvent(this LocalizationModel localizationModel, ILocalize localize)
        {
            Action<LocalizationModel> onChangeLanguageAction = OnChangeLanguageAccessor(ref localizationModel);
            onChangeLanguageAction -= localize.OnLocalize;
            OnChangeLanguageAccessor(ref localizationModel) = onChangeLanguageAction;
        }
    }
}
