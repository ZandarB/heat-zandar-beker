using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player movement")]
    [SerializeField] public float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpHeatMod;
    [SerializeField] private float moveHeatMod;

    [Header("Mineables and Furnace")]

    [SerializeField] public TextMeshProUGUI coalCounterText;
    public float coalNum = 0f;
    [SerializeField] public float coalIncrement = 10f;

    [SerializeField] public TextMeshProUGUI saltCounterText;
    [SerializeField] public float saltIncrement = 10f;
    public float saltNum = 0f;

    [SerializeField] public TextMeshProUGUI pickText;
    [SerializeField] public TextMeshProUGUI drillText;

    [SerializeField] public GameObject furnace;
    [SerializeField] ProgressBarController progress;

    [SerializeField] public GameObject minePrompt;


    [Header("Misc")]
    [SerializeField] public bool hasDrill = false;
    [SerializeField] public GameObject drill;
    [SerializeField] public bool hasPick = false;
    [SerializeField] public GameObject pick;
    [SerializeField] public GameObject redWarning;
    [SerializeField] public Light flame;
    [SerializeField] GameObject drillingMinigame;
    [SerializeField] public GameObject gameUI;

    bool isInRange = false;
    public bool gameHasStarted = false;
    private Collider currentInteractable;
    private CharacterController controller;
    private Vector3 moveInput;
    private Vector3 velocity;
    public bool allowedToMove = true;
    public static PlayerController Instance;
    Animator animator;


    private Vector2 lastMoveDirection = new Vector2(0, -1); //This helps the animator decide what direction to use for the idle

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (!hasDrill)
        {
            drillText.text = "No Drill!";
            drill.SetActive(false);
        }

        if (!hasPick)
        {
            pickText.text = "No Pick!";
            pick.SetActive(false);
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        if (allowedToMove)
        {
            moveInput = value.Get<Vector2>();

        }
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed || !controller.isGrounded || !allowedToMove)
            return;

        AudioController.Instance.PlaySound("playerJump");

        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        if (currentInteractable == null)
        {
            return;
        }

        if (gameHasStarted && drillingMinigame.activeInHierarchy)
        {
            drillingMinigame.GetComponent<DrillMinigame>().OnExternalInteract();
            return;
        }

        //Coal
        if (currentInteractable.CompareTag("Coal") && hasPick && isInRange)
        {
            AudioController.Instance.PlaySound("oreMining");
            coalNum += coalIncrement;
            CoalController coalController = currentInteractable.GetComponent<CoalController>();
            coalController.totalCoal -= coalIncrement;

            if (coalController.totalCoal <= 0)
            {
                minePrompt.SetActive(false);
            }
        }

        //Salt
        else if (currentInteractable.CompareTag("Salt") && hasPick && isInRange)
        {
            AudioController.Instance.PlaySound("oreMining");
            saltNum += saltIncrement;
            SaltController saltController = currentInteractable.GetComponent<SaltController>();
            saltController.totalSalt -= saltIncrement;

            if (saltController.totalSalt <= 0)
            {
                minePrompt.SetActive(false);
            }
        }

        //Furnace
        else if (currentInteractable.CompareTag("Furnace"))
        {
            if (coalNum > 0)
            {
                AudioController.Instance.PlaySound("furnaceEnter");
                progress.currentValue += coalNum;
                coalNum = 0f;
            }
        }

        else if (currentInteractable.CompareTag("BreakableWall") && hasDrill && !gameHasStarted)
        {
            gameHasStarted = true;
            allowedToMove = false;

            DrillMinigame minigame = drillingMinigame.GetComponent<DrillMinigame>();
            minigame.AssignWall(currentInteractable.gameObject);
            gameUI.SetActive(false);
            drillingMinigame.SetActive(true);
            minePrompt.SetActive(false);
        }
    }

    public void PickUpDrill()
    {
        hasDrill = true;
        drill.SetActive(true);
        drillText.text = "Drill!";
    }

    public void PickUpPick()
    {
        hasPick = true;
        pickText.text = "Pick!";
        pick.SetActive(true);
    }

    void Update()
    {

        if (!allowedToMove)
        {
            moveInput = Vector2.zero;
            controller.Move(Vector3.zero);
            animator.SetFloat("Speed", 0);
            velocity.x = 0;
            velocity.z = 0;
        }

        // Camera-relative movement
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        Vector3 move = camForward * moveInput.y + camRight * moveInput.x;

        controller.Move(move * speed * Time.deltaTime);


        Vector2 input = moveInput;

        //Only update facing direction when player is moving
        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 moveDir = camForward * input.y + camRight * input.x;

            Vector2 animatorDirection = new Vector2(moveDir.x, moveDir.z);

            if (animatorDirection.magnitude > 0.01f)
            {
                animatorDirection.Normalize();
                lastMoveDirection = animatorDirection; //Remember last direction
            }
        }

        //Use last known direction (for idle animation if not moving)
        animator.SetFloat("MoveX", lastMoveDirection.x);
        animator.SetFloat("MoveY", lastMoveDirection.y);

        //Speed for idle/run switching
        animator.SetFloat("Speed", moveInput.sqrMagnitude);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        coalCounterText.SetText($"{coalNum.ToString()}");
        saltCounterText.SetText($"{saltNum.ToString()}");

        if (velocity.y > 0.01f)
        {
            progress.Decrease(Time.deltaTime * jumpHeatMod);
        }
        else if (moveInput.sqrMagnitude > 0.01f)
        {
            progress.Decrease(Time.deltaTime * moveHeatMod);
        }

        updateLight();
    }


    public void OnTriggerEnter(Collider other)
    {
        //Pick up coal
        if (other.gameObject.CompareTag("Coal") && hasPick == true)
        {
            currentInteractable = other;
            minePrompt.SetActive(true);
            isInRange = true;
        }

        //Pick up salt
        if (other.gameObject.CompareTag("Salt") && hasPick == true)
        {
            currentInteractable = other;
            minePrompt.SetActive(true);
            isInRange = true;
        }

        //Deposit coal in furnace, refill heat meter and upgrade meter max by how much coal is deposited
        if (other.gameObject.CompareTag("Furnace"))
        {
            currentInteractable = other;

            if (coalNum > 0)
            {
                AudioController.Instance.PlaySound("skillTreeOpen");

                int coalOverflow = progress.Increase(coalNum);

                coalNum = coalOverflow;
            }
        }

        if (other.CompareTag("BreakableWall") && hasDrill)
        {
            currentInteractable = other;
            minePrompt.SetActive(true);
            isInRange = true;
        }

        // Furnace room ambience
        if (other.gameObject.CompareTag("FurnaceRoom"))
        {
            AudioController.Instance.StopSound("cavernsArea");
            AudioController.Instance.PlaySound("furnaceRoom");

        }

        // Cavern ambience
        else if (other.gameObject.CompareTag("CavernsArea"))
        {
            AudioController.Instance.StopSound("furnaceRoom");
            AudioController.Instance.PlaySound("cavernsArea");

        }
    }

    public void OnTriggerExit(Collider other)
    {
        currentInteractable = null;

        if (other.gameObject.CompareTag("Coal"))
        {
            minePrompt.SetActive(false);
            isInRange = false;
        }

        if (other.gameObject.CompareTag("Salt"))
        {
            minePrompt.SetActive(false);
            isInRange = false;

        }
        if (other.gameObject.CompareTag("BreakableWall") && hasDrill)
        {
            minePrompt.SetActive(false);
            isInRange = false;
        }
    }

    void Die()
    {
        //PlayerController.Instance.gameObject.SetActive(false);
    }

    //Updates the light based on heat level. If gets to 0, dies.
    void updateLight()
    {
        flame.intensity = (progress.currentValue / progress.maxValue) * 50;
        flame.range = (progress.currentValue / progress.maxValue) * 50;


        if (flame.intensity <= 0)
        {
            Die();
        }
    }

    public ProgressBarController getProgress()
    {
        return progress;
    }

}
