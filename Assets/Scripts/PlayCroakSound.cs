using UnityEngine;

public class FrogSound : MonoBehaviour
{
    public AudioClip frogCroak;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        frogCroak = Resources.Load<AudioClip>("Sounds/croak");
    }

    public void PlayFrogCroak()
    {
        audioSource.PlayOneShot(frogCroak);
    }
}
