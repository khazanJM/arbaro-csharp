using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2.DX_Engine.DXShaders
{
    public class DXShadersManager
    {
        private List<FileSystemWatcher> _fswl = new List<FileSystemWatcher>();
        private Dictionary<string, DXShader> _filename_shader_dico = new Dictionary<string, DXShader>();

        public DXShadersManager()
        {
            foreach (string path in Program.DXConfig.ShadersIncludePath)
            {
                FileSystemWatcher fsw = new FileSystemWatcher();
                _fswl.Add(fsw);
                fsw.Filter = "*.hlsl";
                fsw.Path = Path.GetFullPath(path);
                fsw.SynchronizingObject = Program.form;
                fsw.EnableRaisingEvents = true;
                fsw.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Shaders_Changed);
            }
        }

        public string MakeShaderFullPath(string shaderFilename)
        {
            string shaderFullName = "";

            foreach (string prefix in Program.DXConfig.ShadersIncludePath)
            {
                if (File.Exists(prefix + shaderFilename + ".hlsl"))
                {
                    shaderFullName = prefix + shaderFilename + ".hlsl";
                    break;
                }
            }

            return Path.GetFullPath(shaderFullName);
        }

        public DXShader MakeShader(string shaderFilename)
        {
            string shaderFullFileName = MakeShaderFullPath(shaderFilename);

            if (shaderFullFileName != "")
            {
                if (_filename_shader_dico.ContainsKey(shaderFullFileName))
                {
                    return _filename_shader_dico[shaderFullFileName];
                }
                else
                {
                    DXShader shader = new DXShader(shaderFullFileName);
                    _filename_shader_dico.Add(shaderFullFileName, shader);
                    return shader;
                }
            }
            else
            {
                string text = "Unable to find shader file: " + shaderFilename + "\n";
                MessageBox.Show(text, "Shader compiler error !", MessageBoxButtons.OK);
                return null;
            }

        }

        private void fileSystemWatcher_Shaders_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            if (_filename_shader_dico.ContainsKey(e.FullPath))
            {
                DXShader shader = _filename_shader_dico[e.FullPath];
                shader.Dirty = true;
            }
        }

        public void Refresh()
        {
            foreach (DXShader shader in _filename_shader_dico.Values)
            {
                if (shader.Dirty)
                {
                    shader.Init();
                }
            }
        }
    }
}
