using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private PlayerController pc;
    private Transform tr;
    private Rigidbody2D rb;
    private GameManager gm;

    [SerializeField] private float failedSlideBlockMultiplyer;
    private void Awake()
    {
        gm = FindAnyObjectByType<GameManager>();
        pc = GetComponent<PlayerController>();  
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "finish")
        {
            //Debug.Log("Finish");
            gm.WinGame();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "SlidingBlock" && !pc.isSliding)
        {
            if (tr.position.x > collision.gameObject.transform.position.x) FailedSlideBlock(-1);
            if (tr.position.x < collision.gameObject.transform.position.x) FailedSlideBlock(1);
        }
    }

    void FailedSlideBlock(int direction)
    {
        //Debug.Log("Test");
        rb.velocity = new Vector2(0, 0);
        rb.AddForce(Vector2.left * direction * failedSlideBlockMultiplyer, ForceMode2D.Impulse);
    }
}


