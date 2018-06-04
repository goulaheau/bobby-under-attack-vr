    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool Launched;

    public int Score;

    public GameObject Player;

    [SerializeField] private GameObject[] _treasurePrefabs;
    [SerializeField] private Text _scoreText;
    [SerializeField] private Text _finishText;

    private float _respawnTime = 3f;

    private List<GameObject> _treasuresSpawned = new List<GameObject>();

    // Launch a game.
    public void Launch()
    {
        if (Launched)
        {
            Debug.LogError("GameManager: trying to launch a game when one is already launched.");
            return;
        }

        Launched = true;
        Score = 0;
        _respawnTime = 3f;
        _finishText.text = "";
        _scoreText.text = "Score : " + Score;
        _treasuresSpawned = new List<GameObject>();

        StartCoroutine(CreateRandomTreasure());
        StartCoroutine(ReduceRespawnTime());
    }

    
    public void Finish()
    {
        if (!Launched)
        {
            return;
        }

        Launched = false;

        foreach (var treasure in _treasuresSpawned)
        {
            if (treasure.gameObject)
            {
                Destroy(treasure.gameObject);                
            }
        }

        _scoreText.text = "";
        _finishText.text = "Perdu !\nScore : " + Score + "\nLe but est de détruire le plus de boules\navant que l'une d'entre elles ne vous touche.\nRegardez la boule afin de commencer la partie.";

        CreateLauncherTreasure();
    }

    // Update the UI.
    public void UpdateScoreText()
    {
        if (!_scoreText)
        {
            Debug.LogError("GameManager: no Score Text attached to the Game Manager.");
            return;
        }

        _scoreText.text = "Score : " + Score;
    }

    // Get a singleton.
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one GameManager in scene.");
            return;
        }

        Instance = this;
    }

    // Called when the game is launched.
    // It will spawn the first treasure in front of the player.
    private void Start()
    {
        Score = 0;
        _finishText.text =
            "Le but est de détruire le plus de boules\navant que l'une d'entre elles ne vous touche.\nRegardez la boule afin de commencer la partie.";

        CreateLauncherTreasure();
    }

    // Reduce the respawn time every second maximum.
    private IEnumerator ReduceRespawnTime()
    {
        yield return new WaitForSeconds(1f);

        _respawnTime *= 0.99f;

        if (!Launched)
        {
            yield break;
        }

        StartCoroutine(ReduceRespawnTime());
    }

    private void CreateLauncherTreasure()
    {
        if (_treasurePrefabs[0] == null)
        {
            Debug.LogError("GameManager: no treasures prefab to spawn.");
            return;
        }

        Spawn(_treasurePrefabs[0], new Vector3(0f, 0.75f, 2f), true);
    }

    // Create a random treasure every respawnTime time.
    private IEnumerator CreateRandomTreasure()
    {
        var treasurePrefabIndex = Random.Range(0, _treasurePrefabs.Length);
        var treasurePrefab = _treasurePrefabs[treasurePrefabIndex];

        // Move to random new location ±180º horizontal.
        var direction = Quaternion.Euler(
                            0,
                            Random.Range(-180, 180),
                            0
                        ) * Vector3.forward;

        // New location at 7.5m.
        const float distance = 7.5f;
        var newPos = direction * distance;
        newPos.y = 1.5f;

        if (!Launched)
        {
            yield break;
        }

        Spawn(treasurePrefab, newPos, false);

        yield return new WaitForSeconds(_respawnTime);

        StartCoroutine(CreateRandomTreasure());
    }

    // Instantiate a treasure.
    private void Spawn(GameObject treasurePrefab, Vector3 position, bool launcher)
    {
        var treasure = Instantiate(treasurePrefab);

        treasure.transform.localPosition = position;
        treasure.GetComponent<TreasureController>().Launcher = launcher;
        treasure.gameObject.SetActive(true);

        if (launcher)
        {
            treasure.GetComponent<AudioSource>().enabled = false;
        }

        _treasuresSpawned.Add(treasure.gameObject);
    }
}