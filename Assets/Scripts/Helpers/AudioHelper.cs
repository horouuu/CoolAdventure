using UnityEngine;

public static class AudioHelper
{
    public static void playSFX(AudioSource audio)
    {
        audio.PlayOneShot(audio.clip);
    }
}