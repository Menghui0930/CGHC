using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlOpenFloor : MonoBehaviour
{
    [SerializeField] private Sprite normalFloorSprite;
    [SerializeField] private Sprite brokenFloorSprite;
    [SerializeField] private List<SpriteRenderer> theSR = new List<SpriteRenderer>();

    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < theSR.Count; i++)
        {
            theSR[i].sprite = brokenFloorSprite;
            theSR[i].GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Meow"))
        {
            for (int i = 0; i < theSR.Count; i++)
            {
                theSR[i].sprite = normalFloorSprite;
                theSR[i].GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Meow"))
        {
            for (int i = 0; i < theSR.Count; i++)
            {
                theSR[i].sprite = brokenFloorSprite;
                theSR[i].GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
}
