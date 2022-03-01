using UnityEngine;
using static OVRSkeleton;
using System.Collections.Generic;

public class HandleMovementSkeleton : MonoBehaviour
{
    // Start is called before the first frame update
    enum TrackingState { OUT, CONTINUITY, NEW_VALUES };
    public GameObject inputOculus;
    public Transform inputLeap;
    public bool isRight;
    TrackingState leapTrackingState;
    TrackingState oculusTrackingState;
    public Transform OutputHand;
    public Leap.Unity.LeapProvider leapProvider;

    Dictionary<string, Vector3> positionLeap;
    Dictionary<string, Vector3> positionOculus;
    static Vector3 centerOfMassOculus = Vector3.zero;
    static Vector3 centerOfMassLeap = Vector3.zero;
    static int nbValuesOculus = 0;
    static int nbValuesLeap = 0;

    Dictionary<string, Quaternion> rotationLeap;
    Dictionary<string, Quaternion> rotationOculus;

    float coef = 0; // 0 Leap 1 Oculus

    public Material color;

    static Vector3 leapInputOutputOffset = Vector3.zero;

    void Start()
    {
        leapTrackingState = TrackingState.OUT;
        oculusTrackingState = TrackingState.OUT;
        positionLeap = new Dictionary<string, Vector3>();
        positionOculus = new Dictionary<string, Vector3>();
        rotationLeap = new Dictionary<string, Quaternion>();
        rotationOculus = new Dictionary<string, Quaternion>();

        foreach (Transform outputchild in OutputHand.GetComponentsInChildren<Transform>())
        {
            if (mapOutputToOculus(outputchild.name) != BoneId.Invalid)
            {
                positionLeap[outputchild.name] = outputchild.position;
                positionOculus[outputchild.name] = outputchild.position;
                rotationLeap[outputchild.name] = outputchild.rotation;
                rotationOculus[outputchild.name] = outputchild.rotation;
            }
        }
    }
    void Update()
    {
        Leap.Frame frame = leapProvider.CurrentFrame;
        List<Leap.Hand> hands = frame.Hands;
        float leapConfidence = 0;
        foreach (Leap.Hand hand in hands)
        {
            if (hand.IsRight == isRight)
                leapConfidence += hand.Confidence;
        }

        leapTrackingState = UpdateTrackingState(inputLeap.gameObject.activeInHierarchy && leapConfidence > 0.5f, leapTrackingState);
        oculusTrackingState = UpdateTrackingState(inputOculus.GetComponent<OVRHand>().IsTracked && inputOculus.GetComponent<OVRHand>().HandConfidence == OVRHand.TrackingConfidence.High, oculusTrackingState);

        UpdateLeapData();
        UpdateOculusData();

        if (oculusTrackingState == TrackingState.CONTINUITY && leapTrackingState == TrackingState.CONTINUITY)
        {
            coef = 1 - leapConfidence/2;
        }
        else if (leapTrackingState == TrackingState.CONTINUITY)
        {
            coef = 0;
        }
        else if (oculusTrackingState == TrackingState.CONTINUITY)
        {
            coef = 1;
        }

        color.color = coef * Color.blue + (1 - coef) * Color.red;


        if (nbValuesOculus < 500  && oculusTrackingState == TrackingState.CONTINUITY)
        {
            Vector3 newcenterOfMassOculus = Vector3.zero;
            int nbBones = 0;
            foreach (Transform outputchild in OutputHand.GetComponentsInChildren<Transform>())
            {
                if (mapOutputToOculus(outputchild.name) != BoneId.Invalid)
                {
                    newcenterOfMassOculus += positionOculus[outputchild.name];
                    nbBones++;
                }
            }

            centerOfMassOculus = (nbValuesOculus * centerOfMassOculus + newcenterOfMassOculus);
            nbValuesOculus += nbBones;
            centerOfMassOculus = centerOfMassOculus / nbValuesOculus;
        }

        if (nbValuesLeap < 500 && leapTrackingState == TrackingState.CONTINUITY)
        {
            Vector3 newcenterOfMassLeap = Vector3.zero;
            int nbBones = 0;
            foreach (Transform outputchild in OutputHand.GetComponentsInChildren<Transform>())
            {
                if (mapOutputToOculus(outputchild.name) != BoneId.Invalid)
                {
                    newcenterOfMassLeap += positionLeap[outputchild.name];
                    nbBones++;
                }
            }

            centerOfMassLeap = (nbValuesLeap * centerOfMassLeap + newcenterOfMassLeap);
            nbValuesLeap += nbBones;
            centerOfMassLeap = centerOfMassLeap / nbValuesLeap;
        }


        foreach (Transform outputChild in OutputHand.GetComponentsInChildren<Transform>())
        {
            if (mapOutputToOculus(outputChild.name) != BoneId.Invalid)
            {
                Vector3 targertOculus = positionOculus[outputChild.name];
                Vector3 targetLeap = positionLeap[outputChild.name] - centerOfMassLeap + centerOfMassOculus;

                Vector3 targetposition = targetLeap;// Vector3.Lerp(targetLeap, targertOculus, coef);

                outputChild.position = Vector3.MoveTowards(outputChild.position, targetposition, Time.deltaTime * 100);

                Quaternion targetRotation = Quaternion.Slerp(rotationLeap[outputChild.name], rotationOculus[outputChild.name], 0.5f);
                outputChild.rotation = targetRotation;
            }
        }
    }
    void UpdateOculusData()
    { 
        if (oculusTrackingState != TrackingState.OUT)
        {
            foreach (Transform outputchild in OutputHand.GetComponentsInChildren<Transform>())
            {
                BoneId currentID = mapOutputToOculus(outputchild.name);
                if (currentID != BoneId.Invalid)
                {
                    foreach (OVRBone bone in inputOculus.GetComponent<OVRSkeleton>().Bones)
                    {
                        if (bone.Id == currentID)
                        {
                            Vector3 newPos = bone.Transform.position;
                            
                            Quaternion newRot = bone.Transform.rotation;
                            Vector3 axisForward = newRot * Vector3.forward;
                            Vector3 axisUp = newRot * Vector3.up;

                            if (!isRight)
                                newRot = Quaternion.AngleAxis(-90, axisUp) * Quaternion.AngleAxis(180, axisForward) * newRot;
                            else
                                newRot = Quaternion.AngleAxis(90, axisUp) * newRot;
                                
                            positionOculus[outputchild.name] = newPos;
                            rotationOculus[outputchild.name] = newRot;
                            break;
                        }
                    }
                }
            }
        }
    }
    void UpdateLeapData()
    {
        if (leapTrackingState != TrackingState.OUT)
        {
            foreach (Transform inputchild in inputLeap.GetComponentsInChildren<Transform>())
            {
                if (mapOutputToOculus(inputchild.name) != BoneId.Invalid)
                {
                    Vector3 newPos = inputchild.position;
                    Quaternion newRot = inputchild.rotation;
                   
                    positionLeap[inputchild.name] = newPos;
                    rotationLeap[inputchild.name] = newRot;
                }
            }
        }
    }
    TrackingState UpdateTrackingState(bool cond, TrackingState trackingState)
    {
        if (!cond)
        {
            return TrackingState.OUT;
        }
        else if (trackingState == TrackingState.NEW_VALUES)
        {
            return TrackingState.CONTINUITY;
        }
        else
        {
            return TrackingState.NEW_VALUES;
        }
    }
    BoneId mapOutputToOculus(string name)
    {
        switch (name)
        {
            case "Palm":
                return BoneId.Hand_Start;
            case "ThumbProximalJoint":
                return BoneId.Hand_Thumb1;
            case "ThumbDistalJoint":
                return BoneId.Hand_Thumb2;
            case "ThumbTip":
                return BoneId.Hand_Thumb3;
            case "IndexMiddleJoint":
                return BoneId.Hand_Index1;
            case "IndexDistalJoint":
                return BoneId.Hand_Index2;
            case "IndexTip":
                return BoneId.Hand_Index3;
            case "MiddleMiddleJoint":
                return BoneId.Hand_Middle1;
            case "MiddleDistalJoint":
                return BoneId.Hand_Middle2;
            case "MiddleTip":
                return BoneId.Hand_Middle3;
            case "RingMiddleJoint":
                return BoneId.Hand_Ring1;
            case "RingDistalJoint":
                return BoneId.Hand_Ring2;
            case "RingTip":
                return BoneId.Hand_Ring3;
            case "PinkyKnuckle":
                return BoneId.Hand_Pinky0;
            case "PinkyMiddleJoint":
                return BoneId.Hand_Pinky1;
            case "PinkyDistalJoint":
                return BoneId.Hand_Pinky2;
            case "PinkyTip":
                return BoneId.Hand_Pinky3;
        }
        return BoneId.Invalid;
    }
}
