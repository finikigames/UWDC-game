using Core.Extensions;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public CPUPlayer CPUPlayer;

    private void Awake()
    {
        bool withPlayer = PlayerPrefsX.GetBool("WithPlayer");

        if (!withPlayer) {
            CPUPlayer.enabled = true;
            gameObject.SetActive(true);
        }
    }
}