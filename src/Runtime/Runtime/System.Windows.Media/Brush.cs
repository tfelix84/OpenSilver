﻿
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

using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Shapes;
using OpenSilver.Internal;
using OpenSilver.Internal.Media;

namespace System.Windows.Media
{
    /// <summary>
    /// Defines objects used to paint graphical objects. Classes that derive from
    /// Brush describe how the area is painted.
    /// </summary>
    public class Brush : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Brush"/> class.
        /// </summary>
        protected Brush() { }

        private protected Brush(Brush original)
        {
            Opacity = original.Opacity;
            RelativeTransform = original.RelativeTransform;
            Transform = original.Transform;
        }

        internal static Brush Parse(string source) =>
            new SolidColorBrush(Color.Parse(source));

        /// <summary>
        /// Identifies the <see cref="Opacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register(
                nameof(Opacity),
                typeof(double),
                typeof(Brush),
                new PropertyMetadata(1.0, OnPropertyChanged));

        /// <summary>
        /// Gets or sets the degree of opacity of a <see cref="Brush"/>.
        /// </summary>
        /// <returns>
        /// The value of the <see cref="Opacity"/> property is expressed as
        /// a value between 0 and 1.0. The default value is 1.0.
        /// </returns>
        public double Opacity
        {
            get => (double)GetValue(OpacityProperty);
            set => SetValueInternal(OpacityProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RelativeTransform" /> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty RelativeTransformProperty =
            DependencyProperty.Register(
                nameof(RelativeTransform),
                typeof(Transform),
                typeof(Brush),
                null);

        /// <summary>
        /// Gets or sets the transformation that is applied to the brush using relative coordinates.
        /// </summary>
        /// <returns>
        /// The transformation that is applied to the brush using relative coordinates. The default value is null.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Transform RelativeTransform
        {
            get => (Transform)GetValue(RelativeTransformProperty);
            set => SetValueInternal(RelativeTransformProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Transform"/> dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register(
                nameof(Transform),
                typeof(Transform),
                typeof(Brush),
                null);

        /// <summary>
        /// Gets or sets the transformation that is applied to the brush.
        /// </summary>
        /// <returns>
        /// The transformation to apply to the brush.
        /// </returns>
        [OpenSilver.NotImplemented]
        public Transform Transform
        {
            get => (Transform)GetValue(TransformProperty);
            set => SetValueInternal(TransformProperty, value);
        }

        internal virtual Task<string> GetDataStringAsync(UIElement parent) =>
            Task.FromResult(string.Empty);

        internal event EventHandler Changed;

        internal void RaiseBrushChanged() => Changed?.Invoke(this, EventArgs.Empty);

        internal static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Brush)d).RaiseBrushChanged();
        }

        internal virtual ISvgBrush GetSvgElement() => DefaultSvgBrush.Instance;

        private sealed class DefaultSvgBrush : ISvgBrush
        {
            private DefaultSvgBrush() { }

            public static DefaultSvgBrush Instance { get; } = new();

            public void DestroyBrush(Shape shape) { }

            public string GetBrush(Shape shape) => "none";
        }

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool LogBrushDeadWeakReferencesUpdates { get; set; } = false;

        [Obsolete(Helper.ObsoleteMemberMessage)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int UpdateBrushWeakReferencesSecs { get; set; } = 30;
    }
}
