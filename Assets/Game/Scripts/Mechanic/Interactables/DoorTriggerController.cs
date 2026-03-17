using UnityEngine;

public class DoorTriggerController : MonoBehaviour
{
    
    public Animator doorAnimator;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (doorAnimator != null)
            {
                doorAnimator.SetBool("isOpen", true);
                Debug.Log("Cube enter opendoor ! " + doorAnimator.GetBool("isOpen"));
            }
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetBool("isOpen", false);
                Debug.Log("Cube leave closedoor ! " + doorAnimator.GetBool("isOpen"));
            }
        }
    }
}