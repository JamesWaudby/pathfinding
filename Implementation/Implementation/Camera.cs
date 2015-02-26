using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Implementation.GridRepresentation;
using Microsoft.Xna.Framework;

// Tutorial: https://roguesharp.wordpress.com/2014/07/13/tutorial-5-creating-a-2d-camera-with-pan-and-zoom-in-monogame/
using Microsoft.Xna.Framework.Input;

namespace Implementation
{
    public class Camera
    {
        public Vector2 Position { get; private set; }
        public float Zoom { get; private set; }

        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }

        public Vector2 ViewPortCenter
        {
            get
            {
                return new Vector2(ViewportWidth * 0.5f, ViewportHeight * 0.5f);
            }
        }
        public Matrix TranslationMatrix
        {
            get
            {
                return Matrix.CreateTranslation(-(int) Position.X, -(int) Position.Y, 0 ) *
                       Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) * 
                       Matrix.CreateTranslation(new Vector3(ViewPortCenter, 0));
            }
        }

        public Camera()
        {
            Zoom = 1.0f;
        }

        // Call this method with negative values to zoom out
        // or positive values to zoom in. It looks at the current zoom
        // and adjusts it by the specified amount. If we were at a 1.0f
        // zoom level and specified -0.5f amount it would leave us with
        // 1.0f - 0.5f = 0.5f so everything would be drawn at half size.
        public void AdjustZoom(float amount)
        {
            Zoom += amount;
            if (Zoom < 0.75f)
            {
                Zoom = 0.75f;
            }
            else if (Zoom > 1.25f)
            {
                Zoom = 1.25f;
            }
        }

        // Move the camera in an X and Y amount based on the cameraMovement param.
        // if clampToMap is true the camera will try not to pan outside of the
        // bounds of the map.
        public void MoveCamera(Vector2 cameraMovement, bool clampToMap = false)
        {
            Vector2 newPosition = Position + cameraMovement;

            Position = newPosition;
        }

        // Center the camera on specific coordinates
        public void CenterOn(Vector2 position)
        {
            // Set the camera position to be the grid position x grid size.
            Position = position * 32;
        }

        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, TranslationMatrix);
        }

        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition,
                Matrix.Invert(TranslationMatrix));
        }


    }
}
