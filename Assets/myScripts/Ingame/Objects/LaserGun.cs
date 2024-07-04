using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MonoBehaviour
{
    [SerializeField] private bool keepShooting = true;
    [SerializeField] private bool recordShotsToSteamAchievements = false;
    [SerializeField] private float cooldownBetweenShots = 4f;
    [SerializeField] private float cooldownRandomizer = 0.1f;
    [SerializeField] private GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    [SerializeField] private AudioSource shootAudio;
    [SerializeField] private Vector2 soundEffectMinMax = Vector2.one;
    [SerializeField] private Vector2 volumeMinMax = Vector2.one;

    private float savedTime;
    private float shootCooldown = 0f;

    // steam achievement:
    int shotsfired = 0;

    private void Start()
    {
        if (recordShotsToSteamAchievements)
            LoadShotsFired();
    }

    private void LoadShotsFired()
    {
        shotsfired = MySteamAchievementHandler.GetStatProgress(SteamStatNames.STAT_MISSILES_FIRED);
    }

    private void SetAchievementProgressMissileShotsFired()
    {
        MySteamAchievementHandler.SaveAchievementProgress(shotsfired, SteamStatNames.STAT_MISSILES_FIRED, SteamAchNames.ACHIEVEMENT_MISSILES_FIRED_100);
    }

    // Update is called once per frame
    void Update()
    {
        if (!keepShooting) return;
        if (savedTime + shootCooldown < Time.time)
        {
            Shoot();

            savedTime = Time.time;
        }
    }

    public void Shoot()
    {
        shootAudio.pitch = Random.Range(soundEffectMinMax.x, soundEffectMinMax.y);
        shootAudio.volume = Random.Range(volumeMinMax.x, volumeMinMax.y);
        shootAudio.Play();
        var laser = Instantiate(projectilePrefab, projectileSpawnPoint.position, transform.rotation).GetComponent<LaserProjectile>();

        laser.SetDirection(transform.forward);
        shootCooldown = Random.Range(cooldownBetweenShots - cooldownRandomizer, cooldownBetweenShots + cooldownRandomizer);

        shotsfired++;
        if (recordShotsToSteamAchievements)
            SetAchievementProgressMissileShotsFired();
    }


}
