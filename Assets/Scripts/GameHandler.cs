using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameHandler : MonoBehaviour
{
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Tweening Variables")]
    [SerializeField]private float tweeningDuration;
    [SerializeField][Range(0,1)] private float strength;
    [SerializeField] [Range(0, 10)] private int vibrato;
    [SerializeField]private float shakeRandomness = 90f;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Deck Transform")]
    [SerializeField] private Transform deckCard;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private void OnEnable()
    {
        GameManager.OnCardMovetoWasteDeck += CheckAvailableCards;
        GameManager.OnHintButtonPressed += ShowHint;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Show Hint
    private void ShowHint()
    {
        if(GameManager.Instance.MovebleCards.Count != 0)
        {

            foreach(GameObject card in GameManager.Instance.MovebleCards)
            {

                card.transform.DOShakePosition(tweeningDuration, strength, vibrato, shakeRandomness);
                card.transform.DOShakeScale(tweeningDuration, strength, vibrato, shakeRandomness)
                    .OnComplete(() => {

                    card.transform.DOScale(new Vector3(1f, 1f, 1f), tweeningDuration);
                    card.transform.DOLocalMove(card.GetComponent<Card>().spawnPos, tweeningDuration).OnComplete(()=> { GameManager.Instance.canPlayerInterect = true; });
                    

                    });
            }
        }
        else
        {
            deckCard.transform.DOShakePosition(tweeningDuration, strength, vibrato, shakeRandomness);
            deckCard.transform.DOShakeScale(tweeningDuration, strength, vibrato, shakeRandomness)
                .OnComplete(() => {

                    deckCard.transform.DOScale(new Vector3(0.7482949f, 0.5635682f, 0.4275f), tweeningDuration);
                    deckCard.transform.DOLocalMove(new Vector3(-0.0051f, 0.0066f, 0f), tweeningDuration).OnComplete(() => { GameManager.Instance.canPlayerInterect = true; }); 


                });
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Check state when card move to waste deck
    //Check Interectable cards in tableau
    private void CheckAvailableCards()
    {
        int index = 0;
        List<GameObject> AvailableCards = new List<GameObject>();
        foreach (KeyValuePair<int, GameObject> pair in GameManager.Instance.tableauData)
        {

            if (pair.Value.layer == LayerMask.NameToLayer("InterectableCard"))
            {
                AvailableCards.Add(pair.Value);
                index++;
            }

        }


        CheckAvailableMoves(AvailableCards);

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Check the card that has one higher or lower rank and store in local list
    private void CheckAvailableMoves(List<GameObject> availableCards)
    {
        GameObject lastWasteObject = GameManager.Instance.wasteStack.Peek();
        Card lastCardScript = lastWasteObject.GetComponent<Card>();
        int lastCardIndex = lastCardScript.card_id;
        List<GameObject> MovebleCards = new List<GameObject>();
        foreach (GameObject card in availableCards)
        {
            Card cardScript = card.GetComponent<Card>();
            int currentCardID = cardScript.card_id;
            if (currentCardID == 0)
            {
                currentCardID = 13;
            }

            if ((currentCardID + 1) % 13 == lastCardIndex || (currentCardID - 1) % 13 == lastCardIndex)
            {
                MovebleCards.Add(card);
            }

        }
        GameManager.Instance.MovebleCards = MovebleCards;

        _=StartCoroutine(CheckForOutofMoves(MovebleCards));

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //It will check it deck is empty and tableau is not empty and then decide Win or lose
    private IEnumerator CheckForOutofMoves(List<GameObject> MovebleCards)
    {
        yield return new WaitForSeconds(0.1f);

        if (GameManager.Instance.tableauData.Count == 0)
        {
            GameManager.Instance.UpdategameState(GameState.Won);
        }
        else if(MovebleCards.Count == 0 && GameManager.Instance.deckStack.Count == 0)
        {
            GameManager.Instance.UpdategameState(GameState.Lose);
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private void OnDisable()
    {
        GameManager.OnHintButtonPressed -= ShowHint;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
}
