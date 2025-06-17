using UnityEngine;
using System.Collections;

public class MyCarController : MonoBehaviour
{
    private SurfaceEffector2D surfaceEffector2D;
    public float rotationSpeed = 300f; // 초당 회전 각도(도)
    private Rigidbody2D rb;
    private bool onGround = false;

    public float jumpForce = 7f;
    public float defaultSpeed = 7f; // 기본 속도

    private Coroutine boosterCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = 10f;
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<SurfaceEffector2D>(out var effector))
        {
            onGround = true;
            surfaceEffector2D = effector;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<SurfaceEffector2D>(out var effector))
        {
            onGround = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("booster") && surfaceEffector2D != null)
        {
            if (boosterCoroutine != null)
                StopCoroutine(boosterCoroutine);
            boosterCoroutine = StartCoroutine(BoosterRoutine());
        }
    }

    private IEnumerator BoosterRoutine()
    {
        float originalSpeed = surfaceEffector2D.speed;
        surfaceEffector2D.speed = 20f;
        yield return new WaitForSeconds(3f);
        surfaceEffector2D.speed = originalSpeed;
    }

    private float rotationInput;

    private void Update()
    {
        if (surfaceEffector2D == null) return;

        if (onGround)
        {
            // 오른쪽 키를 누르면 15, 떼면 defaultSpeed로 복구
            if (Input.GetKey(KeyCode.RightArrow))
            {
                surfaceEffector2D.speed = 12f;
            }
            else
            {
                surfaceEffector2D.speed = defaultSpeed;
            }
            rotationInput = 0f; // 바닥에선 회전 입력 없음
        }
        else
        {
            // 공중에서만 회전 입력 저장 (오른쪽 키만)
            if (Input.GetKey(KeyCode.RightArrow))
                rotationInput = 1f;
            else
                rotationInput = 0f;
        }

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
                // 공중에서 입력이 없으면 회전 멈춤
                rb.angularVelocity = 0f;
            }
        }
    }

    private void Jump()
    {
        onGround = false;

        if (rb == null) return;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
