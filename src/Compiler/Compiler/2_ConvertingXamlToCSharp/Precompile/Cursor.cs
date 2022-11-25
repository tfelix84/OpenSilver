

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;

namespace DotNetForHtml5.Compiler
{
    /// <summary>
    /// Copied from Runtime
    /// </summary>
    public class Cursor
    {
        private static string[] HtmlCursors { get; }
        internal const int _cursorTypeCount = ((int)CursorType.Eraser) + 1;
        private CursorType _cursorType = CursorType.None;

        static Cursor()
        {
            HtmlCursors = new string[Cursor._cursorTypeCount];
            HtmlCursors[(int)CursorType.None] = "none";
            HtmlCursors[(int)CursorType.No] = "not-allowed";
            HtmlCursors[(int)CursorType.Arrow] = "default";
            HtmlCursors[(int)CursorType.AppStarting] = "progress";
            HtmlCursors[(int)CursorType.Cross] = "crosshair";
            HtmlCursors[(int)CursorType.Help] = "help";
            HtmlCursors[(int)CursorType.IBeam] = "text";
            HtmlCursors[(int)CursorType.SizeAll] = "move";
            HtmlCursors[(int)CursorType.SizeNESW] = "nesw-resize";
            HtmlCursors[(int)CursorType.SizeNS] = "ns-resize";
            HtmlCursors[(int)CursorType.SizeNWSE] = "nwse-resize";
            HtmlCursors[(int)CursorType.SizeWE] = "ew-resize";
            HtmlCursors[(int)CursorType.UpArrow] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.Wait] = "wait";
            HtmlCursors[(int)CursorType.Hand] = "pointer";
            HtmlCursors[(int)CursorType.Pen] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollNS] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollWE] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollAll] = "all-scroll";
            HtmlCursors[(int)CursorType.ScrollN] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollS] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollW] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollE] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollNW] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollNE] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollSW] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ScrollSE] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.ArrowCD] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.Stylus] = "auto"; // not implemented
            HtmlCursors[(int)CursorType.Eraser] = "auto"; // not implemented
        }

        internal Cursor(CursorType cursorType)
        {
            if (IsValidCursorType(cursorType))
            {
                _cursorType = cursorType;
            }
            else
            {
                throw new ArgumentException(string.Format("'{0}' cursor type is not valid.", cursorType));
            }
        }

        internal static bool IsValidCursorType(CursorType cursorType)
        {
            return ((int)cursorType >= (int)CursorType.None && (int)cursorType <= (int)CursorType.Eraser);
        }

        internal string ToHtmlString()
        {
            return HtmlCursors[(int)_cursorType];
        }
    }

    /// <summary>
    ///     An enumeration of the supported cursor types.
    /// </summary>
    internal enum CursorType : int
    {
        /// <summary>
        ///     a value indicating that no cursor should be displayed at all.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Standard No Cursor.
        /// </summary>
        No = 1,

        /// <summary>
        ///     Standard arrow cursor.
        /// </summary>
        Arrow = 2,

        /// <summary>
        ///     Standard arrow with small hourglass cursor.
        /// </summary>
        AppStarting = 3,

        /// <summary>
        ///     Crosshair cursor.
        /// </summary>        
        Cross = 4,

        /// <summary>
        ///     Help cursor.
        /// </summary>        	
        Help = 5,

        /// <summary>
        ///     Text I-Beam cursor.
        /// </summary>
        IBeam = 6,

        /// <summary>
        ///     Four-way pointing cursor.
        /// </summary>
        SizeAll = 7,

        /// <summary>
        ///     Double arrow pointing NE and SW.
        /// </summary>
        SizeNESW = 8,

        /// <summary>
        ///     Double arrow pointing N and S.
        /// </summary>
        SizeNS = 9,

        /// <summary>
        ///     Double arrow pointing NW and SE.
        /// </summary>
        SizeNWSE = 10,

        /// <summary>
        ///     Double arrow pointing W and E.
        /// </summary>
        SizeWE = 11,

        /// <summary>
        ///     Vertical arrow cursor.
        /// </summary>
        UpArrow = 12,

        /// <summary>
        ///     Hourglass cursor.
        /// </summary>
        Wait = 13,

        /// <summary>
        ///     Hand cursor.
        /// </summary>
        Hand = 14,

        /// <summary>
        /// PenCursor
        /// </summary>
        Pen = 15,

        /// <summary>
        /// ScrollNSCursor
        /// </summary>
        ScrollNS = 16,

        /// <summary>
        /// ScrollWECursor
        /// </summary>
        ScrollWE = 17,

        /// <summary>
        /// ScrollAllCursor
        /// </summary>
        ScrollAll = 18,

        /// <summary>
        /// ScrollNCursor
        /// </summary>
        ScrollN = 19,

        /// <summary>
        /// ScrollSCursor
        /// </summary>
        ScrollS = 20,

        /// <summary>
        /// ScrollWCursor
        /// </summary>
        ScrollW = 21,

        /// <summary>
        /// ScrollECursor
        /// </summary>
        ScrollE = 22,

        /// <summary>
        /// ScrollNWCursor
        /// </summary>
        ScrollNW = 23,

        /// <summary>
        /// ScrollNECursor
        /// </summary>
        ScrollNE = 24,

        /// <summary>
        /// ScrollSWCursor
        /// </summary>
        ScrollSW = 25,

        /// <summary>
        /// ScrollSECursor
        /// </summary>
        ScrollSE = 26,

        /// <summary>
        /// ArrowCDCursor
        /// </summary>
        ArrowCD = 27,

        /// <summary>
        /// StylusCursor
        /// </summary>
        Stylus = 28,

        /// <summary>
        /// EraserCursor
        /// </summary>
        Eraser = 29,

        // Update the count in Cursors class and the HtmlCursors array
        // in the Cursor class if there is a new addition here.
    }
}