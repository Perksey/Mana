using System;
using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Mana.Graphics.Geometry;
using Mana.Graphics.Shaders;
using Mana.Utilities;
using Mana.Utilities.Extensions;

namespace Mana.Samples.Basic.Examples
{
    public class ModelExample : Example
    {
        private Mesh _box;
        private ShaderProgram _shader;
        private Camera _camera;
        private CameraController _cameraController;

        private Model _model;
        
        public ModelExample(SampleGame game) : base(game)
        {
            _camera = new Camera();
            _cameraController = new CameraController(_camera);
        }

        public override void Initialize()
        {
            _box = new Mesh(GraphicsDevice, MeshGenerator.CreateBox());
            _shader = AssetManager.Load<ShaderProgram>("./Assets/Shaders/model.json");

            AssetManager.CreateAsyncBatch()
                        .Load(() => _model, "./Assets/Models/NanoSuit/nanosuit.obj")
                        .Start();
        }

        public override void Dispose()
        {
            _box.Dispose();
            _shader.Dispose();
            _model.Dispose();
        }

        public override void Update(float time, float deltaTime)
        {
            var identity = Matrix4x4.Identity;

            _camera.Update();
            _cameraController.Update(time, deltaTime);
            
            _shader.TrySetUniform("model", ref identity);
            _shader.TrySetUniform("view", ref _camera.GetViewMatrix());
            _shader.TrySetUniform("projection", ref _camera.GetProjectionMatrix());
        }

        public override void Render(float time, float deltaTime)
        {
            if (_model == null)
            {
                _box.Render(_shader);
            }
            else
            {
                _model.Render(_shader);
            }
        }
    }

    public class Camera
    {
        private float _fieldOfView = 60f;
        private float _nearPlaneDistance = 0.1f;
        private float _farPlaneDistance = 10000.0f;
        private Vector3 _focusPoint;
        private Vector2 _rotation = new Vector2(-40.0f, 40.0f);
        private float _distance = 2;
        
        private Matrix4x4 _view = Matrix4x4.Identity;
        private Matrix4x4 _inverseView = Matrix4x4.Identity;
        private Matrix4x4 _projection = Matrix4x4.Identity;
        private Matrix4x4 _inverseProjection = Matrix4x4.Identity;

        private Vector3 _position;
        private Vector3 _up;
        private Vector3 _right;

        private bool _viewMatrixDirty = true;
        private bool _projectionMatrixDirty = true;
        
        public float FieldOfView
        {
            get => _fieldOfView;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_fieldOfView == value)
                    return;

                _fieldOfView = value;
                _projectionMatrixDirty = true;
            }
        }
        public float NearPlaneDistance
        {
            get => _nearPlaneDistance;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_nearPlaneDistance == value)
                    return;

                _nearPlaneDistance = value;
                _projectionMatrixDirty = true;
            }
        }
        public float FarPlaneDistance
        {
            get => _farPlaneDistance;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_farPlaneDistance == value)
                    return;

                _farPlaneDistance = value;
                _projectionMatrixDirty = true;
            }
        }
        public Vector3 FocusPoint
        {
            get => _focusPoint;
            set
            {
                if (_focusPoint == value)
                    return;

                _focusPoint = value;
                _viewMatrixDirty = true;
            }
        }
        public Vector2 Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation == value)
                    return;

                _rotation = value;
                _viewMatrixDirty = true;
            }
        }
        public float Distance
        {
            get => _distance;
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_distance == value)
                    return;

                _distance = value;
                _viewMatrixDirty = true;
            }
        }

        public Vector3 Position => _position;
        public Vector3 Up => _up;
        public Vector3 Right => _right;

        public void Update()
        {
            bool matricesChanged = false;

            if (_viewMatrixDirty)
            {
                BuildViewMatrix();
                _viewMatrixDirty = false;
                matricesChanged = true;
            }

            if (_projectionMatrixDirty)
            {
                BuildProjectionMatrix();
                _projectionMatrixDirty = false;
                matricesChanged = true;
            }
        }
        
        private void BuildViewMatrix()
        {
            _inverseView = Matrix4x4.CreateTranslation(new Vector3(0f, 0f, (float)Math.Pow(_distance, 2.23f))) *
                           Matrix4x4.CreateRotationX(MathHelper.DegreesToRadians(_rotation.X)) *
                           Matrix4x4.CreateRotationY(MathHelper.DegreesToRadians(_rotation.Y)) *
                           Matrix4x4.CreateTranslation(_focusPoint);

            
            _view = _inverseView.GetInverse();

            Matrix4x4.Decompose(_inverseView, out _, out Quaternion rotation, out Vector3 translation);

            //_position = this.Entity.Transform.Position = translation;
            _right = Vector3.Transform(Vector3.UnitX, rotation).Normalized();
            _up = Vector3.Transform(Vector3.UnitY, rotation).Normalized();
        }
        
        private void BuildProjectionMatrix()
        {
            _projection = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FieldOfView),
                                                                 16 / 9f,
                                                                 NearPlaneDistance,
                                                                 FarPlaneDistance);

            _inverseProjection = _projection.GetInverse();
        }
        
        public ref Matrix4x4 GetViewMatrix()
        {
            return ref _view;
        }

        public ref Matrix4x4 GetInverseViewMatrix()
        {
            return ref _inverseView;
        }

        public ref Matrix4x4 GetProjectionMatrix()
        {
            return ref _projection;
        }

        public ref Matrix4x4 GetInverseProjectionMatrix()
        {
            return ref _inverseProjection;
        }
    }
    
    public class CameraController
    {
        private Camera _arcball;

        private float _targetDistance;
        private Vector2 _targetRotation;
        private Vector3 _targetFocusPoint;

        public CameraController(Camera camera)
        {
            _arcball = camera;
            
            _targetDistance = _arcball.Distance;
            _targetRotation = _arcball.Rotation;
            _targetFocusPoint = _arcball.FocusPoint;
        }
        
        public void Update(float time, float deltaTime)
        {
            if (!ImGui.IsAnyItemActive())
            {
                bool tumbling = Input.IsMouseDown(MouseButton.Left) && Input.IsKeyDown(Key.LeftAlt);
                bool panning = Input.IsMouseDown(MouseButton.Middle);

                if (tumbling)
                {
                    _targetRotation.Y += Input.MouseDelta.X * -0.25f;
                    _targetRotation.X = MathHelper.Clamp(_targetRotation.X + Input.MouseDelta.Y * (-0.25f * (16 / 9f)), -90, 90);
                }
                else if (panning)
                {
                    if (Input.MouseDelta != Point.Empty)
                    {
                        float f = (float)Math.Pow(_arcball.Distance, 2.23f) * 0.001f;
                        _targetFocusPoint += _arcball.Right * (Input.MouseDelta.X * -f);
                        _targetFocusPoint += _arcball.Up * (Input.MouseDelta.Y * (16 / 9f) * f);
                    }
                }
                else if (Input.MouseWheelDelta != 0)
                {
                    _targetDistance += Input.MouseWheelDelta * -0.2f;
                    _targetDistance = Math.Max(_targetDistance, 1f);
                }
            }

            _arcball.Distance = MathHelper.Lerp(_arcball.Distance, _targetDistance, deltaTime * 16f);
            _arcball.FocusPoint = Vector3.Lerp(_arcball.FocusPoint, _targetFocusPoint, deltaTime * 32f);
            _arcball.Rotation = Vector2.Lerp(_arcball.Rotation, _targetRotation, deltaTime * 32f);
        }
    }
}