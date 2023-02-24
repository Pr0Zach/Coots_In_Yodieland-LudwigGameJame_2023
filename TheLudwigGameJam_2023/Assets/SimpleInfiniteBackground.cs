using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInfiniteBackground : MonoBehaviour
{

    [SerializeField] private Transform player;


    private Transform selfPos;

    private void Awake()
    {
        selfPos = GetComponent<Transform>();
    }
    private void Update()
    {
        Vector3 pos = player.position;

        float x = pos.x;
        x *= .01f;
        x = Mathf.Round(x);
        x *= 100;

        Vector3 newPos = new Vector3(x, 0, 0);

        selfPos.position = newPos;
    }
}
