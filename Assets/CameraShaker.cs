using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{

    [SerializeField]
    bool shakingActive;
    bool shakingActiveOnPreviousFrame;

    [SerializeField]
    [Range(0, 1)]
    private float trauma;
    private float sharedSeed;
    [SerializeField]
    private float decay;
    //high value = faster shake
    [SerializeField]
    float frequency;
    //how far of a distance the camera can move from its original position
    [SerializeField]
    float translationMaxDistance;

    //booleans to see what axis it can move on
    [SerializeField]
    bool canTranslateX = true;
    [SerializeField]
    bool canTranslateY = true;
    [SerializeField]
    bool canTranslateZ = false;

    //is this for influence? strength in axis? original has it a 0to1
    [SerializeField]
    float translateMultiplierX;
    [SerializeField]
    float translateMultiplierY;
    [SerializeField]
    float translateMultiplierZ;

    //how far of an angle the camera can rotation from its original rotation
    [SerializeField]
    float rotationMaxAngle;

    //booleans to see what axis it can move on
    [SerializeField]
    bool canRotateX = false;
    [SerializeField]
    bool canRotateY = false;
    [SerializeField]
    bool canRotateZ = false;
    [SerializeField]
    float rotateMultiplierX;
    [SerializeField]
    float rotateMultiplierY;
    [SerializeField]
    float rotateMultiplierZ;


    //is this even necessary? i thought the whole point of having a parent is to disppear the trauma automatically?
    Vector3 originalPosition;
    Quaternion originalRotation;

    

    // Unique seeds are important to ensure that no predictable patterns will emerge in the movement.
    // Also, using the same seed for the same translation/rotation ensures fluid motion.
    private const float TRANSLATION_X_SEED = 100;
    private const float TRANSLATION_Y_SEED = 200;
    private const float TRANSLATION_Z_SEED = 300;
    private const float ROTATION_X_SEED = 400;
    private const float ROTATION_Y_SEED = 500;
    private const float ROTATION_Z_SEED = 600;
    

    private bool ShakePossible(){
        if(translationMaxDistance > 0 && ( (canTranslateX && translateMultiplierX > 0 ) || (canTranslateY && translateMultiplierY > 0 ) || (canTranslateZ && translateMultiplierZ > 0 ) )){
            return true;
        }
        if(rotationMaxAngle > 0 && ( (canRotateX && rotateMultiplierX > 0) || (canRotateY && rotateMultiplierY > 0) || (canRotateZ && rotateMultiplierZ > 0) )){
            return true;
        }

        return false;
    }

    private float GetPerlinValue(float seed){
        return (Mathf.PerlinNoise(seed, sharedSeed) - 0.5f) * 2f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.P)){
            Shake(0.1f); //will add 0.10 to trauma
        }
    }

    private void LateUpdate() {
        if(shakingActive){
            sharedSeed += Time.deltaTime * Mathf.Pow(trauma, 2f) * frequency;
            //if translate
            if(translationMaxDistance > 0 && (canTranslateX || canTranslateY || canTranslateZ)){
                Vector3 translateTo = new Vector3(
                    canTranslateX && translateMultiplierX > 0 ? (GetPerlinValue(TRANSLATION_X_SEED) * trauma * translateMultiplierX * translationMaxDistance) : 0,
                    canTranslateY && translateMultiplierY > 0 ? (GetPerlinValue(TRANSLATION_Y_SEED) * trauma * translateMultiplierY * translationMaxDistance) : 0,
                    canTranslateZ && translateMultiplierZ > 0 ? (GetPerlinValue(TRANSLATION_Z_SEED) * trauma * translateMultiplierZ * translationMaxDistance) : 0
                );
                transform.localPosition = translateTo + originalPosition;
            }
            //if rotation
            if(rotationMaxAngle > 0 && (canRotateX || canRotateY || canRotateZ)){
                Vector3 rotateTo = new Vector3(
                    canRotateX && rotateMultiplierX > 0 ? (GetPerlinValue(ROTATION_X_SEED) * trauma * rotateMultiplierX * rotationMaxAngle) : 0,
                    canRotateY && rotateMultiplierY > 0 ? (GetPerlinValue(ROTATION_Y_SEED) * trauma * rotateMultiplierY * rotationMaxAngle) : 0,
                    canRotateZ && rotateMultiplierZ > 0 ? (GetPerlinValue(ROTATION_Z_SEED) * trauma * rotateMultiplierZ * rotationMaxAngle) : 0
                );
                transform.localRotation = Quaternion.Euler(rotateTo) * originalRotation;
            }
            trauma = Mathf.Clamp01(trauma - (Time.deltaTime * decay));
            //maybe check if done here and set to not shaking anymore? and set to original position??? idk
            if(trauma <= 0 ){
                Debug.Log("We done shaking");
                shakingActive = false;
                transform.localPosition = originalPosition;
                transform.localRotation = originalRotation;
            }
        }
    }

    public void Shake(float stress){
        trauma = Mathf.Clamp01(trauma + stress);
        if (trauma > 0 && !shakingActive && ShakePossible()){
            shakingActive = true;
            sharedSeed = Random.value;
            
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
        }
    }
}

