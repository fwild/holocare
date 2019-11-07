using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section
{
    public string Name;
    public string Prologue;
    public string Note;
    public string AfterNote;
    public List<Interaction> Interactions;
}

public class ScriptedInteraction : MonoBehaviour
{
    public void Start()
    {
        InteractionHandler.Instance.StartInteractions(SectionOne.Interactions);
    }
    Section SectionOne = new Section()
    {
        Name = "Section One",
        Prologue = "Hi there! Welcome to your new augmented rehabilitation environment. Over the next eight weeks I'm going to prescribe you some exercises that are going to make you stronger, fitter and healthier. Firstly, I just need to ask you a few questions so I know a bit more about you. Are you ready to continue?",
        Note = "To answer the questions say your answer aloud. If there is a list shown on the screen, make sure you pick an answer from that list. If you need to go back for any reason you can by saying \"go back\" Are you ready to continue?",
        AfterNote = "",
        Interactions = new List<Interaction>()
        {
            new Interaction()
            {
                Title = "Gender",
                Description = "Are you male or female?",
                Answers = new List<Answer>()
                {
                    new Answer()
                    {
                        Content = "Male"
                    },
                    new Answer()
                    {
                        Content = "Female"
                    },
                }
            },
            new Interaction()
            {
                Title = "Gender",
                Description = "Are you male or female?",
                Answers = new List<Answer>()
                {
                    new Answer()
                    {
                        Content = "dadasd"
                    },
                    new Answer()
                    {
                        Content = "Female"
                    },
                }
            },
            new Interaction()
            {
                Title = "Gender",
                Description = "Are you male or female?",
                Answers = new List<Answer>()
                {
                    new Answer()
                    {
                        Content = "Male"
                    },
                    new Answer()
                    {
                        Content = "Femasfasfasfale"
                    },
                }
            },
        }
    };
}

