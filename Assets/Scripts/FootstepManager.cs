using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    [Header("Footstep Sounds")]
    public AudioClip[] dirtFootsteps; // Array for dirt sounds
    public AudioClip[] grassFootsteps; // Array for grass sounds
    public AudioClip[] stoneFootsteps; // Array for stone sounds
    public AudioClip[] woodFootsteps; // Array for wood sounds

    [Header("Settings")]
    public float stepInterval = 0.5f; // Time between steps
    public LayerMask groundLayer; // Layer for detecting surface type

    private AudioSource audioSource;
    private float stepTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        stepTimer = 0f;
    }

    void Update()
    {
        // Check if the player is walking
        if (IsWalking())
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= stepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
    }

    private bool IsWalking()
    {
        // Replace this with your walking condition, e.g., checking player velocity
        return Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
    }

    private void PlayFootstep()
    {
        // Detect ground type
        string groundType = GetGroundType();

        // Select a random sound based on the ground type
        AudioClip clip = GetRandomFootstep(groundType);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private string GetGroundType()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, groundLayer))
        {
            // Example of detecting ground types using tags
            if (hit.collider.CompareTag("Dirt")) return "Dirt";
            if (hit.collider.CompareTag("Grass")) return "Grass";
            if (hit.collider.CompareTag("Stone")) return "Stone";
            if (hit.collider.CompareTag("Wood")) return "Wood";
        }
        return "Default"; // Fallback ground type
    }

    private AudioClip GetRandomFootstep(string groundType)
    {
        AudioClip[] clips = null;

        switch (groundType)
        {
            case "Dirt":
                clips = dirtFootsteps;
                break;
            case "Grass":
                clips = grassFootsteps;
                break;
            case "Stone":
                clips = stoneFootsteps;
                break;
            case "Wood":
                clips = woodFootsteps;
                break;
        }

        if (clips != null && clips.Length > 0)
        {
            return clips[Random.Range(0, clips.Length)];
        }
        return null;
    }
}
