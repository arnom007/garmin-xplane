using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LOLA.Extensions;

namespace LauncherCoreLib.Models.Settings;

public class SettingSymbolTable : IModifiableSettingSymbolTable, ISettingSymbolTable, ISettingsProvider
{
	private readonly ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable> _settings;

	private readonly SettingTableConstraintWatcher _constraintWatcher;

	[field: CompilerGenerated]
	public IActiveSetting? NodeSetting
	{
		[CompilerGenerated]
		get;
	}

	public System.Collections.Generic.IEnumerable<IActiveSetting> AllSettings => Enumerable.Select<SettingSymbolTable, IActiveSetting>(Enumerable.Where<SettingSymbolTable>(_settings.BreadthWidthSearch(), (Func<SettingSymbolTable, bool>)((SettingSymbolTable n) => n.NodeSetting != null)), (Func<SettingSymbolTable, IActiveSetting>)((SettingSymbolTable n) => n.NodeSetting));

	public System.Collections.Generic.IEnumerable<IActiveSetting> Settings => Enumerable.Select<SettingSymbolTable, IActiveSetting>(Enumerable.Where<SettingSymbolTable>(Enumerable.Select<ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable>, SettingSymbolTable>(_settings.ImmediateNeighbors(), (Func<ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable>, SettingSymbolTable>)((ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable> kv) => kv.Value)), (Func<SettingSymbolTable, bool>)((SettingSymbolTable n) => n.NodeSetting != null)), (Func<SettingSymbolTable, IActiveSetting>)((SettingSymbolTable n) => n.NodeSetting));

