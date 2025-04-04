using AkanyaTools.UISystem;
using GameCore.UI.PnlWin;
using UnityEngine;

namespace GameCore.Environment
{
    public sealed class GameEndEntry : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Debug.Log("Game End");
                UISystem.Show<PnlWin>();
            }
        }
    }
}