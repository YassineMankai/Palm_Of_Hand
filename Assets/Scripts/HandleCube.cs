using UnityEngine;
using System.Collections.Generic;
using TMPro;
using static OVRSkeleton;

public class HandleCube : MonoBehaviour
{
    public enum interactionState
    {
        SPAWN,
        HIT,
    }

    public interactionState currentInteraction;

    public Transform Tip;
    public GameObject cubePrefab;
    public Transform leftHand;
    public Transform giantHand;
    public List<MoleTile> tiles;
    float interactionResolution = 3.5f;
    public IndicatorBehaviour indicator;

    float score = 0;
    public TextMeshPro text;


    private void Update()
    {
        transform.position = Tip.position;
        transform.rotation = Tip.rotation;
        transform.position += 0.02f * Tip.up;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.name.StartsWith("Capsule"))
            return;

        Transform bone = other.transform.parent.parent.transform;

        string capsuleName = bone.name;
        string correspondantInGiant = mapOutputToGiantHand(capsuleName);

        GameObject giantCapsule = GameObject.Find(correspondantInGiant);

        if (giantCapsule == null)
            return;

        Quaternion current_giantHandRotation = giantCapsule.transform.rotation;
        Vector3 current_giantHandPosition = giantCapsule.transform.position;        
        
        Vector3 localCollision_leftHand_Pos = bone.InverseTransformPoint(other.ClosestPoint(transform.position));

        Quaternion rot = Quaternion.AngleAxis(180, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.up);
        Vector3 localCollision_giantHand_Pos = rot * localCollision_leftHand_Pos;
        Vector3 offset = current_giantHandRotation * localCollision_giantHand_Pos;
        offset.x *= 40;
        offset.y *= 20;
        offset.z *= 40;

        Vector3 interactionPos = current_giantHandPosition + offset + Vector3.up;


        if (currentInteraction == interactionState.SPAWN)
        {
            GameObject a = GameObject.Instantiate(cubePrefab);
            a.transform.position = interactionPos;
        }
        else if (currentInteraction == interactionState.HIT)
        {
            int closestMole = -1;
            float minDistance = float.MaxValue;
            for(int i=0; i <tiles.Count; i++)
            {
                float current_distance = (interactionPos - tiles[i].gameObject.transform.position).magnitude;
                if (tiles[i].currentState != MoleTile.MoleState.DOWN && current_distance < minDistance)
                {
                    minDistance = current_distance;
                    closestMole = i;
                }
            }
            if (closestMole != -1 && minDistance < interactionResolution)
            {
                float gain = tiles[closestMole].ChangeState(true);
                score += gain;
                text.text = $"score {score}";
                indicator.Indicate(tiles[closestMole].gameObject.transform.position);
                
            }
        }
    }
    string mapOutputToGiantHand(string name)
    {
        switch (name)
        {
            case "Palm":
                return "L_Palm";

            case "ThumbProximalJoint":
                return "L_thumb_a";
            case "ThumbDistalJoint":
                return "L_thumb_b";
            case "ThumbTip":
                return "L_thumb_end";

            case "IndexMiddleJoint":
                return "L_index_b";
            case "IndexDistalJoint":
                return "L_index_c";
            case "IndexTip":
                return "L_index_end";

            case "MiddleMiddleJoint":
                return "L_middle_b";
            case "MiddleDistalJoint":
                return "L_middle_c";
            case "MiddleTip":
                return "L_middle_end";

            case "RingMiddleJoint":
                return "L_ring_b";
            case "RingDistalJoint":
                return "L_ring_c";
            case "RingTip":
                return "L_ring_end";

            case "PinkyKnuckle":
                return "L_pinky_a";
            case "PinkyMiddleJoint":
                return "L_pinky_b";
            case "PinkyDistalJoint":
                return "L_pinky_c";
            case "PinkyTip":
                return "L_pinky_end";
        }
        return "Invalid";
    }

}
