using System;

namespace Mana.Graphics
{
    public class GraphicsDevice
    {
        private static GraphicsDevice _instance;
        
        public GraphicsDevice()
        {
            if (_instance != null)
                throw new InvalidOperationException("An instance of GraphicsDevice already exists.");
            
            _instance = this;
        }
    }
}