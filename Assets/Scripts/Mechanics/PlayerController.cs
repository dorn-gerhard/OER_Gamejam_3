using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using static Function;

/// <summary>
/// This is the main class used to implement control of the player.
/// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Function lineFunction;
    public GameObject confidences;
    public AudioClip jumpAudio;
    public AudioClip respawnAudio;
    public AudioClip ouchAudio;

    /// <summary>
    /// Max horizontal speed of the player.
    /// </summary>
    public float maxSpeed = 7;
    /// <summary>
    /// Initial jump velocity at the start of a jump.
    /// </summary>
    public float jumpTakeOffSpeed = 7;

    //public JumpState jumpState = JumpState.Grounded;
    private bool stopJump;
    /*internal new*/ public Collider2D collider2d;
    /*internal new*/ public AudioSource audioSource;
    public Health health;
    public bool controlEnabled = true;

    bool jump;
    Vector2 move;
    SpriteRenderer spriteRenderer;
    //internal Animator animator;
    readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

    public float moveSpeed = 5f;
    public float runMultiplier = 1.5f;
    float currentMoveSpeed;
    public static PlayerController current;

    public Bounds Bounds => collider2d.bounds;

    public float currentConfidence = 10f;
    public float startConfidence = 10f;

    public float currentEnergy = 100f;
    public float startEnergy = 100f;

    public float energyPerShot = 20f;

    public Image confidenceImage;
    public TMP_Text confidenceProcentage;

    public GameObject WinLossScreen;
    public TMP_Text WinLossScreenText;

    public GameObject DeathFX;

    public int weaponsCompleted = -1;
    public int numberOfWeapons = 3;

    public float movementUnfreezeDelay = 0.5f;

    [Header("Spawners")]
    [SerializeField] GameObject LinearEnemiesSpawnwer;
    [SerializeField] GameObject QuadraticEnemiesSpawnwer;
    [SerializeField] GameObject SinEnemiesSpawnwer;


    public LineLengthData GetCurrentWeaponLineLength()
    {
        return GetCurrentFunctionScriptableObject().lineLengthData;
    }

    public FunctionScriptableObject GetCurrentFunctionScriptableObject()
    {
        return currentConfidenceUI.functionScriptableObject;
    }

    public Confidence GetCurrentConfidenceUI()
    {
        return currentConfidenceUI;
    }

    public void ClearAllCalibrationTargetPositionCaches()
    {
        foreach (Confidence confidence in confidences.GetComponentsInChildren<Confidence>(true))
        {
            confidence.SetCalibrationTargetPositionCache(Vector3.zero);
        }
    }

    public void ResetLowEnergyWarningAnimators ()
    {
        foreach (Transform child in confidences.transform)
        {
            var energyWarningAnimator = child.gameObject.GetComponent<Confidence>().energyWarningAnimator;

            energyWarningAnimator.Rebind();
            energyWarningAnimator.Update(0.0f);
        }
    }

    public LvlUpUI lvlUpUI;

    public FixedJoystick fixedJoystick;

    void Awake()
    {
        current = this;

        currentConfidence = startConfidence;
        currentEnergy = startEnergy;

        currentConfidenceUI = confidences.transform.GetChild(0).GetComponent<Confidence>();
        currentConfidenceUI.UpdateConfidenceUI();

        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();
        collider2d = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
    }

    private void Start()
    {
        foreach (Transform child in confidences.transform)
        {
            child.gameObject.SetActive(false);
        }

        PopUpController.current.OpenPopUp("hallo");

        //OnUnlockNextWeapon(true);
        //OnSwitchWeapon(0);
    }

    bool isMovementFrozen = false;
    public void Freeze()
    {
        isMovementFrozen = true;
        GetComponentInChildren<Function>(true).gameObject.SetActive(true);
    }

    public void Unfreeze()
    {
        isMovementFrozen = false;
        GetComponentInChildren<Function>(true).gameObject.SetActive(false);
    }

    void Update()
    {
        currentMoveSpeed = moveSpeed;
        //if (Input.GetKey(KeyCode.LeftShift))
        //{
        //    currentMoveSpeed = moveSpeed * runMultiplier;
        //}



        //if (Input.GetKey(KeyCode.Space))
        //{
        //    if (youWin)
        //    {
        //        Time.timeScale = 1f;
        //        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //    }
        //    else
        //    {
        //        lineFunction.Shoot();
        //    }
        //}


        HandleMovement();
    }

    void HandleMovement()
    {
        if (isMovementFrozen) return;

        Vector3 moveDirection = Vector3.zero;

        // Check input and set movement direction
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector3.up; // Move up
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right; // Move right
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.down; // Move down
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left; // Move left
        }
        
        // Normalize the movement direction to ensure consistent speed in all directions
        moveDirection.Normalize();

        if (fixedJoystick is not null & fixedJoystick.Direction.normalized.magnitude > 0.2)
            moveDirection = fixedJoystick.Direction.normalized;

        // Move the spaceship
        transform.Translate(moveDirection * currentMoveSpeed * Time.deltaTime);
    }

    public void ChangeConfidence(float Amount, FunctionType type)
    {
        float prevConfidenceAmount = currentConfidence;
        Confidence prevConfidence = currentConfidenceUI;
        int prevWeaponIndex = currentConfidenceUI.transform.GetSiblingIndex();

        foreach (Transform child in confidences.transform)
        {
            var confidence = child.gameObject.GetComponent<Confidence>();
            if (confidence.functionScriptableObject.type == type)
            {
                currentConfidence = confidence.confidence;
                currentConfidenceUI = confidence;
            }
        }

        // Do not set back to the old if change unlocks new weapon
        if (ChangeConfidence(Amount) == true) return;
        
        currentConfidence = prevConfidenceAmount;
        currentConfidenceUI = prevConfidence;
    }

    // Returns true if new weapon was unlocked and switched to
    public bool ChangeConfidence(float Amount)
    {
        currentConfidence = Mathf.Clamp(currentConfidence + Amount, 0, 100);
        currentConfidenceUI.confidence = currentConfidence;
        currentConfidenceUI.UpdateConfidenceUI();

        if (currentConfidence >= 100)
        {
            bool allConfidencesFull = true;
            foreach (Confidence confidence in confidences.GetComponentsInChildren<Confidence>(true))
            {
                if (confidence.confidence < 90)
                {
                    allConfidencesFull = false;
                    break;
                }
            }

            if (allConfidencesFull)
            {
                Win();
            }
            else
            {
                return OnUnlockNextWeapon();
            }
            // TODO get update
            // change weapon on click?
        }

        return false;
    }

    public void ChangeEnergy(float Amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + Amount, 0, 100);
        currentConfidenceUI.energy = currentEnergy;
        currentConfidenceUI.UpdateConfidenceUI();
    }

    public bool OnUnlockNextWeapon(bool first = false)
    {
        if (weaponsCompleted >= numberOfWeapons)
        {
            LinearEnemiesSpawnwer.SetActive(true);
            QuadraticEnemiesSpawnwer.SetActive(true);
            SinEnemiesSpawnwer.SetActive(true);

            return false;
        }

        // The next weapon is already unlocked
        if (currentConfidenceUI.transform.GetSiblingIndex() < weaponsCompleted)
        {
            return false;
        }

        // The next weapon is already unlocked
        if (currentConfidenceUI.transform.GetSiblingIndex() + 1 >= confidences.transform.childCount)
        {            
            return false;
        }

        Confidence newWeapon = confidences.transform.GetChild(currentConfidenceUI.transform.GetSiblingIndex() + 1).gameObject.GetComponent<Confidence>();

        if (first)
        {
            newWeapon = confidences.transform.GetChild(0).GetComponent<Confidence>();
        }

        Time.timeScale = 0f;

        weaponsCompleted++;

        PlaySound("levelup");

        lvlUpUI.gameObject.SetActive(true);

        lvlUpUI.Setup(newWeapon);

        OnSwitchWeapon(newWeapon);

        currentConfidenceUI.energy = 100;
        currentConfidenceUI.UpdateConfidenceUI();

        switch (GetCurrentFunctionScriptableObject().type)
        {
            case FunctionType.linear:
                LinearEnemiesSpawnwer.SetActive(true);
                break;

            case FunctionType.quadratic:
                //LinearEnemiesSpawnwer.SetActive(false);
                QuadraticEnemiesSpawnwer.SetActive(true);
                break;

            case FunctionType.sin:
                //QuadraticEnemiesSpawnwer.SetActive(false);
                SinEnemiesSpawnwer.SetActive(true);
                break;
        }

        return true;
    }

    public void OnSwitchWeapon(Confidence newWeapon)
    {
        //if (weaponsCompleted >= numberOfWeapons) return;

        // switch current confidence
        lineFunction.currentWeaponType = newWeapon.functionScriptableObject.type;

        foreach (Transform child in confidences.transform)
        {
            child.gameObject.GetComponent<CanvasGroup>().alpha = 0.3f;
            child.gameObject.GetComponent<Confidence>().energyBar.gameObject.SetActive(false);            
            child.gameObject.GetComponent<Confidence>().switchWeaponIndicator.SetActive(true);
        }

        currentConfidenceUI = newWeapon;
        currentConfidenceUI.gameObject.SetActive(true);
        currentConfidenceUI.energyBar.SetActive(true);
        currentConfidenceUI.switchWeaponIndicator.SetActive(false);
        currentConfidenceUI.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        currentConfidence = currentConfidenceUI.confidence;
        currentEnergy = currentConfidenceUI.energy;

        if (CalibrationController.current.IsCalibrating)
        {
            CalibrationController.current.StartCalibration();
        }
    }

    bool gameOver = false;
    bool youWin = false;
    private Confidence currentConfidenceUI;

    private void GameOver()
    {
        if (gameOver) return;
        gameOver = true;
        enabled = false;
        
        StartCoroutine(OpenGameOverScreen());
    }

    IEnumerator OpenGameOverScreen()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
        WinLossScreen.gameObject.SetActive(true);
        WinLossScreenText.text = "Game Over!";
        yield return null;
    }
    private void Win()
    {
        if (youWin) return;
        youWin = true;
        Time.timeScale = 0f;
        WinLossScreen.gameObject.SetActive(true);
    }

    public void Death()
    {
        print("player death");
        if (health.IsAlive)
        {
            health.Die();
            model.virtualCamera.m_Follow = null;
            model.virtualCamera.m_LookAt = null;
            // player.collider.enabled = false;
            controlEnabled = false;

            if (audioSource && ouchAudio)
                audioSource.PlayOneShot(ouchAudio);

            GameOver();

            GetComponent<SpriteRenderer>().enabled = false;
            Instantiate(DeathFX, transform.position, Quaternion.identity);

            //animator.SetTrigger("hurt");
            //animator.SetBool("dead", true);
            //Simulation.Schedule<PlayerSpawn>(2);
        }
    }

    public void OnTokenCollision(float amount)
    {
        PlaySound("collectable");
        ChangeConfidence(amount);
    }

    public void UnfreezeMovementAfterDelay()
    {
        PlaySound("sfx_lazer1");
        PlaySound("laserpulse");

        StartCoroutine(UnfreezeMovementAfterDelayCoroutine());
    }

    IEnumerator UnfreezeMovementAfterDelayCoroutine()
    {
        isMovementFrozen = true;

        yield return new WaitForSeconds(movementUnfreezeDelay);

        Unfreeze();

        yield return null;
    }

    [Serializable]
    public struct SoundFX
    {
        public string clipName;
        public AudioClip audioClip;
    }

    public List<SoundFX> SoundFXes = new List<SoundFX>();

    public void PlaySound(string clipName)
    {
        foreach (SoundFX soundFX in SoundFXes)
        {
            if (soundFX.clipName == clipName)
            {
                audioSource.PlayOneShot(soundFX.audioClip);
            }
        }
    }
}
