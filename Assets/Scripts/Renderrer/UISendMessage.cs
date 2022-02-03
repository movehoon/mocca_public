using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.Animation
{
    public class UISendMessage : MonoBehaviour
    {
        public InputField inputField;
        public RobotFacialRenderer robotFacialRenderer;
        public MessageTranslator messageTranslator;

        public void SendInputMesage()
        {
            messageTranslator.SetMessage(inputField.text);
            //robotFacialRenderer.Play(inputField.text);
        }
    }
}