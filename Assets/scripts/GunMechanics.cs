using UnityEngine;

public class GunMechanics : MonoBehaviour
{
    private bool isPickedUp = false; // Track if the gun is picked up
    public int ammoCount = 5; // Example ammo count, can be adjusted as needed

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //gun mechanics initialization code can go here
        //gun rotation, firing rate, ammo count, etc.
        Debug.Log("Gun Mechanics Initialized");
        // Example: Set initial ammo count
        ammoCount = 30; // Example ammo count
        Debug.Log("Initial Ammo Count: " + ammoCount);
        // Example: Set firing rate
        float firingRate = 0.5f; // Example firing rate in seconds
        Debug.Log("Firing Rate: " + firingRate + " seconds");
        // Example: Set gun rotation
        transform.rotation = Quaternion.Euler(0, 0, 0); // Example initial rotation
        Debug.Log("Gun Rotation Set to: " + transform.rotation.eulerAngles);
        // Additional initialization can be added here
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickedUp)
        {
            // Gun is picked up and ready to use
            //Debug.Log("Gun is picked up and ready to use.");
            if (Input.GetButtonDown("Fire2")) // Check if the fire button is pressed
            {
                FireGun();
            }
        }
        else
        {
            //Debug.Log("Gun is not picked up.");
        }
    }

    // Call this method from your player or pickup logic when the gun is picked up
    public void PickUpGun()
    {
        isPickedUp = true;
    }

    public void DropGun()
    {
        isPickedUp = false;
    }

    void FireGun()
    {
        if (ammoCount <= 0)
        {
            Debug.Log("Out of ammo!");
            return; // Prevent firing if out of ammo
        }
        //gun firing logic can go here
        //instantiate bullet prefab, apply force, play sound, etc.
        Debug.Log("Gun Fired!");
        // launch random food item
        GameObject bullet = Resources.Load<GameObject>("Bread_bullet"); // Load your bullet prefab
        if (bullet != null)
        {
            Instantiate(bullet, transform.position, transform.rotation);
            //Debug.Log("Launched food item: " + bullet.name);
        }
        else
        {
            Debug.LogError("Failed to load bullet prefab.");
        }
        ammoCount--; // Decrease ammo count
        Debug.Log("Ammo Count: " + ammoCount);
    }
}
