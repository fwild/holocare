using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using RogoDigital.Lipsync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class SectionFourController : Singleton<SectionFourController>
    {
        public enum Advice
        {
            GeneralWellBeing,
            Hypertension,
            CoronaryHeartDisease,
            COPD,
            Asthma,
            Ostearthritis,
            RheumatoidArthritis,
            JointReplacement,
            Osteoporosis,
            SimpleMechanicalBackPain,
            StressDepressionAnxiety,
            Type1Diabetes,
            Type2Diabetes,
        }

        




      [Serializable]
        public struct PanelDatum
        {
            public Advice Adivce;
            public GameObject Panel;
            public LipSyncData AudioTrack;
        }
        Advice m_currentAdvice;
        Dictionary<Advice, PanelDatum> m_Dictionary = new Dictionary<Advice, PanelDatum>();
        [SerializeField]
        GameObject m_rootGO;
        [SerializeField]
        PanelDatum[] m_datum;
        [SerializeField]
        Button m_ToNextButton, m_ToPreviousButton, m_exitSectionButton;
        public void StartSection()
        {
            m_rootGO.SetActive(true);
            ShowPanel(Advice.GeneralWellBeing);
            ShowPreviousButton(m_currentAdvice != Advice.GeneralWellBeing);
            ShowNextButton(m_currentAdvice != Advice.Type2Diabetes);
            m_exitSectionButton.gameObject.SetActive(true);
        }
        private void Start()
        {
            InitializeDictionary();
            m_ToNextButton.OnButtonClicked += ToNextPanel;
            m_ToPreviousButton.OnButtonClicked += ToPreviousPanel;
            m_exitSectionButton.OnButtonClicked += ExitSectionFour;
        }

        private void InitializeDictionary()
        {
            foreach (var item in m_datum)
            {
                m_Dictionary.Add(item.Adivce, item);
            }
        }

        private void ExitSectionFour(GameObject obj)
        {
            foreach (var item in m_Dictionary)
            {
                item.Value.Panel.SetActive(false);
            }
            m_rootGO.SetActive(false);

            UIManager.Instance.ForceShowProceedToSectionButtons(true);
            //UIManager.Instance.ShowMainMenu(true);
        }

        private void ToPreviousPanel(GameObject obj)
        {
            m_currentAdvice--;
            ShowPanel(m_currentAdvice);
            ShowPreviousButton(m_currentAdvice != Advice.GeneralWellBeing);
            ShowNextButton(m_currentAdvice != Advice.Type2Diabetes);

        }

        private void ToNextPanel(GameObject obj)
        {
            m_currentAdvice = m_currentAdvice + 1;
            ShowPanel(m_currentAdvice);
            ShowPreviousButton(m_currentAdvice != Advice.GeneralWellBeing);
            ShowNextButton(m_currentAdvice != Advice.Type2Diabetes);
        }

        private void ShowPanel(Advice advice)
        {
            m_currentAdvice = advice;
            foreach (var item in m_Dictionary)
            {
                item.Value.Panel.SetActive(false);
            }

            m_Dictionary[m_currentAdvice].Panel.SetActive(true);
            UIManager.Instance.m_LipSync.Play(m_Dictionary[m_currentAdvice].AudioTrack);
        }

        private void ShowPreviousButton(bool v)
        {
            m_ToPreviousButton.gameObject.SetActive(v);
        }
        private void ShowNextButton(bool v)
        {
            m_ToNextButton.gameObject.SetActive(v);
        }
    }
}
