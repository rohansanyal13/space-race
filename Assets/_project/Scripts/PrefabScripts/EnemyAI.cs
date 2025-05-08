using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class EnemyAI : MonoBehaviour
    {
        public float moveSpeed = 1f;
        public float minFlipTime = 1f;
        public float maxFlipTime = 10f;
        public float stuckTimeThreshold = 0.5f;
        public float jumpForce = 5f;
        public float speedBoostMultiplier = 1.5f;
        public float speedBoostChance = 0.2f;
        public float randomJumpChance = 0.1f;

        private Rigidbody2D rb;
        private float flipTimer;

        private Vector2 lastPosition;
        private float stuckTimer = 0f;
        private bool attemptedJump = false;

        private System.Random rng;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rng = new System.Random(System.Guid.NewGuid().GetHashCode());
            SetRandomFlipTimer();
            lastPosition = rb.position;
        }

        void FixedUpdate()
        {
            // Move enemy
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);

            // Countdown for random flip
            flipTimer -= Time.fixedDeltaTime;
            if (flipTimer <= 0f)
            {
                TryRandomSpeedBoost();
                TryRandomJump();
                SetRandomFlipTimer();
            }

            // Check if stuck
            float horizontalMovement = Mathf.Abs(rb.position.x - lastPosition.x);
            if (horizontalMovement < 0.01f)
            {
                stuckTimer += Time.fixedDeltaTime;

                if (stuckTimer >= stuckTimeThreshold && !attemptedJump)
                {
                    Jump();
                    attemptedJump = true;
                    stuckTimer = 0f;
                }
                else if (stuckTimer >= stuckTimeThreshold && attemptedJump)
                {
                    Flip();
                    attemptedJump = false;
                    stuckTimer = 0f;
                    SetRandomFlipTimer();
                }
            }
            else
            {
                stuckTimer = 0f;
                attemptedJump = false;
            }

            lastPosition = rb.position;
        }

        private void Flip()
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            moveSpeed *= -1;
        }

        private void Jump()
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        private void TryRandomSpeedBoost()
        {
            if (rng.NextDouble() < speedBoostChance)
            {
                float direction = Mathf.Sign(moveSpeed);
                moveSpeed = Random.Range(1f, 3f) * direction * speedBoostMultiplier;
            }
        }

        private void TryRandomJump()
        {
            if (rng.NextDouble() < randomJumpChance)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        private void SetRandomFlipTimer()
        {
            flipTimer = Random.Range(minFlipTime, maxFlipTime);
        }
    }
}
