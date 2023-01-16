using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] float velocityMultiplier = 1f;

    private void Start()
    {
        transform.LookAt(PlayerHP.player.gameObject.transform);
        rb = GetComponent<Rigidbody>();
        GameObject playerObj = PlayerHP.player.gameObject;
        Vector3 direction = playerObj.transform.position - transform.position;
        rb.velocity = direction * velocityMultiplier;
        Destroy(gameObject, 3f);
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerHP.player.GetDamage(15);
        }
    }
}
