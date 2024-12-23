using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardAnimation : MonoBehaviour
{
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Panles")]
    [SerializeField] private RectTransform winPanel, losePanel;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Variables for cards animation
    [Space]
    [Header("Array List")]
    [SerializeField] private RectTransform[] cards;
    [SerializeField] private Vector3[] targetpos;
    [SerializeField] private Vector3[] currentpos;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Tweening Duration")]
    [SerializeField] private float tweeningDuration;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState state)
    {

        if(state == GameState.Won)
        {
            PanelAnimation(winPanel);
        }
        else if(state == GameState.Lose)
        {
            PanelAnimation(losePanel);
        }
        else
        {
            ResetAllPanel();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
       
        _ = StartCoroutine(StartCardAnimation());
    }

    private IEnumerator StartCardAnimation()
    {
        yield return new WaitForSeconds(0.1f);

        for(int i = 0; i< cards.Length; i++)
        {
            cards[i].DOLocalMove(targetpos[i], tweeningDuration);

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);


        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].DOLocalMove(currentpos[i], tweeningDuration);

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].DOLocalMove(targetpos[i], tweeningDuration);

        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].DOLocalMove(currentpos[i], tweeningDuration);

        }

        yield return new WaitForSeconds(0.5f);

        _ = StartCoroutine(StartCardAnimation());
    }


    private void PanelAnimation(RectTransform panel)
    {
        
        panel.DOAnchorPosY(0, tweeningDuration);

    }

    private void ResetAllPanel()
    {
        winPanel.DOAnchorPosY(720, tweeningDuration);
        losePanel.DOAnchorPosY(720, tweeningDuration);
    }







}
