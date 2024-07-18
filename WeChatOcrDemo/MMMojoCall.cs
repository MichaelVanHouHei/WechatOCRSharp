using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WeChatOcrDemo
{
    
    internal class MMMojoCall
    {
        #region dll Import
        [DllImport("MMMojoCall.dll", EntryPoint = "GetInstanceXPluginMgr")]
        public static extern IntPtr GetInstanceXPluginMgr(int type);

        [DllImport("MMMojoCall.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CallFuncXPluginMgr(IntPtr class_ptr, int mgr_type, string func_name, ref  int ret ,params object[] obj);

        [DllImport("MMMojoCall.dll", EntryPoint = "ReleaseInstanceXPluginMgr")]
        public static extern void ReleaseInstanceXPluginMgr(IntPtr instance);

        [DllImport("MMMojoCall.dll", EntryPoint = "InitMMMojoDLLFuncs")]
        public static extern bool InitMMMojoDLLFuncs(string dllPath);

        [DllImport("MMMojoCall.dll", EntryPoint = "InitMMMojoGlobal")]
        public static extern bool InitMMMojoGlobal(int type, string path);

        [DllImport("MMMojoCall.dll", EntryPoint = "ShutdownMMMojoGlobal")]
        public static extern bool ShutdownMMMojoGlobal();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void OCRUsrReadOnPushDelegate(IntPtr ptr, IntPtr usrData, int reserved);
        delegate void UtilityUsrReadOnPushDelegate(int typeId,  IntPtr data, int dataSize);
        delegate void UtilityUsrReadOnPullDelegate(int typeId,  IntPtr data, int dataSize);
        #endregion
        void OCRUsrReadOnPush(IntPtr ptr, IntPtr usrData, int reserved)
        {
            Console.WriteLine( $"{ptr}-{usrData}-{reserved}" );
        }
        private string OcrExePath;
        private string UsrDirPath;
        private int    Type = 1;
        private IntPtr XPluginInstance;
        public void DoOcr()
        {
            try
            {
                var isInit = InitMMMojoDLLFuncs(UsrDirPath) && InitMMMojoGlobal(0, "");
                if (!isInit) throw new Exception("init MMMojo dll failed");
                XPluginInstance = GetInstanceXPluginMgr(1);
                int                      result = -1;
                OCRUsrReadOnPushDelegate d      = OCRUsrReadOnPush;
                CallFuncXPluginMgr(XPluginInstance, 1 , "SetExePath",          ref result, OcrExePath);
                CallFuncXPluginMgr(XPluginInstance, 1,  "SetUsrLibDir",        ref result, UsrDirPath);
                CallFuncXPluginMgr(XPluginInstance, 1,  "SetCallbackDataMode", ref result, true);
                CallFuncXPluginMgr(XPluginInstance, 1,  "SetReadOnPush",       ref result, Marshal.GetFunctionPointerForDelegate(d));
                CallFuncXPluginMgr(XPluginInstance, 1,  "StartWeChatOCR",      ref result);
                CallFuncXPluginMgr(XPluginInstance, 1,  "DoOCRTask",           ref result ,Environment.CurrentDirectory + @"\test.png");
            }
            catch
            {
                ReleaseInstanceXPluginMgr(XPluginInstance);
                ShutdownMMMojoGlobal();
            }
        }
        
        
        public MMMojoCall WithOcrExe(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new Exception("invalid path");
            this.OcrExePath = path;
            return this;
        }

        public MMMojoCall WithUserDir(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new Exception("invalid path");
            this.UsrDirPath = path;
            return this;
        }
    }
}
