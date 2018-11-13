using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XJK.PInvoke
{
    public static partial class User32
    {
        /// <summary>
        /// Confines the cursor to a rectangular area on the screen. 
        /// If a subsequent cursor position (set by the SetCursorPos function or the mouse) lies outside the rectangle, the system automatically adjusts the position to keep the cursor inside the rectangular area.
        /// </summary>
        /// <param name="lpRect">A pointer to the structure that contains the screen coordinates of the upper-left and lower-right corners of the confining rectangle. 
        /// If this parameter is NULL, the cursor is free to move anywhere on the screen.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll")]
        public static extern bool ClipCursor(ref RECT lpRect);

        /// <summary>
        /// Retrieves the screen coordinates of the rectangular area to which the cursor is confined.
        /// </summary>
        /// <param name="lprc">A pointer to a RECT structure that receives the screen coordinates of the confining rectangle. 
        /// The structure receives the dimensions of the screen if the cursor is not confined to a rectangle.</param>
        /// <returns>If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.To get extended error information, call GetLastError.</returns>
        [DllImport("user32")]
        public static extern bool GetClipCursor(ref RECT lprc);

        /// <summary>
        /// Retrieves the position of the mouse cursor, in screen coordinates.
        /// </summary>
        /// <param name="lpPoint">A pointer to a POINT structure that receives the screen coordinates of the cursor.</param>
        /// <returns>Returns nonzero if successful or zero otherwise. To get extended error information, call GetLastError.</returns>
        [DllImport("user32")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        /// <summary>
        /// Moves the cursor to the specified screen coordinates. 
        /// If the new coordinates are not within the screen rectangle set by the most recent ClipCursor function call, the system automatically adjusts the coordinates so that the cursor stays within the rectangle.
        /// </summary>
        /// <param name="x">The new x-coordinate of the cursor, in screen coordinates.</param>
        /// <param name="y">The new y-coordinate of the cursor, in screen coordinates.</param>
        /// <returns>Returns nonzero if successful or zero otherwise. To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// Displays or hides the cursor.
        /// </summary>
        /// <param name="bShow">If bShow is TRUE, the display count is incremented by one. If bShow is FALSE, the display count is decremented by one.</param>
        /// <returns>The return value specifies the new display counter.</returns>
        /// <remarks>This function sets an internal display counter that determines whether the cursor should be displayed. 
        /// The cursor is displayed only if the display count is greater than or equal to 0. 
        /// If a mouse is installed, the initial display count is 0. If no mouse is installed, the display count is –1.</remarks>
        [DllImport("user32.dll")]
        public static extern int ShowCursor(bool bShow);
    }

}
