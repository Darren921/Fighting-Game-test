using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController[] players;

    private List<InputDevice> availableDevices = new ();

    private int minDistance = 1;

    private void Awake()
    {
        foreach (var device in InputSystem.devices.Where(device => device is Gamepad or Keyboard))
        {
            availableDevices.Add(device);
        }
        onConnect();
        InputSystem.onDeviceChange += (device, change) =>
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    onAdd(device);
                    onConnect();
                    break;
                case InputDeviceChange.Reconnected:
                    onConnect();
                    break;
            }
        };
    }

    private void onAdd(InputDevice device)
    {
        if (!availableDevices.Contains(device)) availableDevices.Add(device);
    }

    private void onConnect()
    {
        for (var i = 0; i < players.Length; i++)
        {
            if (i < availableDevices.Count)
            {
                players[i].InitializePlayer(availableDevices[i]);
                Debug.Log($"Assigned {availableDevices[i].name} to Player {i + 1}");
            }
            else
            {
                Debug.LogWarning($"No input device available for Player {i + 1}");
            }

        }
    }

    void Start()
        {
      
        }




        // Update is called once per frame
        void Update()
        {
            checkIfReversed();
        }

        private void checkIfReversed()
        {
            float distance = Mathf.Abs(players[0].transform.position.x - players[1].transform.position.x);

            if (distance < minDistance)
                return;

            if (players[1].transform.position.x < players[0].transform.position.x)
            {
                players[0].Reversed = true;
                players[1].Reversed = false;
            }
            else
            {
                players[0].Reversed = false;
                players[1].Reversed = true;
            }

            UpdatePlayerDirection(players[0]);
            UpdatePlayerDirection(players[1]);
        }

        private void UpdatePlayerDirection(PlayerController player)
        {
            if (player.isGrounded || player.IsAttacking)
            {
                var targetYRotation = player.Reversed ? 180f : 0f;
                var rotation = player.transform.eulerAngles;
                rotation.y = targetYRotation;
                player.transform.eulerAngles = rotation;
            }
          
        }
}
