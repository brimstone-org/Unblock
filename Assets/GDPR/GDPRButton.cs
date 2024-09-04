using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDPR
{

    public class GDPRButton : MonoBehaviour
    {

        public void OpenGDPR()
        {
            GDPRConsentPanel.Instance.OpenPopUp();
        }
    }

}
