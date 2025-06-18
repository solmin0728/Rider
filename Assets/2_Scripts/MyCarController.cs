using UnityEngine;
using System.Collections;

public class MyCarController : MonoBehaviour
{
    private SurfaceEffector2D surfaceEffector2D;
    public float rotationSpeed = 300f;
    private Rigidbody2D rb;

    public float jumpForce = 7f;
    public float defaultSpeed = 7f;
    public float boostSpeed = 20f;
    public float accelSpeed = 12f;

    private bool onGround = false;
    private float rotationInput;

    // 파티클 시스템 참조
    private ParticleSystem groundParticle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 10f;
        groundParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<SurfaceEffector2D>(out var effector))
        {
            onGround = true;
            surfaceEffector2D = effector;
            // 바닥에 닿으면 파티클 재생
            if (groundParticle != null && !groundParticle.isPlaying)
                groundParticle.Play();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<SurfaceEffector2D>(out var effector))
        {
            onGround = false;
            // 공중에 뜨면 파티클 정지
            if (groundParticle != null && groundParticle.isPlaying)
                groundParticle.Stop();
        }
    }

    private void Update()
    {
        if (surfaceEffector2D == null) return;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            surfaceEffector2D.speed = boostSpeed;
        }
        else if (onGround && Input.GetKey(KeyCode.LeftArrow))
        {
            surfaceEffector2D.speed = 3f;
        }
        else if (onGround && Input.GetKey(KeyCode.RightArrow))
        {
            surfaceEffector2D.speed = accelSpeed;
        }
        else if (onGround)
        {
            surfaceEffector2D.speed = defaultSpeed;
        }

        if (!onGround && Input.GetKey(KeyCode.RightArrow))
            rotationInput = 1f;
        else
            rotationInput = 0f;

        UIManager.Instance.UpdateSurfaceText($"Surface Speed : {surfaceEffector2D.speed:F1}");

        if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            Jump();
        }
        UIManager.Instance.UpdateCarSpeedText($"Car Speed : {rb.linearVelocity.magnitude:F1}");
    }

    private void FixedUpdate()
    {
        if (!onGround)
        {
            if (rotationInput != 0f)
            {
                rb.MoveRotation(rb.rotation + rotationInput * rotationSpeed * Time.fixedDeltaTime);
            }
            else
            {
                rb.angularVelocity = 0f;
            }
        }
    }

    private void Jump()
    {
        onGround = false;
        if (rb == null) return;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        // 점프 시 파티클 정지
        if (groundParticle != null && groundParticle.isPlaying)
            groundParticle.Stop();
    }
}
