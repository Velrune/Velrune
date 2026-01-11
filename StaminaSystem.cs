using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    [Header("Player stamina Settings")]
    [Tooltip("Change maximum of stamina")]
    [SerializeField] private float maxStamina = 100f;
    [Tooltip("When you get tired, you can't run while your stamina won't regenerate to this value")]
    [SerializeField] private float staminaDebuffCap = 15f;
    [Tooltip("Change how many sec's you have to wait, before stamina start's to regenerate")]
    [SerializeField] private float regenDelay = 2f;

    [Space(20)]
    [Header("UI Elements")]
    [SerializeField] private Image staminaBar;
    [SerializeField] private Animator staminaAnimator;

    private float minStamina = 0f;

    private float regenTimer;
    private float currentStamina;

    public bool isRunning { get; private set; }
    public bool isTired { get; private set; }
    void Awake()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (currentStamina < maxStamina && !isRunning)
            RegenerateStamina();
        else if (currentStamina > maxStamina)
        {
            staminaAnimator.SetBool("isVisible", false);
            currentStamina = maxStamina;
        }
        staminaBar.fillAmount = currentStamina / maxStamina;
    }

    public void SpendStamina()
    {
        
        staminaAnimator.SetBool("isVisible", true);
        regenTimer = 0f;
        currentStamina -= 10f * Time.deltaTime;
        if (currentStamina < minStamina)
        {
            currentStamina = minStamina;
            isTired = true;
            isRunning = false;
        }
    }

    public void RegenerateStamina()
    {
        regenTimer += Time.deltaTime;
        if (regenTimer < regenDelay) return;
        currentStamina += 5f * Time.deltaTime;
        if (currentStamina > staminaDebuffCap && isTired)
            isTired = false;
    }

    public void ChangeRunningState(bool value)
    {
        isRunning = value;
    }
}
