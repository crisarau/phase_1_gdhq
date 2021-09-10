using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shot{
    public bool critical;
    public void Chance(int probability = 5){
        float rnd = UnityEngine.Random.Range(0,101);
        critical = rnd <= probability ? true : false;
    }
}
public class Player : MonoBehaviour
{

    [SerializeField]
    private Weapon[] _allWeapons;

    [SerializeField]
    private float _baseSpeed = 6.5f;
    
    private int _maxAmmo;

    private Weapon _currentWeapon;
    private float _currentFireRate;

    [SerializeField]
    private int _currentLife;
    private int _baseLife = 3;
    private float _currentSpeed;
    private float _speedMultiplier = 2.0f;

    [SerializeField]
    private float _sideEdges;

    
    private float _nextFire = 0.0f;

    [Header("I-Frame Settings")]
    private BoxCollider2D collider;
    private SpriteRenderer sprite;
    [SerializeField]
    private float _timePerBlink;


    [Header("Thruster Settings")]
    [SerializeField]
    private float _maxThruster;
    [SerializeField]
    public float _currentThruster = 0.0f;

    private float _thrusterChargeSpeed = 0.5f;
    [SerializeField]
    private bool _thrusterCooldown = false;

    [SerializeField]
    private float _thrusterMultiplier = 3.0f;
    [SerializeField]
    private Transform _thrusterVisual;
    [SerializeField]
    private GameObject ThrusterExplosion;

    public event Action<bool> OnThrusterUpdate;

    [Header("Ammo Settings")]
    [SerializeField]
    private int _currentAmmo;
    private int _shotQueueSize = 10; 
    Queue<Shot> _shotQueue = new Queue<Shot>();

    

    [SerializeField]
    public Transform _shotSpawns;

    public event Action<int,int> OnFireAmmoUpdate;
    public event Action<bool> OnShotEnqueue;
    public event Action<bool> OnShotDequeue;
    
    public event Action<int> OnHealthUpdate;

    [Header("Melee Settings")]
    [SerializeField]
    private meleeAttack[] meleeAttacks;
    [SerializeField]
    private int meleeAttackSelected;
    [SerializeField]
    private float _meleeAttackChargeSpeed = 0.5f;
    [Range(0f, 1f)][SerializeField]
    private float _currentMeleeBarFill = 0f;
    public event Action<float> OnMeleeAttackUpdate;


    //[SerializeField]
    //private float _speed = 4.5f;
    //[SerializeField]
    //private GameObject _laserPrefab;
//
    [Header("PowerUp Settings")]
    [SerializeField]
    private GameObject _shieldVisual;//, _rightEngine, _leftEngine;
    float _shieldAlpha;
    [SerializeField]
    private int currentCriticalProbabilty = 5;

    //[SerializeField]
    //private GameObject _triplePrefab;
    //[SerializeField]
    //private Transform _shootingPoint;
    //[SerializeField]
    //private float _fireRate = 0.25f;

    

//
    //[SerializeField]
    //private int _lives = 3;
    //private float _nextFire = 0.0f;    
    private float horizontalInput;
    private float verticalInput;
    //[SerializeField]
    //private bool tripleShotActive = false;
    //[SerializeField]
    //private bool speedBoostActive = false;
    //[SerializeField]
    //private bool shieldActive = false;
    [SerializeField]
    private int mirrorShotLevel = 0;
    [SerializeField]
    private int homingActive = 0;
    [SerializeField]
    private float _homingAttackChargeSpeed = 0.5f;
    [Range(0f, 1f)][SerializeField]
    private float _currentHomingBarFill = 0f;
    HomingOverlapTarget homing;
    [SerializeField] private float _currentSpecialRate; // this and the next one aren't used anymore!
    private float _nextSpecial = 0.0f;

    UpgradeController upgradeController;
    private float upgradeDelay = 0.2f;
    private float _nextUpgradeSwitch;

