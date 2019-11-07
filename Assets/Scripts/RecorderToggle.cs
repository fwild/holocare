using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class RecorderToggle : MonoBehaviour, IDictationHandler
    {
        [SerializeField]
        [Range(0.1f, 5f)]
        [Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.")]
        private float initialSilenceTimeout = 5f;

        [SerializeField]
        [Range(5f, 60f)]
        [Tooltip("The time length in seconds before dictation recognizer session ends due to lack of audio input.")]
        private float autoSilenceTimeout = 20f;

        [SerializeField]
        [Range(1, 60)]
        [Tooltip("Length in seconds for the manager to listen.")]
        private int recordingTime = 10;

        [SerializeField]
        private TextMesh speechToTextOutput = null;

        private Renderer buttonRenderer;
        private Renderer ButtonRenderer { get { if (buttonRenderer == null) buttonRenderer = GetComponent<Renderer>(); return buttonRenderer; } set { buttonRenderer = value; } }
        [SerializeField]
        TextMesh m_isRecordingEnabled;
        private bool isRecording;
        public bool IsRecording { get { return isRecording; } }
        public event Action<string> OnDictationResulted;
        private void Awake()
        {
            ButtonRenderer = GetComponent<Renderer>();

            DictationInputManager.StartedRecording += DictationInputManager_StartedRecording;
            DictationInputManager.StoppedRecording += DictationInputManager_StoppedRecording;
        }

        private void DictationInputManager_StoppedRecording()
        {
            GetComponent<InteractiveToggle>().HasSelection = false;
            //EnableClicks(true);
        }

        private void DictationInputManager_StartedRecording()
        {
            GetComponent<InteractiveToggle>().HasSelection = true;
            //EnableClicks(false);
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            ToggleRecording();
        }

        public void ToggleRecording()
        {
            if (isRecording)
            {
                isRecording = false;
                StartCoroutine(DictationInputManager.StopRecording());
                speechToTextOutput.color = Color.white;
                SetDictationState(false);
                //ButtonRenderer.enabled = true;
            }
            else
            {
                isRecording = true;
                StartCoroutine(DictationInputManager.StartRecording(
                    gameObject,
                    initialSilenceTimeout,
                    autoSilenceTimeout,
                    recordingTime));
                speechToTextOutput.color = Color.green;
                SetDictationState(true);
                //ButtonRenderer.enabled = false;
            }
        }

        public void OnDictationHypothesis(DictationEventData eventData)
        {
            speechToTextOutput.text = eventData.DictationResult;
        }

        public void OnDictationResult(DictationEventData eventData)
        {
            speechToTextOutput.text = eventData.DictationResult;
            OnDictationResulted?.Invoke(eventData.DictationResult);
        }

        public void OnDictationComplete(DictationEventData eventData)
        {
            if (eventData.DictationResult != "Dictation has timed out. Please try again.")
            {
                OnDictationResulted?.Invoke(eventData.DictationResult);
                speechToTextOutput.text = eventData.DictationResult;
            }
            EnableClicks(true);
        }

        public void OnDictationError(DictationEventData eventData)
        {
            isRecording = false;
            speechToTextOutput.color = Color.red;
            SetDictationState(false);
            //ButtonRenderer.enabled = true;
            speechToTextOutput.text = eventData.DictationResult;
            Debug.LogError(eventData.DictationResult);
            StartCoroutine(DictationInputManager.StopRecording());
            EnableClicks(true);
        }
        void EnableClicks(bool state)
        {
            //GetComponent<Button>().enabled = state;
            GetComponent<BoxCollider>().enabled = state;
            //GetComponent<CompoundButton>().enabled = state;
        }
        private void SetDictationState(bool v)
        {
            //m_isRecordingEnabled.text = v ? "Is Recording" : "Is Disabled";
            //m_isRecordingEnabled.color = v ? Color.green : Color.red;
        }
    }
}
