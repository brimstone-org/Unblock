using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslatedImage : MonoBehaviour {

    [SerializeField]
    private string file;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Start()
    {
        string tag = Localization.LanguageManager.Instance.LanguageTag;

        image.sprite = Resources.Load<Sprite>(file + tag);
    }

}
