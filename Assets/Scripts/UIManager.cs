using Assets.Scripts.Models;
using HoloToolkit.Unity;
using RogoDigital.Lipsync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static Assets.Scripts.QuestionDisplayer;
using Button = HoloToolkit.Unity.Buttons.Button;
namespace Assets.Scripts
{
    public class UIManager : Singleton<UIManager>
    {
        #region UserName Input Related
        [SerializeField]
        GameObject m_userNameParentGameObject;
        [SerializeField]
        InputField m_usernameInputField;
        [SerializeField]
        Button m_loginButton, m_createAccountButton;

        #endregion
        #region GreetingRelated
        [SerializeField]
        TextMesh m_greetingTextMesh;
        [SerializeField]
        public GameObject m_greetingParentGameObject;
        [SerializeField]
        public LipSync m_LipSync;
        [SerializeField]
        LipSyncData m_greetMessage1, m_greetMessage2;
        [SerializeField]
        Button m_continueGreetingPartTwoButton, m_proceedToSectionOneButton, m_proceedToSectionTwoButton, m_proceedToSectionThreeButton, m_proceedToSectionFourButton, m_proceedToExerciseSectionButton, m_toMainMenuButton;

        private void GreetUserPartOne()
        {
            m_greetingParentGameObject.gameObject.SetActive(true);
            m_greetingTextMesh.gameObject.SetActive(true);
            m_greetingTextMesh.text = @"Hi there! Welcome to your new augmented 
rehabilitation environment. Over the next eight weeks 
I’m going to prescribe you some exercises 
that are going to make you stronger, fitter 
and healthier. Firstly, I just need to ask you a 
few questions so I know a bit more about you. 
Are you ready to continue?";
            m_continueGreetingPartTwoButton.gameObject.SetActive(true);
            m_LipSync.Play(m_greetMessage1);
        }

        private void M_continueGreetingPartTwoButton_OnButtonClicked(GameObject obj)
        {

        }

        private void GreetUserPartTwo(GameObject obj)
        {
            m_greetingTextMesh.text =

@"Start by selecting a Section depending on your previous 
session or if it's your first time.
To answer the questions click on the right answers.
If there is a list shown on the screen,
make sure you pick an answer from that list.";

            m_LipSync.Play(m_greetMessage2);

            ShowProceedToSectionButtons(true);
            m_continueGreetingPartTwoButton.gameObject.SetActive(false);
        }

        public void ShowProceedToSectionButtons(bool v)
        {
            m_proceedToSectionOneButton.gameObject.SetActive(v);
            m_proceedToSectionTwoButton.gameObject.SetActive(v);
            m_proceedToSectionThreeButton.gameObject.SetActive(v);
            m_proceedToSectionFourButton.gameObject.SetActive(v);
            m_proceedToExerciseSectionButton.gameObject.SetActive(v);
            m_toMainMenuButton.gameObject.SetActive(v);
        }
        public void ForceShowProceedToSectionButtons(bool v)
        {
            if (!m_greetingParentGameObject.activeSelf)
                m_greetingParentGameObject.SetActive(true);

            m_greetingTextMesh.gameObject.SetActive(false);
            ReportViewer.Instance.m_backToReportsListButton.gameObject.SetActive(false);

            m_proceedToSectionOneButton.gameObject.SetActive(v);
            m_proceedToSectionTwoButton.gameObject.SetActive(v);
            m_proceedToSectionThreeButton.gameObject.SetActive(v);
            m_proceedToSectionFourButton.gameObject.SetActive(v);
            m_proceedToExerciseSectionButton.gameObject.SetActive(v);

            m_toMainMenuButton.gameObject.SetActive(v);
        }
        #endregion
        private void StartSectionOne(GameObject obj)
        {
            //TODO: In case no record of section one exists for given user,
            //Display a dialog to tell the user. For now, we're just blocking the start 
            //of section two
            if (!DatabaseManager.Instance.CanExecuteSection(
                DatabaseManager.Instance.CurrentUsername,
                SectionType.One))
                return;

            QuestionDisplayer.Instance.InitializeQuestions(QuestionDisplayer.SectionType.One);
            m_greetingParentGameObject.gameObject.SetActive(false);
        }
        private void StartSectionTwo(GameObject obj)
        {
            //TODO: In case no record of section one exists for given user,
            //Display a dialog to tell the user. For now, we're just blocking the start 
            //of section two
            if (!DatabaseManager.Instance.CanExecuteSection(
                DatabaseManager.Instance.CurrentUsername,
                SectionType.Two))
                return;

            QuestionDisplayer.Instance.InitializeQuestions(QuestionDisplayer.SectionType.Two);
            m_greetingParentGameObject.gameObject.SetActive(false);
        }
        private void StartSectionThree(GameObject obj)
        {
            //TODO: In case no record of section one exists for given user,
            //Display a dialog to tell the user. For now, we're just blocking the start 
            //of section two
            if (!DatabaseManager.Instance.CanExecuteSection(
                DatabaseManager.Instance.CurrentUsername,
                SectionType.Three))
                return;

            QuestionDisplayer.Instance.InitializeQuestions(QuestionDisplayer.SectionType.Three);
            m_greetingParentGameObject.gameObject.SetActive(false);
        }
        private void StartSectionFour(GameObject obj)
        {
            SectionFourController.Instance.StartSection();
            m_greetingParentGameObject.gameObject.SetActive(false);
        }
        private void StartExerciseSection(GameObject obj)
        {
            var reports = DatabaseManager.Instance.UserReports(
            DatabaseManager.Instance.CurrentUsername);

            if (reports == null)
                return;

            var sectionOneReport = reports.LastOrDefault(report => report.SectionType == QuestionDisplayer.SectionType.One);
            if (sectionOneReport == null)
                return;

            var sectionTwoReport = reports.LastOrDefault(report => report.SectionType == QuestionDisplayer.SectionType.Two);
            if (sectionTwoReport == null)
                return;

            var sectionThreeReport = reports.LastOrDefault(report => report.SectionType == QuestionDisplayer.SectionType.Three);
            if (sectionThreeReport == null)
                return;


            ExerciseSectionController.Instance.StartSection(sectionOneReport, sectionTwoReport, sectionThreeReport);
            m_greetingParentGameObject.gameObject.SetActive(false);
        }
        #region MainMenu Related
        [SerializeField]
        Button m_quizModeButton, m_showReportsButton;
        public void ShowMainMenu(bool active)
        {
            m_quizModeButton.gameObject.SetActive(active);
            m_showReportsButton.gameObject.SetActive(active);
            ReportViewer.Instance.m_backToReportsListButton.gameObject.SetActive(false);
        }

