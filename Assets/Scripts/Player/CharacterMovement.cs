using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController cc;
    private Rigidbody rb;
    private Vector3 velocity;
    private Vector3 input;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField]private float gravity = -15f;
    [SerializeField]private float jumpHeight;
    [SerializeField] private float speed;
    [SerializeField] private float rotSpeedSmooth;
    [SerializeField] private GameObject body;
    private float turnSmoothVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        CharacterMove();
        ////Mecánica salto
        if ((cc.isGrounded) && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        if ((cc.isGrounded && velocity.y < 0f))
        {
            velocity.y -= 2f;
        }
    }

    //private void Character3rdPersonMove()
    //{
    //    input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
    //    Vector3 direction = input.normalized;
    //    if (direction.magnitude >= 0.1f)
    //    {
    //        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    //        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotSpeedSmooth);
    //        transform.rotation = Quaternion.Euler(0f, angle, 0f);

    //        if (Input.GetKey(KeyCode.LeftShift))
    //        {
    //            cc.Move(direction * speed * Time.deltaTime * 1.5f);
    //        }
    //        else cc.Move(direction * speed * Time.deltaTime);

    //    }


    private void CharacterMove()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Vector3 direction = input.normalized;
        if (direction.magnitude >= 0.1f)
        {
        //    float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        //    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotSpeedSmooth);
        //    transform.rotation = Quaternion.Euler(0f, angle, 0f);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                cc.Move(direction * speed * Time.deltaTime * 1.5f);
            }
            else cc.Move(direction * speed * Time.deltaTime);

        }

    }
}
