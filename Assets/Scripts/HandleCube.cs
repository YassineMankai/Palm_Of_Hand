using UnityEngine;
using static OVRSkeleton;

public class HandleCube : MonoBehaviour
{
    public Transform Tip;
    public GameObject cubePrefab1;
    public GameObject cubePrefab2;
    public Transform leftHand;
    public Transform giantHand;
    public enum interactionState
    {
        SPAWN,
        PICK,
        RELEASE,
    }

    public interactionState currentInteraction = interactionState.SPAWN;

    private void Update()
    {
        transform.position = Tip.position;
        transform.rotation = Tip.rotation;
        transform.position -= 0.07f * Tip.up;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.name.StartsWith("Capsule"))
            return;

        Quaternion current_giantHandRotation = giantHand.transform.rotation;
        Vector3 current_giantHandPosition = giantHand.transform.position;

        Debug.Log($"collision {other.name}");
        
        GameObject a;
        if (currentInteraction == interactionState.SPAWN)
            a = GameObject.Instantiate(cubePrefab1);
        else
            a = GameObject.Instantiate(cubePrefab2);

        Vector3 localCollision_leftHand_Pos = leftHand.transform.InverseTransformPoint(other.ClosestPoint(transform.position));

        Quaternion rot = Quaternion.AngleAxis(180, Vector3.forward) * Quaternion.AngleAxis(90, Vector3.up);
        Vector3 localCollision_giantHand_Pos = rot * localCollision_leftHand_Pos;
        Vector3 offset = current_giantHandRotation * localCollision_giantHand_Pos;
        offset.x *= 50;
        offset.y *= 10;
        offset.z *= 50;

        a.transform.position = current_giantHandPosition + offset + 5 * Vector3.up;


        Debug.Log("test rot");
        Debug.Log($"test rot  local to output {1000 * localCollision_leftHand_Pos}");
        Debug.Log($"test rot  local to giant {1000 * localCollision_giantHand_Pos}");
        Debug.Log($"test rot  giant rot {current_giantHandRotation}");
        Debug.Log($"test rot  global {1000 * offset}");
        Debug.Log($"#######");

        /* Vector3 localCollisionPos = other.transform.InverseTransformPoint(other.ClosestPoint(transform.position));
        foreach (OVRBone bone in giantRightHandSkeleton.Bones)
        {
            string capsuleName = OVRSkeleton.BoneLabelFromBoneId(OVRSkeleton.SkeletonType.HandRight, bone.Id) + "_CapsuleCollider";
            if (capsuleName.Equals(other.name))
            {
                GameObject a;
                if (currentInteraction  == interactionState.SPAWN)
                    a = GameObject.Instantiate(cubePrefab1);
                else
                    a = GameObject.Instantiate(cubePrefab2);
                Vector3 newPos = bone.Transform.TransformPoint(localCollisionPos);
                newPos.y += 4;
                a.transform.position = newPos;
                a.name = "Cube" + bone.Id.ToString();

            }
        }*/
    }
}
