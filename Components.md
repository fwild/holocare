# Components

The application is build the following way:

* Main Scene: 
  * Avatar: holds the male and female avatar ready and rigged (incl. eye and lip movement)
  * Animations: in v3_Update/Female/
  * Animators: in v2_Update/Animators/FemaleAssistantAnimator.controller
* Scripts:
  * DatabaseManager.cs: store and retrieve the user data for each Q&A section
  * PulseManager: Spatial Understanding at the beginning of the scene: scan a clean room representation and identify a suitable place to sit the patient, guide through placement of the menu
  * ExerciseSectionController.cs: The decision tree for excluding exercises from the list of offered ones, based on the answers provided in the Q&A
  * InteractionHandler.cs:
  * Models/*: data models
  * QuestionDisplayer.cs: responsible for bringing up the different sections of the Q&A 
  * RecorderButton.cs: speech to text, required for some of the question's answers
  * RecorderToggle.cs: needed for the speech to text
  * ToggleClickListener.cs
  * ReportElementComponent.cs: 
  * ReportViewer.cs: user management and bringing up te reports
  * ScriptedInteraction.cs: dialogue management
  * SectionFourController.cs
  * UIManager.cs: greetings and other main management of the UI
  * UserElementComponent.cs
* Prefabs: panel UI elements

