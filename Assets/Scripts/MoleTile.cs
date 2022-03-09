using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleTile : MonoBehaviour
{
    public Transform Mole;
    public Material normalMaterial;
    public Material superMaterial;

    public enum MoleState { DOWN, NORMAL, SUPER};
    public MoleState currentState;

    float timeToElapse = 0;
    static float lifeSpan = 30;
    static float superLifeSpan = 15;
    static float waitDuration = 5;

    // Start is called before the first frame update
    void Start()
    {
        currentState = MoleState.DOWN;
    }

    // Update is called once per frame
    void Update()
    {
        timeToElapse -= Time.deltaTime;
        if (timeToElapse < 0)
        {
            ChangeState();
        }
    }

    public float ChangeState(bool isHit = false)
    {
        if (currentState == MoleState.DOWN)
        {
            if (isHit)
                return 0;
            float test = Random.Range(0, 4);
            if (test < 2)
            {
                currentState = MoleState.NORMAL;
                Mole.gameObject.GetComponent<Renderer>().material = normalMaterial;
                Mole.position = Mole.position + 1f * Vector3.up;
                timeToElapse = lifeSpan;
            }
            else if (test == 3)
            {
                currentState = MoleState.SUPER;
                Mole.gameObject.GetComponent<Renderer>().material = superMaterial;
                Mole.position = Mole.position + 1f * Vector3.up;
                timeToElapse = superLifeSpan;
            }
            else
            {
                timeToElapse = waitDuration;
            }
            return 0;
        }
        else if (currentState == MoleState.SUPER && isHit)
        {
            currentState = MoleState.NORMAL;
            Mole.gameObject.GetComponent<Renderer>().material = normalMaterial;
            timeToElapse = lifeSpan;
        }
        else
        {
            currentState = MoleState.DOWN;
            Mole.gameObject.GetComponent<Renderer>().material = normalMaterial;
            Mole.position = Mole.position - 1f * Vector3.up;
            timeToElapse = lifeSpan;
        }
        if (isHit)
        {
            return 5;
        }
        return 0;
    }
}
