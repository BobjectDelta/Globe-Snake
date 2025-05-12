using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalAttraction : MonoBehaviour
{
    public float gravityForce = -10;
    public void Attract(Transform bodyTransform, Rigidbody rigidbody)
    {
        Vector3 attractionDirection = (transform.position - bodyTransform.position).normalized;
        Vector3 bodyUpDirection = bodyTransform.up;

        rigidbody.AddForce(attractionDirection * gravityForce);
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUpDirection, -attractionDirection) * bodyTransform.rotation;
        bodyTransform.rotation = Quaternion.Slerp(bodyTransform.rotation, targetRotation, 50 * Time.deltaTime);
    }
}
