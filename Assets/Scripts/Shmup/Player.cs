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
    private int _baseLife = 3;
    private int _maxAmmo;

    private Weapon _currentWeapon;
    private float _currentFireRate;

    private int _currentLife;
    private float _currentSpeed;
    private float _speedMultiplier = 2.0f;

    [SerializeField]
    private float _sideEdges;

    [Header("Thruster Settings")]
    private float _nextFire = 0.0f;
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

    public event Action OnThrusterUpdate;

    [Header("Ammo Settings")]
    [SerializeField]
    private int _currentAmmo;
    private int _shotQueueSize = 10; 
    Queue<Shot> _shotQueue = new Queue<Shot>();

    public event Action<int,int> OnFireAmmoUpdate;
    public event Action<bool> OnShotEnqueue;
    public event Action<bool> OnShotDequeue;

    //[SerializeField]
    //private float _speed = 4.5f;
    //[SerializeField]
    //private GameObject _laserPrefab;
//
    //[SerializeField]
    //private GameObject _shieldVisual, _rightEngine, _leftEngine;
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
    //    //current position at start.
//
        transform.position = new Vector3(0,0,0);
        InitializeWeapon(0);
        _currentSpeed = _baseSpeed;
        _thrusterVisual.gameObject.SetActive(false);
        _currentAmmo = _maxAmmo;

        //populate queue
        //for queue size. enqueue the initialized shot. furhter ones will be recycled.
        for(int i = 0;i < _shotQueueSize;i++){
            Shot temp = new Shot();
            temp.Chance();
            _shotQueue.Enqueue(temp);
            OnShotEnqueue(temp.critical);   
        }
        Debug.Log(_shotQueue.Count);
        OnFireAmmoUpdate(_currentAmmo, _maxAmmo);
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
            }
            OnThrusterUpdate?.Invoke();
        } else{
            //if we pressing go up...if we hit the limit. we enter cooldown and slow down by force
            if(Input.GetKey(KeyCode.LeftShift)){
                _currentThruster += Time.deltaTime * _thrusterChargeSpeed;
                _currentThruster = Mathf.Clamp(_currentThruster, 0.0f, _maxThruster);
                if(_currentThruster == _maxThruster){
                    SetThrusterState(false);
                    Debug.Log("BURNED OUT!, Time to cool down");
                    _thrusterCooldown = true;
                }
                OnThrusterUpdate?.Invoke();
            }
            //if we aren't touching it...we just decrease naturally.
            if(_currentThruster > 0.0f && !Input.GetKey(KeyCode.LeftShift)){
                _currentThruster -= Time.deltaTime * _thrusterChargeSpeed;
                _currentThruster = Mathf.Clamp(_currentThruster, 0.0f, _maxThruster);
                OnThrusterUpdate?.Invoke();
            }
            //if we let go return to normal speed
            if(Input.GetKeyUp(KeyCode.LeftShift)){
                Debug.Log("let go, going down");
                SetThrusterState(false);
            }
            //if we first press, turn to higher speed.
            if(Input.GetKeyDown(KeyCode.LeftShift)){
                Debug.Log("let go, going up");
                SetThrusterState(true);
            }

        }
        

        CalculateMovement();
        if(Input.GetKey(KeyCode.Space) && Time.time > _nextFire && _currentAmmo != 0){
            ShootLaser();
        }
    }

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
        _nextFire = _currentWeapon.Shoot(transform.position, temp);
        _currentAmmo -= 1;
        OnFireAmmoUpdate(_currentAmmo,-1);
        if(_currentAmmo >= _shotQueueSize){
            temp.Chance();
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

    public void AddAmmo(int ammount){
        Debug.Log("AddAmmo called");
        for(int i = _currentAmmo; i < _shotQueueSize; i++){
            Shot temp = new Shot();
            temp.Chance();
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
    //public void Damage(){
//
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
    //}
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
    //private void Death(){
    //    Destroy(this.gameObject);
    //}
//
    //public void TrippleShotActive(){
    //    tripleShotActive = true;
    //    StartCoroutine(trippleShotPowerDownRoutine());
    //}
//
    //IEnumerator trippleShotPowerDownRoutine(){
    //    yield return new WaitForSeconds(5);
    //    tripleShotActive = false; 
    //}
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
}
