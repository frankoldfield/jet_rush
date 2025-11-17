using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float heightChangeSpeed = 5f;

    [Header("Lateral Movement")]
    [SerializeField] private float lateralSpeed = 5f;

    [Header("Height Limits")]
    [SerializeField] private float minHeight = 2f;
    [SerializeField] private float maxHeight = 8f;

    [SerializeField] private float maxRollAngle = 45f;
    private float currentRoll = 0f;



    private Rigidbody rb;
    private float targetHeight;
    private float currentRotationInput;
    private bool isGameOver = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetHeight = transform.position.y;

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
    }

    void Update()
    {
        if (isGameOver) return;
        HandleInput();
        HandleMovement();
        HandleRotation();
    }

    private void HandleInput()
    {
        currentRotationInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow))
            currentRotationInput = -1f;
        else if (Input.GetKey(KeyCode.RightArrow))
            currentRotationInput = 1f;

        if (Input.GetKeyDown(KeyCode.UpArrow))
            targetHeight = maxHeight;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            targetHeight = minHeight;
    }

    private void HandleMovement()
    {
        Vector3 pos = rb.position;

        // 1. Movimiento hacia adelante
        Vector3 forwardMove = transform.forward * forwardSpeed * Time.fixedDeltaTime;

        // 2. Movimiento vertical
        float newY = Mathf.Lerp(pos.y, targetHeight, heightChangeSpeed * Time.fixedDeltaTime);
        Vector3 verticalMove = new Vector3(0, newY - pos.y, 0);

        // 3. Movimiento lateral según el ángulo de rotación en Z
        Vector3 pureRight = transform.right;
        pureRight.y = 0;                        // elimina inclinación
        pureRight.Normalize();                  // normaliza dirección

        float rollFactor = Mathf.Sin(currentRoll * Mathf.Deg2Rad); //Utilizamos la rotación del frame anterior para poder actualizar el movimiento antes que la rotación

        Vector3 lateralMove = -pureRight * rollFactor * lateralSpeed * Time.fixedDeltaTime;


        // 4. Movimiento final combinado
        rb.MovePosition(pos + forwardMove + verticalMove + lateralMove);
    }

    private void HandleRotation()
    {
        float rotationAmount = -currentRotationInput * rotationSpeed * Time.fixedDeltaTime;

        
        if (Mathf.Abs(currentRotationInput) > 0.01f) // Si hay input, aplicamos rotación normal, limitando la inclinación de la nave
        {
            currentRoll += rotationAmount;
            currentRoll = Mathf.Clamp(currentRoll, -maxRollAngle, maxRollAngle);
        }
        else // Si no hay input, volvemos a la rotación neutral
        {
            float returnSpeed = rotationSpeed * 0.5f;
            currentRoll = Mathf.MoveTowards(currentRoll, 0f, returnSpeed * Time.fixedDeltaTime);
        }

        // Aplicamos la rotación final
        rb.MoveRotation(Quaternion.Euler(0f, 0f, currentRoll));
    }

    public void SetGameOver(bool gameOver)
    {
        isGameOver = gameOver;
        if (gameOver)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Building"))
        {
            // Colisión con edificio - Game Over
            Animator animator = this.GetComponent<Animator>();
            animator.SetBool("destroyed", true);
            GameManager.Instance.GameOver();
        }
        else if (collision.gameObject.CompareTag("Collectible"))
        {
            // Recoger objeto
            GameManager.Instance.CollectItem();
            Destroy(collision.gameObject);
        }
    }
}
