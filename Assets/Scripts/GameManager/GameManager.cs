using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public class ReadOnlyAttribute : PropertyAttribute { }
    [SerializeField] private PlayerController[] players;
   [SerializeField] private CharacterSODataBase characterDatabase;

    private readonly List<InputDevice> _availableDevices = new ();

    private const int MinDistance = 1;


    private void Start()
    {
        // CHANGE THIS TO ACCEPT INPUT FROM CHARACTER SELECTION, THIS HURTS TO LEAVE
        foreach (var player in players)
        {
            player.CharacterData = characterDatabase.GetCharacterSoDataBase(0);
        }

        Time.timeScale = 1;
        
        HitDetection.OnDeath += OnPlayerDeath;
        Application.targetFrameRate = 60;
    //    Time.timeScale = 0.1f;
    
        // temp method to add devices to a pool in order to connect them to a player 
        foreach (var device in InputSystem.devices.Where(device => device is Gamepad or Keyboard))
        {
            _availableDevices.Add(device);
        }
        OnConnect();
        InputSystem.onDeviceChange += (device, change) =>
        {
            //May need to add removed, disconnected 
            switch (change)
            {
                case InputDeviceChange.Added:
                    OnAdd(device);
                    OnConnect();
                    break;
                case InputDeviceChange.Reconnected:
                    OnConnect();
                    break;
                case InputDeviceChange.Removed:
                    break;
                case InputDeviceChange.Disconnected:
                    break;
            }
        };
    }

    private void OnPlayerDeath()
    {
        foreach (var player in players)
        {
            if(player.Animator is not null)  player.Animator.enabled = false;
            player.hitBox.SetActive(false);
            if (player.Health <= 0)
            {
                player.gameObject.SetActive(false);
            }
            SceneManager.LoadScene("LogicTest");
        }
    }


    private void OnAdd(InputDevice device)
    {
        if (!_availableDevices.Contains(device)) _availableDevices.Add(device);
    }

    private void OnConnect()
    {
        //var input1 = deviceIndex.GetValueOrDefault(players[0]);
        //players[0].InitializePlayer(input1);
        //var input2 = deviceIndex.GetValueOrDefault(players[1]);
        //players[1].InitializePlayer(input2);
        
        //temp method to give a player controls depending on device connected first 
        ConnectPlayer();
    }

    private void ConnectPlayer()
    {
        for (var i = 0; i < players.Length; i++)
        {
            if (i < _availableDevices.Count)
            {
                players[i].InitializePlayer(_availableDevices[i]);
                Debug.Log($"Assigned {_availableDevices[i].name} to Player {i + 1}");
            }
            else
            {
                if(!_availableDevices.Contains( Keyboard.current) ) players[i].InitializePlayer(Keyboard.current);
                else players[i].InitializePlayer(new Gamepad());
//                Debug.LogWarning($"No input device available for Player {i + 1}");
            }

        }
    }


    // Update is called once per frame
        private void Update()
        {
          CheckIfReversed();
          
        }

       

      

        private void CheckIfReversed()
        {
            //depending on the distance between players, and if they are grounded, reverse (flip) the player 
            var distance = Mathf.Abs(players[0].transform.position.x - players[1].transform.position.x);

            if (distance < MinDistance)
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

        private static void UpdatePlayerDirection(PlayerController player)
        {
            if (!player.IsGrounded) return;
            var targetYRotation = player.Reversed ? 270 : 90f;
            var rotation = player.transform.eulerAngles;
            rotation.y = targetYRotation;
            player.transform.eulerAngles = rotation;

        }
}
