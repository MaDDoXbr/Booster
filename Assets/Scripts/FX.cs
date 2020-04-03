using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FX : MonoBehaviour
{
    public ParticleSystem Particle;
    public AudioSource AudioSource;
    public AudioClip SoundFX;
    public Vector3 Position;

    public void Play()
    {
        if (Particle.gameObject != null && !Particle.isPlaying)
            Particle.Play();
            //Instantiate(Particle.gameObject, Particle.transform.position, Quaternion.identity);
        if (AudioSource != null && !AudioSource.isPlaying)
            AudioSource.Play();
    }
    
    public void Spawn(Vector3 position)
    {
        if (Particle.gameObject != null)
            Instantiate(Particle.gameObject, position, Quaternion.identity);
        if (AudioSource != null && !AudioSource.isPlaying)
            AudioSource.Play();
    }
    
    public void Play(AudioClip soundFX)
    {
        if (Particle.gameObject != null)
            Instantiate(Particle.gameObject, Particle.transform.position, Quaternion.identity);
        if (AudioSource != null && !AudioSource.isPlaying)
            AudioSource.PlayOneShot(soundFX);
    }

    public void Stop()
    {
        if (AudioSource != null && AudioSource.isPlaying)
            AudioSource.Stop();
        if (Particle != null && Particle.isPlaying)
            Particle.Stop();
    }
}
