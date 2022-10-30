using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public bool isGrounded;
    public bool isHoldingWeapon;
    float x;
    float z;
    Vector3 move;
    IKSetup IKSetup_R;
    IKSetup IKSetup_L;
    GameObject heldWeaponGameObject;
    Weapon heldWeapon;

    [SerializeField] Transform groundCheck;
    [SerializeField] Transform lookAtPoint;
    [SerializeField] LayerMask killMask;
    [SerializeField] LayerMask groundMask;
    [SerializeField] GameObject IK_R;
    [SerializeField] GameObject IK_L;
    [SerializeField] float lookMinDist;
    [Range(1, 100)]
    [SerializeField] int jumpHeight;
    [Range(1, 100)]
    [SerializeField] int speed;

    void OnEnable()
    {
        MinigunBehavior.OnMinigunGet += ItemSet;
        RifleBehavior.OnRifleGet += ItemSet;
        SwordBehavior.OnSwordGet += ItemSet;
    }

    void OnDisable()
    {
        MinigunBehavior.OnMinigunGet -= ItemSet;
        RifleBehavior.OnRifleGet -= ItemSet;
        SwordBehavior.OnSwordGet -= ItemSet;
    }

    void Start()
    {
        IKSetup_L = IK_L.GetComponent<IKSetup>();
        IKSetup_R = IK_R.GetComponent<IKSetup>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        setPos();
        InputManage();
        limitSpeed();
    }

    private void setPos()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        if(Physics.CheckSphere(groundCheck.position, 2, groundMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        if(Physics.CheckSphere(groundCheck.position, 2, killMask))
        {
            Destroy(this.gameObject);
        }
        move = new Vector3(x, 0, z).normalized;
        rb.AddForce(move * speed, ForceMode.Force);
    }

    void FixedUpdate()
    {
        SetRotation();
    }

    void InputManage()
    {
        //temp
        if(Input.GetKeyDown("space") && isGrounded)
        {
            jump();
        }
    }

    void limitSpeed()
    {
        Vector3 baseSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if(baseSpeed.sqrMagnitude >= speed * speed)
        {
            Vector3 limit = baseSpeed.normalized * speed;
            rb.velocity = new Vector3(limit.x, rb.velocity.y, limit.z);
        }
    }

    void SetRotation()
    {   
        if((groundCheck.position - lookAtPoint.position).sqrMagnitude >= lookMinDist * lookMinDist)
        {
            transform.LookAt(lookAtPoint);
        }
    }

    void ItemSet(GameObject g)
    {
        if(!isHoldingWeapon)
        {
            isHoldingWeapon = true;
            heldWeaponGameObject = Instantiate(g) as GameObject;
            heldWeapon = heldWeaponGameObject.GetComponent<Weapon>();
            heldWeaponGameObject.GetComponent<Collider>().isTrigger = false;
            heldWeaponGameObject.GetComponent<Rigidbody>().isKinematic = true;
            heldWeaponGameObject.transform.parent = this.gameObject.transform;
            heldWeaponGameObject.transform.localRotation = Quaternion.Euler(0, 90, 0);
            heldWeaponGameObject.transform.localPosition = heldWeapon.getObjPosition(heldWeapon.index);
            IKSetup_R.target = heldWeaponGameObject.transform.Find("Target_R");
            IKSetup_L.target = heldWeaponGameObject.transform.Find("Target_L");
        }
    }

    void jump()
    {
        rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
    }
}