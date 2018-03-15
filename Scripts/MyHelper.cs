using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Education.FeelPhysics
{
    // グローバル定数
    public static class GlobalVar
    {
        public const Boolean shouldDebugLog = true;
        public const Boolean shouldDebugLogInScene = true;
    }

    static public class MyHelper
    {
        // コンソール
        public static void DebugLog(String message)
        {
#if UNITY_EDITOR
            if (GlobalVar.shouldDebugLog)
            {
                StackFrame sf = new StackFrame(1, true);

                String methodName = sf.GetMethod().ToString();  // 未使用

                List<string> filePathElementList = new List<string>();
                filePathElementList.AddRange(sf.GetFileName().Split('\\'));
                String fileName = filePathElementList.Last();

                int lineNumber = sf.GetFileLineNumber();
                UnityEngine.Debug.Log(fileName + "(" + lineNumber + "): " + methodName + "が呼び出されました。" + message);
            }
#endif
        }
    }
}