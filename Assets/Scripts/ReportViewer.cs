using Assets.Scripts.Models;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class ReportViewer : Singleton<ReportViewer>
    {
        [SerializeField]
        Button m_userElementPrefab, m_userReportElementPrefab, m_nextPageButton, m_previousPageButton, m_exitAllButton;
        public Button m_backToReportsListButton;
        [SerializeField]
        Transform m_rootTransform;
        [SerializeField]
        TextMesh m_header;
        [SerializeField]
        GameObject m_UIRoot;
        List<GameObject> m_createdUsersList = new List<GameObject>(), m_createdReportsList = new List<GameObject>();
        int m_currentPage = 0;
        public int PageCount = 5;
        public float Spacing = 7.25f;
        private void Start()
        {
            m_nextPageButton.OnButtonClicked += M_nextPageButton_OnButtonClicked;
            m_previousPageButton.OnButtonClicked += M_previousPageButton_OnButtonClicked;
            m_exitAllButton.OnButtonClicked += M_exitAllButton_OnButtonClicked;
            m_backToReportsListButton.OnButtonClicked += M_backToReportsListButton_OnButtonClicked;
        }

        private void M_backToReportsListButton_OnButtonClicked(GameObject obj)
        {
            m_UIRoot.gameObject.SetActive(true);
            m_backToReportsListButton.gameObject.SetActive(false);
            QuestionDisplayer.Instance.ExitWithoutShowingMainMenu();
        }

        private void M_exitAllButton_OnButtonClicked(GameObject obj)
        {
            m_header.text = "";
            CleanUpUsersButtons();
            m_UIRoot.gameObject.SetActive(false);
            UIManager.Instance.ShowMainMenu(true);
        }

        public void ShowUsersList()
        {
            m_isShowingUserReports = false;
            m_UIRoot.gameObject.SetActive(true);
            m_header.text = "All Users";
            DatabaseManager.Instance.Users.Sort(new Comparison<string>((val1, val2) =>
            {
                return val1.CompareTo(val2);
            }));
            ShowUsersPage(0);
        }

        private void ShowUsersPage(int v)
        {
            CleanUpUsersButtons();
            m_currentPage = v;

            int minIndex = v * PageCount;
            int maxIndex = Math.Min((v + 1) * PageCount - 1, DatabaseManager.Instance.Users.Count - 1);

            float cumulatedSpacing = 0;
            for (int i = minIndex; i <= maxIndex; i++)
            {
                GameObject userElementGameObject = Instantiate(m_userElementPrefab.gameObject);
                userElementGameObject.transform.parent = m_rootTransform;
                userElementGameObject.transform.localPosition = Vector3.down * cumulatedSpacing;
                userElementGameObject.transform.localRotation = Quaternion.identity;
                cumulatedSpacing += Spacing;

                UserElementComponent userComponent = userElementGameObject.GetComponent<UserElementComponent>();
                userComponent.Initialize(DatabaseManager.Instance.Users[i], OnUserElementClick);

                m_createdUsersList.Add(userComponent.gameObject);
            }

            m_previousPageButton.gameObject.SetActive(v != 0);
            m_nextPageButton.gameObject.SetActive(((v + 1) * PageCount - 1) < DatabaseManager.Instance.Users.Count - 1);
        }

        private void CleanUpUsersButtons()
        {
            for (int i = m_createdUsersList.Count - 1; i >= 0; i--)
            {
                Destroy(m_createdUsersList[i]);
                m_createdUsersList.RemoveAt(i);
            }
            for (int i = m_createdReportsList.Count - 1; i >= 0; i--)
            {
                Destroy(m_createdReportsList[i]);
                m_createdReportsList.RemoveAt(i);
            }
        }

        void OnUserElementClick(string _userName)
        {
            IEnumerable<ReportInstance> reports = DatabaseManager.Instance.Reports.Where(rp => rp.UserName == _userName);
            if (reports != null && reports.Count() > 0)
                ShowReportsGivenUser(reports, 0);
        }
        int m_reportsCurrentPage;
        string m_selectedUserName;
        bool m_isShowingUserReports;
        IEnumerable<ReportInstance> m_reports;
        private void ShowReportsGivenUser(IEnumerable<ReportInstance> reports, int v)
        {
            m_header.text = "Reports";
            m_reports = reports;
            m_isShowingUserReports = true;
            CleanUpUsersButtons();
            m_reportsCurrentPage = v;

            int minIndex = v * PageCount;
            int maxIndex = Math.Min((v + 1) * PageCount - 1, reports.Count() - 1);

            float cumulatedSpacing = 0;
            for (int i = minIndex; i <= maxIndex; i++)
            {
                GameObject userReportElementGameObject = Instantiate(m_userReportElementPrefab.gameObject);
                userReportElementGameObject.transform.parent = m_rootTransform;
                userReportElementGameObject.transform.localPosition = Vector3.down * cumulatedSpacing;
                userReportElementGameObject.transform.localRotation = Quaternion.identity;
                cumulatedSpacing += Spacing;

                ReportElementComponent reportElementComponent = userReportElementGameObject.GetComponent<ReportElementComponent>();
                reportElementComponent.Initialize(reports.ElementAt(i), OnReportElementClick);

                m_createdReportsList.Add(reportElementComponent.gameObject);
            }

            m_previousPageButton.gameObject.SetActive(v != 0);
            m_nextPageButton.gameObject.SetActive(((v + 1) * PageCount - 1) < reports.Count() - 1);
        }
        void OnReportElementClick(ReportInstance reportInstance)
        {
            m_backToReportsListButton.gameObject.SetActive(true);
            m_UIRoot.gameObject.SetActive(false);
            QuestionDisplayer.Instance.DisplayReport(reportInstance);
        }

        private void M_previousPageButton_OnButtonClicked(GameObject obj)
        {
            if (!m_isShowingUserReports)
                ShowUsersPage(m_currentPage - 1);
            else
                ShowReportsGivenUser(m_reports, m_reportsCurrentPage - 1);
        }

        private void M_nextPageButton_OnButtonClicked(GameObject obj)
        {
            if (!m_isShowingUserReports)
                ShowUsersPage(m_currentPage + 1);
            else
                ShowReportsGivenUser(m_reports, m_reportsCurrentPage + 1);
        }


    }
}
