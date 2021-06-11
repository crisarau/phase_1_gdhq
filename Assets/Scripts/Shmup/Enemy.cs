using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.5f;

    [SerializeField]
    private int _shield = 3;

    [SerializeField]
    private EnemyStats _stats;
    
    [SerializeField]
    private Weapon _weapon;

    enum ShootBehavior{
        RANDOM = 1, TIMESCONSECUTIVE = 2
    };

    [SerializeField]
    ShootBehavior _shootBehavior;
    private float _nextFire;
    private float _nextFrequencyFire;

    [SerializeField]
    private int consecutiveShotsLeft;
    [SerializeField]
    private float currentShotFrequencyMin;
    [SerializeField]
    private float currentShotFrequencyMax;

    [SerializeField]
    private IMovementOption enemyMovementOption;
    private List<EnemyMovementInputs> movementInputs;
    [SerializeField]
    private IEnemyAbility enemyAbility;
    [SerializeField]
    public bool abilityActive;

    [SerializeField]
    private float abilityUsagePercentage;

    [SerializeField]
    private Transform enemyTargetTEST;

    Player _player;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        InitializeEnemy();

        

        _nextFire = Random.Range(currentShotFrequencyMin, currentShotFrequencyMax);

        //using movement sequence and enemy ability...
        //TRYING THE PREMADE INPUT SEQUENCE VERSION
        //creating that sequence..WORKS
        movementInputs = new List<EnemyMovementInputs>();
        movementInputs.Add(new EnemyMovementInputs(0,-1,1));
        //movementInputs.Add(new EnemyMovementInputs(1,-1,10));
        enemyMovementOption = new EM_InputSequence(this, true, movementInputs,_speed);

        //TRYING THE RANDOM POINT MOVE
        //FINDING NEXT POINT AWARE OF PLAYER POSITION.
        //enemyMovementOption = new EM_GoToPosition(this, enemyTargetTEST, true, _speed);


        //TESTING RAM ABILITY
        enemyAbility = new EA_Ram(this, 3f, _speed, 15f, 5f, _player.transform,  enemyTargetTEST);

    }
    
    //sets runtime variables based on enemy type SO.
    public void InitializeEnemy(){
        _speed = _stats.baseSpeed;
        _shield = _stats.baseShield;
    }
    // Update is called once per frame
    void Update()
    {
        //always moving downwards
        //transform.Translate( Vector3.up * -1.0f * _speed * Time.deltaTime);
        ////teleports always to top screen
        //if(transform.position.y < -4.5f) {
        //    transform.position = new Vector3(Random.Range(-5.5f, 5.5f),6.5f, 0);
        //}
        
        //make its move based on current Enemy movement behavior
        if(!abilityActive){
            enemyMovementOption?.Move();
        }

        //can shoot?
        if(Time.time > _nextFire){
            //then shoot
            Shoot(currentShotFrequencyMin, currentShotFrequencyMax);
        }
        //can use ability?// whether we can shoot or not is up to the ability!
        if(!abilityActive && abilityUsagePercentage >= 1.0f){
            abilityActive = true;
            abilityUsagePercentage = 0.0f;
            enemyAbility?.UseEnemyAbility();
        }
        if(abilityActive){
            enemyAbility?.UseEnemyAbility();
            //abilityUsagePercentage += (0.005f * Time.deltaTime);
        }


    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            //damage player
            //nullchecking in case there is no component active.
            Player player = other.transform.GetComponent<Player>();
            if(player != null){
                player.Damage();
            }
            Damage(1);   
        }

        if(other.tag == "PlayerAttack"){
            Damage(1);
            IProjectile proj = other.transform.GetComponent<IProjectile>();
            if(proj != null){
                proj.DegradeProjectile(1);
            }
        }
    }

    public void Damage(int damageDealt){
        _shield -= damageDealt;
        if(_shield == 0){
            _speed = 0;
            Destroy(this.gameObject);
        }
    }

    public void Shoot(float minfreq, float maxfreq){
        //chance to switch shotbehavior
        Shot temp = new Shot();
        temp.Chance();
                        
        switch(_shootBehavior){
            case ShootBehavior.RANDOM:
                _nextFire = _weapon.Shoot( transform.position,transform.position.y > _player.transform.position.y ? Quaternion.Euler(0,0,transform.eulerAngles.z -180f) : Quaternion.identity,temp);
                _nextFire += Random.Range(minfreq, maxfreq);

                break;
            case ShootBehavior.TIMESCONSECUTIVE:
                    if (consecutiveShotsLeft > 0){
                        _nextFire = _weapon.Shoot(transform.position,transform.position.y > _player.transform.position.y ? Quaternion.Euler(0,0,transform.eulerAngles.z -180f) : Quaternion.identity,temp);
                        consecutiveShotsLeft -= 1;
                    }else{
                        //switch to other type idk
                        ChangeShootingBehavior(1);
                    }
                break;
        }
    }

    public void ChangeShootingBehavior(int value){
        switch(value){
            case 1:
                _shootBehavior = ShootBehavior.RANDOM;  
                break;
            case 2:
                _shootBehavior = ShootBehavior.TIMESCONSECUTIVE;
                break;
        }
    }
}
