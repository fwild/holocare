using Newtonsoft.Json;
using RogoDigital.Lipsync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Question
    {
        public QuestionID ID { get; set; }
        public string Content { get; set; }
        public bool IsFreestyle { get; set; }
        public List<string> Responses { get; set; }
        public int SelectedResponse { get; set; } = -1;
        public string Answer { get; set; }
        [JsonIgnore]
        public SliderContent SliderContent { get; set; }
        [JsonIgnore]
        public Func<QuestionSelectionArguments> NavigationFunction;
        [JsonIgnore]
        public LipSyncData AudioTrack;
    }
    public class SliderContent
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string MinDescription { get; set; }
        public string MaxDescription { get; set; }
        public float SelectedValue { get; set; }
    }
    public enum QuestionID
    {
        Gender,
        YearBorn,
        Height,
        Weigh,
        //Obese,
        CancerType,
        CancerTreatmentEnd,
        CardiovascularDiseases,
        RespiratoryDisorders,
        SufferFromOsteoarthritis,
        OsteoarthritisWhere,
        SufferFromRheumatoidArthritis,
        RheumatoidArthritisWhere,
        SufferFromJointReplacement,
        JointReplacementWhere,
        Osteoporosis,
        SufferFromMechanicalBackPain,
        MechanicalBackPainWhere,
        MentalHealthConditions,
        DiabetesMelltus,
        PhysicalActivityPerweek,
        CurrentPhysicalActivityPerweek,
        PainWhileExercicsing,
        WhereFeelPain,
        ExercicePainScale,
        GoalFromExercicePlan,
        //Section2
        HowUDoin,
        AnyNewPain,
        WhereDoYouFeelPain,
        AnyFatigue,
        AnyProblems,
        SayTheMatter,
        HowIsBodyPartFeelin,
        //Section3
        EnjoyableExercise,
        ConfidenceWhileExercice,
        LikedExercices,
        DislikedExercices,
        UndoableExercices,
        HowHardExerciceSession,
        WorkoutDuration
    }
    public class QuestionSelectionArguments
    {
        public bool ShouldSelect { get; set; }
        public AudioClip AudioClip { get; set; }
        public static Func<QuestionSelectionArguments> DefaultYes
        {
            get
            {
                return () =>
                {
                    return new QuestionSelectionArguments() { ShouldSelect = true, AudioClip = null };
                };

            }
        }
        public static Func<QuestionSelectionArguments> BuildFromArguments(bool shouldSelect, AudioClip clip = null)
        {
            return () =>
            {
                return new QuestionSelectionArguments() { ShouldSelect = shouldSelect, AudioClip = clip };
            };
        }
    }
}
