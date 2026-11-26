using UnityEngine;

public class AudioSafetyFix : MonoBehaviour
{
    void LateUpdate()
    {

        if (float.IsNaN(transform.position.x) || float.IsNaN(transform.position.y) || float.IsNaN(transform.position.z))
        {
            transform.position = Vector3.zero;
        }
    }
}