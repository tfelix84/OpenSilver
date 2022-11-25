

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


#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Specifies the display state of an element.
    /// </summary>
    public enum PreCompileXaml
    {
        /// <summary>
        /// Default value, does not precompile css
        /// </summary>
        None = 0,
        /// <summary>
        /// Precompile to CSS local properties such as Background, CornerRadius, etc
        /// Precompile to CSS some layout info such as Grid structure
        /// </summary>
        BestCompatibility = 1,
        /// <summary>
        /// In addition to the BestCompatibility
        /// Replace most TemplateBindings by hard-coded values taken from the setters of the style where the ControlTemplate is defined. 
        /// Do this before precompiling to CSS. Don’t do this for “Content” and “ContentTemplate” of the ContentPresenter and others that could affect functionality.
        /// Precompile to HTML the shapes (Path, Rectangle…)
        /// </summary>
        Balanced = 2,
        /// <summary>
        /// In addition to the Balanced
        /// Precompile whole HTML of a control
        /// </summary>
        BestPerformance = 3,
    }
}