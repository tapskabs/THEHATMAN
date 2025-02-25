using UnityEngine;

public class movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    public float jumpForce = 10f;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    public float rotationSpeed = 180f;
    private Quaternion targetRotation = Quaternion.identity;

    public float fallDamageThreshold = 10f;
    private bool isDead = false;

    public bool fallDamage = false;

    public enemyWalking em;
    public GameObject enemy;

    private bool isGrounded;
    private bool canJump = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        em = enemy.GetComponent<enemyWalking>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        Move(horizontalInput);

        // Check if the character can jump and is grounded
        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (gameObject.GetComponent<SpriteRenderer>() != null)
            {
                Rotate2DToZero();
            }
            else
            {
                Rotate3DToZero();
            }
        }

        transform.rotation = targetRotation;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * -0.8f, Vector2.down);

        if (hit.collider != null)
        {
            Debug.Log("Hit Collider: " + hit.collider.name);
            Debug.Log("Collider Tag: " + hit.collider.tag);

            if (hit.collider.CompareTag("ground"))
            {
                float heightAboveGround = transform.position.y - hit.point.y;

                Debug.Log("Height Above Ground: " + heightAboveGround);

                if (heightAboveGround >= 10)
                {
                    fallDamage = true;
                    Debug.Log("Fall damage on");
                }
            }
        }
    }

    private void Rotate2DToZero()
    {
        float targetRotation = 0f;
        float currentRotation = transform.eulerAngles.z;
        float rotationDifference = targetRotation - currentRotation;

        if (rotationDifference > 180f)
        {
            rotationDifference -= 360f;
        }
        else if (rotationDifference < -180f)
        {
            rotationDifference += 360f;
        }

        float rotationStep = rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, 0f, rotationDifference > 0f ? Mathf.Min(rotationStep, rotationDifference) : Mathf.Max(-rotationStep, rotationDifference));
    }

    private void Rotate3DToZero()
    {
        Quaternion targetRotation = Quaternion.identity;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


    private void Jump()
    {
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        canJump = false; // Prevent jumping until the player lands
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
            canJump = true; // Reset jump flag when landing
        }

        if (collision.gameObject.CompareTag("ground") && fallDamage)
        {
            Die();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }
    }

    private void Move(float horizontalInput)
    {
        Vector2 movement = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        rb.velocity = movement;
    }

    void Die()
    {
        GameObject o = Instantiate(em.corpse);
        o.transform.position = em.player.transform.position;
        isDead = true;

        em.player.transform.position = em.spawnPosition[em.spawnCount].transform.position;

        fallDamage = false;
        isDead = false;
    }
}