    public event Action<bool> PickUpsMagnetActivation;
    [Range(0f, 1f)][SerializeField]
    private float _currentMagnetBarFill = 0f;
    [SerializeField]
    private float _magnetChargeSpeedUp = 0.25f; 
    [SerializeField]
    private float _magnetChargeSpeedDown = 0.25f; //separated this way for the upgrade. slower down and faster up
    private bool magnetActive; //if u have the upgrade for it
    private bool magnetValueGoingDown; //to keep going down as long as u press the button
    


//
    //private SpawnManager _spawnManager;
    //[SerializeField]
    //private int _score = 0;
//
    //[SerializeField]
    //private UIManager _uiManager;
//
    //[SerializeField]
    //private AudioClip _laserSound;
    //private AudioSource _audioSource;
//

    
    void Start()
    {
        homing = GetComponent<HomingOverlapTarget>();
        collider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        homing.ResizeTargetAmount(homingActive);
        upgradeController = transform.GetComponent<UpgradeController>();
        ChangeMeleeAttackLevel(-1);
        //current position at start.
        //transform.position = new Vector3(0,0,0);

        //get stats from first weapon
        InitializeWeapon(0);
        _currentSpeed = _baseSpeed;
        _thrusterVisual.gameObject.SetActive(false);
        _currentAmmo = _maxAmmo;

        //populate queue
        //for queue size. enqueue the initialized shot. furhter ones will be recycled.
        for(int i = 0;i < _shotQueueSize;i++){
            Shot temp = new Shot();
            temp.Chance(currentCriticalProbabilty);
            _shotQueue.Enqueue(temp);
            OnShotEnqueue(temp.critical);   
        }
        //Debug.Log(_shotQueue.Count);
        OnFireAmmoUpdate(_currentAmmo, _maxAmmo);

        //fill health
        _currentLife = 1;
        for(int i = 1 ; i<_baseLife;i++){
            _currentLife += 1;
            OnHealthUpdate(_currentLife);
        }
        //show shield. 111 = white = original sprite color
        _shieldVisual.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);

//
    //    _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    //    if(_spawnManager== null){
    //        Debug.LogError("The spawnmanager is NULL");
    //    }
    //    _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    //    if(_uiManager== null){
    //        Debug.LogError("The _uiManager is NULL");
    //    }
    //    _shieldVisual.SetActive(false);
//
    //    _audioSource = GetComponent<AudioSource>();
    //    if(_audioSource == null){
    //        Debug.LogError("The audiosource is NULL");
    //    }else{
    //        _audioSource.clip  = _laserSound;
    //    }
    }
