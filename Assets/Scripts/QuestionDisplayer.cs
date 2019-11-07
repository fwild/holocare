using Assets.Scripts.Models;
using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using RogoDigital.Lipsync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class QuestionDisplayer : Singleton<QuestionDisplayer>
    {
        public Dictionary<QuestionID, Question> Dictionary;
        public List<Question> Questions;
        [SerializeField]
        TextMesh m_questionHeader, m_recordedAnswer;
        [SerializeField]
        Button m_toNextQuestionButton, m_toPreviousQuestionButton, m_exitQuizButton;
        [SerializeField]
        RecorderButton m_recorderButton;
        [SerializeField]
        RecorderToggle m_recorderToggle;
        [SerializeField]
        SliderGestureControl m_sliderGestureControl;
        #region Responses Related
        [SerializeField]
        Transform m_responsesParentTransform;
        List<InteractiveToggle> m_createdResponses = new List<InteractiveToggle>();
        [SerializeField]
        InteractiveSet ParentPrefab;
        [SerializeField]
        InteractiveToggle ChildInteractivePrefab;

        InteractiveSet m_createdParentSet;
        #endregion

        private void Start()
        {
            m_toNextQuestionButton.OnButtonClicked += ToNextQuestion_ButtonClick;
            m_toPreviousQuestionButton.OnButtonClicked += ToPreviousQuestion_ButtonClick;
            m_recorderButton.OnDictationResulted += M_recorderButton_OnDictationResulted;
            Button recordHTKButton = m_recorderButton.GetComponent<Button>();
            recordHTKButton.OnButtonClicked += (gObject) =>
            {
                m_recorderButton.ToggleRecording();
            };
            m_exitQuizButton.OnButtonClicked += M_exitQuizButton_OnButtonClicked;

            InteractiveToggle toggle = m_recorderToggle.GetComponent<InteractiveToggle>();
            toggle.PassiveMode = true;

            m_storeSectionOneButton.OnButtonClicked += OnStoreSectionOne_ButtonClick;
            m_storeSectionTwoButton.OnButtonClicked += OnStoreSectionTwo_ButtonClick;
            m_storeSectionThreeButton.OnButtonClicked += OnStoreSectionThree_ButtonClick;

            m_navigateToSectionOneGoalsPanelButton.OnButtonClicked += OnNavigateToGoalsPanelButtonClick;

            m_recorderToggle.OnDictationResulted += M_recorderToggle_OnDictationResulted;

            CameraCache.Main.nearClipPlane = 0.01f;
        }

        private void M_recorderToggle_OnDictationResulted(string obj)
        {
            Dictionary[m_currentQuestionID].Answer = obj;
        }

        private void M_exitQuizButton_OnButtonClicked(GameObject obj)
        {
            Exit();
        }
        public void Exit()
        {
            m_recorderToggle.gameObject.SetActive(false);
            m_sliderGestureControl.gameObject.SetActive(false);
            m_questionHeader.text = "";
            ShowNextArrow(false);
            ShowPreviousArrow(false);
            CleanupCreatedResponses();
            //UIManager.Instance.ShowMainMenu(true);
            ShowRecordingUI(false);
            m_recordedAnswer.text = "";
            m_exitQuizButton.gameObject.SetActive(false);

            ShowEndOfSectionOnePanel(false);
            SetQuestionHeader(" ");
            m_storeSectionOneButton.gameObject.SetActive(false);
            m_storeSectionTwoButton.gameObject.SetActive(false);
            m_storeSectionThreeButton.gameObject.SetActive(false);

            if (!m_isReportingMode)
                UIManager.Instance.ForceShowProceedToSectionButtons(true);
            else
                UIManager.Instance.ShowMainMenu(true);
        }
        public void ExitWithoutShowingMainMenu()
        {
            m_recorderToggle.gameObject.SetActive(false);
            m_sliderGestureControl.gameObject.SetActive(false);
            m_questionHeader.text = "";
            ShowNextArrow(false);
            ShowPreviousArrow(false);
            CleanupCreatedResponses();
            ShowRecordingUI(false);
            m_recordedAnswer.text = "";
            m_exitQuizButton.gameObject.SetActive(false);

            ShowEndOfSectionOnePanel(false);
            SetQuestionHeader(" ");
            m_storeSectionOneButton.gameObject.SetActive(false);
            m_storeSectionTwoButton.gameObject.SetActive(false);
            m_storeSectionThreeButton.gameObject.SetActive(false);
        }

        private void ShowRecordingUI(bool v)
        {
            m_recorderButton.gameObject.SetActive(false);
        }

        private void M_recorderButton_OnDictationResulted(string obj)
        {
            Question currentQuestion = Dictionary[m_currentQuestionID];
            currentQuestion.Answer = obj;
        }
        public void OnSliderValueChanged(float newSliderValue)
        {
            Question currentQuestion = Dictionary[m_currentQuestionID];
            currentQuestion.SliderContent.SelectedValue = newSliderValue;
        }
        private void ToPreviousQuestion_ButtonClick(GameObject obj)
        {
            m_lastTimeChangedQuestion = Time.time;
            m_currentQuestionID--;
            while ((Dictionary[m_currentQuestionID].NavigationFunction != null) && !Dictionary[m_currentQuestionID].NavigationFunction().ShouldSelect)
            {
                m_currentQuestionID--;
            }
            SetCurrentQuestion(m_currentQuestionID, !m_isReportingMode);
            if (DictationInputManager.IsListening)
                StartCoroutine(DictationInputManager.StopRecording());

        }
        float m_lastTimeChangedQuestion = 0;
        private void ToNextQuestion_ButtonClick(GameObject obj)
        {
            m_lastTimeChangedQuestion = Time.time;
            m_currentQuestionID++;
            while ((Dictionary[m_currentQuestionID].NavigationFunction != null) && !Dictionary[m_currentQuestionID].NavigationFunction().ShouldSelect)
            {
                if (m_currentQuestionID == QuestionID.HowIsBodyPartFeelin)
                {
                    if (!m_isReportingMode)
                        OnStoreSectionTwo_ButtonClick(null);
                    else
                        Exit();
                    return;
                }
                m_currentQuestionID++;
            }
            SetCurrentQuestion(m_currentQuestionID, !m_isReportingMode);
            if (DictationInputManager.IsListening)
                StartCoroutine(DictationInputManager.StopRecording());

        }
        bool m_isReportingMode;
        List<Question> SectionOneOldResponses;
        List<Question> GenerateSectionOneQuestions()
        {
            return new List<Question>
            {
                new Question
                {
                    ID=QuestionID.Gender,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content = "What is your gender ?",
                    Responses = new List<string>
                    {
                        "Male","Female"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.Gender)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.YearBorn,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content = "Which year were you born ?",
                    IsFreestyle=true,
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.YearBorn)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.Height,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content = "How tall are you ? (in centimeters)",
                    IsFreestyle=true,
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.Height)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.Weigh,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content = "How much do you weigh ?",
                    IsFreestyle=true,
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.Weigh)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.CancerType,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content="What type of cancer did you suffer from?",
                    Responses=new List<string>{"Breast cancer","Prostate cancer","Lymphoma"},
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.CancerType)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.CancerTreatmentEnd    ,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content="How long ago did your cancer treatment finish?",
                    Responses=new List<string>{
                        "<3 months ago","3-6 months ago","6-12 months ago","1-2years ago","2-3years ago","3-4 years ago","4-5years ago"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.CancerTreatmentEnd)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.CardiovascularDiseases,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content="Do you suffer from any of the following cardiovascular diseases?",
                    Responses=new List<string>{
                        "Hypertension","Angina","Coronary Heart Disease","None"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.CardiovascularDiseases)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.RespiratoryDisorders,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content="Do you suffer from any of the following respiratory disorders?",
                    Responses=new List<string>{
                        "Chronic obstructive pulmonary disease ","Asthma","None",
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.RespiratoryDisorders)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.SufferFromOsteoarthritis,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content="Do you suffer from Osteoarthritis?",
                    Responses=new List<string>{
                        "Yes","No"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.SufferFromOsteoarthritis)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.OsteoarthritisWhere,
                    NavigationFunction=
                    () => { return new QuestionSelectionArguments {
                        ShouldSelect = (Dictionary[QuestionID.SufferFromOsteoarthritis].SelectedResponse==0),
                        AudioClip=null}; },
                    Content="Where do you suffer from Osteoarthritis?",
                    Responses=new List<string>{
                        "Ankle","Knee","Hip","Shoulder","Elbow","Wrist","Hands"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.OsteoarthritisWhere)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.SufferFromRheumatoidArthritis,
                    NavigationFunction= QuestionSelectionArguments.DefaultYes,
                    Content="Do you suffer from Rheumatoid arthritis ?",
                    Responses=new List<string>{
                        "Yes","No"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.SufferFromRheumatoidArthritis)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.RheumatoidArthritisWhere,
                    NavigationFunction=
                    () => { return new QuestionSelectionArguments {
                        ShouldSelect = (Dictionary[QuestionID.SufferFromRheumatoidArthritis].SelectedResponse==0),
                        AudioClip=null}; },
                    Content="Where do you suffer from Rheumatoid arthritis ?",
                    Responses=new List<string>{
                        "Ankle","Knee","Hip","Shoulder","Elbow","Wrist","Hands"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.RheumatoidArthritisWhere)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.SufferFromJointReplacement,
                    NavigationFunction= QuestionSelectionArguments.DefaultYes,
                    Content="Do you suffer from Joint replacement?",
                    Responses=new List<string>{
                        "Yes","No"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.SufferFromJointReplacement)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.JointReplacementWhere,
                    NavigationFunction=
                    () => { return new QuestionSelectionArguments {
                        ShouldSelect = (Dictionary[QuestionID.SufferFromJointReplacement].SelectedResponse==0),
                        AudioClip=null}; },
                    Content="Where do you suffer from Joint replacement?",
                    Responses=new List<string>{
                        "Hip","Knee","Shoulder"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.JointReplacementWhere)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.Osteoporosis,
                    NavigationFunction= QuestionSelectionArguments.DefaultYes,
                    Content="Do you suffer from Osteoporosis?",
                    Responses=new List<string>{
                        "Yes","No"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.Osteoporosis)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.SufferFromMechanicalBackPain,
                    NavigationFunction= QuestionSelectionArguments.DefaultYes,
                    Content="Do you suffer from simple mechanical back pain?",
                    Responses=new List<string>{
                        "Yes","No"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.YearBorn)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.MechanicalBackPainWhere,
                    NavigationFunction=
                    () => { return new QuestionSelectionArguments {
                        ShouldSelect = (Dictionary[QuestionID.SufferFromMechanicalBackPain].SelectedResponse==0),
                        AudioClip=null}; },
                    Content="Where do you suffer from simple mechanical back pain?",
                    Responses=new List<string>{
                        "Upper back","Lower back","Spinal"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.MechanicalBackPainWhere)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.MentalHealthConditions,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content="Do you suffer from any of the following mental health conditions?",
                    Responses=new List<string>{
                        "Stress","Depression","Anxiety","No"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.MentalHealthConditions)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.DiabetesMelltus,
                    NavigationFunction= QuestionSelectionArguments.DefaultYes,
                    Content="Do you suffer from diabetes mellitus?",
                    Responses=new List<string>{
                        "Type 1 diabetes","Type 2 diabetes","No",
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.DiabetesMelltus)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.PhysicalActivityPerweek,
                    //Include Clip Here. "Physical activity is defined as ...."
                    NavigationFunction= QuestionSelectionArguments.DefaultYes,
                    Content="How much physical activity do you currently do a week?",
                    Responses=new List<string>{
                        "<30 minutes a week",
                        "[30-60] minutes per week","[60-150] minutes per week",">150minutes"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.PhysicalActivityPerweek)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.CurrentPhysicalActivityPerweek,
                    //Include also clip here. "exercise is defined as doing a planned"
                    NavigationFunction= QuestionSelectionArguments.DefaultYes,
                    Content="How much exercise are you currently doing a week?",
                    Responses=new List<string>{
                        "<30 minutes a week",
                        "[30-60] minutes per week","[60-150] minutes per week",">150minutes"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.CurrentPhysicalActivityPerweek)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.PainWhileExercicsing,
                    NavigationFunction= QuestionSelectionArguments.DefaultYes,
                    Content="Do you sometimes experience pain when exercising?",
                    Responses=new List<string>{
                        "Yes","No"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.PainWhileExercicsing)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.WhereFeelPain,
                    NavigationFunction=
                    () => { return new QuestionSelectionArguments {
                        ShouldSelect = (Dictionary[QuestionID.PainWhileExercicsing].SelectedResponse==0),
                        AudioClip=null}; },
                    Content="Where do you feel the pain?",
                    Responses=new List<string>{"Left or right wrist","Left or right elbow","Left or right shoulder","Left or right forearm","Left or right upper arm","Left or right ankle","Left or right knee","Left or right hip","Left or right calf","Left or right thigh","Left or right side of chest","Upper back","Lower back","Spinal","Neck","Left or right hand","Left or right foot",
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.WhereFeelPain)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.ExercicePainScale,
                    NavigationFunction=
                    () => { return new QuestionSelectionArguments {
                        ShouldSelect = (Dictionary[QuestionID.PainWhileExercicsing].SelectedResponse==0),
                        AudioClip=null}; },
                    Content="On a scale of 1-10 how much does it hurt when you exercise?",
                    SliderContent=new SliderContent{MinValue=0,MaxValue=10,MinDescription="No Pain",MaxDescription="Worst Pain",SelectedValue=5},
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.ExercicePainScale)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.GoalFromExercicePlan,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content="What do you want to achieve from this exercise plan?",
                    Responses=new List<string>
                    {
                        "Get fitter","Get stronger","Feel better mentally","Maintain a healthy weight",
                        "Improve functional ability","Reduce pain","Being able to parciipate in more social activities",
                        "Get better sleep","Reduce the risk of redeveloping cancer",
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.GoalFromExercicePlan)).FirstOrDefault().AudioTrack
                }
            };

        }
        List<Question> GenerateSectionTwoQuestions()
        {
            SectionOneOldResponses = new List<Question>();
            var currentUserOldReports = DatabaseManager.Instance.Reports.FirstOrDefault(report => report.UserName == DatabaseManager.Instance.CurrentUsername && report.SectionType == SectionType.One);
            if (currentUserOldReports != null)
                SectionOneOldResponses = currentUserOldReports.Questions;
            List<Question> toBeReturned = new List<Question>();
            toBeReturned.Add(
                new Question
                {
                    ID = QuestionID.HowUDoin,
                    NavigationFunction = QuestionSelectionArguments.DefaultYes,
                    Content = "Are you feeling okay today?",
                    Responses = new List<string> { "Yes", "No" },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.HowUDoin)).FirstOrDefault().AudioTrack
                });
            toBeReturned.Add(
                new Question
                {
                    ID = QuestionID.AnyNewPain,
                    //Gotta add the clîps
                    NavigationFunction = QuestionSelectionArguments.DefaultYes,
                    Content = "Do you have any new pain?",
                    Responses = new List<string> { "Yes", "No" },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.AnyNewPain)).FirstOrDefault().AudioTrack
                });
            toBeReturned.Add(
            new Question
            {
                ID = QuestionID.WhereDoYouFeelPain,
                NavigationFunction =
                () => { return new QuestionSelectionArguments { ShouldSelect = Dictionary[QuestionID.AnyNewPain].SelectedResponse == 0, AudioClip = null }; },
                Content = "Where do you feel the pain?",
                Responses = new List<string>{ "Left or right wrist", "Left or right elbow","Left or right shoulder","Left or right forearm",
                    "Left or right upper arm","Left or right ankle","Left or right knee","Left or right hip","Left or right calf","Left or right thigh","Left or right side of chest","Upper back","Lower back","Spinal","Neck","Left or right hand","Left or right food",},
                AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.WhereDoYouFeelPain)).FirstOrDefault().AudioTrack
            });
            toBeReturned.Add(
            //ne9ssa b9eya
            new Question
            {
                ID = QuestionID.AnyFatigue,
                //Gotta add the clîps
                NavigationFunction = QuestionSelectionArguments.DefaultYes,
                Content = "Have you been experiencing any fatigue recently?",
                Responses = new List<string> { "Yes", "No" },
                AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.AnyFatigue)).FirstOrDefault().AudioTrack
            });
            Question cancerTypeQuestion = SectionOneOldResponses.Count == 0 ? null : SectionOneOldResponses.First(quest => quest.ID == QuestionID.CancerType);
            string problem = string.Format("Have you been experiencing any problems related to your {0} cancer recently ?", cancerTypeQuestion == null ? ""
                : cancerTypeQuestion.SelectedResponse != -1 ? cancerTypeQuestion.Responses[cancerTypeQuestion.SelectedResponse] : "");

            toBeReturned.Add(
            //ne9ssa b9eya
            new Question
            {
                ID = QuestionID.AnyProblems,
                //Gotta add the clîps
                NavigationFunction = QuestionSelectionArguments.DefaultYes,
                Content = problem,
                Responses = new List<string> { "Yes", "No" },
                AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.AnyProblems)).FirstOrDefault().AudioTrack
            });
            toBeReturned.Add(new Question
            {
                ID = QuestionID.SayTheMatter,
                Content = "Do you want to say what the matter is? (You have the right not to share this information)",
                IsFreestyle = true,
                NavigationFunction = () =>
                {
                    return new QuestionSelectionArguments { ShouldSelect = Dictionary[QuestionID.AnyProblems].SelectedResponse == 0, AudioClip = null };
                },
                AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.SayTheMatter)).FirstOrDefault().AudioTrack
            });
            toBeReturned.Add(new Question
            {
                ID = QuestionID.HowIsBodyPartFeelin,
                Content = "Could you describe your pain's intensity?",
                SliderContent = new SliderContent { MinValue = 1, MaxValue = 10, MinDescription = "No Pain", MaxDescription = "Worst Pain Possible", SelectedValue = 5 },
                NavigationFunction = () =>
                {
                    return new QuestionSelectionArguments { ShouldSelect = Dictionary[QuestionID.AnyNewPain].SelectedResponse == 0, AudioClip = null };
                },
                AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.HowIsBodyPartFeelin)).FirstOrDefault().AudioTrack
            });
            return toBeReturned;
        }

        List<Question> GenerateSectionThreeQuestions()
        {
            return new List<Question>
            {
                new Question
                {
                    ID=QuestionID.EnjoyableExercise,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content = "Was your exercise session enjoyable ?",
                    Responses = new List<string>
                    {
                        "Yes","Mostly enjoyable","Neither enjoyable or disagreeable","Mostly disagreeable","No"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.EnjoyableExercise)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.ConfidenceWhileExercice,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content = "Did you feel confident doing the exercises ?",
                    Responses = new List<string>
                    {
                        "Yes","Mostly confident","Neither confident or unconfident","mostly unconfident","NO"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.ConfidenceWhileExercice)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.LikedExercices,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content = "What exercices did you like?",
                    IsFreestyle=true,
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.LikedExercices)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.DislikedExercices,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content = "What exercises did you dislike?",
                    IsFreestyle=true,
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.DislikedExercices)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.UndoableExercices,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content = "Were there any exercises that you couldn't do due to pain or for any other reason?",
                    IsFreestyle=true,
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.UndoableExercices)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.HowHardExerciceSession,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content="How hard would you say you found the exercise session?",
                    SliderContent=new SliderContent{MinValue=1,MaxValue=10,MinDescription="Nothing",MaxDescription="Maximual/Exhaustion",SelectedValue=5},
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.HowHardExerciceSession)).FirstOrDefault().AudioTrack
                },
                new Question
                {
                    ID=QuestionID.WorkoutDuration,
                    NavigationFunction=QuestionSelectionArguments.DefaultYes,
                    Content = "Was the workout too long, too short or just right?",
                    Responses = new List<string>
                    {
                        "Too long","Too short","Just right"
                    },
                    AudioTrack = LipSync_SectionOne.Where(p => p.Question.Equals(QuestionID.WorkoutDuration)).FirstOrDefault().AudioTrack
                },

            };

        }
        public enum SectionType { One, Two, Three }
        public void InitializeQuestions(SectionType sectionType)
        {
            QuestionID startingID = 0;
            Dictionary = new Dictionary<QuestionID, Question>();
            switch (sectionType)
            {
                case SectionType.One:
                    Questions = GenerateSectionOneQuestions();
                    startingID = 0;
                    break;
                case SectionType.Two:
                    startingID = QuestionID.HowUDoin;
                    Questions = GenerateSectionTwoQuestions();
                    ShowStoreSectionTwoButton(true);
                    break;
                case SectionType.Three:
                    startingID = QuestionID.EnjoyableExercise;
                    Questions = GenerateSectionThreeQuestions();
                    break;
                default:
                    break;
            }
            if (Dictionary != null) Dictionary.Clear();
            foreach (var quest in Questions)
            {
                Dictionary.Add(quest.ID, quest);
            }
            m_isReportingMode = false;
            SetCurrentQuestion(startingID, !m_isReportingMode);
        }
        public void DisplayReport(ReportInstance report)
        {
            Questions = report.Questions;
            if (Dictionary != null) Dictionary.Clear(); else Dictionary = new Dictionary<QuestionID, Question>();
            foreach (var quest in Questions)
            {
                Dictionary.Add(quest.ID, quest);
            }

            m_isReportingMode = true;
            QuestionID startingID = 0;
            switch (report.SectionType)
            {
                case SectionType.One:
                    startingID = 0;
                    break;
                case SectionType.Two:
                    startingID = QuestionID.HowUDoin;
                    break;
                case SectionType.Three:
                    startingID = QuestionID.EnjoyableExercise;
                    break;
                default:
                    break;
            }
            SetCurrentQuestion(startingID, !m_isReportingMode);
        }

        QuestionID m_currentQuestionID = 0;
        public void SetCurrentQuestion(QuestionID _questionID, bool allowEdit)
        {
            m_exitQuizButton.gameObject.SetActive(true);
            ShowNextArrow(_questionID != QuestionID.GoalFromExercicePlan
                && _questionID != QuestionID.WorkoutDuration
                && (_questionID != QuestionID.HowIsBodyPartFeelin) /*Add Later the end questionIDs of every section*/);
            ShowPreviousArrow(_questionID != QuestionID.Gender
                && _questionID != QuestionID.EnjoyableExercise
                && _questionID != QuestionID.HowUDoin/*Add Later the starting questionIDs of every section*/);
            ShowEndOfSectionOne_ButtonNavigation(allowEdit
                && _questionID == QuestionID.GoalFromExercicePlan);
            //ShowStoreSectionTwoButton(allowEdit &&  Dictionary.ContainsKey(QuestionID.HowUDoin) /*Which means we're in section 2*/
            //    && 
            //        ( (Dictionary[QuestionID.AnyNewPain].SelectedResponse ==0 && _questionID== QuestionID.HowIsBodyPartFeelin) 
            //          || (Dictionary[QuestionID.AnyProblems].SelectedResponse == 0 && _questionID==QuestionID.SayTheMatter && Dictionary[QuestionID.AnyNewPain].SelectedResponse != 0)
            //          || (Dictionary[QuestionID.AnyProblems].SelectedResponse != 0 && _questionID == QuestionID.)
            ShowStoreSectionThreeButton(allowEdit && _questionID == QuestionID.WorkoutDuration);

            m_currentQuestionID = _questionID;
            DisplayQuestion(Dictionary[_questionID], allowEdit);
            EnableRecordingAnswer(Dictionary[_questionID].IsFreestyle);
            EnableSlider(Dictionary[_questionID]);
        }

        private void ShowStoreSectionThreeButton(bool v)
        {
            m_storeSectionThreeButton.gameObject.SetActive(v);
        }
        private void ShowStoreSectionTwoButton(bool v)
        {
            m_storeSectionTwoButton.gameObject.SetActive(v);
        }
        #region End Of Section One Display
        bool m_isShowingSectionOneGoalsPanel;
        [SerializeField]
        GameObject m_GoalsPanel;
        [SerializeField]
        Button m_storeSectionOneButton, m_storeSectionTwoButton, m_storeSectionThreeButton, m_navigateToSectionOneGoalsPanelButton;
        private void ShowEndOfSectionOnePanel(bool m_isShowingSectionOneGoalsPanel)
        {
            m_GoalsPanel.gameObject.SetActive(m_isShowingSectionOneGoalsPanel);
            ShowPreviousArrow(false);
            ShowNextArrow(false);
            m_storeSectionOneButton.gameObject.SetActive(m_isShowingSectionOneGoalsPanel);
            m_navigateToSectionOneGoalsPanelButton.gameObject.SetActive(false);
        }
        private void OnNavigateToGoalsPanelButtonClick(GameObject obj)
        {
            m_exitQuizButton.gameObject.SetActive(false);
            SetQuestionHeader(" ");
            CleanupCreatedResponses();
            ShowEndOfSectionOnePanel(true);
        }
        void OnStoreSectionThree_ButtonClick(GameObject obj)
        {
            DatabaseManager.Instance.StoreInstance(this.Questions, SectionType.Three);
            //UIManager.Instance.ShowMainMenu(true);
            m_exitQuizButton.gameObject.SetActive(false);
            SetQuestionHeader(" ");
            CleanupCreatedResponses();
            ShowEndOfSectionOnePanel(false);
            m_storeSectionThreeButton.gameObject.SetActive(false);
            Exit();
            UIManager.Instance.ShowProceedToSectionButtons(true);
        }
        void OnStoreSectionTwo_ButtonClick(GameObject obj)
        {
            DatabaseManager.Instance.StoreInstance(this.Questions, SectionType.Two);
            Exit();
            //UIManager.Instance.ShowMainMenu(true);
            UIManager.Instance.ShowProceedToSectionButtons(true);
        }
        void OnStoreSectionOne_ButtonClick(GameObject obj)
        {
            DatabaseManager.Instance.StoreInstance(this.Questions, SectionType.One);
            UIManager.Instance.ShowProceedToSectionButtons(true);
            //UIManager.Instance.ShowMainMenu(true);
            ShowEndOfSectionOnePanel(false);
            UIManager.Instance.m_greetingParentGameObject.SetActive(true);
        }
        private void ShowEndOfSectionOne_ButtonNavigation(bool v)
        {
            m_navigateToSectionOneGoalsPanelButton.gameObject.SetActive(v);
        }
        #endregion

        private void EnableSlider(Question question)
        {
            if (question.SliderContent == null)
            {
                m_sliderGestureControl.gameObject.SetActive(false);
                return;
            }
            m_sliderGestureControl.gameObject.SetActive(true);

            m_sliderGestureControl.MaxSliderValue = question.SliderContent.MaxValue;
            m_sliderGestureControl.MinSliderValue = question.SliderContent.MinValue;
            m_sliderGestureControl.transform.Find("MinDescription").GetComponent<TextMesh>().text = question.SliderContent.MinDescription;
            m_sliderGestureControl.transform.Find("MaxDescription").GetComponent<TextMesh>().text = question.SliderContent.MaxDescription;
        }

        private void EnableRecordingAnswer(bool isFreestyle)
        {
            //m_recorderButton.gameObject.SetActive(isFreestyle);
            m_recordedAnswer.gameObject.SetActive(isFreestyle);
            m_recorderToggle.gameObject.SetActive(isFreestyle);
        }

        private void ShowNextArrow(bool active)
        {
            m_toNextQuestionButton.gameObject.SetActive(active);
            //m_storeReportButton.gameObject.SetActive(!active && !m_isReportingMode);
        }

        private void ShowPreviousArrow(bool active)
        {
            m_toPreviousQuestionButton.gameObject.SetActive(active);
        }

        public void DisplayQuestion(Question _question, bool allowEdit)
        {
            SetQuestionHeader(_question.Content);
            CleanupCreatedResponses();

            if (_question.Responses != null && _question.Responses.Count > 0)
                DisplayResponses(_question.Responses, allowEdit);

            m_recordedAnswer.text = _question.Answer;
            if (_question.AudioTrack != null)
                UIManager.Instance.m_LipSync.Play(_question.AudioTrack);
        }

        private void DisplayResponses(List<string> _responses, bool allowEdit)
        {
            CleanupCreatedResponses();
            int maxPerColumn = 9;
            float offset = 0;
            m_createdParentSet = Instantiate(ParentPrefab);
            m_createdParentSet.transform.parent = m_responsesParentTransform;
            m_createdParentSet.transform.localPosition = Vector3.zero;
            m_createdParentSet.transform.localRotation = Quaternion.identity;
            int index = -1;
            bool hasReachedSecondColumn = false;
            foreach (var response in _responses)
            {
                index++;
                if (index == maxPerColumn)
                {
                    hasReachedSecondColumn = true;
                    offset = 0;
                }

                var child = Instantiate(ChildInteractivePrefab);
                child.AllowSelection = allowEdit;
                child.AllowDeselect = false;
                child.transform.parent = m_createdParentSet.transform;
                child.transform.localPosition = new Vector3(hasReachedSecondColumn ? 0.35f : 0, -offset, 0);
                child.transform.localRotation = Quaternion.identity;
                m_createdParentSet.Interactives.Add(child);


                int maxCharPerRowElement = 25;
                var labelTheme = child.GetComponent<LabelTheme>();
                string responseToBeDisplayed = _responses.Count > maxPerColumn ? GenerateMultilineString(response, maxCharPerRowElement) : response;
                labelTheme.Default = responseToBeDisplayed;
                labelTheme.Selected = responseToBeDisplayed;

                offset += 0.057f;

                m_createdResponses.Add(child);
            }

            m_createdParentSet.gameObject.SetActive(true);

            Question currentQuestion = Dictionary[m_currentQuestionID];
            m_createdParentSet.SelectedIndices = currentQuestion.SelectedResponse == -1 ? new List<int> { } : new List<int> { currentQuestion.SelectedResponse };
            if (allowEdit)
            {
                m_createdParentSet.OnSelectionEvents.AddListener(new UnityEngine.Events.UnityAction(() =>
                {
                    currentQuestion.SelectedResponse = m_createdParentSet.SelectedIndices.Count != 0 ? m_createdParentSet.SelectedIndices[0] : -1;
                }));
            }
        }


        private void SetQuestionHeader(string _content)
        {
            m_questionHeader.text = GenerateMultilineString(_content, 35);
        }
        string GenerateMultilineString(string _content, int _lineLengthCount)
        {
            StringBuilder display = new StringBuilder();
            for (int i = 0; i < _content.Length; i++)
            {
                //We've reached the end of a line
                if (i % (_lineLengthCount - 1) == 0 && i != 0)
                {
                    //The words begins before line-jumping, and ends after line-jumping
                    if (_content[i] != ' ' && (i + 1) <= _content.Length - 1 && _content[i + 1] != ' ')
                        display.Append('-');

                    display.Append('\n');
                }
                display.Append(_content[i]);
            }
            return display.ToString();
        }
        private void CleanupCreatedResponses()
        {
            for (int i = m_createdResponses.Count - 1; i >= 0; i--)
            {
                Destroy(m_createdResponses[i].gameObject);
                m_createdResponses.RemoveAt(i);
            }

            Destroy(m_createdParentSet);
        }



        [Serializable]
        public struct QuestionAudio
        {
            public QuestionID Question;
            public LipSyncData AudioTrack;
        }
        public QuestionAudio[] LipSync_SectionOne, LipSync_SectionTwo, LipSync_SectionThree;

    }


}
