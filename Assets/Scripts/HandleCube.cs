using UnityEngine;
using System.Collections.Generic;
using static OVRSkeleton;

public class HandleCube : MonoBehaviour
{
    public Transform Tip;
    public GameObject cubePrefab;
    public Transform leftHand;
    public Transform giantHand;
    public List<MoleTile> tiles;
    float interactionResolution = 5;
    public IndicatorBehaviour indicator;
    public enum interactionState
    {
        SPAWN,
        HIT,
    }

    public interactionState currentInteraction;

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

        Quaternion current_giantHandRotation = giantHand.transform.rotation;
        Vector3 current_giantHandPosition = giantHand.transform.position;        
        
        Vector3 localCollision_leftHand_Pos = leftHand.transform.InverseTransformPoint(other.ClosestPoint(transform.position));

        Quaternion rot = Quaternion.AngleAxis(180, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.up);
        Vector3 localCollision_giantHand_Pos = rot * localCollision_leftHand_Pos;
        Vector3 offset = current_giantHandRotation * localCollision_giantHand_Pos;
        offset.x *= 100;
        offset.y *= 20;
        offset.z *= 100;

        Vector3 interactionPos = current_giantHandPosition + offset + 5 * Vector3.up;


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
                tiles[closestMole].ChangeState(true);
                indicator.Indicate(tiles[closestMole].gameObject.transform.position);
                
            }
        }
    }
}
