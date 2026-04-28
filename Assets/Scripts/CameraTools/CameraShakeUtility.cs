using UnityEngine;
using DG.Tweening;
using System.Collections;

/// <summary>
/// 独立的相机震动方法，通过叠加偏移实现，不干扰其他相机移动
/// </summary>
public class CameraShakeUtility : MonoBehaviour
{
    private bool isShaking = false;
    private Coroutine shakeCoroutine;
    private Vector3 originalPosition;
    private Camera targetCamera;

    /// <summary>
    /// 执行相机震动
    /// </summary>
    /// <param name="camera">目标相机</param>
    /// <param name="intensity">震动强度</param>
    /// <param name="duration">震动持续时间</param>
    /// <param name="frequency">震动频率（越高震动越密集）</param>
    public void Shake(float intensity, float duration, float frequency = 30f)
    {
        var camera = GetComponent<Camera>();
        if (camera == null) return;

        targetCamera = camera;

        // 如果正在震动，先停止当前震动
        if (isShaking && shakeCoroutine != null && camera.gameObject.activeInHierarchy)
        {
            camera.GetComponent<MonoBehaviour>().StopCoroutine(shakeCoroutine);
        }

        // 记录当前位置作为基准位置
        originalPosition = camera.transform.position;

        // 启动新的震动协程
        shakeCoroutine = camera.GetComponent<MonoBehaviour>().StartCoroutine(ShakeCoroutine(intensity, duration, frequency));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration, float frequency)
    {
        isShaking = true;
        float elapsed = 0f;
        float interval = 1f / frequency; // 震动间隔时间
        float timer = 0f;

        while (elapsed < duration)
        {
            if (targetCamera == null) break;

            // 按频率更新震动偏移
            if (timer >= interval)
            {
                // 计算衰减后的强度（随时间减弱）
                float currentIntensity = intensity * (1 - (elapsed / duration));

                // 生成随机偏移（基于原始位置的相对偏移）
                Vector3 offset = new Vector3(
                    Random.Range(-currentIntensity, currentIntensity),
                    Random.Range(-currentIntensity * 0.5f, currentIntensity * 0.5f), // Y轴震动较弱
                    0
                );

                // 应用偏移（叠加在原始位置上）
                targetCamera.transform.position = originalPosition + offset;

                timer = 0f;
            }

            timer += Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 震动结束后恢复原始位置
        if (targetCamera != null)
        {
            targetCamera.transform.position = originalPosition;
        }

        isShaking = false;
    }
}
