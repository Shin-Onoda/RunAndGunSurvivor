using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Serialize] float moveSpeed = 5.0f;
    float inputX;
    float inputY;
    Rigidbody rb;
    [Serialize] float shootSpeed = 6.0f;

    public GameObject bulletPrefabs;
    public GameObject gate;

    private void OnMove(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();
        inputX = inputVector.x;
        inputY = inputVector.y;
        if(inputX != 0)
        {
            rb.linearVelocity = new Vector3 (inputX * moveSpeed, rb.linearVelocity.y, 0);
        }
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Shoot()
    {
            GameObject obj = Instantiate(bulletPrefabs, gate.transform.position, Quaternion.Euler(90, 0, 0));

            //obj.transform.parent = bullets.transform;

            Rigidbody bulletRbody = obj.GetComponent<Rigidbody>();
            bulletRbody.AddForce(new Vector3(0, 0, shootSpeed), ForceMode.Impulse);
    }
}
