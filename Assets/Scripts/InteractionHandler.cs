using HoloToolkit.Unity.Buttons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public static InteractionHandler Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void StartInteractions(List<Interaction> interactions)
    {
        StartCoroutine(startInteractions(interactions));
    }
    private IEnumerator startInteractions(List<Interaction> interactions)
    {
        foreach (Interaction interaction in interactions)
        {
            gameObject.SetActive(false);
            CleanDialog();
            InitializeDialog(interaction);
            gameObject.SetActive(true);

            //wait until user clicks on an answer
            while (!interaction.isAnswerRegistered)
                yield return null;

        }
        gameObject.SetActive(false);
    }

    [SerializeField]
    private GameObject TitleObject, ButtonParent, DescriptionObject;
    [SerializeField]
    private GameObject[] ButtonRow2X = new GameObject[2], ButtonRow3X = new GameObject[2];
    [SerializeField]
    private float RowHeight = .4f;

    private List<Button> buttons;
    private void InitializeDialog(Interaction interaction)
    {
        TitleObject.GetComponent<TextMesh>().text = interaction.Title;
        DescriptionObject.GetComponent<TextMesh>().text = interaction.Description;

        int count = interaction.Answers.Count;
        bool isRow2X = (count == 2 || count == 4);
        buttons = new List<Button>();

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = isRow2X ? ButtonRow2X[i % 2] : ButtonRow3X[i % 3];
            Vector3 targetPosition = prefab.transform.localPosition;
            targetPosition.y = (i / (isRow2X ? 2 : 3)) * RowHeight;
            Button btn = (Instantiate(prefab, ButtonParent.transform) as GameObject).GetComponent<Button>();
            btn.transform.localPosition = targetPosition;
            btn.GetComponentInChildren<TextMesh>().text = interaction.Answers[i].Content;
            btn.OnButtonClicked += ((GameObject obj) =>
            {
                interaction.RegisterAnswer(i);
            });
            buttons.Add(btn);
            btn.gameObject.SetActive(true);
        }
    }

    private void CleanDialog()
    {
        TitleObject.GetComponent<TextMesh>().text = "...";
        DescriptionObject.GetComponent<TextMesh>().text = "...";
        if (buttons != null)
            foreach (var btn in buttons)
            {
                Destroy(btn.gameObject);
            }
    }
}

public class Answer
{
    public string Content;
    public bool JumpNextInteraction = false;
}
public class SliderAnswer : Answer
{
    public int Min = 0;
    public int Max = 1;
    //override something somewhere
    //basically on init show slider prefab, with range and validate button
}
public class NarrationAnswer : Answer
{
    // on init show cherif's work, with next button
}
public class Interaction
{
    //generate UID
    [HideInInspector]
    public int ID;

    public string Title;
    [TextArea]
    public string Description;
    public List<Answer> Answers;

    public bool isAnswerRegistered = false;
    public void RegisterAnswer(int answerID)
    {
        Debug.Log(string.Format("question {0} registered answer {1}. as in q:{2}... and q:{3}...",
            ID, answerID, Title.Substring(0, 4), Answers[answerID].Content.Substring(0, 4)));
        Debug.Log("***************PLEASE WRITE IN JSON****************");
        isAnswerRegistered = true;
    }
}
