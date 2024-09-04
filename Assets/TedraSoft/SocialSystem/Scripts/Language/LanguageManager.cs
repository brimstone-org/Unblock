// http://www.bloodirony.com/blog/how-to-support-multiple-languages-in-unity


using UnityEngine;
using System.Collections.Generic;

namespace TedrasoftSocial{

    /// <summary>
    /// TedraSoft's Language Manager. This handles translations. This class is exclusively used with TranslatedText class.
    /// </summary>
	public class LanguageManager : MonoBehaviour {

        /// <summary>
        /// Default language, in full name.
        /// </summary>
		public string language = "English";

        /// <summary>
        /// Internal language tag.
        /// </summary>
	    private string languageTag = "";

        /// <summary>
        /// Should we use the device language? (If it's available)
        /// </summary>
		public bool useDeviceLanguage = true;

        /// <summary>
        /// Singleton design instance.
        /// </summary>
        /// <value>The instance.</value>
	    public static LanguageManager instance { get; private set; }

        /// <summary>
        /// Are we in debugging mode? If so we can set custom tags.
        /// </summary>
		public bool debugging = false;

        /// <summary>
        /// Language tag to be used while in debug mode.
        /// </summary>
		public string debuggingTag = "_ro";


        /// <summary>
        /// Dictionary containing TODO:FIXME:
        /// </summary>
	    Dictionary<string, string> fields;

        /// <summary>
        /// Dictionary containing all the available tags.
        /// </summary>
		public Dictionary<string, string> languageTags;

        /// <summary>
        /// Font that should be used for all translated text. Can't be left blank.
        /// </summary>
		public Font defaultFont;

        /// <summary>
        /// Class holding special fonts.
        /// </summary>
		[System.Serializable]
		public class FontCategory{
			public Font font;
			public List<string> languages;
		}

        /// <summary>
        /// Special fonts, you can choose special fonts for specific Languages.
        /// </summary>
		public List<FontCategory> specialFonts;




	    // Use this for initialization
	    void Awake () {
	        if (instance == null) {
	            instance = this;
	            DontDestroyOnLoad(this.gameObject);
	        } else if (instance != this) {
	            Destroy(this.gameObject);
	        }
				
	        fields = new Dictionary<string, string>();



            //Uncomment here all needed languages.


			languageTags = new Dictionary<string, string> ();
			languageTags.Add (SystemLanguage.English.ToString(), "");
			//languageTags.Add (SystemLanguage.Romanian.ToString(), "_ro");
			//languageTags.Add (SystemLanguage.German.ToString(), "_de");
			languageTags.Add (SystemLanguage.French.ToString(), "_fr");
			languageTags.Add (SystemLanguage.Spanish.ToString (), "_es");
			languageTags.Add (SystemLanguage.Portuguese.ToString (), "_pt");
			//languageTags.Add (SystemLanguage.Turkish.ToString (), "_tr");
			//languageTags.Add (SystemLanguage.Russian.ToString (), "_ru");
			//languageTags.Add (SystemLanguage.Chinese.ToString (), "_cn_tr");
			//languageTags.Add (SystemLanguage.ChineseTraditional.ToString (), "_cn_tr");
			//languageTags.Add (SystemLanguage.ChineseSimplified.ToString(), "_zh_cn");
			//languageTags.Add (SystemLanguage.Japanese.ToString (), "_ja");
			//languageTags.Add (SystemLanguage.Korean.ToString (), "_kr");
			//languageTags.Add (SystemLanguage.Italian.ToString (), "_it");
			//languageTags.Add (SystemLanguage.Dutch.ToString (), "_nl");


			if (useDeviceLanguage) {
				languageTag = languageTags [Application.systemLanguage.ToString ()];

			} else {
				languageTag = languageTags [language];
			}

			if (debugging)
				languageTag = debuggingTag;

	        LoadLanguage(languageTag);
	    }

        /// <summary>
        /// Set a specific language.
        /// </summary>
        /// <param name="language">Language.</param>
		public void SetLanguage(string language){
			if (debugging)
				return;
			this.language = language;
			languageTag = languageTags [language];
			LoadLanguage (languageTag);
		}

        /// <summary>
        /// Load a specific language's assets from the file.
        /// </summary>
        /// <param name="lang">Lang.</param>
	    private void LoadLanguage(string lang) {
	        fields.Clear();
	        TextAsset textAsset = (TextAsset)Resources.Load("Tedrasoft/SocialSystem/values/values" + languageTag);
	        string allTexts = "";
	        if (textAsset == null) {
				textAsset = (TextAsset)Resources.Load("Tedrasoft/SocialSystem/values/values");
	        }
	        allTexts = textAsset.text;
	        string[] lines = allTexts.Split(new string[] { "\r\n", "\n" },
	            System.StringSplitOptions.None);
	        string key, value;
	        for (int i = 0; i < lines.Length; i++) {
	            if (lines[i].IndexOf("=") >= 0 && !lines[i].StartsWith("#")) {
	                key = lines[i].Substring(0, lines[i].IndexOf("="));
	                value = lines[i].Substring(lines[i].IndexOf("=") + 1,
	                        lines[i].Length - lines[i].IndexOf("=") - 1).Replace("\\n", System.Environment.NewLine);
	                fields.Add(key, value);
	            }
	        }
	    }

        /// <summary>
        /// Get a text with a given key.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="key">Key.</param>
	    public string Get(string key) {
	        return fields[key];
	    }

        /// <summary>
        /// Gets the font if it's exists in the special list or returns the default one.
        /// </summary>
        /// <returns>The font.</returns>
		public Font GetFont(){
			for (int i = 0; i < specialFonts.Count; i++) {
				if(specialFonts[i].languages.Contains(language))
					return specialFonts[i].font;
			}
			return defaultFont;
		}

	}
}