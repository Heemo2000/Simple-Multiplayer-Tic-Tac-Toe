using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public abstract class Popup : Page
    {
        [SerializeField]protected Button okButton;
        [SerializeField]protected Button cancelButton;
    }
}