        private void ShowUsersAllList(GameObject obj)
        {
            if (DatabaseManager.Instance.Reports != null && DatabaseManager.Instance.Reports.Count > 0)
            {
                ReportViewer.Instance.ShowUsersList();
                //ReportViewer.Instance.DisplayReport(DatabaseManager.Instance.Reports.Last());
                ShowMainMenu(false);
            }
        }

        private void M_quizModeButton_OnButtonClicked(GameObject obj)
        {
            m_userNameParentGameObject.gameObject.SetActive(true);
            ShowMainMenu(false);
        }
        #endregion
        private void Start()
        {
            m_loginButton.OnButtonClicked += OnLogin_ButtonClick;
            m_createAccountButton.OnButtonClicked += OnCreateAccount_ButtonClick;
            m_continueGreetingPartTwoButton.OnButtonClicked += GreetUserPartTwo;


            m_proceedToSectionOneButton.OnButtonClicked += StartSectionOne;
            m_proceedToSectionTwoButton.OnButtonClicked += StartSectionTwo;
            m_proceedToSectionThreeButton.OnButtonClicked += StartSectionThree;
            m_proceedToSectionFourButton.OnButtonClicked += StartSectionFour;
            m_proceedToExerciseSectionButton.OnButtonClicked += StartExerciseSection;

            m_quizModeButton.OnButtonClicked += M_quizModeButton_OnButtonClicked;
            m_showReportsButton.OnButtonClicked += ShowUsersAllList;

            m_toMainMenuButton.OnButtonClicked += ToMainMenuClick;
        }

        private void ToMainMenuClick(GameObject obj)
        {
            ForceShowProceedToSectionButtons(false);
            ShowMainMenu(true);
        }

        private void OnCreateAccount_ButtonClick(GameObject obj)
        {
            string userName = m_usernameInputField.text;

            if (DatabaseManager.Instance.UserExists(userName))
                Log("Username already Exists!!");
            else
            {
                DatabaseManager.Instance.StoreUser(userName);
                Login(m_usernameInputField.text);
            }
        }
        private void OnLogin_ButtonClick(GameObject obj)
        {
            string userName = m_usernameInputField.text;
            if (!DatabaseManager.Instance.UserExists(userName))
                Log("User doesn't exist");
            else
                Login(m_usernameInputField.text);
        }
        void Login(string _username)
        {
            Log("");
            DatabaseManager.Instance.CurrentUsername = _username;
            GreetUserPartOne();
            m_userNameParentGameObject.gameObject.SetActive(false);
        }
        [SerializeField]
        TextMesh m_logTextMesh;
        void Log(string _message)
        {
            m_logTextMesh.text = _message;
        }
    }
}
