﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ExperimentManager : MonoBehaviour
{
    //public 
    public string Lobby;

    public string End;

    public ScriptableObject experiment;

    public UIBuilder uimanager;

    public GameObject[] DontDelete;

    public string preQuestionaireRefName;

    public string postQuestionaireRefName;

    public GameObject firsttrigger;

    //private 
    private ExperimentScriptableObject e;

    private bool started;

    private Queue<ExperimentAction> ActionQueue = new Queue<ExperimentAction>();

    private bool Executing = false;

    private List<int> networkTrialOrder;

    void Awake()
    {

        Resources.LoadAll("ScriptableObjects");

        e = (ExperimentScriptableObject)experiment;

        foreach (GameObject g in DontDelete) DontDestroyOnLoad(g);

        //find end of trialobject and link to OnEndOfTrialEvent
        firsttrigger.GetComponent<EndOfTrialManager>().OnEndOfTrialEvent.AddListener(NextAction);
        
    }

    private void Start()
    {
        if (e.Session[0].TrialsOrder == ExperimentTrialsOrder.Network)
        {
            StartCoroutine(StartExperimentAsync());
        }
        else
        {
            StartExperiment();
        }
    }

    public void StartExperiment() {

        if (started) return;
        else started = true;

        createQueuePreTrials();
        createQueueTrials();
        createQueuePostTrials();
    }

    IEnumerator StartExperimentAsync()
    {

        //get trial order with a web request from network 
        if (e.Session[0].NetworkTrialUrl != null)
        {
            yield return StartCoroutine(GetTrialOrderFromNetwork(e.Session[0].NetworkTrialUrl));
        }
        else {
            Debug.Log("A url for the network session needs to be specified");
        }

        createQueuePreTrials();
        createQueueTrials();
        createQueuePostTrials();
    }

    void createQueuePreTrials()
    {
        if (preQuestionaireRefName != null) {

            ExperimentAction questionaire = new ExperimentAction(ExperimentActionType.QuestoinairePart, preQuestionaireRefName);
            ActionQueue.Enqueue(questionaire);
        }
    }

    void createQueuePostTrials()
    {
        if (postQuestionaireRefName != null)
        {

            ExperimentAction questionaire = new ExperimentAction(ExperimentActionType.QuestoinairePart, postQuestionaireRefName);
            ActionQueue.Enqueue(questionaire);
        }
    }

    void createQueueTrials() {

        List<string> stringlist = new List<string>();
        
        foreach (Session s in e.Session)
        {

            if (s.TrialsOrder == ExperimentTrialsOrder.Randomized)
            {
                AddRandomizeTrialOrder(s);//here we can have also another method based on partecipant numbers instead

            }
            else if (s.TrialsOrder == ExperimentTrialsOrder.Network) {

                AddTrialOrderFromNetwork(s);

            }
            else if (s.TrialsOrder == ExperimentTrialsOrder.ScriptableObjectOrder)
            {
                foreach (Trial t in s.trials)
                {
                    ExperimentAction scene = new ExperimentAction(ExperimentActionType.scene, t.sceneName);
                    ActionQueue.Enqueue(scene);

                    ExperimentAction questionaire = new ExperimentAction(ExperimentActionType.QuestoinairePart, t.questionaireRefName);
                    ActionQueue.Enqueue(questionaire);
                }
            }
        }

    }

    void AddTrialOrderFromNetwork(Session s)
    {

        List<int> array = networkTrialOrder;

        List<string> arrayString = new List<string>();

        
        foreach (int i in array)
        {

            arrayString.Add(s.trials[i].sceneName);

            ExperimentAction scene = new ExperimentAction(ExperimentActionType.scene, s.trials[i].sceneName);
            ActionQueue.Enqueue(scene);

            ExperimentAction questionaire = new ExperimentAction(ExperimentActionType.QuestoinairePart, s.trials[i].questionaireRefName);
            ActionQueue.Enqueue(questionaire);
        }

        uimanager.data.sequenceImp = array.ToArray();
        uimanager.data.sequenceExp = arrayString.ToArray();

    }
    
    void AddRandomizeTrialOrder(Session s) {
        
     
        List<int> array = new List<int>();

        List<string> arrayString = new List<string>();

        while (array.Count< s.trials.Length) {

            int newindex = Random.Range(0, s.trials.Length);

            if (array.IndexOf(newindex) == -1)
            {
                array.Add(newindex);
            }
          
        }

        foreach (int i in array) {

            arrayString.Add(s.trials[i].sceneName);

            ExperimentAction scene = new ExperimentAction(ExperimentActionType.scene, s.trials[i].sceneName);
            ActionQueue.Enqueue(scene);

            ExperimentAction questionaire = new ExperimentAction(ExperimentActionType.QuestoinairePart, s.trials[i].questionaireRefName);
            ActionQueue.Enqueue(questionaire);
        }

        uimanager.data.sequenceImp = array.ToArray();
        uimanager.data.sequenceExp = arrayString.ToArray();

    }

    public void NextAction() {

        if (Executing)
        {
            return;
        }
        else
        {
            Executing = true;
            if (ActionQueue.Count > 0)
            {
                StartCoroutine(ExecuteAction(ActionQueue.Dequeue()));
            }
            else
            {
                StartCoroutine(LoadYourAsyncScene(End));
                //SceneManager.LoadScene(End);
            }
            
        }

    }

    IEnumerator ExecuteAction(ExperimentAction a)
    {
        if (a.type == ExperimentActionType.QuestoinairePart)
        {
            //load Lobby scene 
            yield return LoadYourAsyncScene(Lobby);
            //SceneManager.LoadScene(Lobby);

            //Build Questionaire UI
            uimanager.Build(a.ActionName);

            //add listener to move to next action 
            uimanager.OnQuestionairePartCompleted.AddListener(NextAction);
            
        }
        else if (a.type == ExperimentActionType.scene)
        {
            yield return  LoadYourAsyncScene(a.ActionName);
            //SceneManager.LoadScene(a.ActionName);

            //find end of trialobject and link to OnEndOfTrialEvent
            //GameObject.Find("[REPLACEWITHCORRECTNAME]").GetComponent<PositionSerializerAdam>().OnEndOfTrialEvent.AddListener(NextAction);
            //GameObject.Find("EndOfTrialManager").GetComponent<EndOfTrialManager>().OnEndOfTrialEvent.AddListener(NextAction);

            if (!findAndListen("UiContainer")) 
            {
                if (!findAndListen("EndOfTrialManager"))  Debug.LogError("No Event End of trial Found");                
            }
         

        }

        yield return Executing = false;
    }

    bool findAndListen(string name) {

        GameObject container = GameObject.Find(name);

        bool found  = false;

        if (container != null)
        {
            EndOfTrialManager[] eList = container.GetComponentsInChildren<EndOfTrialManager>(true);

            foreach (EndOfTrialManager e in eList)
            {
                e.OnEndOfTrialEvent.AddListener(NextAction);
                found = true;
            }

        }

        return found;

    }

    IEnumerator LoadYourAsyncScene(string scenename)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        TransitionManager.Instance.setLogoLocation();
        TransitionManager.Instance.setWaitingView();
        //Debug.Log("=============== " + scenename);
        yield return new WaitForSeconds(1.0f);
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenename);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void LoadScene(string scenename)
    {
        Debug.Log("sceneName to load: " + scenename);
        SceneManager.LoadScene(scenename);
    }

    IEnumerator GetTrialOrderFromNetwork(string url)
    {

        networkTrialOrder = new List<int>();

        var request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            // Show results as text
            Debug.Log(request.downloadHandler.text);


            foreach (string s in request.downloadHandler.text.Replace("[", "").Replace("]", "").Split(',')) {

                networkTrialOrder.Add(int.Parse(s));

            }


        }

    }
}

public class ExperimentAction {

    public ExperimentActionType type;
    public string ActionName;

    public ExperimentAction(ExperimentActionType scene, string sceneName)
    {
        this.type = scene;
        this.ActionName = sceneName;
    }
}


public enum ExperimentActionType { 

    QuestoinairePart,
    scene,
    Instructions,
}