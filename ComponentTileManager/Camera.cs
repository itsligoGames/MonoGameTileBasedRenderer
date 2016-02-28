using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileManagerNS
{
    class Camera
    {
        Vector2 _camPos = Vector2.Zero;
        Vector2 _worldBound;
        Viewport _view;
        float _scale = 1f;
        public Matrix CurrentCameraTranslation { get
            {
                return Matrix.CreateTranslation(new Vector3(-CamPos, 0)) *
                    Matrix.CreateScale(new Vector3(Scale,Scale,0));
            } }

        public Viewport View
        {
            get
            {
                return _view;
            }

            set
            {
                _view = value;
            }
        }

        public float Scale
        {
            get
            {
                return _scale;
            }

            set
            {
                _scale = value;
            }
        }

        public Vector2 WorldBound
        {
            get
            {
                return _worldBound * _scale;
            }

            set
            {
                _worldBound = value;
            }
        }

        public Vector2 CamPos
        {
            get
            {
                return _camPos;
            }

            set
            {
                _camPos = value;
            }
        }

        public Camera(Vector2 startPos, Vector2 bound, Viewport view)
        {
            CamPos = startPos;
            WorldBound = bound;
            _view = view;
        }

        public void move(Vector2 delta, Viewport v)
        {
            CamPos += delta;
            CamPos = Vector2.Clamp(CamPos, Vector2.Zero, WorldBound - new Vector2(v.Width, v.Height));
        }

        public void follow(Vector2 followPos, Viewport v)
        {
            CamPos = followPos - new Vector2(v.Width/2,v.Height/2) /_scale;
            CamPos = Vector2.Clamp(CamPos, Vector2.Zero, WorldBound / _scale - new Vector2(v.Width, v.Height) / _scale);
        }

    }
}
