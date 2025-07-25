using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animator;
    private bool open = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on door!");
        }
    }

    public void ToggleDoor()
    {
        if (animator == null)
        {
            Debug.LogError("Animator tidak ditemukan!");
            return;
        }

        open = !open;
        animator.SetBool("open", open);
        Debug.Log("Set open ke: " + open);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Tombol E ditekan");

            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj == null)
            {
                Debug.LogError("Player dengan tag 'Player' tidak ditemukan!");
                return;
            }

            float distance = Vector3.Distance(transform.position, playerObj.transform.position);
            Debug.Log("Jarak ke player: " + distance);

            if (distance < 3f)
            {
                Debug.Log("Player dalam jangkauan, membuka/menutup pintu...");
                ToggleDoor();
            }
            else
            {
                Debug.Log("Player terlalu jauh, tidak bisa membuka pintu.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            open = false;
            animator.SetBool("open", open);
        }
    }
}
