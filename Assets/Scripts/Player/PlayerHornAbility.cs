using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;


public class PlayerHornAbility : MonoBehaviour
{ 
    [SerializeField] public GameObject hornObject;
    [Tooltip("This parameter determines what is the radius that the horn is reaching to when using it.")]
    [SerializeField] private float hornRadius = 5f;

    [SerializeField] private Animator playerAnimator;
    
    [SerializeField] private SimpleSonarShader_Object playerSonar;

    [SerializeField] private float hornCooldown = 0.5f;

    [SerializeField] private CinemachineImpulseSource impulseSource; 
    
    
    [Header("Sonar")]
    [SerializeField] private float sonarIntensity = 50f;

    [SerializeField] private int numOfSonars = 3;

    [SerializeField] private float sonarTimeIntervals = 0.5f;

    private float _timeCoolingDown = 0; 
    
    private bool _isHornCooldown = false;
    
    private int _animIDHorn;

   public bool canUseHorn = false;

    // Start is called before the first frame update
    void Start()
    {
        _animIDHorn = Animator.StringToHash("Horn");
    }

    // Update is called once per frame
    void Update()
    {
        if (_isHornCooldown)
        {
            if (_timeCoolingDown <= hornCooldown)
            {
                _timeCoolingDown += Time.deltaTime;
            }

            else
            {
                _isHornCooldown = false;
                Debug.Log("Horn ended cooldown!");
                _timeCoolingDown = 0f;
            }
        }
        
        
    }
    
    private void OnDrawGizmosSelected()
    {
            Color transparent = new Color(0.0f, 0f, 1f, 0.35f);
    
            Gizmos.color = transparent;
            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y, transform.position.z),
                hornRadius);
    }
    
    private IEnumerator SetSonarPulses(float timeBetweenSonars)
    {
        AudioManager.Instance.PlayHornSound(); 
        var playerPosition = transform.position;
        
        for (int i = 0; i < this.numOfSonars; i++){
                playerSonar.StartSonarRing(playerPosition, sonarIntensity);
                if (i == 1)
                {
                    GenerateHornImpulse();
                }
                
                yield return new WaitForSecondsRealtime(timeBetweenSonars);
        }
        
        if (Gamepad.current != null) Gamepad.current.ResetHaptics();
    }

  
    
    /// <summary>
    /// called when player has pressed the blow the horn button (called by third person controller)
    /// </summary>
    public void StartBlowTheHornAnimation()
    {   
        if (!canUseHorn || _isHornCooldown || GameManager.Instance.IsDeerMoving)
        {
            Debug.Log("Player can't use horn yet");
            return;
        }

        if (!GameManager.Instance.InGame())
        {
            Debug.Log("can't use horn in menus");
            return;
        }
       

        // this is for the player movement. 
        playerAnimator.SetTrigger(_animIDHorn);

    }
    
    /// <summary>
    /// called on the animation event OnUseHornEnd()
    /// </summary>
    public void BlowTheHorn()
    {
        StartCoroutine(SetSonarPulses(sonarTimeIntervals));

        _isHornCooldown = true;
        // get all game objects in the radius. 
        Collider[] colliders = Physics.OverlapSphere(transform.position, hornRadius);
        
        AudioManager.Instance.PlayHornSound();
        if (Gamepad.current != null) Gamepad.current.SetMotorSpeeds(0.25f, 0.75f);
        // iterate on every collider and envoke it's on collision enter method
        foreach (Collider collider in colliders)
        {
            collider.SendMessage("HornUsedOnObject", SendMessageOptions.DontRequireReceiver);
        }

    }


    public void SetNewSonarSource(SimpleSonarShader_Object newSonar)
    {
        playerSonar = newSonar;
    }
    
    
    public void GenerateHornImpulse()
    {
        impulseSource.GenerateImpulse();
    }
    
}
