using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private static GameManager instance;

    // Public property to access the instance
    public static GameManager Instance
    {
        get
        {
            // Check if the instance is null (first time access or after Destroy)
            if (instance == null)
            {
                // Find existing instance in the scene
                instance = FindObjectOfType<GameManager>();

                // If no instance found, create a new GameObject with MyManager attached
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(GameManager).Name);
                    instance = singletonObject.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // Ensure the instance isn't destroyed on scene change (optional)
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 90;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Header("Stacks and Lists")]
    public Stack<int> deckStack = new Stack<int>();
    public Stack<GameObject> wasteStack = new Stack<GameObject>();
    public Dictionary<int, GameObject> tableauData = new Dictionary<int, GameObject>();
    public List<GameObject> MovebleCards = new List<GameObject>();

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnCardMovetoWasteDeck;
    public static event Action OnHintButtonPressed;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Main Game State")]
    public GameState state;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Layer to destroy Cards")]
    [SerializeField] private LayerMask DestroyebleObjects;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Player Interection enable")]
    public bool canPlayerInterect = false;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    public void UpdategameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Start:

                break;
            case GameState.SetCardLayout:
                break;
            case GameState.Playing:
                break;
            case GameState.Pause:
                break;
            case GameState.Restart:
                _ = StartCoroutine(ResetAll());
                break;
            case GameState.Won:
                break;
            case GameState.Lose:
                break;
            case GameState.Home:
                _ = StartCoroutine(ResetAll());
                break;
        }

        OnGameStateChanged?.Invoke(newState);

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    public void CardCheckAction()
    {
        OnCardMovetoWasteDeck?.Invoke();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    public void TriggerActionOnHintButtonPressed()
    {
        OnHintButtonPressed?.Invoke();
    }


    private IEnumerator ResetAll()
    {
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in objects)
        {
            if (((1 << obj.layer) & DestroyebleObjects) != 0)
            {
                Destroy(obj);
            }

            
            tableauData.Clear();
            wasteStack.Clear();
            deckStack.Clear();
            MovebleCards.Clear();

        }

        yield return new WaitForSeconds(0.1f);

        if(state == GameState.Restart)
        {

            UpdategameState(GameState.SetCardLayout);
        }
        else if(state == GameState.Home)
        {
            UpdategameState(GameState.Start);
        }

    }

}

//----------------------------------------------------------------------------------------------------------------------------------------------------------------//
//----------------------------------------------------------------------------------------------------------------------------------------------------------------//
//----------------------------------------------------------------------------------------------------------------------------------------------------------------//
//----------------------------------------------------------------------------------------------------------------------------------------------------------------//

public enum GameState
{
    Start,
    SetCardLayout,
    Playing,
    Pause,
    Restart,
    Home,
    Won,
    Lose,

}
