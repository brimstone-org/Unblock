using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Button sound extension to tie button sound to actual object. It will add a Button component if dragged on a GameObject since it's a requirement.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonSoundExtension : MonoBehaviour {

    [SerializeField]
    /// <summary>
    /// The button objects.
    /// </summary>
    Button button;

    private void Awake()
    {
        if(button == null)
            button = GetComponent<Button>();
    }

    // Use this for initialization
    void Start () {
        button.onClick.AddListener(() =>
       {
           SoundManager.Instance.PlayButtonClick();
       });
	}
	
}
