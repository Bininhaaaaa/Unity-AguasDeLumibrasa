using System;
using EasyPeasyFirstPersonController;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class GameplayUIController : MonoBehaviour
{
    [Header("References")]
    public Camera gameplayCamera;
    public GameObject gameplayUI;
    public GameObject SniperAimUI;
    public GameObject CardSelectorUI;
    public GameObject hitParticles;

    [Header("Gun Settings")]
    public Transform gunTransform;     // você só arrasta o transform da arma
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shootClip;

    [Header("Camera Shake")]
    public float shakeDuration = 0.1f;
    public float shakeMagnitude = 0.05f;

    [Header("Gun Recoil")]
    public float recoilKickback = 0.1f;
    public float recoilRotation = 4f;
    public float recoilRecoverySpeed = 8f;

    [Header("Bullet Trail")]
    public Material bulletMaterial;   // neon-like material
    public float bulletWidth = 0.03f;
    public float bulletFadeTime = 0.15f;

    private Vector3 originalCamPos;
    private bool isShaking = false;

    private Vector3 originalGunPos;
    private Quaternion originalGunRot;
    private Vector3 recoilPos;
    private Vector3 recoilRot;
    private bool isAiming;


    void Start()
    {
        FirstPersonController.OnPressAim += ShowSniperAim;
        FirstPersonController.OnReleaseAim += HideSniperAim;
        FirstPersonController.OnPressTab += ShowCardSelector;
        FirstPersonController.OnReleaseTab += HideCardSelector;

        CardSelectorUI.SetActive(false);

        originalCamPos = gameplayCamera.transform.localPosition;

        originalGunPos = gunTransform.localPosition;
        originalGunRot = gunTransform.localRotation;
    }

    void OnDestroy()
    {
        FirstPersonController.OnPressAim -= ShowSniperAim;
        FirstPersonController.OnReleaseAim -= HideSniperAim;
        FirstPersonController.OnPressTab -= ShowCardSelector;
        FirstPersonController.OnReleaseTab -= HideCardSelector;
    }

    void Update()
    {
        // SHOOT
        if (Input.GetMouseButtonDown(0))
        {
            if (muzzleFlash != null)
                muzzleFlash.Play();

            if (audioSource != null && shootClip != null)
                audioSource.PlayOneShot(shootClip);

            ApplyGunRecoil();
            StartCoroutine(CameraShake());

            ShootRay();
        }

        UpdateGunRecoil();
    }

    private void ShootRay()
    {
        Vector3 origin = gameplayCamera.transform.position;
        Vector3 direction = gameplayCamera.transform.forward;

        float maxDistance = 500f;
        float distance = maxDistance;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
        {
            GameObject particlesObj = Instantiate(hitParticles, hit.point, Quaternion.identity);
            particlesObj.transform.position = hit.point;
            Destroy(particlesObj, 1f);

            // Exemplo de dano
            // hit.collider.GetComponent<Health>()?.TakeDamage(20);
        }

        // VisualizeRay(origin, direction, distance);
    }

    private void VisualizeRay(Vector3 origin, Vector3 direction, float length)
    {
        GameObject lineObj = new GameObject("BulletTrail");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        lr.material = bulletMaterial;
        lr.startWidth = bulletWidth;
        lr.endWidth = bulletWidth;
        lr.positionCount = 2;

        Vector3 endPoint = origin + direction.normalized * length;

        lr.SetPosition(0, origin);
        lr.SetPosition(1, endPoint);

        StartCoroutine(FadeAndDestroyLine(lr, bulletFadeTime));
    }

    private System.Collections.IEnumerator FadeAndDestroyLine(LineRenderer lr, float time)
    {
        float elapsed = 0;
        Color startColor = lr.material.color;

        while (elapsed < time)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / time);
            lr.material.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(lr.gameObject);
    }


    // -----------------------------------------------------------
    // GUN RECOIL
    // -----------------------------------------------------------
    private void ApplyGunRecoil()
    {
        recoilPos -= new Vector3(0, 0, recoilKickback);
        recoilRot += new Vector3(-recoilRotation, 0, 0);
    }

    private void UpdateGunRecoil()
    {
        recoilPos = Vector3.Lerp(recoilPos, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
        recoilRot = Vector3.Lerp(recoilRot, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);


        gunTransform.localPosition = originalGunPos + recoilPos;
        gunTransform.localRotation = originalGunRot * Quaternion.Euler(recoilRot);
    }


    // -----------------------------------------------------------
    // CAMERA SHAKE
    // -----------------------------------------------------------
    private System.Collections.IEnumerator CameraShake()
    {
        if (isShaking) yield break;
        isShaking = true;

        float elapsed = 0f;

        float shakeForce = isAiming ? shakeMagnitude * 3f : shakeMagnitude;

        while (elapsed < shakeDuration)
        {
            float x = (Mathf.PerlinNoise(Time.time * 20f, 0f) - 0.5f) * 2f * shakeForce;
            float y = (Mathf.PerlinNoise(0f, Time.time * 20f) - 0.5f) * 2f * shakeForce;

            gameplayCamera.transform.localPosition = originalCamPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        gameplayCamera.transform.localPosition = originalCamPos;
        isShaking = false;
    }


    // -----------------------------------------------------------
    // UI & AIM
    // -----------------------------------------------------------
    private void ShowSniperAim()
    {
        isAiming = true;
        gameplayCamera.fieldOfView = 20;
        gameplayUI.SetActive(false);
        gunTransform.gameObject.SetActive(false);
        SniperAimUI.SetActive(true);
    }

    private void HideSniperAim()
    {
        isAiming = false;
        gameplayUI.SetActive(true);
        SniperAimUI.SetActive(false);
        gunTransform.gameObject.SetActive(true);
        gameplayCamera.fieldOfView = 60;
    }

    private void ShowCardSelector()
    {
        CardSelectorUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0.1f;
    }

    private void HideCardSelector()
    {
        CardSelectorUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }
}
