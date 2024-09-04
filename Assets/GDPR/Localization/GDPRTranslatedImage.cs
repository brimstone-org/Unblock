using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GDPR.Localization
{

    public class GDPRTranslatedImage : MonoBehaviour
    {

        [Tooltip("Resources path to the default language file (without extension)")]
        [SerializeField]
        private string file;

        Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void Start()
        {
            string tag = GDPRLocalization.Instance.LanguageTag;

            image.sprite = Resources.Load<Sprite>(file + tag);
        }

    }
}
