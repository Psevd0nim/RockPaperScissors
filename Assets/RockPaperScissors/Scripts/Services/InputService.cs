using UnityEngine;

namespace MyProject
{
    public class InputService : IService
    {
        private InputSystem_Actions _inputActions;

        public InputService()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
        }
    }
}