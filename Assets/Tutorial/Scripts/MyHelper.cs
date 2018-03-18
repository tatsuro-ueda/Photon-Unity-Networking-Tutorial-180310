using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

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

        #region Private Methods

        #endregion

        #region Public Methods

        public static string FileAndMethodNameWithMessage(string message)
        {
            StackFrame sf = new StackFrame(1, true);

            String methodName = sf.GetMethod().ToString();

            List<string> filePathElementList = new List<string>();
            filePathElementList.AddRange(sf.GetFileName().Split('\\'));
            String fileName = filePathElementList.Last();

            int lineNumber = sf.GetFileLineNumber();
            return fileName + "(" + lineNumber + "): " + methodName + " が呼び出されました。" + message;
        }

        #endregion
    }
}