//
    void Update()
    {

        //logic for thruster mode...we go down regardless of input if we on cooldown mode.
        if(_thrusterCooldown){
            _currentThruster -= Time.deltaTime * _thrusterChargeSpeed;
            _currentThruster = Mathf.Clamp(_currentThruster, 0.0f, _maxThruster);
            if(_currentThruster == 0.0f){
                _thrusterCooldown = false;
                //we reached end no more on cooldown
            }
            OnThrusterUpdate?.Invoke(false); //let UI know
        } else{
            //if we pressing go up...if we hit the limit. we enter cooldown and slow down by force
            if(Input.GetKey(KeyCode.LeftShift)){
                _currentThruster += Time.deltaTime * _thrusterChargeSpeed; //value go up
                _currentThruster = Mathf.Clamp(_currentThruster, 0.0f, _maxThruster);
                if(_currentThruster == _maxThruster){ //we hit limit
                    SetThrusterState(false); //shut down state, no way to turn it back on until we allowed to use again
                    Debug.Log("BURNED OUT!, Time to cool down");
                    //instantiate thruster
                    Instantiate(ThrusterExplosion, new Vector3(transform.position.x, transform.position.y + ThrusterExplosion.transform.localPosition.y, transform.position.z) , Quaternion.identity);
                    //enter cooldown state
                    _thrusterCooldown = true;
                }
                //always update UI
                OnThrusterUpdate?.Invoke(true);
            }
            //if we aren't touching it...we just decrease naturally.
            if(_currentThruster > 0.0f && !Input.GetKey(KeyCode.LeftShift)){
                _currentThruster -= Time.deltaTime * _thrusterChargeSpeed;
                _currentThruster = Mathf.Clamp(_currentThruster, 0.0f, _maxThruster);
                OnThrusterUpdate?.Invoke(false);
            }
            //if we let go return to normal speed
            if(Input.GetKeyUp(KeyCode.LeftShift)){
                //Debug.Log("let go, going down");
                SetThrusterState(false);
            }
            //if we first press, turn to higher speed.
            if(Input.GetKeyDown(KeyCode.LeftShift)){
                //Debug.Log("let go, going up");
                SetThrusterState(true);
            }

        }

        if(Input.GetKeyDown(KeyCode.C) && _currentMeleeBarFill == 1f){
            meleeAttacks[meleeAttackSelected].Attack();
            _currentMeleeBarFill = 0f;
            OnMeleeAttackUpdate(_currentMeleeBarFill);
        }
        if(_currentMeleeBarFill != 1f){
            _currentMeleeBarFill += Time.deltaTime * _meleeAttackChargeSpeed;
            _currentMeleeBarFill = Mathf.Clamp(_currentMeleeBarFill, 0f, 1f);
            //let UI know
            OnMeleeAttackUpdate(_currentMeleeBarFill);
        }

        CalculateMovement();
        if(Input.GetKey(KeyCode.Space) && Time.time > _nextFire && _currentAmmo != 0){
            ShootLaser();
        }

        if(homingActive!=0 && Input.GetKey(KeyCode.H) && _currentHomingBarFill == 1){
            
            ShootHoming();
            _currentHomingBarFill = 0f;
            //letUIknow
        }
        if(_currentHomingBarFill != 1f){
            _currentHomingBarFill += Time.deltaTime * _homingAttackChargeSpeed;
            _currentHomingBarFill = Mathf.Clamp(_currentHomingBarFill, 0f, 1f);
            //let UI know

        }

        if(Input.GetKey(KeyCode.M) && magnetActive && _currentMagnetBarFill == 1f){
            magnetValueGoingDown = true;
            PickUpsMagnetActivation?.Invoke(true);
        }
        if(magnetValueGoingDown){
            _currentMagnetBarFill -= Time.deltaTime * _magnetChargeSpeedDown;
            _currentMagnetBarFill = Mathf.Clamp(_currentMagnetBarFill, 0f, 1f);
        }
        if(_currentMagnetBarFill != 1f && !magnetValueGoingDown){
            _currentMagnetBarFill += Time.deltaTime * _magnetChargeSpeedUp;
            _currentMagnetBarFill = Mathf.Clamp(_currentMagnetBarFill, 0f, 1f);
        }
        if(Input.GetKeyUp(KeyCode.M)){
            magnetValueGoingDown = false;
            _currentMagnetBarFill = 0f;
            PickUpsMagnetActivation?.Invoke(false);
        }

        //UI ABILITY SWITCHING
        if(Input.GetKeyDown(KeyCode.Q) && Time.time > _nextUpgradeSwitch){
            //THIS GETS THE CARD ON THE LEFT move to RIGHT for current position
            // let upgrade controller handle this...
            AddToSwitchUpgradeDelay();
            upgradeController.ChangeCurrentLeft();
        }
        if(Input.GetKeyDown(KeyCode.E)&& Time.time > _nextUpgradeSwitch){
            //THIS GETS THE CARD ON THE RIGHT move to the LEFT for current position
            AddToSwitchUpgradeDelay();
            upgradeController.ChangeCurrentRight();
        }
    }

    //sets thruster speed multiplier or resets when done, also shows thruster visual
    private void SetThrusterState(bool on){
        if(on){
            _currentSpeed *= _thrusterMultiplier;
            _thrusterVisual.gameObject.SetActive(true);
        }else{
            _currentSpeed = _baseSpeed;
            _thrusterVisual.gameObject.SetActive(false);
        }
    }

    public void InitializeWeapon(int _weaponIndex){
        _currentWeapon = _allWeapons[_weaponIndex];
        _maxAmmo = _currentWeapon.baseAmmo;
        _currentFireRate = _currentWeapon.fireRate;
    }


