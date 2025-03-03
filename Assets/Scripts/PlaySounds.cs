using UnityEngine;

public class FrogSoundManager : MonoBehaviour
{
    public AudioSource audioSource;  // The AudioSource to play sounds
    public AudioClip frogCroak;      // Croak sound
    public AudioClip frogJump;     // Ribbit sound
    public AudioClip frogTongue;     // Tongue sound

    void Start()
    {
        // Initialize or assign any required components (if needed)
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();  // Make sure AudioSource is attached
            frogCroak = Resources.Load<AudioClip>("Sounds/croak");
            frogTongue = Resources.Load<AudioClip>("Sounds/tongue_sound");
            frogJump = Resources.Load<AudioClip>("Sounds/jump");
        }
    }

    // Play the croak sound
    public void PlayCroak()
    {
        audioSource.PlayOneShot(frogCroak);
    }

    // Play the ribbit sound
    public void PlayRibbit()
    {
        audioSource.PlayOneShot(frogJump);
    }

    // Play the splash sound
    public void PlaySplash()
    {
        audioSource.PlayOneShot(frogTongue);
    }

    
}
