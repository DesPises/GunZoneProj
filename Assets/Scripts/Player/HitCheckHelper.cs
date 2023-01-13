using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCheckHelper : MonoBehaviour
{
    PlayerAttack player;

    private void Awake()
    {
        player = GetComponentInParent<PlayerAttack>();
    }

    public void RevolverHitCheck()
    {
        player.RevolverHitCheck();
    }

    public void WhipHitCheck()
    {
        player.WhipHitCheck();
    }
}
