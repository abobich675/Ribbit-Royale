using System;
using System.Collections.Generic;
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

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    
    public static RibbitRoyaleLobby Instance { get; private set;}

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs{
        public List<Lobby> lobbyList;
    }

    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float listLobbiesTimer;

    private void Awake(){
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication(){
        if (UnityServices.State != ServicesInitializationState.Initialized){
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0,1000).ToString());
            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private void Update(){
        HandleHeartbeat();
        HandlePeriodicListLobbies();

    }

    private void HandlePeriodicListLobbies(){
        if(joinedLobby == null && AuthenticationService.Instance.IsSignedIn && 
        SceneManager.GetActiveScene().name == Loader.Scene.NetworkLobbyScene.ToString()){

            listLobbiesTimer -= Time.deltaTime;
            if(listLobbiesTimer <= 0f){
                float listLobbiesTimerMax = 3f;
                listLobbiesTimer = listLobbiesTimerMax;
                _ = ListLobbies();
            }
        }
    }

    private void HandleHeartbeat(){
        if(IsLobbyHost()){
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer <= 0f){
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost(){
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async Task ListLobbies(){
        try{
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions{
                Filters = new List<QueryFilter>{
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0",QueryFilter.OpOptions.GT)
                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs{
                lobbyList = queryResponse.Results
            });
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    private async Task<Allocation> AllocateRelay(){
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(RibbitRoyaleMultiplayer.MAX_PLAYER_AMOUNT -1);
            return allocation;

        } catch(RelayServiceException e){
            Debug.Log(e);
            return default;
        }     
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation){
        try{
        string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        return relayJoinCode;
        }catch(RelayServiceException e){
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode){
        try{
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        return joinAllocation;
        }catch (RelayServiceException e){
            Debug.Log(e);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate){
        try{
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, RibbitRoyaleMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions{
            IsPrivate = isPrivate,
            });
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>{
                    {KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            RibbitRoyaleMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

    public async void QuickJoin(){
        try {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            RibbitRoyaleMultiplayer.Instance.StartClient();
        } catch (LobbyServiceException e){
            Debug.Log(e);
        }
    }

        public async void JoinWithId(string lobbyId){
        try{
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));


            RibbitRoyaleMultiplayer.Instance.StartClient();
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    public async void JoinWithCode(string lobbyCode){
        try{
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));


            RibbitRoyaleMultiplayer.Instance.StartClient();
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    public async void DeleteLobby(){
        if (joinedLobby != null){
            try{
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            } catch(LobbyServiceException e){
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby(){
        if (joinedLobby != null){
            try{
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;
            } catch(LobbyServiceException e){
                Debug.Log(e);
            }
        }
    }

    public Lobby GetLobby(){
        return joinedLobby;
    }
}
