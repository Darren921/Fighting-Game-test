using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    internal PlayerController _playerController;

    [SerializeField] private Slider _slider;


    private void Awake()
    {
        HitDetection.OnPlayerHit += UpdateHealth;
    }

    private void OnDestroy()
    {
        HitDetection.OnPlayerHit -= UpdateHealth;    
    }

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _slider.maxValue = _playerController.CharacterData.health;
        _slider.value = _playerController.CharacterData.health;
    }

    private void Update()
    {
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        _slider.value = _playerController.Health;
    }
}