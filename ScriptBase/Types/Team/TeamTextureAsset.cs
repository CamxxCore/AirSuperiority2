using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSuperiority.ScriptBase.Types
{
    public struct TeamTextureAsset
    {
        public TeamTextureAsset(string primaryAsset, string secondaryAsset)
        {
            PrimaryAssetPath = primaryAsset;
            SecondaryAssetPath = secondaryAsset;
        }

        public string PrimaryAssetPath { get; set; }
        public string SecondaryAssetPath { get; set; }
    }
}
