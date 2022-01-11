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
    public PathNode Node;

    private Animator animator;

    public bool Interactable = false;

    [SerializeField] private Sprite[] levelTypeSirSprites;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        this.GetModel<IMapStateModel>().CurNode.RegisterWithInitValue(OnALevelSelected)
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        Node.NodeType.RegisterWithInitValue(OnNodeTypeChange).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnNodeTypeChange(LevelType prevType, LevelType curType) {
        this.GetComponent<SpriteRenderer>().sprite = levelTypeSirSprites[(int) curType];
    }

    private void OnALevelSelected(PathNode prevNode, PathNode newSelectedLevel) {
        if (newSelectedLevel == null){ //floor 0 or load save data
            if (Node.Depth == this.GetModel<IMapGenerationModel>().PathDepth) {
                OnPlayerMeet();
            }
        }
        else {
            if (prevNode == null && newSelectedLevel!=Node) { //floor 0 selected
                //OnPlayerLeave();
                if (Node.Depth == this.GetModel<IMapGenerationModel>().PathDepth) {
                    OnPlayerLeave();
                }
            }

            if (prevNode == Node) {
                Node.ConnectedByNodes.ForEach(node => {
                    if (node != newSelectedLevel)
                    {
                        node.LevelObject.GetComponent<LevelObject>().OnPlayerLeave();
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
                    Node.ConnectedByNodes.ForEach(node => {
                        //Debug.Log($"Select:{gameObject.name}, Meet:{node.LevelObject.gameObject.name}");
                        node.LevelObject.GetComponent<LevelObject>().OnPlayerMeet();
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
        PathNode currentSelectedNode = this.GetModel<IMapStateModel>().CurNode;
       
        Interactable = Node.ConnectedByNodes.Contains(currentSelectedNode);
    }

    private void OnPlayerMoveToNext() {
        animator.SetBool("PlayerComplete", false);
        PathNode currentSelectedNode = this.GetModel<IMapStateModel>().CurNode;

        Interactable = Node.ConnectedByNodes.Contains(currentSelectedNode);
    }
}
