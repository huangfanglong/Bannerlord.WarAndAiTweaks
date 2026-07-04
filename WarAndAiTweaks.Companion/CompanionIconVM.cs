using System;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Library;

namespace WarAndAiTweaks.Companion;

public class CompanionIconVM : ViewModel
{
	public enum IconType
	{
		Companion,
		FriendlyLord,
		EnemyLord
	}

	private float _positionX;

	private float _positionY;

	private float _width;

	private float _height;

	private bool _isVisible;

	private string _companionName;

	private int _fontSize;

	private string _sprite;

	private string _nameColor;

	private IconType _iconType;

	[DataSourceProperty]
	public float PositionX
	{
		get
		{
			return _positionX;
		}
		set
		{
			if (Math.Abs(value - _positionX) > 0.01f)
			{
				_positionX = value;
				base.OnPropertyChangedWithValue(value, "PositionX");
			}
		}
	}

	[DataSourceProperty]
	public float PositionY
	{
		get
		{
			return _positionY;
		}
		set
		{
			if (Math.Abs(value - _positionY) > 0.01f)
			{
				_positionY = value;
				base.OnPropertyChangedWithValue(value, "PositionY");
			}
		}
	}

	[DataSourceProperty]
	public float Width
	{
		get
		{
			return _width;
		}
		set
		{
			if (Math.Abs(value - _width) > 0.01f)
			{
				_width = value;
				base.OnPropertyChangedWithValue(value, "Width");
			}
		}
	}

	[DataSourceProperty]
	public float Height
	{
		get
		{
			return _height;
		}
		set
		{
			if (Math.Abs(value - _height) > 0.01f)
			{
				_height = value;
				base.OnPropertyChangedWithValue(value, "Height");
			}
		}
	}

	[DataSourceProperty]
	public bool IsVisible
	{
		get
		{
			return _isVisible;
		}
		set
		{
			if (value != _isVisible)
			{
				_isVisible = value;
				base.OnPropertyChangedWithValue(value, "IsVisible");
			}
		}
	}

	[DataSourceProperty]
	public string CompanionName
	{
		get
		{
			return _companionName;
		}
		set
		{
			if (value != _companionName)
			{
				_companionName = value ?? string.Empty;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "CompanionName");
			}
		}
	}

	[DataSourceProperty]
	public int FontSize
	{
		get
		{
			return _fontSize;
		}
		set
		{
			if (value != _fontSize)
			{
				_fontSize = value;
				base.OnPropertyChangedWithValue(value, "FontSize");
			}
		}
	}

	[DataSourceProperty]
	public string Sprite
	{
		get
		{
			return _sprite;
		}
		set
		{
			if (value != _sprite)
			{
				_sprite = value ?? string.Empty;
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "Sprite");
			}
		}
	}

	[DataSourceProperty]
	public string NameColor
	{
		get
		{
			return _nameColor;
		}
		set
		{
			if (value != _nameColor)
			{
				_nameColor = value ?? "#FFFFFFFF";
				((ViewModel)this).OnPropertyChangedWithValue<string>(value, "NameColor");
			}
		}
	}

	public IconType Type => _iconType;

	public CompanionIconVM(string companionName, IconType iconType = IconType.Companion)
	{
		_companionName = companionName ?? string.Empty;
		_width = 30f;
		_height = 30f;
		_fontSize = 16;
		_isVisible = false;
		_iconType = iconType;
		WarAndAiTweaksSettings instance = GlobalSettings<WarAndAiTweaksSettings>.Instance;
		if (_iconType == IconType.FriendlyLord)
		{
			_sprite = GetIconSprite(instance?.FriendlyLordIcon?.SelectedValue ?? "Noble");
			_nameColor = GetColorHex(instance?.FriendlyLordNameColor?.SelectedValue ?? "Yellow");
		}
		else if (_iconType == IconType.EnemyLord)
		{
			_sprite = GetIconSprite(instance?.EnemyLordIcon?.SelectedValue ?? "Hero");
			_nameColor = GetColorHex(instance?.EnemyLordNameColor?.SelectedValue ?? "Red");
		}
		else
		{
			_sprite = GetIconSprite(instance?.CompanionIcon?.SelectedValue ?? "Hero");
			_nameColor = GetColorHex(instance?.CompanionNameColor?.SelectedValue ?? "White");
		}
	}

	public static string GetColorHex(string colorName)
	{
		string result = colorName switch
		{
			"White" => "#FFFFFFFF", 
			"Yellow" => "#FFFF00FF", 
			"Red" => "#FF0000FF", 
			"Green" => "#00FF00FF", 
			"Blue" => "#0000FFFF", 
			"Cyan" => "#00FFFFFF", 
			"Magenta" => "#FF00FFFF", 
			"Orange" => "#FFA500FF", 
			"Purple" => "#800080FF", 
			"Black" => "#000000FF", 
			_ => "#FFFFFFFF", 
		};
		return result;
	}

	public static string GetIconSprite(string iconName)
	{
		string result = iconName switch
		{
			"Hero" => "General\\Mission\\hero_icon", 
			"Noble" => "General\\Mission\\noble", 
			"NPC" => "General\\Mission\\npc", 
			"Spear" => "General\\Mission\\spear-brace-active", 
			_ => "General\\Mission\\hero_icon", 
		};
		return result;
	}
}
