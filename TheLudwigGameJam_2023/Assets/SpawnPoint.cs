using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{

    [SerializeField] private int spawnNumber;
    [SerializeField] private float topDeathLine;
    [SerializeField] private float bottomDeathLine;
    [SerializeField] private GameObject spawnPoint;
    private Transform spawnPoint_transform;
    private DeathManager dm;
    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(0, 0, 0, 0);
        dm = FindObjectOfType<DeathManager>();
        spawnPoint.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        spawnPoint_transform = spawnPoint.GetComponent<Transform>();
        //Debug.Log(spawnPoint);
        //dm.spawn = spawnPoint_transform;
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //Debug.Log(spawnPoint);
            if (dm.SpawnNumber < spawnNumber)
            {
                dm.SpawnNumber = spawnNumber;
                dm.spawn = spawnPoint_transform;
                //Debug.Log(spawnPoint);
                if (topDeathLine != 0) dm.deathZoneTop_ = topDeathLine;
                if (bottomDeathLine != 0) dm.deathZoneBottom_ = bottomDeathLine;
            }
        }
    }
}
