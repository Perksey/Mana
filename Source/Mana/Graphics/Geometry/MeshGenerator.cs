/*
 *  http://wiki.unity3d.com/index.php/ProceduralPrimitives#C.23_-_Sphere
 */

using System.Numerics;
using Mana.Graphics.Vertex.Types;
using Mana.Utilities;

namespace Mana.Graphics.Geometry
{
    public static class MeshGenerator
    {
        public static MeshData CreateBox(float width = 1f, float height = 1f, float length = 1f)
        {
            var p0 = new Vector3(-width * .5f, -height * .5f, +length * .5f);
            var p1 = new Vector3(+width * .5f, -height * .5f, +length * .5f);
            var p2 = new Vector3(+width * .5f, -height * .5f, -length * .5f);
            var p3 = new Vector3(-width * .5f, -height * .5f, -length * .5f);
            var p4 = new Vector3(-width * .5f, +height * .5f, +length * .5f);
            var p5 = new Vector3(+width * .5f, +height * .5f, +length * .5f);
            var p6 = new Vector3(+width * .5f, +height * .5f, -length * .5f);
            var p7 = new Vector3(-width * .5f, +height * .5f, -length * .5f);

            Vector3[] vertices =
            {
                p0, p1, p2, p3,
                p7, p4, p0, p3,
                p4, p5, p1, p0,
                p6, p7, p3, p2,
                p5, p6, p2, p1,
                p7, p6, p5, p4,
            };

            Vector3[] normals =
            {
                Vector3Helper.Down, Vector3Helper.Down, Vector3Helper.Down, Vector3Helper.Down,
                Vector3Helper.Left, Vector3Helper.Left, Vector3Helper.Left, Vector3Helper.Left,
                Vector3Helper.Forward, Vector3Helper.Forward, Vector3Helper.Forward, Vector3Helper.Forward,
                Vector3Helper.Backward, Vector3Helper.Backward, Vector3Helper.Backward, Vector3Helper.Backward,
                Vector3Helper.Right, Vector3Helper.Right, Vector3Helper.Right, Vector3Helper.Right,
                Vector3Helper.Up, Vector3Helper.Up, Vector3Helper.Up, Vector3Helper.Up,
            };

            var bottomLeft = new Vector2(0f, 0f);
            var bottomRight = new Vector2(1f, 0f);
            var topLeft = new Vector2(0f, 1f);
            var topRight = new Vector2(1f, 1f);

            Vector2[] texCoords =
            {
                topRight, topLeft, bottomLeft, bottomRight,
                topRight, topLeft, bottomLeft, bottomRight,
                topRight, topLeft, bottomLeft, bottomRight,
                topRight, topLeft, bottomLeft, bottomRight,
                topRight, topLeft, bottomLeft, bottomRight,
                topRight, topLeft, bottomLeft, bottomRight,
            };

            int[] indices =
            {
                3, 1, 0,
                3, 2, 1,

                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,

                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,

                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,

                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,

                3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
                3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
            };

            var outputVertices = new VertexPositionNormalTexture[vertices.Length];
            var outputIndices = new uint[indices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                outputVertices[i].Position = vertices[i];
                outputVertices[i].Normal = normals[i];
                outputVertices[i].TexCoord = texCoords[i];
            }

            for (int i = 0; i < indices.Length; i++)
            {
                outputIndices[i] = (uint)indices[i];
            }

            return new MeshData(outputVertices, outputIndices);
        }
    }
}