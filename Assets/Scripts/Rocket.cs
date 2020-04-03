using System;
using System.Collections;
using System.Net.Security;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Rocket : MonoBehaviour
{
    private Rigidbody rb;
    //public Vector3 force;    //(0,1,0)
    public KeyCode RotateLeftKey = KeyCode.A;
    public KeyCode RotateRightKey = KeyCode.D;
    public KeyCode ThrustKey = KeyCode.W;
    public float thrustForce = 1000f;
    public float rotationForce = 100f;
    public AudioClip ScratchSound;
    public AudioSource audioSource;
    public ParticleTypes Particles;
    
    [Serializable]
    public class ParticleTypes
    {
        public ParticleSystem Thrust;
        public ParticleSystem Explosion;
        public ParticleSystem Scratch;
    }

    private const string WallTag = "Wall";
    private const string LandingPadTag = "LandingPad";
    public float ImpactThreshold = 4.5f;

    private bool IsAlive = true;
    public FX ThrustFX;
    public FX ExplosionFX;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() //Passo da física
    {
        if (!IsAlive)
            return;
        ThrustCheck();
        RotateCheck();
    }

    private void RotateCheck()
    {
        //## ROTATE
        //rb.freezeRotation = true;    //controle manual da rotação
        if (Input.GetKey(RotateLeftKey))
        {
            //print("Rotating Left");
            transform.Rotate(Vector3.forward * rotationForce * Time.deltaTime);
        }
        else if (Input.GetKey(RotateRightKey))
        {
            //print("Rotating Right");
            transform.Rotate(-Vector3.forward * rotationForce * Time.deltaTime);
        }

        var rotAngle = transform.rotation.eulerAngles;
        var pos = transform.position;
        
        transform.rotation = Quaternion.Euler(0f, rotAngle.y, rotAngle.z);
        transform.localPosition = new Vector3(pos.x, pos.y, 0f);
        //rb.freezeRotation = false; //controle físico da rotação
    }

    private void ThrustCheck()
    {
        if (Input.GetKey(ThrustKey))
        {
            //print("Thrusting");
            //rb.AddRelativeForce(force); //Vector3.Up; set mass to 0.1; set drag to .2
            rb.AddRelativeForce(thrustForce * Vector3.up * Time.deltaTime);
            ThrustFX.Play();
        }
        else
        {
            ThrustFX.Stop();
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == LandingPadTag)
            Success();
        else if (col.gameObject.tag == WallTag)
            CheckImpact(col);
    }

    private void CheckImpact(Collision col)
    {
        var impact = col.relativeVelocity.magnitude;
        Debug.Log("Impact Strength: "+impact);
        if (impact > ImpactThreshold)
            Crashed();
        else
            Scratch(col);
    }

    private void Scratch(Collision col)
    {
        Debug.Log("Scratch");
        var contactPosition = col.GetContact(0).point;
        Instantiate(Particles.Scratch.gameObject, contactPosition, Quaternion.identity);
        audioSource.PlayOneShot(ScratchSound);
    }

    private void Crashed()
    {
        //Debug.Log("You Crashed!");
        if (!IsAlive)
            return;
        ExplosionFX.Spawn(transform.position);        
        IsAlive = false;
        
        StartCoroutine(LoadSceneDelayed());
    }

    private IEnumerator LoadSceneDelayed()
    {
        //gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);
        Debug.Log("Reloading Scene");
        SceneManager.LoadScene(0);
    }

    private void Success()
    {
        Debug.Log("You Won!");
    }
}