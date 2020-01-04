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
            ShaderProgramDescription description;
            using StreamReader streamReader = new StreamReader(stream);

            description = JsonConvert.DeserializeObject<ShaderProgramDescription>(streamReader.ReadToEnd());

            string containingPath = Path.GetDirectoryName(sourcePath)
                                    ?? throw new Exception($"Error getting parent for path: {sourcePath}");

            var shaders = new List<Shader>();

            // TODO: Reduce duplicate code.

            if (!string.IsNullOrEmpty(description.Vertex))
            {
                shaders.Add(GetShader(assetManager, renderContext, containingPath, description.Vertex, (r, s) => new VertexShader(r, s)));
            }

            if (!string.IsNullOrEmpty(description.Fragment))
            {
                shaders.Add(GetShader(assetManager, renderContext, containingPath, description.Fragment, (r, s) => new FragmentShader(r, s)));
            }

            if (!string.IsNullOrEmpty(description.Geometry))
            {
                shaders.Add(GetShader(assetManager, renderContext, containingPath, description.Geometry, (r, s) => new GeometryShader(r, s)));
            }

            if (!string.IsNullOrEmpty(description.Compute))
            {
                shaders.Add(GetShader(assetManager, renderContext, containingPath, description.Compute, (r, s) => new ComputeShader(r, s)));
            }

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

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ShaderProgramDescription
    {
        public string Vertex { get; set; } = "";
        public string Fragment { get; set; } = "";
        public string Geometry { get; set; } = "";
        public string Compute { get; set; } = "";
    }
}
