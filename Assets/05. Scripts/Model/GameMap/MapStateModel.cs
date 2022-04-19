using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using MikroFramework.TimeSystem;
using UnityEngine;

namespace MainGame
{
    public interface IMapStateModel : IModel
    {
        BindableProperty<GraphVertex> CurNode { get; set; }
    }
    public class MapStateModel : AbstractModel, IMapStateModel, ICanRegisterAndLoadSavedData
    {
        protected override void OnInit()
        {
            CurNode = this.RegisterAndLoadFromSavedData("cur_node", new BindableProperty<GraphVertex>());
            this.GetSystem<ITimeSystem>().AddDelayTask(0.07f, () => {
                
                if (CurNode.Value != null)
                {
                    
                    CurNode.Value = this.GetSystem<IGameMapGenerationSystem>()
                        .GetPathNodeAtDepthAndOrder(CurNode.Value.Value.Depth, CurNode.Value.Value.Order);
                 
                 
                    Debug.Log(CurNode.Value.Neighbours.Count);
                 
                }
            });
            
           
        }

        public BindableProperty<GraphVertex> CurNode { get; set; }
    }
}