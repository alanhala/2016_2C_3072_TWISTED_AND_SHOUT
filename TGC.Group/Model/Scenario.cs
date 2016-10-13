﻿using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Example;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Scenario : TgcExample
    {
        private TgcScene scene;
        private Car car;
        private CarCamera camera;

        public Scenario(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            var loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(MediaDir + "city-TgcScene.xml");
            car = new Car();
            camera = new CarCamera(car);
            Camara = camera;
        }

        public override void Update()
        {
            PreUpdate();
           
            car.move(Input, ElapsedTime);
            camera.UpdateCamera(car);
        }

        public override void Render()
        {
            PreRender();
            car.render();
            scene.renderAll();
            PostRender();
        }

        public override void Dispose()
        {
            scene.disposeAll();
            car.dispose();
        }
    }
}
