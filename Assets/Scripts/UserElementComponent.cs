using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class UserElementComponent : MonoBehaviour , IInputClickHandler
    {
        string m_userName;
        Action<string> m_callback;
        [SerializeField]
        TextMesh m_userTextMesh;
        public void Initialize(string _userName, Action<string> _callback)
        {
            m_userName = _userName;
            m_userTextMesh.text = m_userName;
            m_callback = _callback;
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            m_callback?.Invoke(m_userName);
        }
    }
}
