

/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Forms;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Windows;

namespace DotNetForHtml5.Compiler
{
    internal static class ProcessingPrecompileCSS
    {
        internal static readonly XNamespace xNamespace = @"http://schemas.microsoft.com/winfx/2006/xaml"; // Used for example for "x:Name" attributes and {x:Null} markup extensions.
        internal static readonly string PreCompiledCssPropertyName = "PreCompiledCss";
        //------------------------------------------------------------
        // This class will pre-compile properties to static css values
        // For example:
        //   CornerRadius="3"
        //   Background="White"
        //  Becomes:
        //   PreCompiledCss="background: rgb(255,255,255); border-radius: 3px;"
        //------------------------------------------------------------

        public static void Process(XDocument doc, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain)
        {
            TraverseNextNode(doc.Root, null, null, reflectionOnSeparateAppDomain, PreCompileXaml.None);
        }

        private static Exception GetConvertException(string value, string destinationTypeFullName)
        {
            return new XamlParseException(
                $"Cannot convert '{value}' to '{destinationTypeFullName}'."
            );
        }

        static string ConvertToCornerRadiusCSS(string source)
        {
            char[] separator = new char[2] { ',', ' ' };
            string[] split = source.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            switch (split.Length)
            {
                case 1:
                    return $"{split[0]}px";

                case 4:
                    return $"{split[0]}px {split[1]}px {split[2]}px {split[3]}px";
            }

            throw GetConvertException(source, "System.Windows.CornerRadius");
        }

        static string ConvertToHTMLCursor(string source)
        {
            if (Enum.TryParse(source, out CursorType type) == false)
            {
                throw GetConvertException(source, "System.Windows.Input.CursorType");
            }

            Cursor cursor = new Cursor(type);
            return cursor.ToHtmlString();
        }

        /// <summary>
        /// Copied from GeneratingCSharpCode.IsAttributeTheXNameAttribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        private static bool IsAttributeTheXNameAttribute(XAttribute attribute)
        {
            bool isXName = (attribute.Name.LocalName == "Name" && attribute.Name.NamespaceName == xNamespace);
            bool isName = (attribute.Name.LocalName == "Name" && string.IsNullOrEmpty(attribute.Name.NamespaceName));
            return isXName || isName;
        }

