using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using LOLA.Logging;
using Microsoft.Win32;

namespace LauncherCoreLib.Models.DeployedEnv;

public class RealityXpDetector : IRealityXpInstallationDetector
{
	private sealed class InstalledProduct
	{
		[field: CompilerGenerated]
		public string RegistryPath
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = string.Empty;


		[field: CompilerGenerated]
		public string DisplayName
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = string.Empty;


		[field: CompilerGenerated]
		public string Publisher
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = string.Empty;

	}

	[CompilerGenerated]
	private sealed class _003CGetValuesFromInstalledPrograms_003Ed__2 : System.Collections.Generic.IEnumerable<InstalledProduct>, System.Collections.IEnumerable, System.Collections.Generic.IEnumerator<InstalledProduct>, System.Collections.IEnumerator, System.IDisposable
	{
		private int _003C_003E1__state;

		private InstalledProduct _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private string rootKey;

		public string _003C_003E3__rootKey;

		private RegistryKey _003Crk_003E5__2;

		private string[] _003C_003E7__wrap2;

		private int _003C_003E7__wrap3;

		private RegistryKey _003Csk_003E5__5;

		InstalledProduct System.Collections.Generic.IEnumerator<InstalledProduct>.Current
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
		public _003CGetValuesFromInstalledPrograms_003Ed__2(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void System.IDisposable.Dispose()
		{
			int num = _003C_003E1__state;
			if ((uint)(num - -4) > 1u && num != 1)
			{
				return;
			}
			try
			{
				if (num != -4 && num != 1)
				{
					return;
				}
				try
				{
				}
				finally
				{
					_003C_003Em__Finally2();
				}
			}
			finally
			{
				_003C_003Em__Finally1();
			}
		}

		private bool MoveNext()
		{
			bool result;
			try
			{
				int num = _003C_003E1__state;
				if (num == 0)
				{
					_003C_003E1__state = -1;
					_003Crk_003E5__2 = Registry.LocalMachine.OpenSubKey(rootKey);
					_003C_003E1__state = -3;
					if (_003Crk_003E5__2 == null)
					{
						result = false;
						goto IL_017b;
					}
					_003C_003E7__wrap2 = _003Crk_003E5__2.GetSubKeyNames();
					_003C_003E7__wrap3 = 0;
					goto IL_015f;
				}
				if (num == 1)
				{
					_003C_003E1__state = -4;
					_003C_003Em__Finally2();
					_003Csk_003E5__5 = null;
					goto IL_0151;
				}
				result = false;
				goto end_IL_0000;
				IL_0151:
				_003C_003E7__wrap3++;
				goto IL_015f;
				IL_015f:
				if (_003C_003E7__wrap3 >= _003C_003E7__wrap2.Length)
				{
					_003C_003E7__wrap2 = null;
					result = false;
					goto IL_017b;
				}
				string text = _003C_003E7__wrap2[_003C_003E7__wrap3];
				_003Csk_003E5__5 = _003Crk_003E5__2.OpenSubKey(text);
				_003C_003E1__state = -4;
				if (_003Csk_003E5__5 == null)
				{
					goto IL_0142;
				}
				InstalledProduct installedProduct;
				try
				{
					installedProduct = new InstalledProduct
					{
						RegistryPath = rootKey + "\\" + text,
						DisplayName = ((_003Csk_003E5__5.GetValue("DisplayName") as string) ?? string.Empty),
						Publisher = ((_003Csk_003E5__5.GetValue("Publisher") as string) ?? string.Empty)
					};
				}
				catch (System.Exception e)
				{
					Logger.LogFormattedException(e, "Detecting Reality XP at " + text + ".");
					goto IL_0142;
				}
				_003C_003E2__current = installedProduct;
				_003C_003E1__state = 1;
				result = true;
				goto end_IL_0000;
				IL_017b:
				_003C_003Em__Finally1();
				goto end_IL_0000;
				IL_0142:
				_003C_003Em__Finally2();
				goto IL_0151;
				end_IL_0000:;
			}
			catch
			{
				//try-fault
				((System.IDisposable)this).Dispose();
				throw;
			}
			return result;
		}

		bool System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		private void _003C_003Em__Finally1()
		{
			_003C_003E1__state = -1;
			if (_003Crk_003E5__2 != null)
			{
				((System.IDisposable)_003Crk_003E5__2).Dispose();
			}
		}

		private void _003C_003Em__Finally2()
		{
			_003C_003E1__state = -3;
			if (_003Csk_003E5__5 != null)
			{
				((System.IDisposable)_003Csk_003E5__5).Dispose();
			}
		}

		[DebuggerHidden]
		void System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		System.Collections.Generic.IEnumerator<InstalledProduct> System.Collections.Generic.IEnumerable<InstalledProduct>.GetEnumerator()
		{
			_003CGetValuesFromInstalledPrograms_003Ed__2 _003CGetValuesFromInstalledPrograms_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CGetValuesFromInstalledPrograms_003Ed__ = this;
			}
			else
			{
				_003CGetValuesFromInstalledPrograms_003Ed__ = new _003CGetValuesFromInstalledPrograms_003Ed__2(0);
			}
			_003CGetValuesFromInstalledPrograms_003Ed__.rootKey = _003C_003E3__rootKey;
			return _003CGetValuesFromInstalledPrograms_003Ed__;
		}

		[DebuggerHidden]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (System.Collections.IEnumerator)((System.Collections.Generic.IEnumerable<InstalledProduct>)this).GetEnumerator();
		}
	}

	private const string REALITY_XP_PUBLISHER_NAME = "Reality XP";

	public IObservable<string> RealityXpInstallationSigns()
	{
		return Enumerable.Select<InstalledProduct, string>(Enumerable.Where<InstalledProduct>(Enumerable.Concat<InstalledProduct>(GetValuesFromInstalledPrograms("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall"), GetValuesFromInstalledPrograms("SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall")), (Func<InstalledProduct, bool>)((InstalledProduct ip) => string.Equals(ip.Publisher, "Reality XP", (StringComparison)5))), (Func<InstalledProduct, string>)((InstalledProduct ip) => string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "Found installed Reality XP program '{0}' at '{1}'", (object)ip.DisplayName, (object)ip.RegistryPath))).ToObservable();
	}

	[IteratorStateMachine(typeof(_003CGetValuesFromInstalledPrograms_003Ed__2))]
	private static System.Collections.Generic.IEnumerable<InstalledProduct> GetValuesFromInstalledPrograms(string rootKey)
	{
		RegistryKey rk = Registry.LocalMachine.OpenSubKey(rootKey);
		try
		{
			if (rk == null)
			{
				yield break;
			}
			string[] subKeyNames = rk.GetSubKeyNames();
			foreach (string text in subKeyNames)
			{
				RegistryKey sk = rk.OpenSubKey(text);
				try
				{
					if (sk != null)
					{
						InstalledProduct installedProduct;
						try
						{
							installedProduct = new InstalledProduct
							{
								RegistryPath = rootKey + "\\" + text,
								DisplayName = ((sk.GetValue("DisplayName") as string) ?? string.Empty),
								Publisher = ((sk.GetValue("Publisher") as string) ?? string.Empty)
							};
						}
						catch (System.Exception e)
						{
							Logger.LogFormattedException(e, "Detecting Reality XP at " + text + ".");
							goto end_IL_008c;
						}
						yield return installedProduct;
					}
					end_IL_008c:;
				}
				finally
				{
					((System.IDisposable)sk)?.Dispose();
				}
			}
		}
		finally
		{
			((System.IDisposable)rk)?.Dispose();
		}
	}
}
