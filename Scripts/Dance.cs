using StarterAssets;
using UnityEngine;

public class Dance : MonoBehaviour
{
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (starterAssetsInputs.dance)
        {
            animator.SetBool("dance",true);
            starterAssetsInputs.dance = false;

            Invoke("stopDance", 2f);
        }
        else
        {
            //animator.SetBool("dance", false);
        }
    }

    public void stopDance()
    {
        animator.SetBool("dance", false);
    }
}
