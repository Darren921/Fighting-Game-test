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
            if (players[1].transform.position.x < players[0].transform.position.x)
            {
                players[0].reversed = true;
                players[1].reversed = false;
            }
            else
            {
                players[0].reversed = false;
                players[1].reversed = true;

            }
        }

}
