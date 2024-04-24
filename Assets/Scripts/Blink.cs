using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    // Start is called before the first frame update
    public float blinkTime = 0.1f;
    public float blinkInterval = 0.5f;

    public float timer = 20.0f; //Time before self destruct

    //Make blue or red as the secondary color selectable from editor
    public Color secondaryColor = new Color(0, 0.78f, 1, 1);

     public GameObject playerPrefab;




    IEnumerator BlinkColor()
    {
        while (true)
        {
            GetComponent<SpriteRenderer>().color = secondaryColor;
            yield return new WaitForSeconds(blinkTime);
            GetComponent<SpriteRenderer>().color = Color.white;
            yield return new WaitForSeconds(blinkTime);
        }
    }

    //Destroy self after adjustable time
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void DeathTimer()
    {
        timer -= Time.deltaTime; 
        if (timer <= 0)
        {
            DestroySelf();
        }
    }


    void Start()
    {
        //Make self change from 00C7FF to white nonstop
        StartCoroutine(BlinkColor());

    }

    // Update is called once per frame
    void Update()
    {
        DeathTimer();
        
    }
}
