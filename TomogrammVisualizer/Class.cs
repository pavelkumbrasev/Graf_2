using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace TomogrammVisualizer
{
    class Bin
    {
        public static int X, Y, Z;
        public static short[] array;


        public Bin() { }

        public void readBIN(string path)
        {
            if (File.Exists(path))
            {
                BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));

                X = reader.ReadInt32();
                Y = reader.ReadInt32();
                Z = reader.ReadInt32();

                int arraySize = X * Y * Z;
                array = new short[arraySize];
                for (int i=0;i<arraySize; i++)
                {
                    array[i] = reader.ReadInt16();
                }
            }
        }
    }

    class View
    {
        public void SetupView(int width, int height)
        {
            GL.ShadeModel(ShadingModel.Smooth);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, Bin.X, 0, Bin.Y, -1, 1);
            GL.Viewport(0, 0, width, height);
        }

        public int clamp(int value, int min = 0, int max = 255)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public Color TransferFunction(short value)
        {
            int min = 0, max = 2000;
            int newVal = clamp((value - min) * 255 / (max - min));
            return Color.FromArgb(255, newVal, newVal, newVal);
        }

        public void DrawQuads(int layerNumber)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int x_coord = 0; x_coord < Bin.X - 1; x_coord++)
            {
                GL.Begin(BeginMode.QuadStrip);
                for (int y_coord = 0; y_coord < Bin.Y - 1; y_coord++)
                {
                    short value;

                    value = Bin.array[x_coord + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord, y_coord);

                    value = Bin.array[x_coord + 1 + y_coord * Bin.X + layerNumber * Bin.X * Bin.Y];
                    GL.Color3(TransferFunction(value));
                    GL.Vertex2(x_coord + 1, y_coord);
                       
                }

                GL.End();
            }
        }
    }
}
