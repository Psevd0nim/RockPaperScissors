using System;
using Fusion;
using UnityEngine;

namespace MyProject
{
    public enum RpsRoundResult : byte
    {
        Win,
        Lose,
        Draw
    }

    public class NetworkPlayerEntity : NetworkBehaviour, IAfterSpawned
    {
        public event Action SelectedElementChanged;
        public event Action ScoreChanged;

        [Networked, OnChangedRender(nameof(NotifySelectedElementChanged))]
        public RPSElementType SelectedElement { get; set; }

        [Networked, OnChangedRender(nameof(NotifyScoreChanged))]
        public int Score { get; set; }

        [Networked]
        public string Nickname { get; set; }

        public override void Spawned()
        {
            if (HasStateAuthority == false)
                return;

            Nickname = PlayerPrefs.GetString("PlayerName", "Player123");
        }

        public void AfterSpawned()
        {
            PlayerRegistry.Instance.AddPlayerEntity(this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            PlayerRegistry.Instance.RemovePlayerEntity(this);
        }

        public void SetSelectedElement(RPSElementType elementType)
        {
            SelectedElement = elementType;
        }

        public void AddPoint()
        {
            Score++;
        }

        public void ResetSelectedElement()
        {
            SelectedElement = RPSElementType.None;
        }

        public void Reset()
        {
            SelectedElement = RPSElementType.None;
            Score = 0;
        }

        private void NotifySelectedElementChanged()
        {
            SelectedElementChanged?.Invoke();
        }

        private void NotifyScoreChanged()
        {
            ScoreChanged?.Invoke();
        }
    }
}
