using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.BindableProperty;
using UnityEngine;

namespace MainGame
{
    public interface IMapGenerationModel : IModel, ICanGetModel {
        public BindableProperty<int> PathDepth { get; set; }
        public BindableProperty<int> PathWidth { get; set; }

        public List<float> NormalLevelPossibilityValues { get; }
        public List<LevelType> NormalLevelTypes { get; }
        public Dictionary<LevelType, float> NormalLevelPossibilities { get; }
    }
    public class MapGenerationModel : AbstractModel, IMapGenerationModel, ICanRegisterAndLoadSavedData {
        private IMapGenerationConfigModel configModel;

        protected override void OnInit() {
            configModel = this.GetModel<IMapGenerationConfigModel>();

            PathDepth = this.RegisterAndLoadFromSavedData("path_depth", new BindableProperty<int>(configModel.PathDepth));
            PathWidth = this.RegisterAndLoadFromSavedData("path_width", new BindableProperty<int>(configModel.PathWidth));

            NormalLevelPossibilities = this.RegisterAndLoadFromSavedData<Dictionary<LevelType, float>>("normal_level_poss", new Dictionary<LevelType, float>());

            if (NormalLevelPossibilities.Count==0) {
              
                foreach (KeyValuePair<LevelType, float> configModelNormalLevelPossibility in configModel.NormalLevelPossibilities) {
                    NormalLevelPossibilities.Add(configModelNormalLevelPossibility.Key,
                        configModelNormalLevelPossibility.Value);
                }
            }

           
        }

        
        public BindableProperty<int> PathDepth { get; set; } = new BindableProperty<int>();
        public BindableProperty<int> PathWidth { get; set; } = new BindableProperty<int>();

        public List<float> NormalLevelPossibilityValues
        {
            get
            {

                List<float> normalLevelPossibilityValues = new List<float>();
                var valueEnumerator = NormalLevelPossibilities.Values.GetEnumerator();

                while (valueEnumerator.MoveNext())
                {
                    normalLevelPossibilityValues.Add(valueEnumerator.Current);
                }

                return normalLevelPossibilityValues;
            }
        }
        public List<LevelType> NormalLevelTypes
        {
            get
            {
                List<LevelType> res = new List<LevelType>();
                var enumerator = NormalLevelPossibilities.Keys.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    res.Add(enumerator.Current);
                }

                return res;
            }
        }

        public Dictionary<LevelType, float> NormalLevelPossibilities { get; private set; } = new Dictionary<LevelType, float>();
    }
}
