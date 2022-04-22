using System;
using System.Collections;
using System.Collections.Generic;
using MainGame;
using MikroFramework.Architecture;
using MikroFramework.Event;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    private Vector3 targetPos;

    private float xInterval;
    private float yInterval;
    private Vector2 startLocation;
    private IMapGenerationModel mapModel;

    private Camera camera;
    private float lastScroolValue = 0;
    private float moveCamWhenScrollCooldown = 0.5f;


    private void Awake() {
        targetPos = new Vector3(transform.position.x, transform.position.y, -30);
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

        targetPos = new Vector3((xRange.x + xRange.y) / 2, yRange.x,-30);
    }

    private void Update() {
        moveCamWhenScrollCooldown -= Time.deltaTime;
        if (Mathf.Abs(lastScroolValue) < 0.01f && lastScroolValue!= Input.GetAxis("Mouse ScrollWheel") && moveCamWhenScrollCooldown<=0) {
            Vector3 pos = camera.ScreenToWorldPoint(Input.mousePosition);
            targetPos = new Vector3(pos.x, pos.y, targetPos.z);
            moveCamWhenScrollCooldown = 0.5f;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            CardGame.Interface.ReBoot();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        lastScroolValue = Input.GetAxis("Mouse ScrollWheel");

        if (lastScroolValue > 0) {
            targetPos -= new Vector3(0,0,Time.deltaTime * scrollSpeed);
            
        }

        if (lastScroolValue < 0)
        {
            targetPos += new Vector3(0, 0, Time.deltaTime * scrollSpeed);

        }

        camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, sizeRange.x, sizeRange.y);

        UpdateCameraPosRange();

        if (Input.GetKey(KeyCode.W)) {
            targetPos += new Vector3(0, cameraSpeed * Time.deltaTime,0);
        } else if (Input.GetKey(KeyCode.S)) {
            targetPos -= new Vector3(0, cameraSpeed * Time.deltaTime,0);
        } else if (Input.GetKey(KeyCode.A)) {
            targetPos -= new Vector3(cameraSpeed * Time.deltaTime, 0,0);
        } else if (Input.GetKey(KeyCode.D)) {
            targetPos += new Vector3(cameraSpeed * Time.deltaTime, 0,0);
        }

        targetPos = new Vector3(Mathf.Clamp(targetPos.x, xRange.x, xRange.y),
            Mathf.Clamp(targetPos.y, yRange.x, yRange.y), Mathf.Clamp(targetPos.z, -50,10));

       

        transform.position = Vector3.Lerp(transform.position,
            targetPos, lerp);
    }

    private void UpdateCameraPosRange() {
        float widthRange = camera.aspect * camera.fieldOfView;
        float heightRange = camera.fieldOfView;

        float minX = startLocation.x + widthRange - 5;
        float maxX = startLocation.x + xInterval * mapModel.PathWidth - widthRange + 130;
        xRange = new Vector2(minX, maxX);

        float minY = startLocation.y + heightRange - 45;
        float maxY = startLocation.y + yInterval * mapModel.PathDepth - widthRange + 145;
        yRange = new Vector2(minY, maxY);
    }
}
