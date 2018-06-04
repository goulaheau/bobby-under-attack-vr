using System;
using UnityEngine;

public class TreasureController : MonoBehaviour
{
    // Specify if this treasure is the launcher of the game.
    public bool Launcher;

    private const float Speed = 1f;

    // When a treasure is targeted by the player.
    public void OnClick()
    {
        // Launch a game if not launched.
        if (Launcher)
        {
            GameManager.Instance.Launch();
            Destroy(gameObject);
        }
        // Score and destroy the treasure.
        else
        {
            if (!GameManager.Instance.Launched)
            {
                return;
            }

            ++GameManager.Instance.Score;
            GameManager.Instance.UpdateScoreText();
            Destroy(gameObject);
        }
    }

    // Move the treasure in the direction of the player.
    private void FixedUpdate()
    {
        if (Launcher)
        {
            return;
        }

        var treasureCenter = gameObject.GetComponent<Collider>().bounds.center;
        var playerCenter = GameManager.Instance.Player.gameObject.GetComponent<Collider>().bounds.center;

        var direction = Vector3.Normalize(playerCenter - treasureCenter);
        transform.Translate(direction * Time.fixedDeltaTime * Speed);
    }

    // Finish the game when a treasure touch the player.
    private void Update()
    {
        if (
            Math.Abs(transform.position.x) > 0.1f ||
            Math.Abs(transform.position.z) > 0.1f
        )
        {
            return;
        }

        GameManager.Instance.Finish();
    }
}