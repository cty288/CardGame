using System;
using System.Collections;
using System.Collections.Generic;
using MainGame;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;

public class MapCameraControl : AbstractMikroController<CardGame> {
    [SerializeField] 
    private float cameraSpeed = 1;

    [SerializeField] 
    private float lerp = 0.1f;

    private Vector2 xRange;
    private Vector2 yRange;

    [SerializeField]
    private Vector2 sizeRange = new Vector2(10,40);

    [SerializeField]
    private float scrollSpeed = 50;

    private Vector2 targetPos;

    private float xInterval;
    private float yInterval;
    private Vector2 startLocation;
    private IMapGenerationModel mapModel;

    private Camera camera;
    private float lastScroolValue = 0;
    private float moveCamWhenScrollCooldown = 0.5f;


    private void Awake() {
        targetPos = transform.position;
        camera = GetComponent<Camera>();
    }

    private void Start() {
        mapModel = this.GetModel<IMapGenerationModel>();
        this.RegisterEvent<OnMapLoaded>(OnMapGenerated).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnMapGenerated(OnMapLoaded e) {
        xInterval = PathGeneratorViewController.Singleton.CellXInterval;
        yInterval = PathGeneratorViewController.Singleton.CellYInterval;
        startLocation = PathGeneratorViewController.Singleton.startLocation.position;
        UpdateCameraPosRange();

        targetPos = new Vector2((xRange.x + xRange.y) / 2, yRange.x);
    }

    private void Update() {
        moveCamWhenScrollCooldown -= Time.deltaTime;
        if (Mathf.Abs(lastScroolValue) < 0.01f && lastScroolValue!= Input.GetAxis("Mouse ScrollWheel") && moveCamWhenScrollCooldown<=0) {
            targetPos = camera.ScreenToWorldPoint(Input.mousePosition);
            moveCamWhenScrollCooldown = 0.5f;
        }

        lastScroolValue = Input.GetAxis("Mouse ScrollWheel");

        if (lastScroolValue > 0) {
            camera.orthographicSize -= Time.deltaTime * scrollSpeed;
            
        }

        if (lastScroolValue < 0)
        {
            camera.orthographicSize += Time.deltaTime * scrollSpeed;
           
        }

        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, sizeRange.x, sizeRange.y);

        UpdateCameraPosRange();

        if (Input.GetKey(KeyCode.W)) {
            targetPos += new Vector2(0, cameraSpeed * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.S)) {
            targetPos -= new Vector2(0, cameraSpeed * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.A)) {
            targetPos -= new Vector2(cameraSpeed * Time.deltaTime, 0);
        } else if (Input.GetKey(KeyCode.D)) {
            targetPos += new Vector2(cameraSpeed * Time.deltaTime, 0);
        }

        targetPos = new Vector2(Mathf.Clamp(targetPos.x, xRange.x, xRange.y),
            Mathf.Clamp(targetPos.y, yRange.x, yRange.y));

       

        transform.position = Vector3.Lerp(transform.position,
            new Vector3(targetPos.x, targetPos.y, -10), lerp);
    }

    private void UpdateCameraPosRange() {
        float widthRange = camera.aspect * camera.orthographicSize;
        float heightRange = camera.orthographicSize;

        float minX = startLocation.x + widthRange - 5;
        float maxX = startLocation.x + xInterval * mapModel.PathWidth - widthRange + 130;
        xRange = new Vector2(minX, maxX);

        float minY = startLocation.y + heightRange - 15;
        float maxY = startLocation.y + yInterval * mapModel.PathDepth - widthRange + 100;
        yRange = new Vector2(minY, maxY);
    }
}
