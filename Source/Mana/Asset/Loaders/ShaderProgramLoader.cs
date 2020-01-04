using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mana.Graphics;
using Mana.Graphics.Shaders;
using Newtonsoft.Json;

namespace Mana.Asset.Loaders
{
    public class ShaderProgramLoader : IAssetLoader<ShaderProgram>
    {
        public ShaderProgram Load(AssetManager assetManager,
                                  RenderContext renderContext,
                                  Stream stream,
                                  string sourcePath)
        {
            using StreamReader streamReader = new StreamReader(stream);

            var description = JsonConvert.DeserializeObject<ShaderProgramDescription>(streamReader.ReadToEnd());

            string containingPath = Path.GetDirectoryName(sourcePath)
                                    ?? throw new Exception($"Error getting parent for path: {sourcePath}");

            var shaders = new List<Shader>();

            void LoadShader<T>(string relativeShaderPath, Func<RenderContext, string, T> factory)
                where T : Shader
            {
                if (string.IsNullOrEmpty(relativeShaderPath))
                    return;

                shaders.Add(GetShader(assetManager, renderContext, containingPath, relativeShaderPath, factory));
            }

            LoadShader(description.Vertex, (r, s) => new VertexShader(r, s));
            LoadShader(description.Fragment, (r, s) => new FragmentShader(r, s));
            LoadShader(description.Geometry, (r, s) => new GeometryShader(r, s));
            LoadShader(description.Compute, (r, s) => new ComputeShader(r, s));

            ShaderProgram program = new ShaderProgram(renderContext);

            foreach (var shader in shaders)
            {
                program.AttachShader(shader);
            }

            program.Link();

            program.ShaderSourcePaths = shaders.Select(x => x.SourcePath).ToArray();

            foreach (var shader in shaders)
            {
                program.DetachShader(shader);
            }

            foreach (var shader in shaders)
            {
                shader.Dispose();
            }

            return program;
        }

        private T GetShader<T>(AssetManager assetManager,
                               RenderContext renderContext,
                               string containingPath,
                               string relative,
                               Func<RenderContext, string, T> factory)
            where T : Shader
        {
            lock (assetManager.AssetLock)
            {
                var path = Path.Combine(containingPath, relative);

                var stream = assetManager.GetStreamFromPath(path);

                string source = "";

                using (var streamReader = new StreamReader(stream))
                    source = streamReader.ReadToEnd();

                var shader = factory(renderContext, source);

                shader.SourcePath = path;

                return shader;
            }
        }
    }

    [Serializable]
    internal class ShaderProgramDescription
    {
        public string Vertex { get; set; } = "";
        public string Fragment { get; set; } = "";
        public string Geometry { get; set; } = "";
        public string Compute { get; set; } = "";
    }
}
