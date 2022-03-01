using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorBehaviour : MonoBehaviour
{
    // Start is called before the first frame update

    float waitTime = 15;

    // Update is called once per frame
    void Update()
    {
        if (waitTime > 0 && waitTime < 15)
        {
            waitTime -= Time.deltaTime;
        }
        else if(waitTime < 0)
        {
            transform.position = new Vector3(10, 3.5f, 0);
        }
    }

    public void Indicate(Vector3 pos)
    {
        waitTime = 4;
        transform.position = new Vector3(pos.x, 3.5f, pos.z); ;
    }
}
