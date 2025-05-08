using Platformer;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public bool hurtsPlayer1 = true;
    public bool hurtsPlayer2 = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1") && hurtsPlayer1)
        {
            GameManager.instance.DamagePlayer("Player1");
        }
        else if (other.CompareTag("Player2") && hurtsPlayer2)
        {
            GameManager.instance.DamagePlayer("Player2");
        }
    }
}
