
using UnityEngine;

public abstract class InputResolverStep : MonoBehaviour
{
    private void OnEnable()
    {
        Register();
    }

    private void OnDisable()
    {
        Unregister();
    }

    private void Register()
    {
        GameManager.Instance.InputResolver.AddSolveableInputStep(this);
    }

    private void Unregister()
    {
        GameManager.Instance.InputResolver.RemoveSolveableInputStep(this);
    }

    // Handle all input in this method.
    // Return value is whether or not to continue processing other inputs this frame
    // (true: Process other steps, false: this step is the last to process input this frame)
    public abstract InputResolver.InputResolution ResolveInput(); // TODO Would be great if we can pass Input as a param here, so we don't have global access to it

}
