using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using UnityEngine;

namespace Education.FeelPhysics
{
    /// <summary>
    /// ヘルパーメソッドが入ったクラス
    /// </summary>
    public static class MyHelper
    {
        #region Public Methods

        /// <summary>
        /// Debug.Logにファイル名、行番号、実行しているメソッド名を付ける
        /// </summary>
        /// <param name="message">DebugLogに表示させるメッセージ</param>
        /// <returns>ファイル名、行番号、実行しているメソッド名、messageの文字列を返す</returns>
        public static string FileAndMethodNameWithMessage(string message)
        {
            StackFrame sf = new StackFrame(1, true);

            string methodName = sf.GetMethod().ToString();

            List<string> filePathElementList = new List<string>();
            filePathElementList.AddRange(sf.GetFileName().Split('\\'));
            string fileName = filePathElementList.Last();

            int lineNumber = sf.GetFileLineNumber();
            return fileName + "(" + lineNumber + "): " + methodName + " が呼び出されました。" + message;
        }

        #endregion
    }

    /*
    /// <summary>
    /// ※未使用
    /// グローバル定数が入ったクラス
    /// </summary>
    public static class GlobalVar
    {
        /// <summary>
        /// ※未使用
        /// </summary>
        public const bool ShouldDebugLog = true;

        /// <summary>
        /// ※未使用
        /// </summary>
        public const bool ShouldDebugLogInScene = true;
    }
    */
}