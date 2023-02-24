using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallcheck : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform Top;
    [SerializeField] private Transform Center;
    [SerializeField] private Transform Bottom;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [HideInInspector] public bool wallColision;

    private float checkRadius = .01f;
    private void Update()
    {
        wallColision = Physics2D.OverlapCircle(Top.position, checkRadius, wallLayer) || Physics2D.OverlapCircle(Center.position, checkRadius, wallLayer) || Physics2D.OverlapCircle(Bottom.position, checkRadius, wallLayer);
        if (!wallColision && Physics2D.OverlapCircle(Center.position, checkRadius, groundLayer)) wallColision = true;
    }
}
