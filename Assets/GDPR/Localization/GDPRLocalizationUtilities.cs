using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDPR.Localization
{

    public static class LanguagePair
    {

        static Dictionary<SystemLanguage, string> languageTags = new Dictionary<SystemLanguage, string>();

        static LanguagePair()
        {
            languageTags.Add(SystemLanguage.English, "");
            languageTags.Add(SystemLanguage.Romanian, "_ro");
            languageTags.Add(SystemLanguage.German, "_de");
            languageTags.Add(SystemLanguage.French, "_fr");
            languageTags.Add(SystemLanguage.Spanish, "_es");
            languageTags.Add(SystemLanguage.Portuguese, "_pt");
            languageTags.Add(SystemLanguage.Turkish, "_tr");
            languageTags.Add(SystemLanguage.Russian, "_ru");
            languageTags.Add(SystemLanguage.Chinese, "_cn_tr");
            languageTags.Add(SystemLanguage.ChineseTraditional, "_cn_tr");
            languageTags.Add(SystemLanguage.ChineseSimplified, "_zh_cn");
            languageTags.Add(SystemLanguage.Japanese, "_ja");
            languageTags.Add(SystemLanguage.Korean, "_kr");
            languageTags.Add(SystemLanguage.Italian, "_it");
            languageTags.Add(SystemLanguage.Dutch, "_nl");
            languageTags.Add(SystemLanguage.Unknown, "");
            languageTags.Add(SystemLanguage.Arabic, "_ar");
            languageTags.Add(SystemLanguage.Norwegian, "_no");
        }

        public static string GetTag(SystemLanguage language)
        {
            try
            {
                return languageTags[language];
            } catch (KeyNotFoundException)
            {
                throw new Exceptions.LanguageFileMissing(language.ToString() + ". Consider adding it to the languages dictionary in LanguageManagerUtilities.cs");
            }
        }

    }

    [System.Serializable]
    public class FontCategory
    {
        public Font font;
        public List<SystemLanguage> languages;
    }

    public enum TextFormatting { Unchanged, UpperCase, LowerCase, TitleCase, SentenceCase}
}
