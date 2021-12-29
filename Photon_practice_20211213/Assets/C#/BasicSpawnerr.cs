using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;           //�ޥ�Fusion�R�W�Ŷ�
using Fusion.Sockets;
using System.Collections.Generic;
using System;

//INetworkRunnerCallbacks �s�u���澹�^�I����.Runner���澹�B�z�欰��|�^�I����������k
/// <summary>
/// �s�u�򩳥ͦ���
/// </summary>
public class BasicSpawnerr : MonoBehaviour, INetworkRunnerCallbacks
{
    #region ���
    [Header("�Ш��P�[�J�ж����")]
    public InputField inputFieldCreateRoom;
    public InputField inpubtFieldJoinRoom;
    [Header("���a�����")]
    public NetworkPrefabRef goPlayer;
    [Header("�e���s�u")]
    public GameObject goCanvas;
    [Header("������r")]
    public Text textVersion;
    [Header("���a�ͦ���m")]
    public Transform[] traSpawnPoints;

    /// <summary>
    /// ���a��J�ж��W��
    /// </summary>
    private string roomNameInput;
    /// <summary>
    /// �s�u���澹
    /// </summary>
    private NetworkRunner runner;
    /// <summary>
    /// ���a��ƶ��X: ���a�ѦҸ�T�A���a�s�u����
    /// </summary>
    private Dictionary<PlayerRef, NetworkObject> players = new Dictionary<PlayerRef, NetworkObject>();
    private string stringVersion = "SHIH Copyright 2022 | Version ";
    #endregion

    #region �ƥ�
    private void Awake()
    {
        textVersion.text = stringVersion + Application.version;
    }
    #endregion

    #region ��k
    /// <summary>
    /// ���s�I���I�s : �Ыةж�
    /// </summary>
    public void BtnCreateRoom()
    {
        roomNameInput = inputFieldCreateRoom.text;
        print("�Ыةж�: " + roomNameInput);
        StartGame(GameMode.Host); //�[�J�ж���Host
    }

    /// <summary>
    /// ���s�I���I�s: �[�J�ж�
    /// </summary>
    public void BtnJoinRoom()
    {
        roomNameInput = inpubtFieldJoinRoom.text;
        print("�[�J�ж�: " + roomNameInput);
        StartGame(GameMode.Client); //�[�J�ж���Client
    }

    //async �D�P�B�B�z: ����t�ήɳB�z�s�u
    /// <summary>
    /// �}�l�s�u�C��
    /// </summary>
    /// <param name="mode">�s�u�Ҧ�: �D���B�Ȥ�</param>
            //�P�B
    private async void StartGame(GameMode mode)
    {
        print("<color=yellow>�}�l�s�u </color>");

        runner = gameObject.AddComponent<NetworkRunner>(); //�s�u���澹  =�K�[����<�s�u���澹>
        runner.ProvideInput = true;                        //�s�u���澹.�O�_���ѿ�J = �O

        //���ݳs�u:�C���s�u�Ҧ��B�ж��W�١B�s�u�᪺�����B�����޲z��
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomNameInput,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        print("<color=yellow>�s�u���� </color>");
        goCanvas.SetActive(false);
    }
    #endregion

    #region Fusion �^�I�禡�ϰ�
    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {

    }

    /// <summary>
    /// ���a�s�u��J�欰
    /// </summary>
    /// <param name="runner">�s�u���澹</param>
    /// <param name="input">��J��T</param>
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData inputData = new NetworkInputData();                    //�s�W �s��u��J��� ���c

        #region �ۭq��J�ץ�P���ʸ�T
        if (Input.GetKey(KeyCode.W)) inputData.direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) inputData.direction += Vector3.back;
        if (Input.GetKey(KeyCode.A)) inputData.direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) inputData.direction += Vector3.right;

        inputData.inputFire = Input.GetKey(KeyCode.Mouse0); //���� �o�g
        #endregion
        #region �ƹ��y�гB�z
        inputData.positionMouse = Input.mousePosition;                                  //���o �ƹ��y��
        inputData.positionMouse.z = 60;                                                 //�]�w �ƹ��y��Z�b-�i�H����3D����A�j����v����Y
        
        Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(inputData.positionMouse); //�z�LAPI�N�ƹ��ର�@�ɮy��
        inputData.positionMouse = mouseToWorld;                                         //�x�s�ഫ�᪺�ƹ��y��
        #endregion
        input.Set(inputData);                               //��J��T.�]�w(�s�u��J��T)



    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    /// <summary>
    /// ���a���\�[�J�ж���
    /// </summary>
    /// <param name="runner">�s�u���澹</param>
    /// <param name="player">���a��T</param>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        //�H���ͦ��I = Unity���H���d��(0,�ͦ���m�ƶq)
        int randomSpawnPoint = UnityEngine.Random.Range(0, traSpawnPoints.Length);
        //�s�u����,�ͦ�(����B�y�СB���סB���a��T)
        NetworkObject playerNetworkObject = runner.Spawn(goPlayer, traSpawnPoints[randomSpawnPoint].position, Quaternion.identity, player);
        //�N���a�ѦҸ�T�P���a�s�u����K�[��r�嶰�X��
        players.Add(player, playerNetworkObject);
    }

    /// <summary>
    /// ���a���}�ж���
    /// </summary>
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //�p�G ���}�����a�s�u���� �s�b �N�R��
        if (players.TryGetValue(player, out NetworkObject playerNetworkObject))
        {
            runner.Despawn(playerNetworkObject);  //�s�u���澹�A�����ͦ�(�Ӫ��a�s�u���󲾰�)
            players.Remove(player);                //���a���X.����(�Ӫ��a)
        }
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }
    #endregion
}
