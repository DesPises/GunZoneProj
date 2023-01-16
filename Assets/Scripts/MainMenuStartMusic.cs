using UnityEngine;

public class MainMenuStartMusic : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        SoundManager.soundManager.PlayOST(0);
    }
}
