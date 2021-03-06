using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{

    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    public List<PointEffector2D> pointEffectors = new List<PointEffector2D>();

    public delegate void TakeDamageEvent(bool redWon);
    public static event TakeDamageEvent OnTakeDamageEvent;
    
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    private Rigidbody2D m_Rigidbody2D;
    private Vector3 m_Velocity = Vector3.zero;

    private bool isAttracted = false;
    private bool damangeActivated;

    private float initGravityScale;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public void ResetCharacter()
    {
        damangeActivated = false;
    }

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;

        initGravityScale = m_Rigidbody2D.gravityScale;

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((gameObject.layer == 10 && collision.gameObject.layer == 11) 
            || (gameObject.layer == 9 && collision.gameObject.layer == 12))
        {
            if (!damangeActivated)
            {
                damangeActivated = true;
                OnTakeDamageEvent(gameObject.layer == 10);
            }
        }
    }

    private void Update()
    {
        if (pointEffectors.Count <= 0)
        {
            return;
        }
        
        isAttracted = false;

        foreach (PointEffector2D effector in pointEffectors)
        {
            if (effector.enabled)
            {
                isAttracted = true;
            }
        }
        
        m_Rigidbody2D.gravityScale = isAttracted ? 0 : initGravityScale;
    }


    public void Move(float move, bool jump)
    {
        if (isAttracted)
        {
            return;
        }
        
        move *= runSpeed;
        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl && (move > 0.001 || move < -0.001))
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

}