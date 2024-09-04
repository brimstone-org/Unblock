using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace TedrasoftSocial{

    /// <summary>
    /// Class that handles text translation. Drop it over texts, put the key as in the file.
    /// </summary>
	[RequireComponent(typeof(Text))]
	public class TranstatedText : MonoBehaviour {

        /// <summary>
        /// Key for a text.
        /// </summary>
	    public string key;

        /// <summary>
        /// The text to be changed, it gets autoselected because it's a required component.
        /// </summary>
	    Text text;

        /// <summary>
        /// Don't do things when enabled.
        /// </summary>
		private bool skipEnable = true;
	    
	    void Start() {
	        text = GetComponent<Text>();
	        UpdateText();
			skipEnable = false;
	    }

		void OnEnable(){
			if (skipEnable)
				return;
			Debug.Log ("Enable");
			UpdateText ();
		}

	    public void UpdateText() {
			text.font = LanguageManager.instance.GetFont ();
	        text.text = LanguageManager.instance.Get(key);
	    }

	}
}