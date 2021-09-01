using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class EnemyController : MonoBehaviour
{
    //runtime speed and shield
    [SerializeField]
    private float _speed;

    [SerializeField]
    private int _shield;

    //current EnemyStatsSO, allows us to recycle enemies
    [SerializeField]
    private EnemyStats _stats;

    //Current WeaponSO
    [SerializeField]
    private Weapon _weapon;

    //TYPES OF BEHAVIORS


    private EnemyShotController shotController;
    [SerializeField]
    private EnemyShotRotator shotControllerRotator;
    //type of current shotBehavior
    [SerializeField]
    Enemy.ShootBehavior _shootBehavior;
    //next time allowed to fire
    private float _nextFire;

    [SerializeField]
    private int consecutiveShotsLeft;
    [SerializeField]
    private float currentShotFrequencyMin;
    [SerializeField]
    private float currentShotFrequencyMax;


    //MOVEMENT
    [SerializeField]
    private IMovementOption enemyMovementOption;


    //ABILITIES AND REACTIVE MANEUVER
    [SerializeField]
    public bool abilityActive;
    public bool reactiveManeuverActive;
    public bool reactiveColliderActive;
    [SerializeField]
    Collider2D evasiveCollider;
    private IEnemyAbility enemyAbility;
    private IEnemyEvasionManeuver evasiveManeuver;



    //WHEN TO USE ABILITY, default time to wait for attempt, time decided upon, chance of it happening, minimum and maximimum additional buffer to baseline
    private float timeForNextAbilityLaunchAttempt; //actually includes the time
    [SerializeField]
    private float minTimeToAttemptAbilityLaunch;
    [SerializeField]
    private float maxTimeToAttemptAbilityLaunch;
    //WHEN TO ACTIVATE MANEUVER COLLIDER
    [SerializeField]
    private float howLongToKeepColliderOn;
    [SerializeField]
    private float timeToTurnOnManeuverCollider; //in real time
    [SerializeField]
    private float timeToTurnOffManeuverCollider; ////in real time
    [SerializeField]
    private float baselineManeuverColliderLaunchCycleDuration;
    //keeping this one flat automatic for now instead of having a min and max
    
    [SerializeField]
    private Transform enemyTargetTEST;

    //testing self finding target
    //[SerializeField]
    //private GameObject targetIndicator;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float aggression;

    [SerializeField]
    private Transform playerRef;

    //exit strategy
    [SerializeField]
    private float timerToExit; //set it on start or initalization, lol i don't feel like dividing them into classes ill just go like this haha
    //have a different script take control over lol
    private bool markedForExit; 
    //will pass all resources to exit. if somehow killed during exit sequence. 

    [SerializeField]
    private int placeInEnemyManager;
    [SerializeField]
    private int placeInGameplayWave;

    private int exitStrategyOption;

    delegate void RetreatOption();
    RetreatOption retreatStrategy;

    //sets runtime variables based on enemy type SO.
    public void InitializeEnemy(){
        Debug.Log("initalize");
        _speed = _stats.baseSpeed;
        _shield = _stats.baseShield;
        markedForExit = false;
        reactiveColliderActive = false;
        abilityActive = false;
        ChangeEvasiveColliderStatus(false);
        timeForNextAbilityLaunchAttempt = Time.time + Random.Range(minTimeToAttemptAbilityLaunch,maxTimeToAttemptAbilityLaunch);
        timeToTurnOnManeuverCollider = Time.time + baselineManeuverColliderLaunchCycleDuration;
        DeactivateAutomaticAim();

    }

    public float GetSpeed(){
        return _speed;
    }
    private void Start() {
        Debug.Log("START");
        //get reference to evasiveCollider
        evasiveCollider = transform.GetChild(0).GetComponent<Collider2D>();
        //shotController references
        shotController = transform.GetChild(1).GetComponent<EnemyShotController>();
        shotControllerRotator = transform.GetChild(1).GetComponent<EnemyShotRotator>();
        //initialize from stats granted
        InitializeEnemy();
        shotController.SetProjectilePrefab(_weapon.projectile);
        shotController.SetProjectileFireRate(_weapon.fireRate);

        //initialize fire rate
        _nextFire = Random.Range(currentShotFrequencyMin, currentShotFrequencyMax);
    }

    private void Update() {

        //remember that maneuver > ability > move, it's up to the ability if it wants to use move() or use its own thing
        
        //can shoot?
        if(Time.time > _nextFire){
            Shoot(currentShotFrequencyMin,currentShotFrequencyMax);
        }

        //have we been marked for death? control now in death sequence return and cancel everything else
        if(Time.time >= timerToExit && !abilityActive && !reactiveColliderActive){
            ChangeEvasiveColliderStatus(false);
            markedForExit = true;
        }

        if(markedForExit){
            //call to its exit option, they will take care of disabling and returning to pool when done.
            retreatStrategy();
            return;
        }
        //check maneuver collider status, can we turn it on?
        if(!reactiveColliderActive && Time.time >= timeToTurnOnManeuverCollider && !abilityActive && Time.time < timerToExit){
            
            //if not on, turn it on by setting up the coroutine
            //colliderActivationCoroutine = StartCoroutine(ManeuverColliderActivate(howLongToKeepColliderOn));
            ChangeEvasiveColliderStatus(true);

            timeToTurnOffManeuverCollider = Time.time + howLongToKeepColliderOn;

        }else if(reactiveColliderActive && Time.time >= timeToTurnOffManeuverCollider){
            //turn it off
            ChangeEvasiveColliderStatus(false);
            timeToTurnOnManeuverCollider = Time.time + baselineManeuverColliderLaunchCycleDuration;
        }
        //why i chose not to use a coroutine...idk if it's on or not and if i need to cancel it...!
        
        //since maneuver takes over...
        //will it be turned off by the maneuver? i hope so lol
        if(reactiveManeuverActive){
            //setting up the maneuver for use is done when the collision happens!
            evasiveManeuver?.UseEnemyAbility();
            return;
        }
        //is ability current active? use, else time to activate
        if(abilityActive){
            enemyAbility?.UseEnemyAbility();
            //else can we turn it on?
        }else if (!abilityActive && Time.time >= timeForNextAbilityLaunchAttempt && Time.time < timerToExit){
            //this is where we can check if the percentage makes it go through or not...if it does...activate and setup otherwise just call move

            //what if the collider was on during this time? turn it off? or leave it up to the ability?
            ChangeEvasiveColliderStatus(false);


            //setting up ability for new use
            abilityActive = true;
            Debug.Log("ACTIVATING ABILITY");
            enemyAbility?.UseEnemyAbility();
            //should reset the timers! or should the ability itself do that?
        }else{
            //check if it has a set target?
            //maybe make a validate() function at the movement level? different validation? because one would only need access to inputs while another would need a target!
            enemyMovementOption?.Move();
        }

        //should abilities also be responsible for shutting themselves down to start the cycle again???

    }


    private void OnTriggerEnter2D(Collider2D other) {
        

        if(other.tag == "Untagged"){
            return;
        }

        if(other.tag == "Player" && this.name.Contains("Enemy") ){
            //damage player
            //nullchecking in case there is no component active.
            Player player = other.transform.GetComponent<Player>();
            if(player != null){
                player.Damage();
            }
            Damage(1);   
        }

        if(other.tag == "PlayerAttack"){
            //Debug.Log("bullet COLLISION");
            Damage(1);
            IProjectile proj = other.transform.GetComponent<IProjectile>();
            if(proj != null){
                proj.DegradeProjectile(1);
            }
        }
    }

    public void Damage(int damageDealt){
        
        Debug.Log("took damage!");
        _shield -= damageDealt;
        if(_shield <= 0){
            _speed = 0;
            //Destroy(this.gameObject);
            EnemyDeath();
        }
    }

    public void Shoot(){
        Shoot(currentShotFrequencyMin,currentShotFrequencyMax);
    }

     public void Shoot(float minfreq, float maxfreq){
        //Debug.Log("CALLING SHOOT");
        switch(_shootBehavior){
            case Enemy.ShootBehavior.RANDOM:
                //_nextFire = _weapon.Shoot( transform.position,transform.position.y > _player.transform.position.y ? Quaternion.Euler(0,0,transform.eulerAngles.z -180f) : Quaternion.identity,temp);
                _nextFire = shotController.Shoot();
                _nextFire += Random.Range(minfreq, maxfreq);

                break;
            case Enemy.ShootBehavior.TIMESCONSECUTIVE:
                    if (consecutiveShotsLeft > 0){
                        //_nextFire = _weapon.Shoot(transform.position,transform.position.y > _player.transform.position.y ? Quaternion.Euler(0,0,transform.eulerAngles.z -180f) : Quaternion.identity,temp);
                        _nextFire = shotController.Shoot();
                        consecutiveShotsLeft -= 1;
                    }else{
                        //switch to other type idk
                        ChangeShootingBehavior(1);
                    }
                break;
        }
    }

    private void EnemyDeath(){
        Debug.Log("I GOT KILLED");
        //deactivate hurtbox and hitbox.
        
        //add to scoreboard
        LevelController.ChangeScoreBoard(placeInGameplayWave);
        //return to the manager
        ReturnToPool();
        //deactivate itself.
        //this.gameObject.SetActive(false);
    }

    private void EnemyRetreat(){
        //deactivate hurtbox and hitbox.
        ReturnToPool();
        //return to the manager
        //deactivate itself.
        //this.gameObject.SetActive(false);
    }

    private void ReturnToPool(){
        EnemyManager.ReturnEnemyToPool(placeInEnemyManager);
        
    }

    public void SetLifetime(float lifetime){
        //timerToExit = Time.time + lifetime;
        timerToExit = lifetime;
    }
    public void SetEnemyExitStrategy(int option){
        switch(option){
            case 0:
                //just disappear lol
                retreatStrategy = LeaveInPlace;
                break;
            default:
                break;
        }
    }

    public void LeaveInPlace(){
        Debug.Log("leaving!");
        EnemyRetreat();
    }

    public void ChangeShootingBehavior(int value){
        switch(value){
            case 1:
                _shootBehavior = Enemy.ShootBehavior.RANDOM;  
                break;
            case 2:
                _shootBehavior = Enemy.ShootBehavior.TIMESCONSECUTIVE;
                break;
        }
    }

    public void ChangeEvasiveColliderStatus(bool colliderstatus){
        evasiveCollider.enabled = colliderstatus;
    }
    public void ChangeEvasiveAbilityStatus(bool abilityStatus, Collider2D colinfo){
        reactiveManeuverActive = abilityStatus;
        ChangeEvasiveColliderStatus(false);
        evasiveManeuver?.SetCollisionInfo(colinfo);
    }  

    public void SetAbilityType(IEnemyAbility ability){
        enemyAbility = ability;
    }

    public void SetMovementType(IMovementOption ability){
        enemyMovementOption = ability;
    }

    public void SetManeuverType(IEnemyEvasionManeuver ability){
        evasiveManeuver = ability;
    }

    public void SetStats(EnemyStats stats){
        _stats = stats;
        InitializeEnemy();
    }

    public void SetWeapon(Weapon projectile){
        _weapon = projectile;
    }

    public float GetCurrentAgression(){
        return aggression;
    }

    public void SetShootingBehaviors(Enemy.ShootBehavior defaultBehavior, int consecutiveShots, float minFreq, float maxFreq){
        _shootBehavior = defaultBehavior;
        consecutiveShotsLeft = consecutiveShots;
        currentShotFrequencyMin = minFreq;
        currentShotFrequencyMax = maxFreq;
    }

    public void SetConsecutiveShots(int shots){
        SetShootingBehaviors(Enemy.ShootBehavior.TIMESCONSECUTIVE, shots, currentShotFrequencyMin, currentShotFrequencyMax);
    }

    public void SetShootControllerSettings(int atOnce, float min, float max, bool spread){
        transform.GetChild(1).GetComponent<EnemyShotController>().SetSettings(atOnce, min, max, spread);
    }

    public void SetPlaceInManager(int positionIndexInPool){
        placeInEnemyManager = positionIndexInPool;
    }

    public void SetGameplayWaveBelongingTo(int gameplaywave){
        placeInGameplayWave = gameplaywave;
    }

    public void SetAbilityBoundaries(float minAb, float maxAb){
        minTimeToAttemptAbilityLaunch = minAb;
        maxTimeToAttemptAbilityLaunch = maxAb;
    }
    public void SetManeuverBoundaries(float length, float attemptTime){
        howLongToKeepColliderOn = length;
        baselineManeuverColliderLaunchCycleDuration = attemptTime;
    }

    public void SetAbilityStatus(bool state){
        abilityActive = state;
    }

    public void RollForNextAbilityLaunch(){
        timeForNextAbilityLaunchAttempt = Time.time + Random.Range(minTimeToAttemptAbilityLaunch,maxTimeToAttemptAbilityLaunch);
    }

    public void ActivateAutomaticAim(){
        shotControllerRotator.enabled = true;
        shotControllerRotator.SetAimTarget(playerRef);
        shotControllerRotator.SetAutoAim(true);
    }

    public void DeactivateAutomaticAim(){
        shotControllerRotator.SetAutoAim(false);
        shotControllerRotator.enabled = false;
        //reset rotation
        shotControllerRotator.transform.rotation = Quaternion.identity;
    }

    //IEnumerator ManeuverColliderActivate(float seconds){
    //    //turn collider on
    //    ChangeEvasiveColliderStatus(true);
    //    yield return new WaitForSeconds(seconds);
    //    ChangeEvasiveColliderStatus(false);
    //    //turn collider off.
    //}
}
