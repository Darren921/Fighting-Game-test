using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private PlayerController _playerController;

    [SerializeField] private Slider _slider;


    private void Awake()
    {
        HitDetection.OnPlayerHit += UpdateHealth;
        _playerController = GetComponent<PlayerController>();
      
    }

    private void OnDestroy()
    {
        HitDetection.OnPlayerHit -= UpdateHealth;    
    }

    private void Start()
    {
        _slider.maxValue = _playerController.CharacterData.health;
        _slider.value = _playerController.CharacterData.health;
        UpdateHealth();
    }



    private void UpdateHealth()
    {
        _slider.value = _playerController.Health;
    }
}