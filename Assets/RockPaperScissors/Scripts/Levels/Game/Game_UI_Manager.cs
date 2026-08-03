using UnityEngine;

namespace MyProject
{
    public class Game_UI_Manager : UI_Manager
    {
        [SerializeField] private Transition_UI _transition_UI;

        public void SetTransitionStatus(bool status)
        {
            _transition_UI.SetTransitionStatus(status);
        }
    }
}
