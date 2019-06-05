using System;
using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Mana.Utilities.Extensions;

namespace Mana.IMGUI.TextEditor
{
    class TextEditorBuffer
    {
        private bool _active = false;
        
        private string _text = "";
        private int _lineCount = 1;
        private int _longestLineLength = 0;
        
        private ImDrawListPtr _draw;
        private Vector2 _regionStart;
        private Vector2 _regionSize;
        private bool _isActive = false;

        private int _charWidth = 7;
        private int _charHeight = 10;
        
        private int _paddingTop = -5;
        private int _paddingLeft = -5 + (7 * 4);
        
        private int _linePaddingTop = 0;
        private int _linePaddingBottom = 3;

        public void Begin()
        {
            if (_active)
                throw new InvalidOperationException();

            var necessaryArea = new Vector2((_longestLineLength + 4) * _charWidth,
                                            (_lineCount - 1) * (_charHeight + _linePaddingTop + _linePaddingBottom));

            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Color(30, 30, 30).ToUint());
            ImGui.BeginChild("Editor", 
                             Vector2.Zero,
                             true, 
                             ImGuiWindowFlags.NoMove | ImGuiWindowFlags.HorizontalScrollbar);
            
            _draw = ImGui.GetWindowDrawList();
            _regionStart = ImGuiHelper.GetCursorScreenPos();
            _regionSize = ImGuiHelper.GetContentRegionAvail();

            ImGui.InvisibleButton("", necessaryArea);

            if (ImGui.IsAnyItemActive() || (!ImGui.IsItemActive() && ImGui.GetIO().MouseDown[0]))
                _isActive = false;
            
            if (ImGui.IsItemActive())
                _isActive = true;
            
            _active = true;
        }
        
        public void End()
        {
            if (!_active)
                throw new InvalidOperationException();

            ImGui.EndChild();
            ImGui.PopStyleVar();
            ImGui.PopStyleColor();
            _active = false;
        }

        public void DrawBackground(int colX, int colY, Color color)
        {
            if (!_active)
                throw new InvalidOperationException();
            
            float x = (colX * _charWidth) + _paddingLeft;
            float y = _linePaddingTop + (colY * (_charHeight + _linePaddingBottom + _linePaddingTop)) + _paddingTop;

            _draw.AddRectFilled(_regionStart + new Vector2(x, y - _linePaddingTop), 
                                _regionStart + new Vector2(x, y) + new Vector2(_charWidth, _charHeight + _linePaddingTop + _linePaddingBottom), 
                                color.ToUint());
        }
        
        public void DrawCharacter(int colX, int colY, Color color, char c)
        {
            if (!_active)
                throw new InvalidOperationException();
            
            float x = (colX * _charWidth) + _paddingLeft;
            float y = _linePaddingTop + (colY * (_charHeight + _linePaddingBottom + _linePaddingTop)) + _paddingTop;
            string str = c.ToString();

            _draw.AddText(_regionStart + new Vector2(x, y), color.ToUint(), str);
        }

        public void SetText(string text)
        {
            _text = text;
            
            int colX = 0;
            int colY = 0;
            
            for (int i = 0; i < _text.Length; i++)
            {
                char c = _text[i];

                if (c == '\n')
                {
                    colY++;
                    colX = 0;
                }
                else if (c == '\r')
                {
                    // Ignored
                }
                else
                {
                    colX++;
                }

                if (colX + 1 > _longestLineLength)
                {
                    _longestLineLength = colX + 1;
                }

                if (colY + 1 > _lineCount)
                {
                    _lineCount = colY + 1;
                }
            }
        }

        public void DrawBuffer()
        {
            int colX = 0;
            int colY = 0;

            for (int i = 0; i < _lineCount; i++)
            {
                // for (int x = -4; x < 0; x++)
                // {
                //     DrawBackground(x, i, Color.Black);
                // }

                int lineNumber = i + 1;
                
                if (lineNumber > 0) DrawCharacter(-2, i, Color.DarkCyan, (lineNumber % 10).ToString()[0]);
                if (lineNumber > 9) DrawCharacter(-3, i, Color.DarkCyan, ((lineNumber % 100) / 10).ToString()[0]);
                if (lineNumber > 99) DrawCharacter(-4, i, Color.DarkCyan, ((lineNumber % 1000) / 100).ToString()[0]);
                if (lineNumber > 999) DrawCharacter(-5, i, Color.DarkCyan, ((lineNumber % 10000) / 1000).ToString()[0]);
            }
            
            for (int i = 0; i < _text.Length; i++)
            {
                char c = _text[i];

                if (c == '\n')
                {
                    colY++;
                    colX = 0;
                }
                else if (c == '\r')
                {
                    // Ignored
                }
                else
                {
                    DrawCharacter(colX, colY, Color.White, c);
                    colX++;
                }
            }
        }

        public Point GetMouseCoordinate()
        {
            Point mouse = (ManaWindow.MainWindow.Input.MousePosition.ToVector2() - _regionStart).ToPoint();
            return LocalToCoordinate(mouse);
        }
        
        public Point LocalToCoordinate(Point local)
        {
            float x = local.X;
            float y = local.Y;
            
            x = (local.X - _paddingLeft) / (float)_charWidth;
            y = (local.Y - (_linePaddingTop + _paddingTop)) / (float)(_charHeight + _linePaddingBottom + _linePaddingTop);
            
            return new Point((int)x, (int)y);
        }
    }
}