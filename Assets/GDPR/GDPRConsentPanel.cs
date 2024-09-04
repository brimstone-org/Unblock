using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDPR
{

    public class GDPRConsentPanel : MonoBehaviour
    {

        public static GDPRConsentPanel Instance { get; private set; }

        [SerializeField]
        private GameObject popUpLandscape;
        [SerializeField]
        private GameObject popUpPortrait;

        private GameObject popUp;

        [SerializeField]
        private UnityEngine.UI.Text[] policyDisplay;

        [SerializeField]
        List<GameObject> enableAfterConsent;

        [SerializeField]
        private string policyLink = "http://tedrasoft.com/privacy/privacy_policy_app.html";

        private bool personalized = false;
        private const string prefStorageKey = "gdprconsent";
        private const string prefFirstTimeKey = "gdprfirst";

        public event System.Action OnConsentChange;

        public int AsInt { get { return personalized ? 1 : 0; } }
        public bool AsBool { get { return personalized; } }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            if (Screen.width > Screen.height)
                popUp = popUpLandscape;
            else
                popUp = popUpPortrait;

            ChangeItemsState(false);

            foreach(var text in policyDisplay)
                text.text = policyLink;
            personalized = PlayerPrefs.GetInt(prefStorageKey, 0) == 1;
            if (PlayerPrefs.GetInt(prefFirstTimeKey, 1) == 1)
                OpenPopUp();
            else
            {
                ChangeConsent(personalized);
            }

        }

        public void ChangeConsent(bool value)
        {
            personalized = value;
            PlayerPrefs.SetInt(prefFirstTimeKey, 0);
            PlayerPrefs.SetInt(prefStorageKey, value ? 1 : 0);
            if (OnConsentChange != null)
                OnConsentChange();

            ChangeItemsState(true);
        }

        public void OpenPopUp()
        {
            popUp.gameObject.SetActive(true);
        }

        public void OpenPrivacyPolicy()
        {
            Application.OpenURL(policyLink);
        }

        public void ChangeItemsState(bool state)
        {
            foreach (var i in enableAfterConsent)
                i.gameObject.SetActive(state);
        }
    }
}