	[field: CompilerGenerated]
	public string SettingsName
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		protected set;
	}

	public SettingSymbolTable()
		: this(null, null)
	{
	}

	public SettingSymbolTable(string tableName)
		: this(null, null)
	{
		SettingsName = tableName;
	}

	public SettingSymbolTable(IActiveSetting? setting, SettingSymbolTable? parent)
	{
		_settings = new ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable>(this);
		if (parent != null)
		{
			_settings.AddOrUpdate(SettingIdentifier.Parent, parent);
		}
		NodeSetting = setting;
		_constraintWatcher = new SettingTableConstraintWatcher(this);
		SettingsName = GetDefaultSettingName(setting);
	}

	public IModifiableSettingSymbolTable CreateNestedTable(SettingIdentifier localIdentifier)
	{
		SettingSymbolTable settingSymbolTable = CreateOrReplaceSetting(localIdentifier, null);
		settingSymbolTable.SettingsName = localIdentifier;
		return settingSymbolTable;
	}

	public T AddOrUpdateNestedTable<T>(SettingIdentifier localIdentifier, T nestedTable) where T : ISettingSymbolTable
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!(nestedTable is SettingSymbolTable settingSymbolTable))
		{
			throw new InvalidCastException("nestedTable must be a normal SettingSymbolTable.");
		}
		ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable> concurrentDirectedGraph = _settings.AddOrUpdate(localIdentifier, settingSymbolTable);
		settingSymbolTable._settings.AddOrUpdate(SettingIdentifier.Parent, this);
		concurrentDirectedGraph.Value.SettingsName = localIdentifier;
		return nestedTable;
	}

	public ISettingSymbolTable? GetNestedTable(SettingIdentifier localIdentifier)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable> byEdge = _settings.GetByEdge(localIdentifier);
		if (byEdge != null)
		{
			if (IsASettingNode(byEdge))
			{
				throw new InvalidOperationException("The " + localIdentifier + " setting identifier points to a setting not a table. Use GetSetting to get it.");
			}
			return byEdge.Value;
		}
		return null;
	}

	public ISettingSymbolTable? RemoveNestedTable(SettingIdentifier localIdentifier)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable> byEdge = _settings.GetByEdge(localIdentifier);
		if (byEdge != null)
		{
			if (IsASettingNode(byEdge))
			{
				throw new InvalidOperationException("The " + localIdentifier + " setting identifier points to a setting not a table. Use RemoveSetting to remove it.");
			}
			if (_settings.Remove(new KeyValuePair<SettingIdentifier, ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable>>(localIdentifier, byEdge)))
			{
				if (byEdge.Value.GetNestedTable(SettingIdentifier.Parent) == this)
				{
					byEdge.Value.RemoveNestedTable(SettingIdentifier.Parent);
				}
				return byEdge.Value;
			}
		}
		return null;
	}

	public IActiveSetting? GetByScopedIdentifier(SettingIdentifier id)
	{
		ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable> byEdge = _settings.GetByEdge(id);
		if (byEdge == null)
		{
			return _settings.GetByEdge(SettingIdentifier.Parent)?.Value.GetByScopedIdentifier(id);
		}
		return byEdge.Value.NodeSetting;
	}

	public IActiveSetting AddSetting(SettingIdentifier localIdentifier, ISettingDefinition settingDefinition)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		SettingIdentifier localIdentifier2 = localIdentifier;
		if (IsASettingNode(this))
		{
			throw new InvalidOperationException("Cannot add a setting a node that already has one.");
		}
		ActiveSetting activeSetting = new ActiveSetting(settingDefinition, _constraintWatcher, () => BuildRuntimePath(localIdentifier2));
		CreateOrReplaceSetting(localIdentifier2, activeSetting);
		return activeSetting;
	}

	public bool ContainsSetting(SettingIdentifier settingIdentifier)
	{
		if (_settings.ContainsEdge(settingIdentifier))
		{
			return IsASettingNode(_settings.GetByEdge(settingIdentifier));
		}
		return false;
	}

	public IActiveSetting? GetSetting(SettingIdentifier settingIdentifier)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable> byEdge = _settings.GetByEdge(settingIdentifier);
		if (byEdge != null)
		{
			if (!IsASettingNode(byEdge))
			{
				throw new InvalidOperationException("The " + settingIdentifier + " setting identifier points to a table not a setting. Use GetNestedTable to get it.");
			}
			return byEdge.Value.NodeSetting;
		}
		return null;
	}

	public IActiveSetting? RemoveSetting(SettingIdentifier localIdentifier)
	{
		return _settings.RemoveEdge(localIdentifier)?.Value.NodeSetting;
	}

	public virtual string ToString()
	{
		object obj;
		if (NodeSetting != null)
		{
			obj = ((object)NodeSetting).ToString();
			if (obj == null)
			{
				return "No setting name";
			}
		}
		else
		{
			obj = "Nested Settings Table";
		}
		return (string)obj;
	}

	protected System.Collections.Generic.IEnumerable<SettingSymbolTable> Ancestors()
	{
		ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable> byEdge = _settings.GetByEdge(SettingIdentifier.Parent);
		if (byEdge != null)
		{
			return Enumerable.Concat<SettingSymbolTable>(byEdge.Value.Yield(), byEdge.Value.Ancestors());
		}
		return Enumerable.Empty<SettingSymbolTable>();
	}

	private static bool IsASettingNode(ConcurrentDirectedGraph<SettingIdentifier, SettingSymbolTable> node)
	{
		return IsASettingNode(node.Value);
	}

	private static bool IsASettingNode(SettingSymbolTable table)
	{
		return table.NodeSetting != null;
	}

	private static string GetDefaultSettingName(IActiveSetting? setting)
	{
		return ((object)setting?.LocalId)?.ToString() ?? "ROOT";
	}

	private string BuildRuntimePath(SettingIdentifier localIdentifier)
	{
		return string.Join(".", Enumerable.Concat<string>(Enumerable.Select<SettingSymbolTable, string>(Enumerable.Concat<SettingSymbolTable>(Enumerable.Reverse<SettingSymbolTable>(Ancestors()), this.Yield()), (Func<SettingSymbolTable, string>)((SettingSymbolTable st) => st.SettingsName)), ((object)localIdentifier).ToString().Yield()));
	}

	private SettingSymbolTable CreateOrReplaceSetting(SettingIdentifier localIdentifier, IActiveSetting? setting)
	{
		return _settings.AddOrUpdate(localIdentifier, new SettingSymbolTable(setting, this)).Value;
	}
}
