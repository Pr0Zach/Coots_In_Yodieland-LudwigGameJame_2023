using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RespawnController : MonoBehaviour
{

    private GameManager gm;

    private DeathManager dm;
    private void Awake()
    {
        
        gm = FindAnyObjectByType<GameManager>();
        dm = FindAnyObjectByType<DeathManager>();
    }
    public void Respawn(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Canceled) dm.Respawn();
    }

    public void Restart(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed) gm.StartGame();
    }
}
