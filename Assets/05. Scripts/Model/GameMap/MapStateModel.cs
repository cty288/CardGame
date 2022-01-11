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
        BindableProperty<PathNode> CurNode { get; set; }
    }
    public class MapStateModel : AbstractModel, IMapStateModel, ICanRegisterAndLoadSavedData
    {
        protected override void OnInit()
        {
            CurNode = this.RegisterAndLoadFromSavedData("cur_node", new BindableProperty<PathNode>());
            this.GetSystem<ITimeSystem>().AddDelayTask(0.07f, () => {
                
                if (CurNode.Value != null)
                {
                    CurNode.Value = this.GetSystem<IGameMapSystem>()
                        .GetPathNodeAtDepthAndOrder(CurNode.Value.Depth, CurNode.Value.Order);
                }
            });
            
           
        }

        public BindableProperty<PathNode> CurNode { get; set; }
    }
}