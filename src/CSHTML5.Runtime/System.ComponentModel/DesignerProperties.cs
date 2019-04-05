﻿
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif
namespace System.ComponentModel
{
    /// <summary>
    /// Provides attached properties that can be used to communicate with a designer.
    /// </summary>
    public static class DesignerProperties
    {
        /// <summary>
        /// Gets the value of the System.ComponentModel.DesignerProperties.IsInDesignMode
        /// attached property for the specified System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element from which the property value is read.</param>
        /// <returns>
        /// The System.ComponentModel.DesignerProperties.IsInDesignMode property value
        /// for the element.
        /// </returns>
        public static bool GetIsInDesignMode(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("The element parameter cannot be null");
            return (bool)element.GetValue(IsInDesignModeProperty);
        }
        /// <summary>
        /// Sets the value of the System.ComponentModel.DesignerProperties.IsInDesignMode
        /// attached property to a specified element.
        /// </summary>
        /// <param name="element">The element to which the attached property is written.</param>
        /// <param name="value">The needed System.Boolean value.</param>
        public static void SetIsInDesignMode(DependencyObject element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException("The element parameter cannot be null");
            element.SetValue(IsInDesignModeProperty, value);
        }
        /// <summary>
        /// Identifies the System.ComponentModel.DesignerProperties.IsInDesignMode attached
        /// property.
        /// </summary>
        public static readonly DependencyProperty IsInDesignModeProperty =
            DependencyProperty.RegisterAttached("IsInDesignMode", typeof(bool), typeof(DesignerProperties), new PropertyMetadata(false));



        /// <summary>
        /// Gets a value that indicates whether the element is running in the context
        /// of a designer.
        /// </summary>
        public static bool IsInDesignTool { get { return false; } } //Note: In SL, it seems that there is an accessible "set" but I don't see the point so I removed it for now.

        ///// <summary>
        ///// Gets or sets a value that indicates whether all the metadata associated with
        ///// an assembly or just the xmlns namespace definitions should be refreshed in
        ///// the designer when an assembly is recompiled.
        ///// </summary>
        //public static bool RefreshOnlyXmlnsDefinitionsOnAssemblyReplace { get; set; }

    }
}