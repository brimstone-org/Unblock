using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GDPR.Localization
{
    /// <summary>
    /// Embedded version of LanguageManager 3.2
    /// </summary>
    public class GDPRLocalization : MonoBehaviour
    {

        public static GDPRLocalization Instance { get; private set; }

        [Header("Options")]
        [SerializeField]
        [Tooltip("Used if <<UseDeviceLanguage>> is unchecked")]
        private SystemLanguage defaultLanguage = SystemLanguage.English;
        private SystemLanguage language = SystemLanguage.English;
        private string languageTag = "";
        public string LanguageTag { get { return languageTag; } }
        [SerializeField]
        private bool useDeviceLanguage = false;
        [SerializeField]
        SystemLanguage[] supportedLanguages = { SystemLanguage.English };

        private Dictionary<string, string> fields;
        private Dictionary<SystemLanguage, string> languageTags;

        [Header("Fonts")]
        [SerializeField]
        private Font defaultFont;
        [SerializeField]
        private List<FontCategory> specialFonts;

        [Header("Resources")]
        [SerializeField]
        private string fileLocation = "Languages/values";

        [Header("Debugging")]
        [SerializeField]
        private bool debugging = false;
        [SerializeField]
        private string debuggingTag = "_dbg";

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            language = defaultLanguage;

            fields = new Dictionary<string, string>();

            languageTags = new Dictionary<SystemLanguage, string>();
            for (int i = 0; i < supportedLanguages.Length; i++)
            {
#if UNITY_EDITOR
                if (Resources.Load(fileLocation + LanguagePair.GetTag(supportedLanguages[i])) == null)
                    Debug.LogError("LANGUAGE MANAGER: Language File Missing: " + supportedLanguages[i]);
#endif
                languageTags[supportedLanguages[i]] = LanguagePair.GetTag(supportedLanguages[i]);
            }

            if (useDeviceLanguage)
                language = Application.systemLanguage;

            if (languageTags.ContainsKey(language))
                languageTag = languageTags[language];
            else
                languageTag = LanguagePair.GetTag(defaultLanguage);

            LoadLanguage(languageTag);
        }

        public void SetLanguage(SystemLanguage language)
        {
            this.language = language;
            languageTag = languageTags[language];
            LoadLanguage(languageTag);
        }

        private void LoadLanguage(string lang)
        {
            fields.Clear();
            TextAsset textAsset = (TextAsset)Resources.Load(fileLocation + languageTag);

            if (textAsset == null)
            {
                throw new Exceptions.LanguageFileMissing(lang);
            }
            string allTexts = string.Empty;
            allTexts = textAsset.text;
            string[] lines = allTexts.Split(new string[] { "\r\n", "\n" },
                System.StringSplitOptions.None);
            string key, value;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#"))
                    continue;
                if (lines[i].IndexOf("=") >= 0 && !lines[i].StartsWith("#"))
                {
                    key = lines[i].Substring(0, lines[i].IndexOf("="));
                    value = lines[i].Substring(lines[i].IndexOf("=") + 1,
                        lines[i].Length - lines[i].IndexOf("=") - 1).Replace("\\n", System.Environment.NewLine);
                    fields.Add(key, value);
                }
            }
        }

        public static string Get(string key)
        {
            if (Instance == null)
                throw new Exceptions.NoInstance();

            string result;
            if (Instance.fields.TryGetValue(key, out result))
            {
                if (Instance.debugging)
                    result += Instance.debuggingTag;
                return result;
            }

            throw new Exceptions.NoKeyFound(key);
        }

        public static Font GetFont()
        {
            if (Instance == null)
                throw new Exceptions.NoInstance();

            for (int i = 0; i < Instance.specialFonts.Count; i++)
            {
                if (Instance.specialFonts[i].languages.Contains(Instance.defaultLanguage))
                    return Instance.specialFonts[i].font;
            }
            return Instance.defaultFont;
        }

    }
}
