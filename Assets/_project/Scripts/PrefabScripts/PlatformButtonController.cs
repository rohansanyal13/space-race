using UnityEngine;

public class PlatformButtonController : ButtonController
{
    [SerializeField] private PlatformController platformController;

    protected override void OnButtonPressed()
    {
        platformController?.SetRaised(true);
    }

    protected override void OnButtonReleased()
    {
        platformController?.SetRaised(false);
    }
}
