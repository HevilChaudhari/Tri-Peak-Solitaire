using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class cardSpawner : MonoBehaviour
{

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Card Prefabs")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject[] rows;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Deck Transform")]
    [SerializeField] private Transform deck;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private int totalcards = 52;
    private int totalNumberofRow = 4;
    private int totalCardsPerRow = 3;
    private int num = 0;
    public int sortingOrderIncrement = 1;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [SerializeField] private float tweeningDuration;
    private float totalLength = 10.5f;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Card Index List")]
    [SerializeField]private List<int> cardIndexes = new List<int>();

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    // Start is called before the first frame update


    private void OnEnable()
    {
        GameManager.OnGameStateChanged += SpawnCards;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private void SpawnCards(GameState state)
    {
        if(state == GameState.SetCardLayout)
        {


            _ = StartCoroutine(CardSpawner());
        }

        if(state == GameState.Restart || state == GameState.Start)
        {
            totalcards = 52;
            totalCardsPerRow = 3;
            sortingOrderIncrement = 0;
            totalLength = 10.5f;

            for (int i = 0; i < totalcards; i++)
            {
                cardIndexes.Add(i);
            }
        }

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Spawn Cards and make Tableau

    private IEnumerator CardSpawner()
    {

        //Spawn cards and set onTableau
        for(int row = 1;row <= totalNumberofRow; row++)
        {

            //local variables
            float startPoint = -(totalLength / 2);
            float minOffsetBetweenCards = 0;
            float gapFactor = 0;
            //

            for (int i=0 ;i < totalCardsPerRow; i++)
            {
                //---------------------------------------------------------
                //row 1
                if (row == 1)
                {
                    
                    Vector2 spawnPos = new Vector2(startPoint + gapFactor, totalNumberofRow - row);
                    CardSpawn(spawnPos, row);
                    minOffsetBetweenCards += 1.75f;
                    gapFactor += 5.25f;
                }
                //row2
                else if(row == 2)
                {
                    if(i < 3)
                    {
                        //First object
                        Vector2 spawnPos = new Vector2(startPoint + gapFactor + minOffsetBetweenCards , totalNumberofRow - row);
                        CardSpawn(spawnPos, row);
                        minOffsetBetweenCards += 1.75f;

                        //Second Object
                        spawnPos = new Vector2(startPoint + minOffsetBetweenCards + gapFactor, totalNumberofRow - row);
                        CardSpawn(spawnPos, row);
                        gapFactor += 3.5f;
                    }
                }
                //row 3 and 4
                else
                {

                    Vector2 spawnPos = new Vector2(startPoint + minOffsetBetweenCards, totalNumberofRow - row);
                    CardSpawn(spawnPos, row);
                    minOffsetBetweenCards += 1.75f;
                }

                yield return new WaitForSeconds(0.05f);

            }

            //---------------------------------------------------------
            //length increse for next row

            totalLength += 1.75f;

            //---------------------------------------------------------
            //to make perfect card tableau layout

            if (row == 3)
                totalCardsPerRow += 1;
            else
                totalCardsPerRow += 3;
        }

        //---------------------------------------------------------
        //SpawnCards and make a deck
        for (int i = 0;i < totalcards; i++)
        {

            int index = GetUniqueRandomIndex();
            GameManager.Instance.deckStack.Push(index);
            if(i == totalcards - 1)
            {
                Invoke("ChangeToPlayingState", 0.5f);
            }

        }
        cardIndexes.Clear();
       

    }

    private void ChangeToPlayingState()
    {
        GameManager.Instance.UpdategameState(GameState.Playing);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Random Unique Number Generator

    private int GetUniqueRandomIndex()
    {
        //---------------------------------------------------------
        int randomIndex = Random.Range(0, cardIndexes.Count);
        int randomNumber = cardIndexes[randomIndex];
        //---------------------------------------------------------
        cardIndexes.RemoveAt(randomIndex);
        return randomNumber;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //Card Spawner

    private void CardSpawn(Vector2 spawnPos, int rowIndex)
    {
        //---------------------------------------------------------
        GameObject card = Instantiate(cardPrefab,deck.position,Quaternion.identity);
        int index = GetUniqueRandomIndex();
        //---------------------------------------------------------
        card.name = "Card" + num;
        card.GetComponent<Card>().index = index;
        //---------------------------------------------------------

        if (!GameManager.Instance.tableauData.ContainsKey(index))
        {
            GameManager.Instance.tableauData.Add(index, card);
        }

        //---------------------------------------------------------
        card.transform.SetParent(rows[rowIndex - 1].transform);
        card.transform.DOLocalMove(spawnPos, tweeningDuration).OnComplete(()=> { card.GetComponent<Card>().spawnPos = card.transform.localPosition; });


        //---------------------------------------------------------

        totalcards--;
        num++;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= SpawnCards;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
}
