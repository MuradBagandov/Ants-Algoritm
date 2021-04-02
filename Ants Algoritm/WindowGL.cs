using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Platform.Windows;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;
using OpenTK.Input;

namespace Ants_Algoritm
{
    class WindowGL : GameWindow
    {
        const float fieldWidth = 100.0f;
        float coeffWindowsWidthToFieldWidth;
        float fieldHeight;
        List<Vector2> points = new List<Vector2>();

        public WindowGL(int width, int height) : base(width, height, new GraphicsMode(32, 24, 4, 16), "Ants", GameWindowFlags.FixedWindow)
        {
            fieldHeight = fieldWidth * Height / (float)Width;
            coeffWindowsWidthToFieldWidth = width / fieldWidth;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            points.Add(ConvertWindowLocationToFieldLocation(new Vector2(e.X, e.Y)));
            base.OnMouseDown(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            //Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(45.0f * 0.01745f, (float)Width / (float)Height, 1f, 100f);
            //GL.LoadMatrix(ref perspective);
            //GL.Ortho(-30, 30, -30, 30, -1, 1);

            GL.Ortho(-fieldWidth/2, fieldWidth / 2, fieldHeight / 2, -fieldHeight / 2, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LineWidth(1f);
            GL.PointSize(10);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Color3(1.0, 1.0, 1.0);
            GL.Begin(BeginMode.Points);

            foreach (Vector2 item in points)
                    GL.Vertex2(item.X, item.Y);
            GL.End();

            SwapBuffers();
            base.OnRenderFrame(e);
        }

        public Vector2 ConvertWindowLocationToFieldLocation(Vector2 windowLocation)
        {
            var a = windowLocation / coeffWindowsWidthToFieldWidth;
            a.X -= fieldWidth / 2;
            a.Y -= fieldHeight / 2;
            return a;
        }
    }
}
