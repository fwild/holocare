using Assets.Scripts.Models;
using HoloToolkit.Unity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.QuestionDisplayer;

namespace Assets.Scripts
{
    public class DatabaseManager : Singleton<DatabaseManager>
    {
        public string CurrentUsername { get; set; }
        public List<ReportInstance> Reports { get; private set; }
        public List<string> Users { get; private set; }
        private void Start()
        {
            if (!PlayerPrefs.HasKey("V2"))
            {
                PlayerPrefs.DeleteAll();
                string usersPath = Path.Combine(Application.persistentDataPath, "Users.json");
                string reportsPath = Path.Combine(Application.persistentDataPath, "Reports.json");
                if (File.Exists(usersPath))
                    File.Delete(usersPath);
                if (File.Exists(reportsPath))
                    File.Delete(reportsPath);

                PlayerPrefs.SetString("V2", "Value");
            }
            LoadUsers();
            LoadReports();
        }

        private void LoadUsers()
        {
            string usersPath = Path.Combine(Application.persistentDataPath, "Users.json");
            if (!File.Exists(usersPath))
            {
                Users = new List<string>();
                string json = JsonConvert.SerializeObject(Users);
                File.WriteAllText(usersPath, json);
            }
            else
            {
                Users =
                    JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(usersPath));
            }
        }

        private void LoadReports()
        {
            string reportsPath = Path.Combine(Application.persistentDataPath, "Reports.json");
            if (!File.Exists(reportsPath))
            {
                Reports = new List<ReportInstance>();
                string json = JsonConvert.SerializeObject(Reports);
                File.WriteAllText(reportsPath, json);
            }
            else
            {
                Reports =
                    JsonConvert.DeserializeObject<List<ReportInstance>>(File.ReadAllText(reportsPath));
            }
        }

        public void StoreInstance(List<Question> _questions, SectionType sectionType)
        {
            DateTime dateTaken = DateTime.Now;
            ReportInstance instance = new ReportInstance()
            {
                SectionType = sectionType,
                Questions = _questions,
                TakenAt = string.Format("HH:MM:SS DD/MM/YYYY | {0}:{1}:{2} {3}/{4}/{5}",
                    dateTaken.Hour, dateTaken.Minute, dateTaken.Second, dateTaken.Day, dateTaken.Month, dateTaken.Year),
                UserName = CurrentUsername,
                WeighType = CalculateWeigh(sectionType, _questions)
            };

            Debug.LogFormat("Stored Report with WeighType: {0}", instance.WeighType);
            Reports.Add(instance);

            string reportsPath = Path.Combine(Application.persistentDataPath, "Reports.json").Replace("\\\\", "\\").Replace("\\", "/");
            File.WriteAllText(reportsPath,
                JsonConvert.SerializeObject(Reports));
        }

        private WeighType CalculateWeigh(SectionType sectionType, List<Question> questions)
        {
            if (sectionType != SectionType.One)
                return WeighType.NaN;

            Question heightQuestion = questions.First(quest => quest.ID == QuestionID.Height);
            string heightAnswer = heightQuestion.Answer;
            if (!string.IsNullOrEmpty(heightAnswer) && heightAnswer.Contains("."))
                heightAnswer = heightAnswer.Replace(".", "");
            int height = 0; float weight = 0;
            if (!int.TryParse(heightAnswer, out height))
                return WeighType.NaN;

            Question weightQuestion = questions.First(quest => quest.ID == QuestionID.Weigh);
            string weightAnswer = weightQuestion.Answer;
            if (!string.IsNullOrEmpty(weightAnswer) && weightAnswer.Contains("."))
                weightAnswer = weightAnswer.Replace(".", "");
            if (!float.TryParse(weightAnswer, out weight))
                return WeighType.NaN;

            if (height == 0)
                return WeighType.NaN;

            float ratio = weight / (height * height);
            return (ratio >= 25) ? WeighType.Obese : WeighType.NonObese;
        }

        public void StoreUser(string _user)
        {
            Users.Add(_user);

            string usersPath = Path.Combine(Application.persistentDataPath, "Users.json").Replace("\\\\", "\\").Replace("\\", "/");
            File.WriteAllText(usersPath,
                JsonConvert.SerializeObject(Users));
        }
        public bool UserExists(string _user)
        {
            return Users.Contains(_user);
        }

        public IEnumerable<ReportInstance> UserReports(string userName)
        {
            return Reports.Where(report => report.UserName == userName);
        }

        public bool CanExecuteSection(string userName, SectionType sectionType)
        {
            if (sectionType == SectionType.One)
                return true;

            SectionType previousSectionType = sectionType - 1;

            IEnumerable<ReportInstance> reports = DatabaseManager.Instance.UserReports(
                DatabaseManager.Instance.CurrentUsername);

            if (reports == null || reports.Count() == 0)
                return false;

            var sectionOne = reports
                .Where(report => report.SectionType == previousSectionType);
            if (sectionOne == null || sectionOne.Count() == 0)
                return false;

            return true;
        }
    }
}