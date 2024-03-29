﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Hazard_Sweep.Classes
{
    class Door : Sprite
    {
        private Rectangle activationArea;
        private Vector2 exitLocation;
        private int destination;
        private bool isActive;

        public Door(Game game, string textureFile, Vector2 position, Vector2 exitLocation, bool isActive, int destination)
            : base(game, textureFile, position, 7)
        {
            this.exitLocation = exitLocation;
            if (textureFile == "Images//door")
                activationArea = new Rectangle((int)position.X - 50, (int)position.Y - 50, 100, 100);
            if (textureFile == "Images//DoorClosed")
                activationArea = new Rectangle((int)position.X - 50, (int)position.Y - 50, 125, 300);
            this.isActive = isActive;
            this.destination = destination;
        }

        //update method
        public override void Update(GameTime gameTime)
        {
            if (isActive)
            {
                base.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (isActive)
            {
                if (textureFile != "Images//door")
                    base.Draw(gameTime);
            }
        }

        //get activation area
        public Rectangle getActivationArea()
        {
            return activationArea;
        }

        //get exit location
        public Vector2 getExitLocation()
        {
            return exitLocation;
        }

        //get destination
        public int getDestination()
        {
            return destination;
        }

        //returns if door is active
        public bool getActive()
        {
            return isActive;
        }

        //return position
        public Vector2 GetPosition()
        {
            return position;
        }
    }
}
