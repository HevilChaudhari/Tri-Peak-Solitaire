using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Variables For Tweening

    [Space]
    [Header("Tweening Variable")]
    [SerializeField] private float tweeningDuration;
    [SerializeField] private float buttonClickedTweeningDuration;
    [SerializeField] [Range(0, 1)] private float strength;
    [SerializeField] [Range(0, 10)] private int vibrato;
    [SerializeField] private float shakeRandomness = 90f;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("UI Objects")]
    [SerializeField] private GameObject[] panels;
    [SerializeField] private Transform deckSlot, wasteSlot;
    [SerializeField] private RectTransform playBtn,homeBtn,restartBtn,hintBtn;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private bool isHintBtnPressed = false;
    private bool isRestartBtnPressed = false;
    

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void Start()
    {
        GameManager.Instance.UpdategameState(GameState.Start);
        playBtn.DOScale(new Vector3(1.25f, 1.25f, 1.25f), tweeningDuration).SetLoops(-1,LoopType.Yoyo);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private void GameManager_OnGameStateChanged(GameState state)
    {
        panels[0].SetActive(state == GameState.Start);
        panels[1].SetActive(state == GameState.Playing);
        panels[2].SetActive(state == GameState.Won);
        panels[3].SetActive(state == GameState.Lose);

        if(state == GameState.Restart) 
               isRestartBtnPressed=false;

        deckSlot.gameObject.SetActive(state == GameState.Playing || state == GameState.SetCardLayout || state == GameState.Won || state == GameState.Lose);
        wasteSlot.gameObject.SetActive(state == GameState.Playing || state== GameState.SetCardLayout || state == GameState.Won || state == GameState.Lose);

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    public void PlayButtonPressed()
    {
        playBtn.DOShakeScale(buttonClickedTweeningDuration, strength, vibrato, shakeRandomness).OnComplete(()=> { GameManager.Instance.UpdategameState(GameState.SetCardLayout); });
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    public void HintButtonPressed()
    {
        if (!isHintBtnPressed)
        {
            hintBtn.DOShakeScale(buttonClickedTweeningDuration, strength, vibrato, shakeRandomness);
            GameManager.Instance.TriggerActionOnHintButtonPressed();
            GameManager.Instance.canPlayerInterect = false;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    public void HomeButtonPressed()
    {
        homeBtn.DOShakeScale(buttonClickedTweeningDuration, strength, vibrato, shakeRandomness).OnComplete(() => { GameManager.Instance.UpdategameState(GameState.Home); });

    }

    public void RestartButtonPressed()
    {
        if (!isRestartBtnPressed) 
        {
            isRestartBtnPressed = true; 
            restartBtn.DOShakeScale(buttonClickedTweeningDuration, strength, vibrato, shakeRandomness).OnComplete(() => { GameManager.Instance.UpdategameState(GameState.Restart); });
        }
    }


    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

}
