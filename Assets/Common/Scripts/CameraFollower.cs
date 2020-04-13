using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target;
    Vector3 m_cameraOffset;

    void Awake()
    {
        m_cameraOffset = this.transform.position - target.position;
    }

    void FixedUpdate()
    {

        this.transform.position = new Vector3(
            target.position.x + m_cameraOffset.x,
            this.transform.position.y,
            target.position.z + m_cameraOffset.z
        );
    }
}
