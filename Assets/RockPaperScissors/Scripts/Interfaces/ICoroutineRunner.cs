using System.Collections;
using UnityEngine;

namespace MyProject
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator enumerator);
    }
}