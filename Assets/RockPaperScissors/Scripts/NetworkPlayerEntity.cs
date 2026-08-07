using System;
using Fusion;

namespace MyProject
{
    public enum RpsChoice : byte
    {
        None,
        Rock,
        Paper,
        Scissors
    }

    public enum RpsRoundResult : byte
    {
        Win,
        Lose,
        Draw
    }

    public class NetworkPlayerEntity : NetworkBehaviour
    {
        public event Action EntityChanged;

        public bool CanSelectChoice => HasStateAuthority && Choice == RpsChoice.None;

        [Networked, OnChangedRender(nameof(NotifyEntityChanged))]
        public RpsChoice Choice { get; set; }

        [Networked, OnChangedRender(nameof(NotifyEntityChanged))]
        public int Score { get; set; }

        public void SelectChoice(RpsChoice choice)
        {
            if (CanSelectChoice == false || choice == RpsChoice.None)
                return;

            Choice = choice;
        }

        public void AddPoint()
        {
            if (HasStateAuthority)
                Score++;
        }

        public void ResetChoice()
        {
            if (HasStateAuthority)
                Choice = RpsChoice.None;
        }

        public void ResetMatch()
        {
            if (HasStateAuthority == false)
                return;

            Choice = RpsChoice.None;
            Score = 0;
        }

        private void NotifyEntityChanged()
        {
            EntityChanged?.Invoke();
        }
    }
}
