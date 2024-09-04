using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;

namespace GDPR.Localization
{

    [RequireComponent(typeof(Text))]
    public class GDPRTranslatedText : MonoBehaviour
    {
        [SerializeField]
        private string key;
        [SerializeField]
        private TextFormatting format;
        [SerializeField][Tooltip("Use the Text component font instead of the LanguageManager font")]
        private bool useTextFont = false;

        private Text text;
        private bool skipEnable = true;


        void Start()
        {
            text = GetComponent<Text>();
            UpdateText();
            skipEnable = false;
        }

        void OnEnable()
        {
            if (skipEnable)
                return;

            UpdateText();
        }

        public void UpdateText()
        {
            if(!useTextFont)
                this.text.font = GDPRLocalization.GetFont();
            string text = GDPRLocalization.Get(key);

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

            switch (format)
            {
                case TextFormatting.Unchanged:
                    this.text.text = text;
                    break;
                case TextFormatting.UpperCase:
                    this.text.text = text.ToUpper();
                    break;
                case TextFormatting.LowerCase:
                    this.text.text = text.ToLower();
                    break;
                case TextFormatting.TitleCase:
                    this.text.text = ti.ToTitleCase(text);
                    break;
                case TextFormatting.SentenceCase:
                    if(text.Length > 0)
                        this.text.text = ti.ToUpper(text[0]).ToString();
                    if(text.Length >= 2)
                        this.text.text += text.Substring(1);
                    break;
            }

        }

    }

}