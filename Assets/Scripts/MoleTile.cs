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

    float waitTime = 10;
    // Start is called before the first frame update
    void Start()
    {
        currentState = MoleState.DOWN;
        Mole.position = Mole.position - 0.5f * Vector3.up;
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

    public void ChangeState(bool isHit = false)
    {
        if (currentState == MoleState.DOWN)
        {
            if (isHit)
                return;
            float test = Random.Range(0, 11);
            if (test > 2 && test <=7)
            {
                currentState = MoleState.NORMAL;
                Mole.gameObject.GetComponent<Renderer>().material = normalMaterial;
                Mole.position = Mole.position + 0.5f * Vector3.up;
                waitTime = 10;
            } 
            else if (test > 7)
            {
                currentState = MoleState.SUPER;
                Mole.gameObject.GetComponent<Renderer>().material = superMaterial;
                Mole.position = Mole.position + 0.5f * Vector3.up;
                waitTime = 5;
            }
            
        }
        else if (currentState == MoleState.SUPER && isHit)
        {
            currentState = MoleState.NORMAL;
            Mole.gameObject.GetComponent<Renderer>().material = normalMaterial;
            waitTime = 5;
        }
        else
        {
            currentState = MoleState.DOWN;
            Mole.gameObject.GetComponent<Renderer>().material = normalMaterial;
            Mole.position = Mole.position - 0.5f * Vector3.up;
            waitTime = 10;
        }
    }
}