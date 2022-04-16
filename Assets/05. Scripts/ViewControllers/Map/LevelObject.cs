using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Plugins.Core.PathCore;
using MainGame;
using MikroFramework.Architecture;
using MikroFramework.Event;
using MikroFramework.Pool;
using MikroFramework.TimeSystem;
using UnityEngine;

public class LevelObject : AbstractMikroController<CardGame> {
    [SerializeField]
    private LevelType levelType;
    public LevelType LevelType {
        get => levelType;
        set => levelType = value;
    }

    [HideInInspector]
    public GraphVertex Node;

    private Animator animator;

    public bool Interactable = false;

    [SerializeField] private Sprite[] levelTypeSirSprites;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        this.GetModel<IMapStateModel>().CurNode.RegisterWithInitValue(OnALevelSelected)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        Node.Value.LevelType.RegisterWithInitValue(OnNodeTypeChange).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNodeTypeChange(LevelType prevType, LevelType curType) {
        this.GetComponent<SpriteRenderer>().sprite = levelTypeSirSprites[(int) curType];
    }

    private void OnALevelSelected(GraphVertex prevNode, GraphVertex newSelectedLevel) {
        if (newSelectedLevel == null){ //floor 0 or load save data
            if (Node.Value.PointOnMap.y == this.GetModel<IMapGenerationModel>().PathDepth) {
                OnPlayerMeet();
            }
        }
        else {
            if (prevNode == null && newSelectedLevel!=Node) { //floor 0 selected
                //OnPlayerLeave();
                if (Node.Value.PointOnMap.y == this.GetModel<IMapGenerationModel>().PathDepth) {
                    OnPlayerLeave();
                }
            }

            if (prevNode == Node) {
                Node.Neighbours.ForEach(node => {
                    if (node != newSelectedLevel)
                    {
                        node.Value.LevelObject.GetComponent<LevelObject>().OnPlayerLeave();
                    }
                });
                this.GetSystem<ITimeSystem>().AddDelayTask(0.02f, () => {
                    OnPlayerMoveToNext();
                });

            }

            if (newSelectedLevel == Node) {
                //selected this
                OnPlayerSelect();
                this.GetSystem<ITimeSystem>().AddDelayTask(0.05f, () => {
                    Node.Neighbours.ForEach(node => {
                        //Debug.Log($"Select:{gameObject.name}, Meet:{node.LevelObject.gameObject.name}");
                        node.Value.LevelObject.GetComponent<LevelObject>().OnPlayerMeet();
                    });
                });

            }

           
        } 
    }

    private void OnPlayerMeet() {
      
        animator.SetBool("PlayerReach", true);
        Interactable = true;
        //Debug.Log(gameObject.name +" "+  Interactable);
    }

    private void OnPlayerSelect() {
        animator.SetBool("PlayerComplete", true);
        Interactable = false;
    }

    private void OnPlayerLeave() {
        animator.SetBool("PlayerReach", false);
        animator.SetBool("PlayerComplete", false);
        GraphVertex currentSelectedNode = this.GetModel<IMapStateModel>().CurNode;
       
        Interactable = Node.Neighbours.Contains(currentSelectedNode);
    }

    private void OnPlayerMoveToNext() {
        animator.SetBool("PlayerComplete", false);
        GraphVertex currentSelectedNode = this.GetModel<IMapStateModel>().CurNode;

        Interactable = Node.Neighbours.Contains(currentSelectedNode);
    }
}
