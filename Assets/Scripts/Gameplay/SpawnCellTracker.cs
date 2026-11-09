using System;
using UnityEngine;

public class SpawnCellTracker : MonoBehaviour
{
    private Action onDestroyed;
    public void Setup(Action callback) => onDestroyed = callback;
    private void OnDestroy() => onDestroyed?.Invoke();
}