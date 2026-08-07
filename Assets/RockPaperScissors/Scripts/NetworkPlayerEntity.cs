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
        public event Action ChoiceChanged;
        public event Action ScoreChanged;

        [Networked, OnChangedRender(nameof(NotifyChoiceChanged))]
        public RpsChoice Choice { get; set; }

        [Networked, OnChangedRender(nameof(NotifyScoreChanged))]
        public int Score { get; set; }

        public void SelectChoice(RpsChoice choice)
        {
            Choice = choice;
        }

        public void AddPoint()
        {
            Score++;
        }

        public void ResetChoice()
        {
            Choice = RpsChoice.None;
        }

        public void Reset()
        {
            Choice = RpsChoice.None;
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
