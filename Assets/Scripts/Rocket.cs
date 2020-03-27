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
    public AudioSource audioSource;
    public ParticleTypes Particles;
    [Serializable]
    public class ParticleTypes
    {
        public ParticleSystem Thrust;
        public ParticleSystem Explosion;
        public ParticleSystem Scratch;
    }

    public AudioClip ScratchSound;
    
    private const string WallTag = "Wall";
    private const string LandingPadTag = "LandingPad";
    public float ImpactThreshold = 4.5f;

    private bool IsAlive = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() //Passo da física
    {
        if (!IsAlive)
            return;
        //## THRUST
        if (Input.GetKey(ThrustKey))
        {
            //print("Thrusting");
            //rb.AddRelativeForce(force); //Vector3.Up; set mass to 0.1; set drag to .2
            rb.AddRelativeForce(thrustForce * Vector3.up * Time.deltaTime);
            if (audioSource != null && !audioSource.isPlaying)
                audioSource.Play();
            if (Particles.Thrust != null && !Particles.Thrust.isPlaying)
                Particles.Thrust.Play();
        } else {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
            if (Particles.Thrust != null && Particles.Thrust.isPlaying)
                Particles.Thrust.Stop();
        }
      
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
        Instantiate(Particles.Explosion.gameObject, transform.position, Quaternion.identity);        
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