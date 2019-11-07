using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Models;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Assets.Scripts
{
    public class ReportElementComponent : MonoBehaviour, IInputClickHandler
    {
        ReportInstance m_reportInstance;
        Action<ReportInstance> m_callback;
        [SerializeField]
        TextMesh m_reportSectionTextMesh, m_reportTakenAtTextMesh;
        public void Initialize(ReportInstance reportInstance, Action<ReportInstance> onReportElementClick)
        {
            m_reportInstance = reportInstance;
            m_callback = onReportElementClick;

            m_reportSectionTextMesh.text = reportInstance.SectionType.ToString();
            m_reportTakenAtTextMesh.text = reportInstance.TakenAt;
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            m_callback?.Invoke(m_reportInstance);
        }
    }
}
