using UnityEngine;
using static OVRSkeleton;
public class HandleMovement : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject inputOculus;
    public Transform inputLeap;
    public bool isRight;

    public Transform OutputHand;

    Vector3 previousLeapPos;
    Quaternion previousRotation;
    bool hasPreviousLeapData;

    void Start()
    {
        previousLeapPos = new Vector3(0, 0, 0);
        previousRotation = Quaternion.identity;
        hasPreviousLeapData = false;
    }

    void Update()
    {
        Quaternion rotationalCorrection;
        Vector3 deltaPosition;

        if (inputOculus.GetComponent<OVRHand>().IsTracked && inputOculus.GetComponent<OVRHand>().HandConfidence == OVRHand.TrackingConfidence.High)
        {
            deltaPosition = inputOculus.transform.position - OutputHand.parent.transform.position - 0.01f * inputOculus.transform.right;
            Quaternion goal = inputOculus.transform.rotation;
            if (isRight)
            {
                goal = new Quaternion(goal.x, -goal.y, -goal.z, goal.w);
                Vector3 axis = goal * Vector3.forward;
                goal = Quaternion.AngleAxis(180, axis) * goal;
            }
            else
            {
                goal = Quaternion.AngleAxis(180, inputOculus.transform.up) * goal;
            }
            rotationalCorrection = goal * Quaternion.Inverse(OutputHand.parent.transform.localRotation);
        }
        else if (inputLeap.gameObject.activeInHierarchy && hasPreviousLeapData)
        {
            deltaPosition = inputLeap.parent.position - previousLeapPos;
            rotationalCorrection = inputLeap.parent.localRotation * Quaternion.Inverse(previousRotation);
        } else
        {
            deltaPosition = Vector3.zero;
            rotationalCorrection = Quaternion.identity;
        }

        Debug.Log(deltaPosition);
        
        
        if (inputLeap.gameObject.activeInHierarchy) 
        {
            previousLeapPos = inputLeap.parent.position;
            previousRotation = inputLeap.parent.localRotation;
            hasPreviousLeapData = true;
        }
        else
        {
            hasPreviousLeapData = false;
        }
       
        if (inputLeap.gameObject.activeInHierarchy)
        {
            foreach (Transform inputchild in inputLeap.GetComponentsInChildren<Transform>())
            {
                foreach (Transform outputChild in OutputHand.GetComponentsInChildren<Transform>())
                {
                    if (inputchild.name == outputChild.name)
                    {
                        outputChild.localPosition = inputchild.localPosition;
                        outputChild.localRotation = inputchild.localRotation;
                    }
                }
            }
        }

        OutputHand.parent.transform.position += deltaPosition;
        OutputHand.parent.transform.localRotation = rotationalCorrection * OutputHand.parent.transform.localRotation;
    }

}
