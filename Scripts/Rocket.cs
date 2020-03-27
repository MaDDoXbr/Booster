using UnityEngine;
 
public class Rocket : MonoBehaviour
{
    private Rigidbody rb;
    //public Vector3 force;    //(0,1,0)
    public KeyCode RotateLeftKey = KeyCode.A;
    public KeyCode RotateRightKey = KeyCode.D;
    public KeyCode ThrustKey = KeyCode.W;
    public ParticleSystem Particle;
    public AudioSource audioSource;
    public float thrustForce = 1000f;
    public float rotationForce = 100f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() //Passo da física
    {
        //## THRUST
        if (Input.GetKey(ThrustKey))
        {
            //print("Thrusting");
            //rb.AddRelativeForce(force); //Vector3.Up; set mass to 0.1; set drag to .2
            rb.AddRelativeForce(thrustForce * Vector3.up * Time.deltaTime);
            if (audioSource != null && !audioSource.isPlaying)
                audioSource.Play();
            if (Particle != null && !Particle.isPlaying)
                Particle.Play();
        } else {
            if (audioSource != null && audioSource.isPlaying)
                audioSource.Stop();
            if (Particle != null && Particle.isPlaying)
                Particle.Stop();
        }
      
        //## ROTATE
        rb.freezeRotation = true;    //controle manual da rotação
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
        rb.freezeRotation = false; //controle físico da rotação
    }
}