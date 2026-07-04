using System.Collections.ObjectModel;
using TaleWorlds.Library;

namespace WarAndAiTweaks.Companion;

public class CompanionHighlighterVM : ViewModel
{
	private MBBindingList<CompanionIconVM> _companions;

	[DataSourceProperty]
	public MBBindingList<CompanionIconVM> Companions
	{
		get
		{
			return _companions;
		}
		set
		{
			if (value != _companions)
			{
				_companions = value;
				((ViewModel)this).OnPropertyChangedWithValue<MBBindingList<CompanionIconVM>>(value, "Companions");
			}
		}
	}

	public CompanionHighlighterVM()
	{
		_companions = new MBBindingList<CompanionIconVM>();
	}

	public override void OnFinalize()
	{
		base.OnFinalize();
		((Collection<CompanionIconVM>)(object)_companions)?.Clear();
	}
}