//
    void CalculateMovement(){
        horizontalInput = Input.GetAxis("Horizontal"); //difference between getaxis and raw...raw is sanppier but you can't do both keys at once. getaxis allows this and feels a bit unresponsive.
        verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput,verticalInput,0).normalized;
        
        transform.Translate(direction *  _currentSpeed * Time.deltaTime);
        
        //better way to restrict movement in y axis
        float _yClamp = Mathf.Clamp(transform.position.y, -4, 6);
        transform.position = new Vector3(transform.position.x, _yClamp, 0);
        
        //wrapping around
        if(transform.position.x > _sideEdges){
            transform.position = new Vector3((-1.0f *_sideEdges),transform.position.y, 0);
        }else if(transform.position.x < (-1.0f *_sideEdges)) {
            transform.position = new Vector3(_sideEdges,transform.position.y, 0);
        }
    }
    void ShootLaser(){
        
        Shot temp = _shotQueue.Dequeue();
        OnShotDequeue(temp.critical);

        //basic shot
        _nextFire = _currentWeapon.Shoot(_shotSpawns.GetChild(0).position,Quaternion.identity, temp);
        if(mirrorShotLevel != 0){
            Debug.Log("diagonal mirrorshot");
            _currentWeapon.Shoot(_shotSpawns.GetChild(0).position,Quaternion.Euler(0,0,-180), temp);
        }
        //there should be a way to exclude the left and right ones from mirror...they are repeated twice on each side lol
        for(int i = 1; i < _shotSpawns.childCount ;i++){
            if(_shotSpawns.GetChild(i).gameObject.activeSelf){
                _currentWeapon.Shoot(_shotSpawns.GetChild(i).position,_shotSpawns.GetChild(i).rotation,temp);
                if(mirrorShotLevel > 1 && (i != 5 || i != 6)){
                    //if mirrorshotlevel above one and not doing the left and right horizontal shots...then shoot
                    _currentWeapon.Shoot(_shotSpawns.GetChild(i).position,Quaternion.Euler(0,0,_shotSpawns.GetChild(i).eulerAngles.z -180f), temp);
                }
            }
        }
        

        _currentAmmo -= 1;
        OnFireAmmoUpdate(_currentAmmo,-1);
        if(_currentAmmo >= _shotQueueSize){
            temp.Chance(currentCriticalProbabilty);
            _shotQueue.Enqueue(temp);
            OnShotEnqueue(temp.critical);
        }
        Debug.Log(_shotQueue.Count);
        
            
        //the next time it will allow to fire in the future.
        //_nextFire = Time.time + _fireRate;
//
    //    if(tripleShotActive){
    //        Instantiate(_triplePrefab, transform.position, Quaternion.identity);
    //    }else{
            //Instantiate(_laserPrefab, _shootingPoint.position, Quaternion.identity);
    //    }
    //    //sound effect
    //    _audioSource.Play();
    }

    public void ShootHoming(){
        Debug.Log("we firing homing!");
        homing.Fire();
        _nextSpecial = Time.time + _currentSpecialRate;
    }
    public void AddAmmo(int ammount){
        for(int i = _currentAmmo; i < _shotQueueSize; i++){
            Shot temp = new Shot();
            temp.Chance(currentCriticalProbabilty);
            _shotQueue.Enqueue(temp);
            OnShotEnqueue(temp.critical);
        }
        _currentAmmo = Mathf.Clamp(_currentAmmo + ammount, 0, _maxAmmo);
        OnFireAmmoUpdate(_currentAmmo,-1);
    }
    public float getMaxThruster(){
        return _maxThruster;
    }
//
    public void Heal(){
        if(_currentLife != _baseLife){
            _currentLife = Mathf.Clamp(_currentLife + 1, 0, _baseLife);
            OnHealthUpdate(_currentLife);

            _shieldAlpha = ((float)_currentLife-1)/((float)_baseLife - 1);
            _shieldVisual.GetComponent<SpriteRenderer>().color = new Color(1,1,1,_shieldAlpha);
        }
    }
    public void Damage(){
        StartCoroutine(DamagedInvincibilityFrames());
        _currentLife = Mathf.Clamp(_currentLife - 1, 0, _baseLife);
        OnHealthUpdate(_currentLife);
        upgradeController.Downgrade();
        if(_currentLife==0){
            Death();
            return;
        }
        
        _shieldAlpha = ((float)_currentLife-1)/((float)_baseLife - 1);
        _shieldVisual.GetComponent<SpriteRenderer>().color = new Color(1,1,1,_shieldAlpha);
    //    if(shieldActive){
    //        shieldActive = false;
    //        _shieldVisual.SetActive(false);
    //        return;
    //    }
//
    //    _lives -= 1;
    //    UpdateDamageVisuals(_lives);
    //    _uiManager.UpdateLives(_lives);
    //    if(_lives<1){
    //        //tell spawn manager that we died so stop spawning.
    //        _spawnManager.OnPlayerDeath();
    //        Death();
    //    }
    }
//
    //private void UpdateDamageVisuals(int _damage){
    //    switch(_damage){
    //        case 2:
    //            _rightEngine.SetActive(true);
    //        break;
    //        case 1:
    //            _leftEngine.SetActive(true);
    //        break;
    //    }
    //}
//
    public void ActivateShots(int shot1, int shot2 = 0){
        _shotSpawns.GetChild(shot1).gameObject.SetActive(true);
        _shotSpawns.GetChild(shot2).gameObject.SetActive(true);

        StartCoroutine(extraShotPowerDownRoutine(shot1, shot2));

    }
    private void Death(){
        Destroy(this.gameObject);
    }
