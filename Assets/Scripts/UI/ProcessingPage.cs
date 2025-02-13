using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    public class ProcessingPage : Page
    {
        [SerializeField]private TMP_Text processingText;

        public void SetProcessingText(string text)
        {
            processingText.text = text;
        }
    }
}
