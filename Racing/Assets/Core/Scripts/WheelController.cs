using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] WheelCollider frontWheel;
    [SerializeField] WheelCollider backWheel;
    [SerializeField] Transform frontTransform;
    [SerializeField] Transform backTransform;

    private void FixedUpdate()
    {
        //Update wheel meshes
        UpdateWheel(frontWheel, frontTransform);
        UpdateWheel(backWheel, backTransform);
    }

    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        //Get Wheel collider state
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        //Set wheel transform state
        trans.position = position;
        trans.rotation = rotation;
    }
}
