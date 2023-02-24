using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Modifiyer : MonoBehaviour
{


    [SerializeField] private Material material;
    [SerializeField] private GameObject player;
    [SerializeField] private Gradient color_1;
    [SerializeField] private Gradient color_2;
    [SerializeField] private float sizeMultiplyer;

    private Transform tr;

    private void Awake()
    {
        tr = player.GetComponent<Transform>();
    }
    void Update()
    {
        Color color1_ = color_1.Evaluate(Mathf.Clamp(sizeMultiplyer * tr.position.x, 0, 1));
        Color color2_ = color_2.Evaluate(Mathf.Clamp(sizeMultiplyer * tr.position.x, 0, 1));
        material.SetColor("_Color_1", color1_);
        material.SetColor("_Color_2", color2_);

    }
}