//
    //public void TrippleShotActive(){
    //    tripleShotActive = true;
    //    StartCoroutine(trippleShotPowerDownRoutine());
    //}

    //public void MirrorShotActive(){
    //    mirrorActive = true;
    //    StartCoroutine(mirrorShotPowerDownRoutine());
    //}


    //public void MirrorShotUpgradeActivation(bool state){
    //    mirrorActive = state;
    //}
//
    //IEnumerator trippleShotPowerDownRoutine(){
        //yield return new WaitForSeconds(5);
        //tripleShotActive = false; 
    //}
    //IEnumerator mirrorShotPowerDownRoutine(){
    //    yield return new WaitForSeconds(5);
    //    mirrorActive = false; 
    //}

    IEnumerator extraShotPowerDownRoutine(int shot1, int shot2){
        yield return new WaitForSeconds(5);
        _shotSpawns.GetChild(shot1).gameObject.SetActive(false);
        _shotSpawns.GetChild(shot2).gameObject.SetActive(false); 
    }
//
    //public void SpeedPowerUpActive(){
    //    _speed = _speed * _speedMultiplier;
    //    speedBoostActive = true;
    //    StartCoroutine(speedPowerDownRoutine());
    //}
//
    //IEnumerator speedPowerDownRoutine(){
    //    yield return new WaitForSeconds(5);
    //    _speed = _speed / _speedMultiplier;
    //    speedBoostActive = false;
    //}
//
    //public void ShieldPowerUpActive(){
    //    _shieldVisual.SetActive(true);
    //    shieldActive = true;
    //}
//
    //public void AddToScore(int points){
    //    _score += 10;
    //    //tell UIManager
    //    _uiManager.UpdateScore(_score);
    //}
//
    IEnumerator DamagedInvincibilityFrames(){
        collider.enabled = false;
        for(int i = 0; i < 10; i++){
            sprite.color = Color.clear;
            yield return new WaitForSeconds(_timePerBlink);
            sprite.color = Color.white;
            yield return new WaitForSeconds(_timePerBlink);
        }
        collider.enabled = true;
    }

    public void AddToSwitchUpgradeDelay(){
        _nextUpgradeSwitch = Time.time + upgradeDelay;
    }

    //since we are just setting...we can use this as a reset if pass 0 as argument
    public void ChangeCurrentCriticalProbability(int level){
        if(level == -1){
            currentCriticalProbabilty = 5;
        }else if(level == 0){
            currentCriticalProbabilty = 10;
        }else if(level == 1){
            currentCriticalProbabilty = 15;
        }else if(level == 2){
            currentCriticalProbabilty = 20;
        }
    }
    public void DeactivateAllMultiShots(){
        for (int i = 1; i < 7; i++)
        {
            _shotSpawns.GetChild(i).gameObject.SetActive(false);   
        }
    }

    public void ChangeCurrentMirrorShotLevel(int level){
        if(level == -1){
            mirrorShotLevel = 0;
        }else if(level == 0){
            mirrorShotLevel = 1;
        }else if(level == 1){
            mirrorShotLevel = 2;
        }
    }

    public void ChangeHomingLevel(int level){
        if(level == -1){
            homingActive = 0;
        }else if(level == 0){
            homingActive = 1;
            homing.ResizeTargetAmount(homingActive);
        }else if(level == 1){
            homingActive = 2;
            homing.ResizeTargetAmount(homingActive);
        }else if(level == 2){
            homingActive = 3;
            homing.ResizeTargetAmount(homingActive);
        }
    }

    public void ChangeMeleeAttackLevel(int level){
        if(level == -1){
            meleeAttackSelected = 0;
            meleeAttacks[0].transform.gameObject.SetActive(true);
            meleeAttacks[1].transform.gameObject.SetActive(false);
        }else if(level == 0){
            meleeAttackSelected = 1;
            meleeAttacks[1].transform.gameObject.SetActive(true);
            meleeAttacks[0].transform.gameObject.SetActive(false);
        }
    }
    public void ChangeMeleeChargeSpeed(bool state){
        if(state){
            _meleeAttackChargeSpeed = 0.35f;
        }else{
            _meleeAttackChargeSpeed = 0.15f;
        }
        
    }
    public void ChangeHomingChargeSpeed(bool state){
        if(state){
            _homingAttackChargeSpeed = 0.35f;
        }else{
            _homingAttackChargeSpeed = 0.15f;
        }
    }

    

    public void ChangeMagnetStatus(bool state){
        if(state){
            magnetActive = true;
        }else{
            magnetActive = false;
        }

    }
}
