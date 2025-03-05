using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NUnit.Framework;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random; // If you want to default to Unity's Random


public class RibbitRoyaleLobby : MonoBehaviour{
    // Used to join the server
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    // Singleton instance 
    public static RibbitRoyaleLobby Instance { get; private set;}
    // Trigger this event when the lobby list changes 
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged; 
    public class OnLobbyListChangedEventArgs : EventArgs{
        // List of lobbys that are available to join
        public List<Lobby> lobbyList;
    }
    // The lobby the user is currently in 
    private Lobby joinedLobby;
    // Heartbeat timer is used to help keep the lobby alive (active)
    private float heartbeatTimer;
    // Timer for diplaying a lobby periodically
    private float listLobbiesTimer;
    

    private void Awake(){
        // Set a singleton instance 
        Instance = this;
        // Prevent this from being destoyed when the scene is loaded
        DontDestroyOnLoad(gameObject);
        // Initialize the authentifacation service
        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication(){
        // Check if unity services are initialize 
        if (UnityServices.State != ServicesInitializationState.Initialized){
            // Create initialization options
            InitializationOptions initializationOptions = new InitializationOptions();
            // Assign a random profile id 
            initializationOptions.SetProfile(Random.Range(0,1000).ToString());
            // Initialize the unity services
            await UnityServices.InitializeAsync();
            // Sign in anonymously
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Update(){
        // Maintain the lobby heartbeat
        HandleHeartbeat();
        // Periodically list available lobbies
        HandlePeriodicListLobbies();
    }

    private void HandlePeriodicListLobbies(){
        // Check if the user has not joined a lobby yet, user is authenticatied and the active scene is the network lobby scene
        if(joinedLobby == null && AuthenticationService.Instance.IsSignedIn && 
        SceneManager.GetActiveScene().name == Loader.Scene.NetworkLobbyScene.ToString()){
            // Reduce the timer
            listLobbiesTimer -= Time.deltaTime;
            // When the timer reaches 0 
            if(listLobbiesTimer <= 0f){
                // reset the time 
                float listLobbiesTimerMax = 3f;
                // set the lobby timer to the max timer lobby 
                listLobbiesTimer = listLobbiesTimerMax;
                _ = ListLobbies();
            }
        }
    }

    private void HandleHeartbeat(){
        // If the user is the host
        if(IsLobbyHost()){
            // reduce the heartbeat timer
            heartbeatTimer -= Time.deltaTime;
            // Once the timer reaches 0
            if(heartbeatTimer <= 0f){
                // The max the heartbeat timer is at is 15 seconds 
                float heartbeatTimerMax = 3f;
                // Set the heartbeat timer to 
                heartbeatTimer = heartbeatTimerMax;
                //Send the heartbeat to the lobby to make sure it stays active
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    // This function will check if the player is the host
    private bool IsLobbyHost(){
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async Task ListLobbies(){
        try{
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions{
                Filters = new List<QueryFilter>{
                    // Filter for lobbies with available slots 
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0",QueryFilter.OpOptions.GT)
                }
            };
            // Get lobby list 
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            // Trigger event with updated lobby list 
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs{
                lobbyList = queryResponse.Results
            });
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async Task<Allocation> AllocateRelay(){
        try{
            // Allocate relay with max players
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(RibbitRoyaleMultiplayer.MAX_PLAYER_AMOUNT -1);
            return allocation;

        } catch(RelayServiceException e){
            Debug.Log(e);
            return default;
        }     
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation){
        try{
            // Get the relay join code, passing in the allocation id
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }catch(RelayServiceException e){
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode){
        try{
            // Join the allocation, by passing a string that is the relay join code
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }catch (RelayServiceException e){
            Debug.Log(e);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate){
        try{
            // Allocates a lobby with the lobby name, max player amount (4), and whether or not the user selects private or public lobby
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, RibbitRoyaleMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions{
            IsPrivate = isPrivate,
            });
            // Allocate the relay service
            Allocation allocation = await AllocateRelay();
            // Create a relay join code based off of the allocation on the relay
            string relayJoinCode = await GetRelayJoinCode(allocation);
            // Update the lobby list with the name of the lobby
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>{
                    {KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                }
            });
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            // Start the host session
            RibbitRoyaleMultiplayer.Instance.StartHost();
            // Load the character select scene over the network
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    public async void QuickJoin(){
        try {
            // JoinedLobby is set to information of the lobby used from quick join function
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            // Retrieve the relay join code from the data of joined lobby 
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            // Join the relay lobby from the relay join code 
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            // Client correctly connects to the unity relay service and securely communicates with other players using DTLS
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            // Start a client instance
            RibbitRoyaleMultiplayer.Instance.StartClient();
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

        public async void JoinWithId(string lobbyId){
        try{
            // Set the joined lobby to the lobby id that is joining through clicking on the lobby name
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            // Relay join code is retrieved from the data dictionary using the relay join code key
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            // Join the relay server using the relay join code
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            // Client correctly connects to the unity relay service and securely communicates with other players using DTLS
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            // Start the client instance
            RibbitRoyaleMultiplayer.Instance.StartClient();
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    public async void JoinWithCode(string lobbyCode){
        try{
            // Set the joined lobby to the lobby the user joins by using the code 
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            // Set the relay join code to the joined lobbies relay koin code value
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            // Allocate the join 
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            // Client correctly connects to the unity relay service and securely communicates with other players using DTLS
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            // Create a client instance
            RibbitRoyaleMultiplayer.Instance.StartClient();
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    // When a lobby is deleted (ended) the delete lobbby asyc function will be used passing the id of the lobby and set the joined lobby variable to null
    public async void LeaveLobby(){
        if (joinedLobby != null){
            try{
                Debug.Log($"[RibbitRoyaleLobby] Leaving lobby: {joinedLobby.Id}");
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                joinedLobby = null;
                Debug.Log("[RibbitRoyaleLobby] Successfully left the lobby.");
            } catch(LobbyServiceException e){
                Debug.LogError($"[RibbitRoyaleLobby] Error leaving lobby: {e}");
            }
        } else {
            Debug.LogWarning("[RibbitRoyaleLobby] LeaveLobby() called, but there is no joined lobby.");
        }
    }

    public async void DeleteLobby(){
        if (joinedLobby != null){
            try{
                Debug.Log($"[RibbitRoyaleLobby] Deleting lobby: {joinedLobby.Id}");
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
                Debug.Log("[RibbitRoyaleLobby] Successfully deleted the lobby.");
            } catch(LobbyServiceException e){
                Debug.LogError($"[RibbitRoyaleLobby] Error deleting lobby: {e}");
            }
        } else {
            Debug.LogWarning("[RibbitRoyaleLobby] DeleteLobby() called, but there is no joined lobby.");
        }
    }


    // This function will return the lobby the user has connected to
    public Lobby GetLobby(){
        return joinedLobby;
    }
}
