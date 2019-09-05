using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TestAtlas : MonoBehaviour
{
    private Image _img;
    // Start is called before the first frame update

    void Awake()
    {
        _img = GetComponent<Image>();
        print("Awake");
        SpriteAtlasManager.atlasRequested += SpriteAtlasManagerOnAtlasRequested;
        SpriteAtlasManager.atlasRegistered += atlas => print($"Register atlas  {atlas.name}");
    }

    private void SpriteAtlasManagerOnAtlasRequested(string arg1, Action<SpriteAtlas> cb)
    {
        print("request is called");
        var atlas_en = Resources.Load<SpriteAtlas>("en");
        print(atlas_en.CanBindTo(_img.sprite));
        cb(atlas_en);
        _img.sprite = atlas_en.GetSprite(_img.sprite.name);
        // var atlas_cn = Resources.Load<SpriteAtlas>("cn");
        // print(atlas_cn.CanBindTo(_img.sprite));
        // cb(atlas_cn);
    }

    void OnEnable()
    {
        print("OnEnable");
    }
    void Start()
    {
        print("start");
    }

    private bool _updated;
    // Update is called once per frame
    void Update()
    {
        if(_updated) return;

        _updated = true;
        print("update");

    }
}
