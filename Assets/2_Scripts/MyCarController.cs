using UnityEngine;

public class MyCarController : MonoBehaviour
{
    private SurfaceEffector2D surfaceEffector2D;
    public float rotationSpeed = 300f; // �ʴ� ȸ�� ����(��)
    private Rigidbody2D rb;
    private bool onGround = false;

    public float jumpForce = 7f;

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

    private float rotationInput;

    private void Update()
    {
        if (surfaceEffector2D == null) return;

        if (onGround)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                surfaceEffector2D.speed = 9f;
            }
            rotationInput = 0f; // �ٴڿ��� ȸ�� �Է� ����
        }
        else
        {
            // ���߿����� ȸ�� �Է� ���� (������ Ű��)
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
                // ���߿��� �Է��� ������ ȸ�� ����
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
