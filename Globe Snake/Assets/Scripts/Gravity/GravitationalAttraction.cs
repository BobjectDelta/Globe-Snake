using UnityEngine;

public class GravitationalAttraction : MonoBehaviour
{
    public float gravityForce = -10;
    public void Attract(Transform bodyTransform, Rigidbody rigidbody)
    {
        Vector3 attractionDirection = (transform.position - bodyTransform.position).normalized;

        rigidbody.AddForce(attractionDirection * gravityForce);
        Quaternion targetRotation = Quaternion.FromToRotation(bodyTransform.up, -attractionDirection) * bodyTransform.rotation;
        bodyTransform.rotation = Quaternion.Slerp(bodyTransform.rotation, targetRotation, 50 * Time.deltaTime);
    }
}
