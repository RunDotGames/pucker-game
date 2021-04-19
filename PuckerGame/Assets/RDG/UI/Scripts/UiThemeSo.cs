using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIThemeColorType {
    Surface,
    OnSurface,
    LightSurface,
    DarkSurface,
    Primary,
    OnPrimary,
    DarkPrimary,
    LightPrimary,
    Secondary,
    OnSecondary,
    LightSecondary,
    DarkSecondary
}

public enum UIThemeFontType {
    HeadlineOne, HeadlineTwo, HeadlineThree, Subtitle, Body, Button
}

public enum UiThemeShapeType {
    Square, RoundedSquare, Circle, Check
}

[Serializable]
public class UiThemeColor {
    public string name;
    public UIThemeColorType type;
    public Color color;
}

[Serializable]
public class UiThemeFont {
    public string name;
    public UIThemeFontType type;
    public int size;
    public Font font;
    public bool isCaps;
    public float spacing;

}

[Serializable]
public class UiThemeShape {
    public string name;
    public UiThemeShapeType type;
    public Sprite sprite;
    public Sprite shadow;
    public Sprite outline;
    public float shadowSize;
    public bool preserveAspect = true;
    public Image.Type imageType = Image.Type.Sliced;
}

[Serializable]
public class UiThemeConfig {
    public List<UiThemeColor> colors;
    public List<UiThemeFont> fonts;
    public List<UiThemeShape> shapes;
}

public class UiTheme {
    private readonly UiThemeConfig config;
    private readonly Dictionary<UIThemeColorType, UiThemeColor> colors = new Dictionary<UIThemeColorType, UiThemeColor>();
    private readonly Dictionary<UIThemeFontType, UiThemeFont> fonts = new Dictionary<UIThemeFontType, UiThemeFont>();
    private readonly Dictionary<UiThemeShapeType, UiThemeShape> shapes = new Dictionary<UiThemeShapeType, UiThemeShape>();

    public UiTheme(UiThemeConfig config) {
        this.config = config;
        foreach (var aColor in config.colors) {
            colors[aColor.type] = aColor;
        }
        foreach (var aFont in config.fonts) {
            fonts[aFont.type] = aFont;
        }
        foreach (var aShape in config.shapes) {
            shapes[aShape.type] = aShape;
        }
    }

    public UiThemeColor GetColor(UIThemeColorType type) {
        return colors[type];
    }
    public UiThemeFont GetFont(UIThemeFontType type) {
        var font = fonts[type];
        if (font == null) {
            throw new Exception($"font type ${type} is undefined for theme");
        }
        return font;
    }

    public UiThemeShape GetShape(UiThemeShapeType shape) {
        return shapes[shape];
    }

}

[CreateAssetMenu(menuName = "RDG/UI/Theme")]
public class UiThemeSo : ScriptableObject {
    
    public UiThemeConfig theme;

    public UiTheme NewTheme() {
        return new UiTheme(theme);
    }
    
}
