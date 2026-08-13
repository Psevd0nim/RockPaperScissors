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

    public class NetworkPlayerEntity : NetworkBehaviour
    {
        public event Action ChoiceChanged;
        public event Action ScoreChanged;

        [Networked, OnChangedRender(nameof(NotifyChoiceChanged))]
        public RPSElementType Choice { get; set; }

        [Networked, OnChangedRender(nameof(NotifyScoreChanged))]
        public int Score { get; set; }

        [Networked]
        public string Nickname { get; set; }

        public override void Spawned()
        {
            PlayerRegistry.Instance.AddPlayerEntity(this);

            if (HasStateAuthority == false)
                return;

            Nickname = PlayerPrefs.GetString("PlayerName", "Player123");
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            PlayerRegistry.Instance.RemovePlayerEntity(this);
        }

        public void SelectChoice(RPSElementType elementType)
        {
            Choice = elementType;
        }

        public void AddPoint()
        {
            Score++;
        }

        public void ResetChoice()
        {
            Choice = RPSElementType.None;
        }

        public void Reset()
        {
            Choice = RPSElementType.None;
            Score = 0;
        }

        private void NotifyChoiceChanged()
        {
            ChoiceChanged?.Invoke();
        }

        private void NotifyScoreChanged()
        {
            ScoreChanged?.Invoke();
        }
    }
}
