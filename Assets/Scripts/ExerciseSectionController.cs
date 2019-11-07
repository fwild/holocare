using Assets.Scripts;
using Assets.Scripts.Models;
using HoloToolkit.Unity.Buttons;
using RogoDigital.Lipsync;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExerciseSectionController : MonoBehaviour
{
    public static ExerciseSectionController Instance;

    public List<ExerciseData> Exercises;
    private void Awake()
    {
        Instance = this;
        Exercises = new List<ExerciseData>();
        foreach (Exercise exercise in Enum.GetValues(typeof(Exercise)).Cast<Exercise>())
        {
            Exercises.Add(new ExerciseData(exercise));
        }
    }
    [SerializeField]
    Animator m_animator;
    [SerializeField]
    GameObject m_rootGO;
    [SerializeField]
    Button m_ToNextButton, m_ToPreviousButton, m_exitSectionButton;


    public enum Exercise
    {
        A1_1éArm_Circles,
        A1_2éSide_Bend,
        A1_3éBig_Bird,
        A1_4éLittle_Bird,
        A1_5éPraying_Mantis,
        A1_6éHead_Nods,
        A1_7éShoulder_Taps,
        A1_8éWrist_Circles,
        A1_9éRolling_Shrug,
        A2_1éHip_Circles,
        A2_2éAnkle_Circles,
        A2_3éHigh_Knees,
        A2_4éSide_Steps,
        A3_1éAnkle_Hop,
        A3_2éHalf_Jacks,
        A3_3éWalk_On_The_Spot,
        B1_1éSquat,
        B1_2éLunge,
        B1_3éGlute_Bridge,
        B1_4éSit_To_Stand,
        B1_5éLunge_And_Balance,
        B1_6éClam,
        B1_7éGlute_Kickback,
        B1_8éCalf_Raise,
        B1_9éRdls,
        B2_1éWall_Press,
        B2_2éKneeling_Push_Up,
        B2_3éPush_Up,
        B3_1éBanded_Pull_Apart,
        B3_2éBent_Over_Row,
        B3_3éWytas,
        B3_4éProne_Press,
        B3_5éBack_Burns,
        B3_6éW_Holds,
        B4_1éLateral_Raises,
        B4_2éSeated_Ohp,
        B4_3éStanding_Ohp,
        B5_1éCrunches,
        B5_2éLying_Leg_Raises,
        B5_3éPlank,
        B5_4éOblique_Heal_Touch,
        B5_5éTorso_Twist,
        B5_6éSide_Plank,
        B6_1éTable_Tops,
        B6_2éQuad_Superman,
        B6_3éKegels,
        B6_4éSide_Lying_Bicycle,
        B7_1éBicep_Curl,
        B7_2éChair_Dip,
        B8_1éJogging_On_The_Spot,
        B8_2éJumping_Jacks,
        B8_3éHalf_Jacks,
        B8_4éAnkle_Hops,
        B8_5éWalk_On_The_Spot,
        B8_6éHigh_Knees,
        C1_1éJogging_On_The_Spot,
        C1_2éWalking_On_The_Spot,
        C1_3éHalf_Jacks,
        C1_4éHigh_Knees,
        C2_1éHamstrings_Stretch,
        C2_2éQuadriceps_Stretch,
        C2_3éCalf_Stretch,
        C2_4éGlutes_Stretch,
        C3_1éUpper_Back_Stretch,
        C3_2éNeck_Side_Stretch,
        C3_3éTriceps_Stretch,
        C3_4éChest_Stretch,
        C4_1éCat_Stretch,
        C4_2éCobra,
        C4_3éLow_Cobra
    }

    [Serializable]
    public class ExerciseData
    {
        public Exercise Exercise;
        //public GameObject Panel;
        public string Name;
        public int AnimatorBool;
        public bool isExluded;
        public ExerciseData(Exercise exercise)
        {
            Exercise = exercise;
            Name = exercise.ToString().Split('é')[1].Replace('_', ' ');
            AnimatorBool = Animator.StringToHash(exercise.ToString().Split('é')[0]);
            isExluded = false;
        }
    }

    int m_currentExerciseID;
    public void StartSection(ReportInstance firstSection, ReportInstance secondSection, ReportInstance thirdSection)
    {
        m_rootGO.SetActive(true);
        PullExercises(firstSection, secondSection, thirdSection);
        ShowPanel();
        m_exitSectionButton.gameObject.SetActive(true);
    }
    void PullExercises(ReportInstance firstSection, ReportInstance secondSection, ReportInstance thirdSection)
    {
        m_ActiveExercises = new List<ExerciseData>(Exercises);

        if (firstSection.WeighType == WeighType.Obese)
        {
            var exercises = new List<Exercise> { Exercise.A3_1éAnkle_Hop, Exercise.B8_1éJogging_On_The_Spot, Exercise.B8_2éJumping_Jacks, Exercise.B8_4éAnkle_Hops, Exercise.C1_1éJogging_On_The_Spot };
            for (int j = 0; j < exercises.Count; j++)
            {
                var existing = m_ActiveExercises.FirstOrDefault(mExec => mExec.Exercise == exercises[j]);
                if (existing != null)
                    m_ActiveExercises.Remove(existing);
            }
        }

        for (int i = 0; i < firstSection.Questions.Count; i++)
        {
            var exercises = ExcludeExercicesFromQuestion(firstSection.Questions[i]);

            if (exercises != null)
                for (int j = 0; j < exercises.Count; j++)
                {
                    var existing = m_ActiveExercises.FirstOrDefault(mExec => mExec.Exercise == exercises[j]);
                    if (existing != null)
                        m_ActiveExercises.Remove(existing);
                }
        }
        for (int i = 0; i < secondSection.Questions.Count; i++)
        {
            var exercises = ExcludeExercicesFromQuestion(secondSection.Questions[i]);

            if (exercises != null)
                for (int j = 0; j < exercises.Count; j++)
                {
                    var existing = m_ActiveExercises.FirstOrDefault(mExec => mExec.Exercise == exercises[j]);
                    if (existing != null)
                        m_ActiveExercises.Remove(existing);
                }
        }

        for (int i = 0; i < thirdSection.Questions.Count; i++)
        {
            var exercises = ExcludeExercicesFromQuestion(thirdSection.Questions[i]);

            if (exercises != null)
                for (int j = 0; j < exercises.Count; j++)
                {
                    var existing = m_ActiveExercises.FirstOrDefault(mExec => mExec.Exercise == exercises[j]);
                    if (existing != null)
                        m_ActiveExercises.Remove(existing);
                }
        }

        Debug.LogFormat("Pulled {0} from {1} exercises", m_ActiveExercises.Count, Exercises.Count);
    }
    //Could have used code reflection to auto-generate it, but less performant
    Dictionary<string, Exercise> Map = new Dictionary<string, Exercise>
    {
        {"A1.1",Exercise.A1_1éArm_Circles },
        {"A1.2",Exercise.A1_2éSide_Bend},
        {"A1.3",Exercise.A1_3éBig_Bird},
        {"A1.4",Exercise.A1_4éLittle_Bird},
        {"A1.5",Exercise.A1_5éPraying_Mantis },
        {"A1.6",Exercise.A1_6éHead_Nods},
        {"A1.7",Exercise.A1_7éShoulder_Taps},
        {"A1.8",Exercise.A1_8éWrist_Circles},
        {"A1.9",Exercise.A1_9éRolling_Shrug},
        {"A2.1",Exercise.A2_1éHip_Circles },
        {"A2.2",Exercise.A2_2éAnkle_Circles},
        {"A2.3",Exercise.A2_3éHigh_Knees},
        {"A2.4",Exercise.A2_4éSide_Steps },
        {"A3.1",Exercise.A3_1éAnkle_Hop},
        {"A3.2",Exercise.A3_2éHalf_Jacks},
        {"A3.3",Exercise.A3_3éWalk_On_The_Spot},
        {"B1.1",Exercise.B1_1éSquat},
        {"B1.2",Exercise.B1_2éLunge},
        {"B1.3",Exercise.B1_3éGlute_Bridge},
        {"B1.4",Exercise.B1_4éSit_To_Stand},
        {"B1.5",Exercise.B1_5éLunge_And_Balance},
        {"B1.6",Exercise.B1_6éClam},
        {"B1.7",Exercise.B1_7éGlute_Kickback},
        {"B1.8",Exercise.B1_8éCalf_Raise},
        {"B1.9",Exercise.B1_9éRdls},
        {"B2.1",Exercise.B2_1éWall_Press},
        {"B2.2",Exercise.B2_2éKneeling_Push_Up},
        {"B2.3",Exercise.B2_3éPush_Up},
        {"B3.1",Exercise.B3_1éBanded_Pull_Apart},
        {"B3.2",Exercise.B3_2éBent_Over_Row},
        {"B3.3",Exercise.B3_3éWytas},
        {"B3.4",Exercise.B3_4éProne_Press},
        {"B3.5",Exercise.B3_5éBack_Burns},
        {"B3.6",Exercise.B3_6éW_Holds},
        {"B4.1",Exercise.B4_1éLateral_Raises},
        {"B4.2",Exercise.B4_2éSeated_Ohp},
        {"B4.3",Exercise.B4_3éStanding_Ohp},
        {"B5.1",Exercise.B5_1éCrunches},
        {"B5.2",Exercise.B5_2éLying_Leg_Raises},
        {"B5.3",Exercise.B5_3éPlank},
        {"B5.4",Exercise.B5_4éOblique_Heal_Touch},
        {"B5.5",Exercise.B5_5éTorso_Twist},
        {"B5.6",Exercise.B5_6éSide_Plank},
        {"B6.1",Exercise.B6_1éTable_Tops},
        {"B6.2",Exercise.B6_2éQuad_Superman},
        {"B6.3",Exercise.B6_3éKegels},
        {"B6.4",Exercise.B6_4éSide_Lying_Bicycle},
        {"B7.1",Exercise.B7_1éBicep_Curl},
        {"B7.2",Exercise.B7_2éChair_Dip},
        {"B8.1",Exercise.B8_1éJogging_On_The_Spot},
        {"B8.2",Exercise.B8_2éJumping_Jacks},
        {"B8.3",Exercise.B8_3éHalf_Jacks},
        {"B8.4",Exercise.B8_4éAnkle_Hops},
        {"B8.5",Exercise.B8_5éWalk_On_The_Spot},
        {"B8.6",Exercise.B8_6éHigh_Knees},
        {"C1.1",Exercise.C1_1éJogging_On_The_Spot},
        {"C1.2",Exercise.C1_2éWalking_On_The_Spot},
        {"C1.3",Exercise.C1_3éHalf_Jacks},
        {"C1.4",Exercise.C1_4éHigh_Knees},
        {"C2.1",Exercise.C2_1éHamstrings_Stretch},
        {"C2.2",Exercise.C2_2éQuadriceps_Stretch},
        {"C2.3",Exercise.C2_3éCalf_Stretch},
        {"C2.4",Exercise.C2_4éGlutes_Stretch},
        {"C3.1",Exercise.C3_1éUpper_Back_Stretch},
        {"C3.2",Exercise.C3_2éNeck_Side_Stretch},
        {"C3.3",Exercise.C3_3éTriceps_Stretch},
        {"C3.4  ",Exercise.C3_4éChest_Stretch},
    };
    public List<Exercise> ExcludeExercicesFromQuestion(Question question)
    {
        List<string> toBeExluded = null;
        switch (question.ID)
        {
            case QuestionID.CancerType:
                break;
            case QuestionID.CancerTreatmentEnd:
                break;
            case QuestionID.SufferFromOsteoarthritis:
                break;
            case QuestionID.OsteoarthritisWhere:
                {
                    if (question.SelectedResponse == 0)
                        toBeExluded = new List<string>() { "A2.3, A2.4, A3.1, A3.2, A3.3, B1.1, B1.2, B1.5, B1.8, B8.1, B8.2, B8.3, B8.4, B8.5, B8.6, C1.1, C1.2, C1.3, C1.4" };
                    else if (question.SelectedResponse == 1)
                        toBeExluded = new List<string>() { "A2.3, A2.4, A3.1, A3.2, A3.3, B1.1, B1.2, B1.4, B1.5, B1.9, B6.2, B6.4, B8.1, B8.2, B8.3, B8.4, B8.5, B8.6, C1.1, C1.2, C1.3, C1.4, C2.2, C4.1" };
                    else if (question.SelectedResponse == 2)
                        toBeExluded = new List<string> { "B1.1, B1.2, B1.3, B1.4, B1.5, B1.6, B1.7, B1.9, B5.1, B5.2, B5.4, B6.2, B6.4, B8.1, B8.2, B8.3, B8.4, B8.6, C1.1, C1.2, C1.3, C1.4" };
                    else if (question.SelectedResponse == 3)
                        toBeExluded = new List<string> { "A1.7, B2.2, B2.3, B3.3, B3.4, B3.5, B3.6, B4.1, B4.2, B4.3, B6.2, B7.2" };
                    else if (question.SelectedResponse == 4)
                        toBeExluded = new List<string> { "B2.1, B2.2, B2.3, B4.2, B4.3, B5.3, B5.6, B7.2, C4.3" };
                    else if (question.SelectedResponse == 5)
                        toBeExluded = new List<string> { "B2.2, B2.3, B7.2, C4.2" };
                    else if (question.SelectedResponse == 6)
                        toBeExluded = new List<string> { "B2.1, B2.2, B2.3, B3.1, B3.2, B4.1, B4.2, B4.3, B7.1, B7.2" };
                }
                break;
            case QuestionID.SufferFromRheumatoidArthritis:
                break;
            case QuestionID.RheumatoidArthritisWhere:
                {
                    if (question.SelectedResponse == 0)
                        toBeExluded = new List<string>() { "A2.3, A2.4, A3.1, A3.2, A3.3, B1.1, B1.2, B1.5, B1.8, B8.1, B8.2, B8.3, B8.4, B8.5, B8.6, C1.1, C1.2, C1.3, C1.4" };
                    else if (question.SelectedResponse == 1)
                        toBeExluded = new List<string>() { "A2.3, A2.4, A3.1, A3.2, A3.3, B1.1, B1.2, B1.4, B1.5, B1.9, B6.2, B6.4, B8.1, B8.2, B8.3, B8.4, B8.5, B8.6, C1.1, C1.2, C1.3, C1.4, C2.2, C4.1" };
                    else if (question.SelectedResponse == 2)
                        toBeExluded = new List<string> { "B1.1, B1.2, B1.3, B1.4, B1.5, B1.6, B1.7, B1.9, B5.1, B5.2, B5.4, B6.2, B6.4, B8.1, B8.2, B8.3, B8.4, B8.6, C1.1, C1.2, C1.3, C1.4" };
                    else if (question.SelectedResponse == 3)
                        toBeExluded = new List<string> { "A1.7, B2.2, B2.3, B3.3, B3.4, B3.5, B3.6, B4.1, B4.2, B4.3, B6.2, B7.2" };
                    else if (question.SelectedResponse == 4)
                        toBeExluded = new List<string> { "B2.1, B2.2, B2.3, B4.2, B4.3, B5.3, B5.6, B7.2, C4.3" };
                    else if (question.SelectedResponse == 5)
                        toBeExluded = new List<string> { "B2.2, B2.3, B7.2, C4.2" };
                    else if (question.SelectedResponse == 6)
                        toBeExluded = new List<string> { "B2.1, B2.2, B2.3, B3.1, B3.2, B4.1, B4.2, B4.3, B7.1, B7.2" };
                }
                break;
            case QuestionID.MechanicalBackPainWhere:
                {
                    if (question.SelectedResponse == 0)
                        toBeExluded = new List<string>() { "B1.3, B3.1, B3.2, B3.3, B3.4, B3.5, B3.6" };
                    else if (question.SelectedResponse == 1)
                        toBeExluded = new List<string>() { "A3.1, B1.1, B1.3, B1.7, B1.9, B2.3, B3.2, B5.1, B5.3, B5.4, B5.5, B5.6, B8.1, B8.2, C1.1, C4.2, C4.3" };
                    else if (question.SelectedResponse == 2)
                        toBeExluded = new List<string> { "A3.1, B1.1, B1.3, B1.7, B1.9, B2.3, B3.2, B5.1, B5.3, B5.4, B5.5, B5.6, B8.1, B8.2, C1.1, C4.2, C4.3" };
                }
                break;
            case QuestionID.MentalHealthConditions:
                break;
            case QuestionID.DiabetesMelltus:
                break;
            case QuestionID.PhysicalActivityPerweek:
                break;
            case QuestionID.CurrentPhysicalActivityPerweek:
                break;
            case QuestionID.PainWhileExercicsing:
                break;
            case QuestionID.WhereFeelPain:
                {
                    if (question.SelectedResponse == 0)
                        toBeExluded = new List<string>() { "B2.2, B2.3, B7.2, C4.2" };
                    else if (question.SelectedResponse == 1)
                        toBeExluded = new List<string>() { "B2.1, B2.2, B2.3, B4.2, B4.3, B5.3, B5.6, B7.2, C4.3" };
                    else if (question.SelectedResponse == 2)
                        toBeExluded = new List<string> { "A1.7, B2.2, B2.3, B3.3, B3.4, B3.5, B3.6, B4.1, B4.2, B4.3, B6.2, B7.2" };
                    else if (question.SelectedResponse == 3)
                        toBeExluded = new List<string> { "B2.2, B2.3, B3.1, B3.2, B7.1" };
                    else if (question.SelectedResponse == 4)
                        toBeExluded = new List<string> { "B2.1, B2.2, B2.3, B4.2, B4.3, B7.1, B7.2" };
                    else if (question.SelectedResponse == 5)
                        toBeExluded = new List<string> { "A2.3, A2.4, A3.1, A3.2, A3.3, B1.1, B1.2, B1.5, B1.8, B8.1, B8.2, B8.3, B8.4, B8.5, B8.6, C1.1, C1.2, C1.3, C1.4 " };
                    else if (question.SelectedResponse == 6)
                        toBeExluded = new List<string> { "A2.3, A2.4, A3.1, A3.2, A3.3, B1.1, B1.2, B1.4, B1.5, B1.9, B6.2, B6.4, B8.1, B8.2, B8.3, B8.4, B8.5, B8.6, C1.1, C1.2, C1.3, C1.4, C2.2, C4.1" };
                    else if (question.SelectedResponse == 7)
                        toBeExluded = new List<string> { "B1.1, B1.2, B1.3, B1.4, B1.5, B1.6, B1.7, B1.9, B5.1, B5.2, B5.4, B6.2, B6.4, B8.1, B8.2, B8.3, B8.4, B8.6, C1.1, C1.2, C1.3, C1.4" };
                    else if (question.SelectedResponse == 8)
                        toBeExluded = new List<string> { "B1.1, B1.2, B1.3, B1.4, B1.5, B1.6, B1.7, B1.9, B5.1, B5.2, B5.4, B6.2, B6.4, B8.1, B8.2, B8.3, B8.4, B8.6, C1.1, C1.2, C1.3, C1.4" };
                    else if (question.SelectedResponse == 9)
                        toBeExluded = new List<string> { "B1.1, B1.2, B8.1, B8.2, C1.1" };
                    else if (question.SelectedResponse == 10)
                        toBeExluded = new List<string> { "B2.1, B2.2, B2.3, B3.3, B3.4, B3.5, B3.6, B7.2, C4.2" };
                    else if (question.SelectedResponse == 11)
                        toBeExluded = new List<string> { "A2.3, B1.1, B1.3, B1.5, B2.2, B2.3, B3.3, B3.4, B3.5, B3.6, B5.1, B5.2, B5.3, B5.4, B5.5, B5.5, B5.6, B8.1, B8.2, B8.4, B8.6, C1.1, C1.4, C4.2" };
                    else if (question.SelectedResponse == 12)
                        toBeExluded = new List<string> { "B1.3, B3.1, B3.2, B3.3, B3.4, B3.5, B3.6" };
                    else if (question.SelectedResponse == 13)
                        toBeExluded = new List<string> { "A3.1, B1.1, B1.3, B1.7, B1.9, B2.3, B3.2, B5.1, B5.2, B5.3, B5.4, B5.5, B5.6, B8.1, B8.2, C1.1, C4.2, C4.3" };
                    else if (question.SelectedResponse == 14)
                        toBeExluded = new List<string> { "A3.1, B1.1, B1.3, B1.7, B1.9, B2.3, B3.2, B5.1, B5.2, B5.3, B5.4, B5.5, B5.6, B8.1, B8.2, C1.1, C4.2, C4.3" };
                    else if (question.SelectedResponse == 15)
                        toBeExluded = new List<string> { "B3.3, B3.6, B4.1, B5.1" };
                    else if (question.SelectedResponse == 16)
                        toBeExluded = new List<string> { "B2.1, B2.2, B2.3, B3.1, B3.2, B4.1, B4.2, B4.3, B7.1, B7.2" };
                    else if (question.SelectedResponse == 17)
                        toBeExluded = new List<string> { "A2.3, A2.4, A3.1, A3.2, A3.3, B1.1, B1.2, B1.3, B1.4, B1.5, B1.8, B1.9, B8.1, B8.2, B8.3, B8.4, B8.5, B8.6, C1.1, C1.2, C1.3, C1.4" };
                }
                break;
            case QuestionID.ExercicePainScale:
                {
                    if (question.SliderContent != null && question.SliderContent.SelectedValue >= 4)
                        toBeExluded = new List<string> { "A3.1, B8.1, B8.2, B8.4, C1.1,C1.2,B2.2,B2.3,C4.2,B5.4,B5.5" };
                }
                break;
            case QuestionID.GoalFromExercicePlan:
                break;
            case QuestionID.HowUDoin:
                break;
            case QuestionID.AnyNewPain:
                break;
            //Same as QuestionID.WhereFeelPain
            case QuestionID.WhereDoYouFeelPain:
                {
                    if (question.SelectedResponse == 0)
                        toBeExluded = new List<string>() { "B2.2, B2.3, B7.2, C4.2" };
                    else if (question.SelectedResponse == 1)
                        toBeExluded = new List<string>() { "B2.1, B2.2, B2.3, B4.2, B4.3, B5.3, B5.6, B7.2, C4.3" };
                    else if (question.SelectedResponse == 2)
                        toBeExluded = new List<string> { "A1.7, B2.2, B2.3, B3.3, B3.4, B3.5, B3.6, B4.1, B4.2, B4.3, B6.2, B7.2" };
                    else if (question.SelectedResponse == 3)
                        toBeExluded = new List<string> { "B2.2, B2.3, B3.1, B3.2, B7.1" };
                    else if (question.SelectedResponse == 4)
                        toBeExluded = new List<string> { "B2.1, B2.2, B2.3, B4.2, B4.3, B7.1, B7.2" };
                    else if (question.SelectedResponse == 5)
                        toBeExluded = new List<string> { "A2.3, A2.4, A3.1, A3.2, A3.3, B1.1, B1.2, B1.5, B1.8, B8.1, B8.2, B8.3, B8.4, B8.5, B8.6, C1.1, C1.2, C1.3, C1.4 " };
                    else if (question.SelectedResponse == 6)
                        toBeExluded = new List<string> { "A2.3, A2.4, A3.1, A3.2, A3.3, B1.1, B1.2, B1.4, B1.5, B1.9, B6.2, B6.4, B8.1, B8.2, B8.3, B8.4, B8.5, B8.6, C1.1, C1.2, C1.3, C1.4, C2.2, C4.1" };
                    else if (question.SelectedResponse == 7)
                        toBeExluded = new List<string> { "B1.1, B1.2, B1.3, B1.4, B1.5, B1.6, B1.7, B1.9, B5.1, B5.2, B5.4, B6.2, B6.4, B8.1, B8.2, B8.3, B8.4, B8.6, C1.1, C1.2, C1.3, C1.4" };
                    else if (question.SelectedResponse == 8)
                        toBeExluded = new List<string> { "B1.1, B1.2, B1.3, B1.4, B1.5, B1.6, B1.7, B1.9, B5.1, B5.2, B5.4, B6.2, B6.4, B8.1, B8.2, B8.3, B8.4, B8.6, C1.1, C1.2, C1.3, C1.4" };
                    else if (question.SelectedResponse == 9)
                        toBeExluded = new List<string> { "B1.1, B1.2, B8.1, B8.2, C1.1" };
                    else if (question.SelectedResponse == 10)
                        toBeExluded = new List<string> { "B2.1, B2.2, B2.3, B3.3, B3.4, B3.5, B3.6, B7.2, C4.2" };
                    else if (question.SelectedResponse == 11)
                        toBeExluded = new List<string> { "A2.3, B1.1, B1.3, B1.5, B2.2, B2.3, B3.3, B3.4, B3.5, B3.6, B5.1, B5.2, B5.3, B5.4, B5.5, B5.5, B5.6, B8.1, B8.2, B8.4, B8.6, C1.1, C1.4, C4.2" };
                    else if (question.SelectedResponse == 12)
                        toBeExluded = new List<string> { "B1.3, B3.1, B3.2, B3.3, B3.4, B3.5, B3.6" };
                    else if (question.SelectedResponse == 13)
                        toBeExluded = new List<string> { "A3.1, B1.1, B1.3, B1.7, B1.9, B2.3, B3.2, B5.1, B5.2, B5.3, B5.4, B5.5, B5.6, B8.1, B8.2, C1.1, C4.2, C4.3" };
                    else if (question.SelectedResponse == 14)
                        toBeExluded = new List<string> { "A3.1, B1.1, B1.3, B1.7, B1.9, B2.3, B3.2, B5.1, B5.2, B5.3, B5.4, B5.5, B5.6, B8.1, B8.2, C1.1, C4.2, C4.3" };
                    else if (question.SelectedResponse == 15)
                        toBeExluded = new List<string> { "B3.3, B3.6, B4.1, B5.1" };
                    else if (question.SelectedResponse == 16)
                        toBeExluded = new List<string> { "B2.1, B2.2, B2.3, B3.1, B3.2, B4.1, B4.2, B4.3, B7.1, B7.2" };
                    else if (question.SelectedResponse == 17)
                        toBeExluded = new List<string> { "A2.3, A2.4, A3.1, A3.2, A3.3, B1.1, B1.2, B1.3, B1.4, B1.5, B1.8, B1.9, B8.1, B8.2, B8.3, B8.4, B8.5, B8.6, C1.1, C1.2, C1.3, C1.4" };
                }
                break;
            case QuestionID.AnyFatigue:
                break;
            case QuestionID.AnyProblems:
                break;
            case QuestionID.SayTheMatter:
                break;
            case QuestionID.HowIsBodyPartFeelin:
                {
                    if (question.SliderContent != null && question.SliderContent.SelectedValue >= 4)
                        toBeExluded = new List<string> { "A3.1, B8.1, B8.2, B8.4, C1.1,C1.2,B2.2,B2.3,C4.2,B5.4,B5.5" };
                }
                break;
            case QuestionID.EnjoyableExercise:
                break;
            case QuestionID.ConfidenceWhileExercice:
                break;
            case QuestionID.LikedExercices:
                break;
            case QuestionID.DislikedExercices:
                break;
            case QuestionID.UndoableExercices:
                break;
            case QuestionID.HowHardExerciceSession:
                break;
            case QuestionID.WorkoutDuration:
                break;
            default:
                break;
        }

        if (toBeExluded == null)
            return null;

        string[] array = toBeExluded[0].Replace(" ", "").Split(',');
        List<Exercise> excluded = new List<Exercise>();
        for (int i = 0; i < array.Length; i++)
        {
            if (Map.Keys.Contains(array[i]))
                excluded.Add(Map[array[i]]);
        }

        return excluded;
    }
    public List<ExerciseData> m_ActiveExercises;
    private void Start()
    {
        m_ToNextButton.OnButtonClicked += ToNextPanel;
        m_ToPreviousButton.OnButtonClicked += ToPreviousPanel;
        m_exitSectionButton.OnButtonClicked += ExitSectionExercise;
    }
    void SetupSession()
    {
        m_ActiveExercises = new List<ExerciseData>(this.Exercises);
        List<Question> questions = new List<Question>();
        for (int i = 0; i < questions.Count; i++)
        {
            List<Exercise> toBeExcluded = ExcludeExercicesFromQuestion(questions[i]);
            if (toBeExcluded != null)
                for (int j = 0; j < toBeExcluded.Count; j++)
                {
                    var toBeRemoved = m_ActiveExercises.FirstOrDefault(ex => ex.Exercise == toBeExcluded[j]);
                    if (toBeRemoved != null)
                        m_ActiveExercises.Remove(toBeRemoved);
                }
        }
    }
    void SetupExercises()
    {
        m_ActiveExercises = new List<ExerciseData>();
        m_ActiveExercises = (from p in Exercises where p.isExluded == false select p).ToList();
    }


    public void ExitSectionExercise(GameObject obj)
    {
        m_rootGO.SetActive(false);
        StopAnimations();
        UIManager.Instance.ForceShowProceedToSectionButtons(true);

    }

    private void StopAnimations()
    {
        foreach (var item in Exercises)
        {
            m_animator.SetBool(item.AnimatorBool, false);
        }
    }

    private void ToPreviousPanel(GameObject obj)
    {
        if (m_currentExerciseID - 1 >= 0)
        {
            SetCurrentExercise(m_currentExerciseID - 1);
        }
    }

    private void ToNextPanel(GameObject obj)
    {
        if (m_currentExerciseID + 1 < m_ActiveExercises.Count)
        {
            SetCurrentExercise(m_currentExerciseID + 1);
        }
    }
    [SerializeField]
    TextMesh m_ContentText, m_HeaderObject;

    private void ShowPanel()
    {
        StopAnimations();
        if (m_ActiveExercises.Count == 0)
        {
            m_HeaderObject.text = "No exercises for you, sorry.";
            return;
        }
        m_ContentText.text = "";
        foreach (var item in m_ActiveExercises)
        {
            m_ContentText.text += item.Name + "\n";

        }
        SetCurrentExercise(0);

    }

    private void SetCurrentExercise(int id)
    {
        m_HeaderObject.text = m_ActiveExercises[id].Name;
        m_animator.SetBool(m_ActiveExercises[m_currentExerciseID].AnimatorBool, false);
        m_currentExerciseID = id;
        m_animator.SetBool(m_ActiveExercises[m_currentExerciseID].AnimatorBool, true);

        ShowPreviousButton(m_currentExerciseID > 0);
        ShowNextButton(m_currentExerciseID < m_ActiveExercises.Count - 1);
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
