using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    private bool _paused;
    private readonly List<PlayerController> _players = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterPlayer(PlayerController player)
    {
        if (!_players.Contains(player))
            _players.Add(player);
    }

    public void UnregisterPlayer(PlayerController player)
    {
        _players.Remove(player);
    }

    public void SetPaused(bool paused)
    {
        _paused = paused;
        Time.timeScale = paused ? 0f : 1f;

        foreach (var player in _players)
        {
            if (paused)
                player.OnDisablePlayer();
            else
                player.OnEnablePlayer();
        }
    }

    public bool IsPaused => _paused;
}