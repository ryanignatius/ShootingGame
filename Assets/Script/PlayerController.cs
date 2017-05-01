using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public const float maxPosition = 10f;
    public const float bulletLife = 2f;
    public const float bulletSpeed = 10f;
    public const float horizontalSpeed = 150f;
    public const float verticalSpeed = 5f;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * horizontalSpeed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * verticalSpeed;

        transform.Rotate(0, x, 0);
        if (z > 0)
        {
            transform.Translate(0, 0, z);
            if (transform.position.x > maxPosition)
            {
                transform.position = new Vector3(maxPosition, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < -maxPosition)
            {
                transform.position = new Vector3(-maxPosition, transform.position.y, transform.position.z);
            }
            if (transform.position.z > maxPosition)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, maxPosition);
            }
            else if (transform.position.z < -maxPosition)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -maxPosition);
            }
        }
        Camera.main.transform.position = transform.position;
        Camera.main.transform.rotation = transform.rotation;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }
    }

    // This [Command] code is called on the Client …
    // … but it is run on the Server!
    [Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after x seconds
        Destroy(bullet, bulletLife);
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
        transform.position = new Vector3(Random.Range(-maxPosition, maxPosition), 0, Random.Range(-maxPosition, maxPosition));
        Camera.main.transform.position = transform.position;
    }
}
