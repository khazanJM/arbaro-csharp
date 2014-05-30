using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2.DX_Engine.DXShaders
{
    public class DXShader : IDisposable
    {
        public Effect DXEffect = null;
        public string ShaderFullFilename = "";
        public bool Dirty = true;

        protected bool _isValid = true;

        public DXShader(string shaderFullFilename)
        {
            ShaderFullFilename = shaderFullFilename;
            Init();
        }

        public void SetParameter(string varName, object varValue)
        {
            EffectVariable ev = DXEffect.GetVariableByName(varName);
            if (ev.IsValid)
            {
                if (varValue is bool) ev.AsScalar().Set((bool)varValue);
                else if (varValue is int) ev.AsScalar().Set((int)varValue);
                else if (varValue is uint) ev.AsScalar().Set((uint)varValue);
                else if (varValue is float) ev.AsScalar().Set((float)varValue);
                else if (varValue is double) ev.AsScalar().Set((float)(double)varValue);
                else if (varValue is Vector2) ev.AsVector().Set((Vector2)varValue);
                else if (varValue is Vector3) ev.AsVector().Set((Vector3)varValue);
                else if (varValue is Vector4[]) ev.AsVector().Set((Vector4[])varValue);
                else if (varValue is Vector4) ev.AsVector().Set((Vector4)varValue);
                else if (varValue is Matrix) ev.AsMatrix().SetMatrix((Matrix)varValue);
                //else if (varValue is DXTexture2DClass) ev.AsShaderResource().SetResource((varValue as DXTexture2DClass).TextureResourceView);
                else if (varValue is ShaderResourceView) ev.AsShaderResource().SetResource(varValue as ShaderResourceView);
                else if (varValue == null) { ev.AsShaderResource().SetResource(null); }
                else { /* ... */ }
            }
        }

        public void Init()
        {
            if (DXEffect != null) DXEffect.Dispose();

            CompilationResult cr = null;

            EffectFlags EFFECT_FLAGS = EffectFlags.None;
#if DEBUG
            ShaderFlags SHADER_FLAGS = ShaderFlags.Debug;
#else
            ShaderFlags SHADER_FLAGS = ShaderFlags.OptimizationLevel3;
#endif
            try
            {
                _isValid = true;
                cr = ShaderBytecode.CompileFromFile(ShaderFullFilename, "fx_5_0", SHADER_FLAGS, EFFECT_FLAGS, null, new IncludeFX(Program.DXConfig.ShadersIncludePath));
                DXEffect = new Effect(Program.Renderer.DXDevice, cr.Bytecode);
                cr.Bytecode.Dispose();
                Dirty = false;
            }
            catch (Exception e)
            {
                _isValid = false;
                string text = "Compilation failed for shader: " + ShaderFullFilename + "\n" + e.Message;
                if (MessageBox.Show(text, "Shader compiler", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
                {
                    Init();
                }
            }
        }

        //
        // IDisposable
        //
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // cleanup managed stuff
                }

                // cleanup unmanaged stuff    
                if (DXEffect != null) DXEffect.Dispose();

                disposed = true;
            }
        }

        ~DXShader()
        {
            Dispose(false);
        }
    }


    //
    //      Include Manager
    //
    class IncludeFX : Include, ICallbackable
    {
        private string[] _includeDirectory;

        public IDisposable Shadow { get { return null; } set { } }
        public virtual void Dispose() { }

        public IncludeFX(string[] includePath)
        {
            _includeDirectory = new string[includePath.Count()];
            includePath.CopyTo(_includeDirectory, 0);
        }

        public void Close(Stream stream)
        {
            stream.Close();
        }

        public Stream Open(IncludeType type, string fileName, Stream parentStream)
        {
            foreach (string s in _includeDirectory)
            {
                if (File.Exists(s + "\\" + fileName))
                    return new FileStream(s + "\\" + fileName, FileMode.Open);
            }

            return null;
        }
    }


  

}
