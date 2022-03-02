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

    float waitTime = 0;
    static float lifeSpan = 20;
    static float superLifeSpan = 10;

    // Start is called before the first frame update
    void Start()
    {
        currentState = MoleState.DOWN;

    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;
        if (waitTime < 0)
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
            float test = Random.Range(0, 11);
            if (test > 2 && test <= 7)
            {
                currentState = MoleState.NORMAL;
                Mole.gameObject.GetComponent<Renderer>().material = normalMaterial;
                Mole.position = Mole.position + 1f * Vector3.up;
                waitTime = lifeSpan;
            }
            else if (test > 7)
            {
                currentState = MoleState.SUPER;
                Mole.gameObject.GetComponent<Renderer>().material = superMaterial;
                Mole.position = Mole.position + 1f * Vector3.up;
                waitTime = superLifeSpan;
            }
            else
            {
                waitTime = lifeSpan;
            }
            return 0;
        }
        else if (currentState == MoleState.SUPER && isHit)
        {
            currentState = MoleState.NORMAL;
            Mole.gameObject.GetComponent<Renderer>().material = normalMaterial;
            waitTime = lifeSpan;
        }
        else
        {
            currentState = MoleState.DOWN;
            Mole.gameObject.GetComponent<Renderer>().material = normalMaterial;
            Mole.position = Mole.position - 1f * Vector3.up;
            waitTime = lifeSpan;
        }
        if (isHit)
        {
            return 5;
        }
        return 0;
    }
}
