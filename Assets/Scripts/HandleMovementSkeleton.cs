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

    Dictionary<string, Vector3> positionLeap;
    Dictionary<string, Vector3> positionOculus;
    Dictionary<string, Vector3> offsetLeap;
    Dictionary<string, Vector3> offsetOculus;
    Dictionary<string, Quaternion> rotationLeap;
    Dictionary<string, Quaternion> rotationOculus;
    Dictionary<string, Quaternion> rotationalVelocityLeap;
    Dictionary<string, Quaternion> rotationalVelocityOculus;

    float recenterTime = 0;
    float coefOculus = 0;
    float coefLeap = 0;

    public Material color;

    void Start()
    {
        leapTrackingState = TrackingState.OUT;
        oculusTrackingState = TrackingState.OUT;
        positionLeap = new Dictionary<string, Vector3>();
        positionOculus = new Dictionary<string, Vector3>();
        offsetLeap = new Dictionary<string, Vector3>();
        offsetOculus = new Dictionary<string, Vector3>();
        rotationLeap = new Dictionary<string, Quaternion>();
        rotationOculus = new Dictionary<string, Quaternion>();
        rotationalVelocityLeap = new Dictionary<string, Quaternion>();
        rotationalVelocityOculus = new Dictionary<string, Quaternion>();

        foreach (Transform outputchild in OutputHand.GetComponentsInChildren<Transform>())
        {
            if (mapOutputToOculus(outputchild.name) != BoneId.Invalid)
            {
                positionLeap[outputchild.name] = outputchild.position;
                positionOculus[outputchild.name] = outputchild.position;
                offsetLeap[outputchild.name] = outputchild.position;
                offsetOculus[outputchild.name] = outputchild.position;
                rotationLeap[outputchild.name] = outputchild.rotation;
                rotationOculus[outputchild.name] = outputchild.rotation;
                rotationalVelocityLeap[outputchild.name] = Quaternion.identity;
                rotationalVelocityOculus[outputchild.name] = Quaternion.identity;
            }
        }
    }
    void Update()
    {
        leapTrackingState = UpdateTrackingState(inputLeap.gameObject.activeInHierarchy, leapTrackingState);
        oculusTrackingState = UpdateTrackingState(inputOculus.GetComponent<OVRHand>().IsTracked && inputOculus.GetComponent<OVRHand>().HandConfidence == OVRHand.TrackingConfidence.High, oculusTrackingState);
        UpdateLeapData();
        UpdateOculusData();


        if (oculusTrackingState == TrackingState.CONTINUITY)
            recenterTime -= Time.deltaTime;
        else if (oculusTrackingState == TrackingState.NEW_VALUES)
            recenterTime += Time.deltaTime * 2;

        Debug.Log($"Evolution of recenter {recenterTime}");

        if (oculusTrackingState == TrackingState.OUT && leapTrackingState == TrackingState.OUT)
        {
            return;
        }

        if ((recenterTime <= 0 && oculusTrackingState != TrackingState.OUT) || 
            (oculusTrackingState == TrackingState.NEW_VALUES && leapTrackingState == TrackingState.NEW_VALUES))
        {
            foreach (Transform outputChild in OutputHand.GetComponentsInChildren<Transform>())
            {
                if (mapOutputToOculus(outputChild.name) != BoneId.Invalid)
                {
                    outputChild.position = positionOculus[outputChild.name];
                    outputChild.rotation = rotationOculus[outputChild.name];
                }
            }
            recenterTime = 50;
            return;
        }
        
        if (oculusTrackingState == TrackingState.CONTINUITY && leapTrackingState == TrackingState.CONTINUITY)
        {
            coefOculus = Mathf.Max(0.8f, coefOculus + Time.deltaTime * (0.8f - coefOculus) / 10);
            coefLeap = Mathf.Max(0.2f, coefLeap + Time.deltaTime * (0.2f - coefLeap) / 10);
        }
        else if (leapTrackingState == TrackingState.CONTINUITY)
        {
            coefOculus = Mathf.Max(0, coefOculus + Time.deltaTime * (0 - coefOculus) / 10);
            coefLeap = Mathf.Max(1, coefLeap + Time.deltaTime * (1 - coefLeap) / 10);
        }
        else if (oculusTrackingState == TrackingState.CONTINUITY)
        {
            coefOculus = Mathf.Max(1, coefOculus + Time.deltaTime * (1 - coefOculus) / 10);
            coefLeap = Mathf.Max(0, coefLeap + Time.deltaTime * (0 - coefLeap) / 10);
        }

        color.color = coefOculus * Color.blue + coefLeap * Color.red;

        foreach (Transform outputChild in OutputHand.GetComponentsInChildren<Transform>())
        {
            if (mapOutputToOculus(outputChild.name) != BoneId.Invalid)
            {
                Vector3 offset = coefOculus * (positionOculus[outputChild.name] - offsetOculus[outputChild.name]) + coefLeap * (positionLeap[outputChild.name] - offsetLeap[outputChild.name]);
                outputChild.position = offsetOculus[outputChild.name] + offset;
                outputChild.rotation = rotationOculus[outputChild.name]; 
            }
        }
    }
    void UpdateOculusData()
    {
        if (oculusTrackingState != TrackingState.OUT)
        {
            foreach (Transform inputchild in inputLeap.GetComponentsInChildren<Transform>())
            {
                BoneId currentID = mapOutputToOculus(inputchild.name);
                if (currentID != BoneId.Invalid)
                {
                    foreach (OVRBone bone in inputOculus.GetComponent<OVRSkeleton>().Bones)
                    {
                        if (bone.Id == currentID)
                        {
                            Vector3 newPos = bone.Transform.position;
                            Quaternion newRot = bone.Transform.rotation;
                            Vector3 axisForward = newRot * Vector3.forward;
                            Vector3 axisRight = newRot * Vector3.right;
                            Vector3 axisUp = newRot * Vector3.up;
                            newRot = Quaternion.AngleAxis(-90, axisUp) * Quaternion.AngleAxis(180, axisForward) * newRot;

                            if (oculusTrackingState == TrackingState.CONTINUITY)
                            {
                                rotationalVelocityOculus[inputchild.name] = newRot * Quaternion.Inverse(rotationOculus[inputchild.name]);
                            }
                            else if (oculusTrackingState == TrackingState.NEW_VALUES)
                            {
                                rotationalVelocityOculus[inputchild.name] = Quaternion.identity;
                                offsetOculus[inputchild.name] = newPos;
                            }

                            positionOculus[inputchild.name] = newPos;
                            rotationOculus[inputchild.name] = newRot;
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

                    if (oculusTrackingState == TrackingState.CONTINUITY)
                    {
                        rotationalVelocityLeap[inputchild.name] = newRot * Quaternion.Inverse(rotationLeap[inputchild.name]);
                    }
                    else if (oculusTrackingState == TrackingState.NEW_VALUES)
                    {
                        rotationalVelocityLeap[inputchild.name] = Quaternion.identity;
                        offsetLeap[inputchild.name] = newPos;
                    }

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
            case "Wrist":
                return BoneId.Hand_WristRoot;
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
