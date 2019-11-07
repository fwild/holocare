using HoloToolkit.Unity.InputModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class ToggleClickListener : MonoBehaviour, IInputClickHandler
    {
        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (!DictationInputManager.IsListening)
                StartCoroutine(DictationInputManager.StartRecording());
            else
                StartCoroutine(DictationInputManager.StopRecording());
        }
    }
}
