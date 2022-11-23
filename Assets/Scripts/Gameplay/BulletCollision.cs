using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    [SerializeField] private ParticleSystem proyectil;
    [SerializeField] private ParticleSystem impacto;
    [SerializeField] private ParticleSystem impactoS;

    public List<ParticleCollisionEvent> collisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();

    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = proyectil.GetCollisionEvents(other, collisionEvents);
        int i = 0;

        if (i < numCollisionEvents)
        {
            Vector3 pos = collisionEvents[i].intersection;
            if (other.CompareTag("enemigo"))
            {
                impacto.transform.position = pos;
                impacto.Play();
                //other.GetComponent<StateMachine_Enemy>().DamageEnemy(10);
            }
            else
            {
                impactoS.transform.position = pos;
                impactoS.Play();
            }
        }



    }
}
