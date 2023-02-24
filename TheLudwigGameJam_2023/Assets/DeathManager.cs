using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathManager : MonoBehaviour
{

    [SerializeField] private GameObject Player;
    [SerializeField] private Transform FirstCheckPoint;

    [SerializeField] private float deathZoneBottom;
    [SerializeField] private float deathZoneTop;


    [HideInInspector] public float deathZoneBottom_;
    [HideInInspector] public float deathZoneTop_;

    [HideInInspector] public Transform spawn;


    [HideInInspector] public int SpawnNumber;


    private Transform playerTransform;
    private Rigidbody2D rb;
    private void Awake()
    {

        playerTransform = Player.GetComponent<Transform>();
        rb = Player.GetComponent<Rigidbody2D>();

        deathZoneTop_ = deathZoneTop;
        deathZoneBottom_ = deathZoneBottom;
        spawn = FirstCheckPoint;
    }

    void Update()
    {
        if (playerTransform.position.y < deathZoneBottom_) Respawn();
        if (playerTransform.position.y > deathZoneTop_) Respawn();
    }
    
    public void Respawn()
    {
        playerTransform.position = spawn.position;
        rb.velocity = new Vector2(0, 0);
    }

    public void RestartGame()
    {
        deathZoneTop_ = deathZoneTop;
        deathZoneBottom_ = deathZoneBottom;
        Player.transform.position = FirstCheckPoint.position;
        SpawnNumber = 0;
        spawn = FirstCheckPoint;
    }


}
