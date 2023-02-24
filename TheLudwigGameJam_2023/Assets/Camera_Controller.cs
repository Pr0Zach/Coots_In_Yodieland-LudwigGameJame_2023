using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{

    [SerializeField] private GameObject player;


    private Transform cameraTransform;

    private Transform playerTransform;

    [SerializeField] private float lerpTime;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float speedOffsetStartSpeed;
    [SerializeField] private float speedOffset;
    // Start is called before the first frame update

    private Rigidbody2D rb;

    void Start()
    {
        
        cameraTransform = GetComponent<Transform>();
        playerTransform = player.GetComponent<Transform>();
        rb = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float speedOffset_ = 0;
        //if (rb.velocity.x > speedOffsetStartSpeed) speedOffset_ = speedOffset;
        //if (rb.velocity.x < -speedOffsetStartSpeed) speedOffset_ = -speedOffset;
        Vector3 pos = new Vector3(playerTransform.position.x + offset.x + speedOffset_, playerTransform.position.y + offset.y, -10);
        Vector3 smoothedPosition = Vector3.Lerp(cameraTransform.position, pos, lerpTime);
        cameraTransform.position = smoothedPosition;
    }



    IEnumerator moveCamera(float curOffset, float newOffset, float time)
    {

        yield return new WaitForSeconds(time);
    }
}