        private static string PrecompileProperty(PreCompileXaml precompileOption, string targetTypeName, string propertyName, string propertyValue, object setterOrAttribute)
        {
            bool removeObject = false;
            string compiledCSS = "";
            if (propertyName == PreCompiledCssPropertyName)
            {
                compiledCSS = propertyValue;
            }
            if (propertyName == "Background")
            {
                if (targetTypeName == "Border" || targetTypeName == "TextBlock" || targetTypeName == "Grid")
                {
                    string htmlColor = Color.ConvertToHtmlColor(propertyValue);
                    compiledCSS = $"background:{htmlColor};backgroundColor:{htmlColor};backgroundColorAlpha:{htmlColor};";
                }
                else
                {
                    // Other controls have ControlTemplate
                    removeObject = true;
                }

                // Check IsHitTestVisible property
                bool IsHitTestVisible = true;

                if (setterOrAttribute is XElement element)
                {
                    if (GeneratingCSharpCode.IsSetter(element))
                    {
                        XElement settersElement = element.Parent;
                        List<XNode> childNodes = settersElement.Nodes().ToList();
                        for (int i = childNodes.Count - 1; i >= 0; i--)
                        {
                            if (childNodes[i] is XElement childElement && childNodes[i].NodeType == XmlNodeType.Element)
                            {
                                if (GeneratingCSharpCode.IsSetter(childElement))
                                {
                                    XAttribute property = childElement.Attribute(XName.Get("Property"));
                                    XAttribute value = childElement.Attribute(XName.Get("Value"));

                                    if (property != null && property.Value == "IsHitTestVisible" && value != null && value.Value.ToLower() == "false")
                                    {
                                        IsHitTestVisible = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (setterOrAttribute is XAttribute attribute)
                {
                    XElement parentElement = attribute.Parent;
                    XAttribute attrIsHitTestVisible = parentElement.Attribute(XName.Get("IsHitTestVisible"));
                    if (attrIsHitTestVisible != null && attrIsHitTestVisible.Value.ToLower() == "false")
                    {
                        IsHitTestVisible = false;
                    }
                }

                if (IsHitTestVisible)
                {
                    compiledCSS += "pointerEvents:auto;";
                }
            }
            if (propertyName == "Foreground")
            {
                if (targetTypeName == "Border" || targetTypeName == "TextBlock" || targetTypeName == "Grid")
                {
                    string htmlColor = Color.ConvertToHtmlColor(propertyValue);
                    compiledCSS = $"color:{htmlColor};colorAlpha:{htmlColor};";
                }
                else
                {
                    // Other controls have ControlTemplate
                    removeObject = true;
                }
            }
            if (propertyName == "BorderBrush")
            {
                if (targetTypeName == "Border" || targetTypeName == "TextBlock" || targetTypeName == "Grid")
                {
                    string htmlColor = Color.ConvertToHtmlColor(propertyValue);
                    compiledCSS = $"borderColor:{htmlColor};";
                }
                else
                {
                    // Other controls have ControlTemplate
                    removeObject = true;
                }
            }
            if (propertyName == "Opacity")
            {
                compiledCSS = $"opacity:{propertyValue};"; 
            }
            if (propertyName == "CornerRadius")
            {
                compiledCSS = $"borderRadius:{ConvertToCornerRadiusCSS(propertyValue)};";
            }
            if (propertyName == "Cursor")
            {
                compiledCSS = $"cursor:{ConvertToHTMLCursor(propertyValue)};";
            }

            if (removeObject || compiledCSS.Length > 0)
            {
                if (setterOrAttribute is XElement element)
                {
                    element.Remove();
                }
                if (setterOrAttribute is XAttribute attribute)
                {
                    attribute.Remove();
                }
            }

            return compiledCSS;
        }

        private static string GetBindedStaticValue(XElement styleTargetElement, string bindedTargetProperty)
        {
            if (styleTargetElement == null)
                return "";

            XElement styleSetters = null;
            List<XNode> childNodes = styleTargetElement.Nodes().ToList();
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (childNodes[i] is XElement childElement && childNodes[i].NodeType == XmlNodeType.Element)
                {
                    if (childElement.Name.LocalName == "Style.Setters")
                    {
                        styleSetters = childElement;
                        break;
                    }
                }
            }
            if (styleSetters == null)
                return "";

            childNodes = styleSetters.Nodes().ToList();
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (childNodes[i] is XElement childElement && childNodes[i].NodeType == XmlNodeType.Element)
                {
                    if (GeneratingCSharpCode.IsSetter(childElement))
                    {
                        XAttribute property = childElement.Attribute(XName.Get("Property"));
                        XAttribute value = childElement.Attribute(XName.Get("Value"));

                        if (property != null && property.Value == bindedTargetProperty)
                        {
                            if (value == null)
                                return "";

                            return value.Value;
                        }
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Convert LinearGradientBrush to html css style
        /// </summary>
        /// <param name="parentElement">Border</param>
        /// <param name="colorElement">Border.Background</param>
        /// <param name="brushElement">LinearGradientBrush StartPoint=".7,0" EndPoint=".7,1"</param>
        /// <returns></returns>
        private static void ConvertLinearGradientBrush(XElement parentElement, XElement colorElement, XElement brushElement)
        {
            XElement gradientStops = null;
            List<XNode> childNodes = brushElement.Nodes().ToList();
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (childNodes[i] is XElement childElement && childNodes[i].NodeType == XmlNodeType.Element)
                {
                    if (childElement.Name.LocalName == "LinearGradientBrush.GradientStops")
                    {
                        gradientStops = childElement;
                        break;
                    }
                }
            }
            if (gradientStops == null)
                return;

            XAttribute attrStartPoint = brushElement.Attribute(XName.Get("StartPoint"));
            XAttribute attrEndPoint = brushElement.Attribute(XName.Get("EndPoint"));

            if (attrStartPoint == null || attrEndPoint == null)
                return;

            Point StartPoint, EndPoint;
            try
            {
                StartPoint = Point.Parse(attrStartPoint.Value);
                EndPoint = Point.Parse(attrEndPoint.Value);
            }
            catch(Exception)
            {
                return;
            }

            double alpha;
            if (StartPoint == EndPoint)
            {
                alpha = 0;
            }
            else
            {
                double height = 1; //Note: 1 is ok since it is the ratio that is important
                double width = 1;

                double startX = StartPoint.X;
                double startY = StartPoint.Y;
                double endX = EndPoint.X;
                double endY = EndPoint.Y;

                double XVariation = (endX - startX) * width;
                double YVariation = (endY - startY) * height;
                if (XVariation < 0)
                {
                    // Note: these are to "fix" the value returned by the ArcTan (the result
                    // is basically the same angle but in the opposite direction)
                    XVariation = -XVariation;
                    YVariation = -YVariation;
                }

                // Note: this is basically _angle in radians, and that takes into consideration
                // the possibility of width and height being different.
                alpha = Math.Atan2(XVariation, YVariation) - Math.PI / 2;
            }
            double angle = alpha * 180 / Math.PI;
            // for browser
            angle = 360 - angle + 90;

            string gradientHtml = $"linear-gradient({angle}deg";

            childNodes = gradientStops.Nodes().ToList();
            for (int i = 0; i < childNodes.Count; i++)
            {
                if (childNodes[i] is XElement childElement && childNodes[i].NodeType == XmlNodeType.Element)
                {
                    if (childElement.Name.LocalName == "GradientStop") 
                    {
                        // <GradientStop Color="#FFFFFFFF" Offset="0" />
                        XAttribute attrColor = childElement.Attribute(XName.Get("Color"));
                        XAttribute attrOffset = childElement.Attribute(XName.Get("Offset"));

                        if (attrColor == null || attrOffset == null)
                            return;

                        double offset;
                        if (double.TryParse(attrOffset.Value, out offset) == false)
                            return;

                        gradientHtml += $", {Color.ConvertToHtmlColor(attrColor.Value)} {(offset * 100)}%";
                    }
                }
            }
            gradientHtml += ")";

            int index = colorElement.Name.LocalName.IndexOf(".");
            if (index == -1)
                return;

            string propertyName = colorElement.Name.LocalName.Substring(index + 1);
            string compiledCSS = "";
            if (propertyName == "Background")
            {
                compiledCSS = $"background:{gradientHtml};backgroundColor:{gradientHtml};backgroundColorAlpha:{gradientHtml};";
            }
            if (propertyName == "Foreground")
            {
                compiledCSS = $"color:{gradientHtml};colorAlpha:{gradientHtml};";
            }
            if (propertyName == "BorderBrush")
            {
                compiledCSS = $"borderColor:{gradientHtml};";
            }

            if (compiledCSS.Length > 0)
            {
                colorElement.Remove();

                XAttribute precompiledCSSProperty = parentElement.Attribute(XName.Get(PreCompiledCssPropertyName));
                if (precompiledCSSProperty == null)
                {
                    parentElement.SetAttributeValue(XName.Get(PreCompiledCssPropertyName), compiledCSS);
                }
                else
                {
                    parentElement.SetAttributeValue(XName.Get(PreCompiledCssPropertyName), precompiledCSSProperty.Value + compiledCSS);
                }
            }
        }

        private static string TraverseNextNode(XNode currentNode, XElement styleTargetElement, XElement parentElement, ReflectionOnSeparateAppDomainHandler reflectionOnSeparateAppDomain, PreCompileXaml precompileOption)
        {
            XElement currentElement = currentNode as XElement;
            if (currentElement != null)
            {
                XAttribute attribute = currentElement.Attribute(XName.Get("PreCompileXaml"));
                if (attribute != null)
                {
                    PreCompileXaml preCompile;
                    if (Enum.TryParse<PreCompileXaml>(attribute.Value, out preCompile))
                    {
                        precompileOption = preCompile;
                    }
                }

                if (GeneratingCSharpCode.IsStyle(currentElement))
                {
                    styleTargetElement = currentElement;

                    XAttribute attrTargetType = styleTargetElement.Attribute(XName.Get("TargetType"));
                    if (attrTargetType == null)
                    {
                        throw new XamlParseException("Style must declare a TargetType.");
                    }
                }
            }

            if (currentElement != null)
            {
                if (currentElement.Name.LocalName.EndsWith(".Effect"))
                {
                    return "";
                }

                List<string> listPreCompiledCSS = new List<string>();
                List<XNode> childNodes = ((XElement)currentNode).Nodes().ToList(); // Node: we convert to list because the code inside the loop below is going to modify the collection, so a "foreach" is not appropriate.
                for (int i = childNodes.Count - 1; i >= 0; i--)
                {
                    string ret = TraverseNextNode(childNodes[i], styleTargetElement, ((XElement)currentNode), reflectionOnSeparateAppDomain, precompileOption);
                    if (ret.Length > 0)
                    {
                        listPreCompiledCSS.Add(ret);
                    }
                }

                string PreCompiledCss = string.Join(" ", listPreCompiledCSS.ToArray());

                XAttribute xNameAttr = currentElement.Attributes().FirstOrDefault(attr => IsAttributeTheXNameAttribute(attr));

                if (precompileOption != PreCompileXaml.None)
                {
                    if (GeneratingCSharpCode.IsStyle(currentElement) && PreCompiledCss.Length > 0)
                    {
                        XElement preStyleProperty = new XElement(XName.Get("Setter", GeneratingCSharpCode.DefaultXamlNamespace));
                        preStyleProperty.SetAttributeValue(XName.Get("Property"), PreCompiledCssPropertyName);
                        preStyleProperty.SetAttributeValue(XName.Get("Value"), PreCompiledCss);

                        childNodes = ((XElement)currentNode).Nodes().ToList();
                        for (int i = childNodes.Count - 1; i >= 0; i--)
                        {
                            if (childNodes[i] is XElement childElement && childNodes[i].NodeType == XmlNodeType.Element)
                            {
                                if (childElement.Name.LocalName == "Style.Setters")
                                {
                                    childElement.Add(preStyleProperty);
                                    break;
                                }
                            }
                        }

                        return "";
                    }

                    if (GeneratingCSharpCode.IsSetter(currentElement))
                    {
                        XAttribute property = currentElement.Attribute(XName.Get("Property"));
                        XAttribute value = currentElement.Attribute(XName.Get("Value"));

                        if (property == null || value == null)
                            return "";

                        string tartetType = styleTargetElement.Attribute(XName.Get("TargetType")).Value;

                        string precompiled = PrecompileProperty(precompileOption, tartetType, property.Value, value.Value, currentElement);
                        if (precompiled.Length > 0)
                        {
                            /*XElement setterValue = new XElement(XName.Get("Setter.Value", GeneratingCSharpCode.DefaultXamlNamespace));
                            XElement nullValue = new XElement(XName.Get("NullExtension", @"http://schemas.microsoft.com/winfx/2006/xaml"));
                            setterValue.Add(nullValue);
                            value.Remove();
                            currentElement.Add(setterValue);*/

                            // currentElement.Remove();
                        }
                        return precompiled;
                    }

                    // Replace TemplateBindings by hard-coded value from setters
                    if (currentElement.Name.LocalName.Contains("."))
                    {
                        bool isBinding = false;
                        for (int i = childNodes.Count - 1; i >= 0; i--)
                        {
                            if (childNodes[i] is XElement childElement && childNodes[i].NodeType == XmlNodeType.Element)
                            {
                                if (childElement.Name.LocalName == "TemplateBindingExtension")
                                {
                                    string bindedTargetProperty = childElement.Attribute(XName.Get("Path")).Value;
                                    string bindedStaticValue = GetBindedStaticValue(styleTargetElement, bindedTargetProperty);

                                    if (bindedStaticValue.Length > 0)
                                    {
                                        string[] splittedAttachedProperty = currentElement.Name.LocalName.Split('.');
                                        string bindedProperty = splittedAttachedProperty[1];
                                        string bindedCss = PrecompileProperty(precompileOption, parentElement.Name.LocalName, bindedProperty, bindedStaticValue, currentElement);
                                        if (bindedCss.Length > 0)
                                        {
                                            XAttribute precompiledCSSProperty = parentElement.Attribute(XName.Get(PreCompiledCssPropertyName));
                                            if (precompiledCSSProperty == null)
                                            {
                                                parentElement.SetAttributeValue(XName.Get(PreCompiledCssPropertyName), bindedCss);
                                            }
                                            else
                                            {
                                                parentElement.SetAttributeValue(XName.Get(PreCompiledCssPropertyName), precompiledCSSProperty.Value + bindedCss);
                                            }
                                        }
                                    }

                                    isBinding = true;
                                    break;
                                }

                                if (childElement.Name.LocalName == "LinearGradientBrush")
                                {
                                    ConvertLinearGradientBrush(parentElement, currentElement, childElement);
                                }
                            }
                        }

                        if (isBinding)
                            return "";
                    }
                    
                    // Compile attributes to css styles
                    if (styleTargetElement != null && currentElement.HasAttributes)
                    {
                        if (reflectionOnSeparateAppDomain.DoesTypeContainMemberOfTypeString(PreCompiledCssPropertyName, currentElement.Name.Namespace.NamespaceName, currentElement.Name.LocalName) == false)
                            return "";

                        XAttribute[] xAttributes = currentElement.Attributes().ToArray();
                        string cssStyleAttribute = "";
                        for(int i = xAttributes.Length - 1; i >= 0; i--)
                        {
                            XAttribute attribute = xAttributes[i];
                            cssStyleAttribute += PrecompileProperty(precompileOption, currentElement.Name.LocalName, attribute.Name.LocalName, attribute.Value, attribute);
                        }

                        if (cssStyleAttribute.Length > 0)
                        {
                            currentElement.SetAttributeValue(XName.Get(PreCompiledCssPropertyName), cssStyleAttribute);
                        }
                    }
                }

                return PreCompiledCss;
            }

            return "";
        }
    }
}
