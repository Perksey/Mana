using ImGuiNET;

namespace Mana.Example.Basic.Interface
{
    public abstract class EditorWindow
    {
        public bool IsShown { get; set; } = true;
        
        public abstract string Name { get; }
        public abstract void Render(float time, float deltaTime);

        public void RenderBase(float time, float deltaTime)
        {
            if (!IsShown)
                return;

            PreRender();

            bool open = IsShown;
            if (ImGui.Begin(Name, ref open))
            {
                Render(time, deltaTime);
            }
            
            ImGui.End();

            IsShown = open;
            
            PostRender();
        }

        protected virtual void PreRender()
        {
        }

        protected virtual void PostRender()
        {
        }
    }
}