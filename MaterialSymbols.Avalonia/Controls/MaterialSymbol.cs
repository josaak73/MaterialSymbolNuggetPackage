using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace MaterialSymbols.Avalonia.Controls;

public class MaterialSymbol : Control
{
    public static readonly StyledProperty<string> IconProperty =
        AvaloniaProperty.Register<MaterialSymbol, string>(nameof(Icon), string.Empty);

    public static readonly StyledProperty<SymbolWeight> WeightProperty =
        AvaloniaProperty.Register<MaterialSymbol, SymbolWeight>(nameof(Weight), SymbolWeight.W400);

    public static readonly StyledProperty<SymbolStyle> StyleProperty =
        AvaloniaProperty.Register<MaterialSymbol, SymbolStyle>(nameof(Style), SymbolStyle.Outlined);

    public static readonly StyledProperty<bool> FilledProperty =
        AvaloniaProperty.Register<MaterialSymbol, bool>(nameof(Filled), false);

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<MaterialSymbol, IBrush?>(nameof(Foreground), null);

    private readonly PathIcon _pathIcon;

    public MaterialSymbol()
    {
        _pathIcon = new PathIcon();
        VisualChildren.Add(_pathIcon);
        LogicalChildren.Add(_pathIcon);
        
        UpdateGeometry();
        UpdateForeground();
    }

    public string Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public SymbolWeight Weight
    {
        get => GetValue(WeightProperty);
        set => SetValue(WeightProperty, value);
    }

    public SymbolStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }

    public bool Filled
    {
        get => GetValue(FilledProperty);
        set => SetValue(FilledProperty, value);
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    static MaterialSymbol()
    {
        AffectsRender<MaterialSymbol>(IconProperty, WeightProperty, StyleProperty, FilledProperty, ForegroundProperty);
        AffectsMeasure<MaterialSymbol>(IconProperty, WeightProperty, StyleProperty, FilledProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IconProperty ||
            change.Property == WeightProperty ||
            change.Property == StyleProperty ||
            change.Property == FilledProperty)
        {
            UpdateGeometry();
        }
        else if (change.Property == ForegroundProperty)
        {
            UpdateForeground();
        }
    }

    private void UpdateGeometry()
    {
        if (string.IsNullOrEmpty(Icon))
        {
            _pathIcon.Data = null;
            return;
        }

        var pathData = SymbolResourceProvider.GetPath(Icon, Weight, Style, Filled);
        
        if (!string.IsNullOrEmpty(pathData))
        {
            try
            {
                _pathIcon.Data = StreamGeometry.Parse(pathData);
            }
            catch
            {
                _pathIcon.Data = null;
            }
        }
        else
        {
            _pathIcon.Data = null;
        }
    }

    private void UpdateForeground()
    {
        if (Foreground != null)
        {
            _pathIcon.Foreground = Foreground;
        }
        else
        {
            _pathIcon.ClearValue(PathIcon.ForegroundProperty);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var desiredSize = new Size(
            double.IsNaN(Width) ? 24 : Width,
            double.IsNaN(Height) ? 24 : Height
        );
        
        // Constrain to available size
        desiredSize = new Size(
            Math.Min(desiredSize.Width, availableSize.Width),
            Math.Min(desiredSize.Height, availableSize.Height)
        );
        
        _pathIcon.Measure(desiredSize);
        return desiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _pathIcon.Arrange(new Rect(finalSize));
        return finalSize;
    }
}