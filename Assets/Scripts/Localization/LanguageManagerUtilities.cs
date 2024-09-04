using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{

    public static class LanguagePair
    {

        static Dictionary<SystemLanguage, string> languageTags = new Dictionary<SystemLanguage, string>();

        static LanguagePair()
        {
            Debug.LogWarning("LANGUAGE MANAGER: Some languages tags are not added");
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
            return languageTags[language];
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
