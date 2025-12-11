using UnityEngine;

public class OptionAnimatorController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayOptionAnimation()
    {
        if (animator != null)
        {
            animator.Play("Options", 0, 0f);
        }
    }
}
