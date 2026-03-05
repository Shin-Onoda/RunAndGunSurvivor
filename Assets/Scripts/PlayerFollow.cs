using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    Vector3 diff;

    public GameObject target;
    public float followSpeed = 5;

    void Start()
    {
        diff = target.transform.position - transform.position;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            target.transform.position - diff,
            Time.deltaTime * followSpeed
        );
    }
}
