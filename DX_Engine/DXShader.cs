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

namespace Arbaro2.DX_Engine
{
    public class DXShader
    {
        public Effect DXEffect = null;
        public string ShaderFullFilename = "";

        protected bool _isValid = true;

        public DXShader(string shaderFilename) 
        {
            bool exists = false;
            foreach (string prefix in Program.DXConfig.ShadersIncludePath) {
                if (File.Exists(prefix + shaderFilename + ".hlsl")) {
                    exists = true;
                    ShaderFullFilename = prefix + shaderFilename + ".hlsl";
                    break;
                }
            }

            if (exists) Init();
            else {
                string text = "Unable to find shader file: " + ShaderFullFilename + "\n";
                MessageBox.Show(text, "Shader compiler error !", MessageBoxButtons.OK);
            }
        }

        private void Init()
        {
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
