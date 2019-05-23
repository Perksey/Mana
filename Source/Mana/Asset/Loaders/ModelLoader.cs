using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Assimp;
using Mana.Graphics;
using Mana.Graphics.Geometry;
using Mana.Graphics.Vertex.Types;
using OpenTK.Graphics.OpenGL4;

using Mesh = Mana.Graphics.Geometry.Mesh;
using Matrix4x4 = System.Numerics.Matrix4x4;

using TextureWrapMode = Assimp.TextureWrapMode;
using WrapModeGL = OpenTK.Graphics.OpenGL4.TextureWrapMode;

namespace Mana.Asset.Loaders
{
    public class ModelLoader : IAssetLoader<Model>
    {
        public static readonly AssimpContext Importer = new AssimpContext();
        
        public Model Load(AssetManager manager, Stream sourceStream, string sourcePath)
        {
            var scene = Importer.ImportFile(sourcePath, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);
            
            if (scene == null || (scene.SceneFlags & SceneFlags.Incomplete) == SceneFlags.Incomplete || scene.RootNode == null)
                throw new InvalidOperationException("Error importing asset.");

            var meshes = new List<Mesh>();
            var directory = Path.GetDirectoryName(sourcePath);
            
            ProcessNode(manager, scene, meshes, directory, scene.RootNode, Matrix4x4.Identity);
            
            return new Model(manager.GraphicsDevice, meshes.ToArray());
        }

        private void ProcessNode(AssetManager manager, Scene scene, List<Mesh> meshes, string sourceDirectory, Node node, Matrix4x4 transform)
        {
            var temp = node.Transform;
            var currentTransform = Unsafe.As<Assimp.Matrix4x4, Matrix4x4>(ref temp);
            var newTransform = transform * currentTransform;

            // Process all of the current node's meshes.
            for (int i = 0; i < node.MeshCount; i++)
                meshes.Add(ProcessMesh(manager, scene.Meshes[node.MeshIndices[i]], sourceDirectory, scene, newTransform));
            
            // Recurse.
            for (int i = 0; i < node.ChildCount; i++)
                ProcessNode(manager, scene, meshes, sourceDirectory, node.Children[i], newTransform);
        }

        private Mesh ProcessMesh(AssetManager manager, Assimp.Mesh assimpMesh, string directory, Scene scene, Matrix4x4 transform)
        {
            var vertices = new VertexPositionNormalTexture[assimpMesh.VertexCount];
            var indices = assimpMesh.GetUnsignedIndices();

            for (int i = 0; i < assimpMesh.VertexCount; i++)
            {
                var position = assimpMesh.Vertices[i];
                vertices[i].Position = Unsafe.As<Vector3D, Vector3>(ref position);

                var normal = assimpMesh.Normals[i];
                vertices[i].Normal = Unsafe.As<Vector3D, Vector3>(ref normal);

                if (assimpMesh.HasTextureCoords(0))
                {
                    var texCoord = assimpMesh.TextureCoordinateChannels[0][i];
                    vertices[i].TexCoord = new Vector2(texCoord.X, 1 - texCoord.Y);
                }
            }

            return new Mesh(manager.GraphicsDevice, new MeshData(vertices, indices))
            {
                Textures = GetMeshTextureInfo(manager, directory, scene, assimpMesh),
                Transform = transform,
            };
        }

        private MeshTextures GetMeshTextureInfo(AssetManager manager, string sourcePath, Scene scene, Assimp.Mesh mesh)
        {
            var meshTextures = new MeshTextures();

            if (mesh.MaterialIndex < 0)
                return meshTextures;

            var material = scene.Materials[mesh.MaterialIndex];
            //var textures = material.GetAllMaterialTextures();

            Texture2D LoadTexture(TextureSlot s, TextureType t) => this.LoadTexture(manager, sourcePath, s, t);

            if (material.HasTextureDiffuse)
                meshTextures.Diffuse = LoadTexture(material.TextureDiffuse, TextureType.Diffuse);
            
            if (material.HasTextureSpecular)
                meshTextures.Specular = LoadTexture(material.TextureSpecular, TextureType.Specular);
            
            if (material.HasTextureLightMap)
                meshTextures.MetallicRoughness = LoadTexture(material.TextureLightMap, TextureType.Lightmap);

            return meshTextures;
        }
        
        private Texture2D LoadTexture(AssetManager manager, string sourcePath, TextureSlot slot, TextureType type)
        {
            var texture = manager.Load<Texture2D>(Path.Combine(sourcePath, slot.FilePath));

            var prevBoundTexture = GL.GetInteger(GetPName.TextureBinding2D);
            var prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GetWrapModeGL(slot.WrapModeU));
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GetWrapModeGL(slot.WrapModeV));
            
            GL.ActiveTexture((TextureUnit)prevActiveTexture);
            GL.BindTexture(TextureTarget.Texture2D, prevBoundTexture);
            
            return texture;
        }

        private WrapModeGL GetWrapModeGL(TextureWrapMode wrapMode)
        {
            switch (wrapMode)
            {
                case TextureWrapMode.Wrap:
                    return WrapModeGL.Repeat;
                case TextureWrapMode.Clamp:
                    return WrapModeGL.ClampToEdge;
                case TextureWrapMode.Mirror:
                    return WrapModeGL.MirroredRepeat;
                case TextureWrapMode.Decal:
                    return WrapModeGL.ClampToBorder;
                default:
                    throw new ArgumentOutOfRangeException(nameof(wrapMode), wrapMode, null);
            }
        }
    }
}