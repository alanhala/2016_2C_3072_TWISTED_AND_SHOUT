using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Utils;

namespace TGC.Group.Model
{
    class Wheel
    {
        private TgcMesh mesh;
        private Matrix initialPositionMatrix;

        public Wheel(Boolean isFrontWheel, Boolean isRightWheel)
        {
            var loader = new TgcSceneLoader();
            var xPosition = isRightWheel ? -24 : 22;
            var zPosition = isFrontWheel ? 33 : -32;
            initialPositionMatrix = Matrix.RotationY((float)FastMath.PI_HALF)
                * Matrix.Translation(new Vector3(xPosition, 0, zPosition));
            Mesh = loader.loadSceneFromFile(Game.Default.MediaDirectory + "Rueda\\Rueda-TgcScene.xml").Meshes[0];
            Mesh.AutoTransformEnable = false;
            Mesh.Transform = initialPositionMatrix;
           
        }

        public TgcMesh Mesh { get; set; }

        public void move(Matrix movementMatrix)
        {
            Mesh.Transform = initialPositionMatrix * movementMatrix;
        }

    }
}
