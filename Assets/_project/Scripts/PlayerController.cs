using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        public float movingSpeed = 5f;
        public float jumpForce = 8f;
        public ParticleSystem landingEffect;

        private bool facingRight = true; // should default to true
        [HideInInspector]
        public bool deathState = false;

        private bool isGrounded;
        private bool wasInAir = false;
        public Transform groundCheck;

        public KeyCode leftKey;
        public KeyCode rightKey;
        public KeyCode jumpKey;

        private Rigidbody2D rb;
        private Animator animator;
        private GameManager gameManager;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            // Safer GameManager grab
            if (GameManager.instance != null)
            {
                gameManager = GameManager.instance;
            }
            else
            {
                Debug.LogWarning("GameManager not found in scene!");
            }
        }

        private void FixedUpdate()
        {
            CheckGround();

            // Landing effect
            if (isGrounded && wasInAir)
            {
                PlayLandingEffect();
            }

            wasInAir = !isGrounded;
        }

        void Update()
        {
            float moveInput = 0f;

            if (Input.GetKey(leftKey))
                moveInput = -1f;
            if (Input.GetKey(rightKey))
                moveInput = 1f;

            // Movement
            rb.linearVelocity = new Vector2(moveInput * movingSpeed, rb.linearVelocity.y);

            // Animations
            if (moveInput != 0)
            {
                animator.SetInteger("playerState", 1); // Run animation
                if (moveInput > 0 && !facingRight)
                    Flip();
                else if (moveInput < 0 && facingRight)
                    Flip();
            }
            else if (isGrounded)
            {
                animator.SetInteger("playerState", 0); // Idle animation
            }

            // Jumping
            if (Input.GetKeyDown(jumpKey) && isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                animator.SetInteger("playerState", 2); // Jump animation
                wasInAir = true;
            }

            // Mid-air animation
            if (!isGrounded)
            {
                animator.SetInteger("playerState", 2); // Jump animation
            }
        }

        private void Flip()
        {
            facingRight = !facingRight;
            Vector3 scaler = transform.localScale;
            scaler.x *= -1;
            transform.localScale = scaler;
        }

        private void CheckGround()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f);
            isGrounded = false;
            foreach (var collider in colliders)
            {
                if (collider.gameObject != gameObject)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        private void PlayLandingEffect()
        {
            if (landingEffect != null)
            {
                Vector3 effectPosition = transform.position;
                effectPosition.y = groundCheck.position.y;
                landingEffect.transform.position = effectPosition;
                landingEffect.Play();
            }
        }

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (other.CompareTag("Coin"))
        //     {
        //         if (gameManager != null)
        //         {
        //             gameManager.coinsCounter += 1;
        //         }
        //         Destroy(other.gameObject);
        //     }
        //     else if (other.CompareTag("Fuel"))
        //     {
        //         if (gameManager != null)
        //         {
        //             gameManager.AddFuel(1);
        //         }
        //         Destroy(other.gameObject);
        //     }
        // }
    }
}
