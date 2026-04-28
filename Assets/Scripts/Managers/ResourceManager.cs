using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    [Header("Entity Prefabs")]
    public List<Entity> entities;
    [Header("Bullet Prefabs")]
    public List<Bullet> bullets;
    [Header("Background Prefabs")]
    public List<Background> backgrounds;
    [Header("Card Prefabs")]
    public List<Card> cards;
    [Header("Effect Prefabs")]
    public List<Effect> effects;
    [Header("Musics")]
    public List<AudioClip> musics;
    [Header("Sounds")]
    public List<AudioClip> sounds;
    [Header("Voices")]
    public List<AudioClip> voices;
    [Header("Materials")]
    public List<Material> materials;
    [Header("Misc Prefabs")]
    public List<GameObject> miscs;
}
