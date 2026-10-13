using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Principal;

namespace LauncherCoreLib.Models.DeployedEnv;

public class ConfidentialAliasProvider : IConfidentialAliasProvider
{
	[CompilerGenerated]
	private sealed class _003CGenerateAliases_003Ed__7 : System.Collections.Generic.IEnumerable<ConfidentialAlias>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<ConfidentialAlias>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private ConfidentialAlias _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private string _003Cusername_003E5__2;

		ConfidentialAlias System.Collections.Generic.IEnumerator<ConfidentialAlias>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object System.Collections.IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CGenerateAliases_003Ed__7(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			string fileName;
			string machineName;
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
			{
				_003C_003E1__state = -1;
				string name = ((ClaimsIdentity)WindowsIdentity.GetCurrent()).Name;
				if (!string.IsNullOrEmpty(name))
				{
					_003C_003E2__current = new ConfidentialAlias(name, "(DOMAIN_AND_USER)");
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_0062;
			}
			case 1:
				_003C_003E1__state = -1;
				goto IL_0062;
			case 2:
			{
				_003C_003E1__state = -1;
				string text = char.ToUpper(_003Cusername_003E5__2[0], CultureInfo.CurrentCulture) + _003Cusername_003E5__2.Substring(1);
				if (!string.Equals(text, _003Cusername_003E5__2, (StringComparison)4))
				{
					_003C_003E2__current = new ConfidentialAlias(text, "(MSI_USER)");
					_003C_003E1__state = 3;
					return true;
				}
				goto IL_0104;
			}
			case 3:
				_003C_003E1__state = -1;
				goto IL_0104;
			case 4:
				_003C_003E1__state = -1;
				goto IL_0144;
			case 5:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0062:
				_003Cusername_003E5__2 = Environment.UserName;
				if (!string.IsNullOrEmpty(_003Cusername_003E5__2))
				{
					_003C_003E2__current = new ConfidentialAlias(_003Cusername_003E5__2, "(USER)");
					_003C_003E1__state = 2;
					return true;
				}
				goto IL_0144;
				IL_0104:
				fileName = Path.GetFileName(Environment.GetFolderPath((SpecialFolder)40));
				if (!string.Equals(fileName, _003Cusername_003E5__2, (StringComparison)4))
				{
					_003C_003E2__current = new ConfidentialAlias(fileName, "(PROFILE_NAME)");
					_003C_003E1__state = 4;
					return true;
				}
				goto IL_0144;
				IL_0144:
				machineName = Environment.MachineName;
				if (machineName != null)
				{
					_003C_003E2__current = new ConfidentialAlias(machineName, "(MACHINE_NAME)");
					_003C_003E1__state = 5;
					return true;
				}
				break;
			}
			return false;
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<ConfidentialAlias> System.Collections.Generic.IEnumerable<ConfidentialAlias>.GetEnumerator()
		{
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				return this;
			}
			return new _003CGenerateAliases_003Ed__7(0);
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<ConfidentialAlias>)this).GetEnumerator();
		}
	}

	[field: CompilerGenerated]
	public static ConfidentialAliasProvider Instance
	{
		[CompilerGenerated]
		get;
	} = new ConfidentialAliasProvider();


	[field: CompilerGenerated]
	public System.Collections.Generic.IEnumerable<ConfidentialAlias> Aliases
	{
		[CompilerGenerated]
		get;
	}

	public ConfidentialAliasProvider()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Aliases = (System.Collections.Generic.IEnumerable<ConfidentialAlias>)(object)ImmutableArray.ToImmutableArray<ConfidentialAlias>(GenerateAliases());
	}

	[IteratorStateMachine(typeof(_003CGenerateAliases_003Ed__7))]
	private static System.Collections.Generic.IEnumerable<ConfidentialAlias> GenerateAliases()
	{
		string name = ((ClaimsIdentity)WindowsIdentity.GetCurrent()).Name;
		if (!string.IsNullOrEmpty(name))
		{
			yield return new ConfidentialAlias(name, "(DOMAIN_AND_USER)");
		}
		string username = Environment.UserName;
		if (!string.IsNullOrEmpty(username))
		{
			yield return new ConfidentialAlias(username, "(USER)");
			string text = char.ToUpper(username[0], CultureInfo.CurrentCulture) + username.Substring(1);
			if (!string.Equals(text, username, (StringComparison)4))
			{
				yield return new ConfidentialAlias(text, "(MSI_USER)");
			}
			string fileName = Path.GetFileName(Environment.GetFolderPath((SpecialFolder)40));
			if (!string.Equals(fileName, username, (StringComparison)4))
			{
				yield return new ConfidentialAlias(fileName, "(PROFILE_NAME)");
			}
		}
		string machineName = Environment.MachineName;
		if (machineName != null)
		{
			yield return new ConfidentialAlias(machineName, "(MACHINE_NAME)");
		}
	}
}
