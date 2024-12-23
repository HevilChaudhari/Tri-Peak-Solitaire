using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ControlHandler : MonoBehaviour
{
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Transform and Objects")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform waste;
    [SerializeField] private Transform deck;
    [SerializeField] private Transform wasteCardHolder;
    [SerializeField] private GameObject crossSignPrefab;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Tweening Duration Variable")]
    [SerializeField] private float tweeningDuration = 0.5f;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//


    private bool DeckCardisMoving = false;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    [Space]
    [Header("Sorting order")]
    public int sortingOrderIncrement = 1;
    private int currentSortingOrder = 0;

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += CanInterect;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private void CanInterect(GameState state)
    {
        GameManager.Instance.canPlayerInterect = state == GameState.Playing;

        if(state == GameState.SetCardLayout)
        {
            deck.GetChild(1).gameObject.SetActive(true);
        }

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    // Update is called once per frame
    void Update()
    {
            //TouchControl
            if(Input.touchCount > 0 && GameManager.Instance.canPlayerInterect)
            {
                Touch touch = Input.GetTouch(0);

                if(touch.phase == TouchPhase.Began)
                {
                    //Variables for Raycasting
                    Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                    RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
                    //
                    //---------------------------------------------------------
                        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("InterectableCard"))
                        {
                            GameObject card = hit.collider.gameObject;

                            //---------------------------------------------------------

                            if (!GameManager.Instance.wasteStack.Contains(card) || GameManager.Instance.wasteStack.Count == 0)
                            {

                                MoveCardToWasteDeck(card,touchPos);
                            }
                            else
                            {
                                Debug.Log("Already Contain");
                            }

                            //---------------------------------------------------------

                        }
                        else if(hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Deck") && !DeckCardisMoving)
                        {
                                MoveCardFromDeck();
                        }
                    //---------------------------------------------------------

                }

            }

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //If condition satifies then card will move from tableau to wastes deck slot
    private void MoveCardToWasteDeck(GameObject card,Vector3 touchPos)
    {

        if (GameManager.Instance.wasteStack.Count == 0)
        {
            //---------------------------------------------------------
            GameManager.Instance.wasteStack.Push(card);
            Card cardScript = card.GetComponent<Card>();
            card.transform.SetParent(null);
            card.layer = LayerMask.NameToLayer("WasteCards");
            //---------------------------------------------------------
            //DOtween use to move card
            StartCoroutine(cardScript.FlipCard360(360));
            card.transform.DOMove(waste.position, tweeningDuration)
                .OnComplete(() => {

                    GameManager.Instance.tableauData.Remove(card.GetComponent<Card>().index);
                    card.transform.SetParent(wasteCardHolder);
                    GameManager.Instance.Invoke("CardCheckAction", 0.3f);



                });

            //---------------------------------------------------------
            currentSortingOrder += sortingOrderIncrement;
            card.GetComponent<SpriteRenderer>().sortingOrder = currentSortingOrder;
            //---------------------------------------------------------
        }
        else
        {
            //Local Variables

            GameObject lastWasteObject = GameManager.Instance.wasteStack.Peek();
            Card lastCardScript = lastWasteObject.GetComponent<Card>();
            Card currentCardScript = card.GetComponent<Card>();
            int currentCardIndex = currentCardScript.card_id;
            int lastCardIndex = lastCardScript.card_id;

            //
            //---------------------------------------------------------

            if (currentCardIndex == 0)
            {
                currentCardIndex = 13;
            }

            //---------------------------------------------------------

            if ((currentCardIndex + 1) % 13 == lastCardIndex || (currentCardIndex - 1) % 13 == lastCardIndex)
            {
                //---------------------------------------------------------
                GameManager.Instance.wasteStack.Push(card);
                card.transform.SetParent(null);
                card.layer = LayerMask.NameToLayer("WasteCards");
                //---------------------------------------------------------
                //DOTween use to move card
                StartCoroutine(currentCardScript.FlipCard360(360));
                card.transform.DOMove(waste.position, tweeningDuration)
                    .OnComplete(() => {

                        GameManager.Instance.tableauData.Remove(card.GetComponent<Card>().index);
                        card.transform.SetParent(wasteCardHolder);
                        GameManager.Instance.Invoke("CardCheckAction", 0.3f); 

                    });

                //---------------------------------------------------------
                currentSortingOrder += sortingOrderIncrement;
                card.GetComponent<SpriteRenderer>().sortingOrder = currentSortingOrder;
                //---------------------------------------------------------
            }
            else
            {
                Vector3 crosspos = new Vector3(touchPos.x, touchPos.y, 0f);
                GameObject cross = Instantiate(crossSignPrefab, crosspos, Quaternion.identity);
                cross.transform.localScale = new Vector3(0, 0, 0);
                cross.transform.DOScale(new Vector3(0.25f,0.25f, 0.25f), 0.2f).OnComplete(()=> {
                    cross.transform.DOScale(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.InOutBack).OnComplete(()=> {

                        Destroy(cross);

                    });


                });
            }


        }


    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //If condition satifies then card will move from deck slot to wastes deck slot
    private void MoveCardFromDeck()
    {
        //
        DeckCardisMoving = true;
        //

        if (GameManager.Instance.deckStack.Count > 0)
        {
            //Local Variables
            int lastCardFromStackDeck = GameManager.Instance.deckStack.Peek();
            GameObject deckCard = Instantiate(cardPrefab, deck.position, Quaternion.identity);
            Card cardScript = deckCard.GetComponent<Card>();
            //
            //---------------------------------------------------------
            deckCard.transform.localScale = Vector3.zero;
            cardScript.SpawnOndeck(lastCardFromStackDeck);
            //---------------------------------------------------------

            if (GameManager.Instance.deckStack.Count == 1)
            {
                deck.GetChild(1).gameObject.SetActive(false);
            }

            //---------------------------------------------------------
            //DOTween use to move and scale card
            GameManager.Instance.wasteStack.Push(deckCard);
            deckCard.transform.DOScale(new Vector3(1f, 1f, 1f), Time.deltaTime * tweeningDuration);
            StartCoroutine(cardScript.FlipCard360(180));
            deckCard.transform.DOMove(waste.position, tweeningDuration)
                .OnComplete(() =>
                {

                    int poppedValue = GameManager.Instance.deckStack.Pop();
                    DeckCardisMoving = false;
                    deckCard.transform.SetParent(wasteCardHolder);
                    GameManager.Instance.Invoke("CardCheckAction", 0.3f);

                });

            //---------------------------------------------------------
            currentSortingOrder += sortingOrderIncrement;
            deckCard.GetComponent<SpriteRenderer>().sortingOrder = currentSortingOrder;
            //---------------------------------------------------------

        }
        else
        {
            Debug.Log("Deck Stack is Empty");
        }
       
    }


    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= CanInterect;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//
    //----------------------------------------------------------------------------------------------------------------------------------------------------------------//

}
