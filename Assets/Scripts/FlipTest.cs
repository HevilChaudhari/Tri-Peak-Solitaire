using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTest : MonoBehaviour
{
    private SpriteRenderer render;
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;


    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _ = StartCoroutine(FlipCard360());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator FlipCard360()
    {

       
            for (float i = 360f; i >= 0; i -= 10)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                //---------------------------------------------------------
                if (i == 270)
                {
                    render.sprite = backSprite;
                }
                else if(i == 90)
                {
                render.sprite = frontSprite;
            }
                //---------------------------------------------------------
                yield return new WaitForSeconds(0.02f);


            }

    }
}
