using Fusion;
using UnityEngine;

/// <summary>
/// 連線輸入資料
/// 保存連線玩家輸入資料
/// </summary>
public struct NetworkInputData: INetworkInput
{
    /// <summary>
    /// 坦克移動方向
    /// </summary>
    public Vector3 direction;
    /// <summary>
    /// 是否點擊左鍵
    /// </summary>
    public bool inputFire;
    /// <summary>
    /// 滑鼠座標
    /// </summary>
    public Vector3 positionMouse;

}
