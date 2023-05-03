using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMove : MonoBehaviour
{
    public Transform destination;
    public float moveSpeed = 6f;
    public int numSouls;

    float dist = 100;

    public void StartMove(Transform dest)
    {
        destination = dest;
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        dist = 100;
        while (dist > 0.1f)
        {
            Vector3 to = destination.position + new Vector3(0, 1, 0);
            transform.LookAt(to);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            dist = (to - transform.position).magnitude;
            yield return null;
        }
        PlayerCharacter pc = destination.GetComponent<PlayerCharacter>();
        if (pc)
        {
            pc.souls += numSouls;
            Destroy(gameObject);
        }
    }
}

