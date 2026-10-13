using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GTI.Net;
using LOLA.Logging;
using LOLA.StringHelpers.Paths;
using LOLA.Tasks;
using LauncherCoreLib.Models.EventLogging;
using LauncherCoreLib.Models.EventLogging.DataDescriptors;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.TrainerPackages;

public static class CatalogRefugeExtensions
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_0
	{
		public GtiListingEntry entry;

		public IOException e;

		public AppCenterPropertyName messagePropertyName;

		public AppCenterPropertyName messageEndingPropertyName;
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CCopyToRefugeAndFallbackOnCatalog_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<PotentialPackageDirectory> _003C_003Et__builder;

		public GtiListingEntry entry;

		public ICatalogPackageRefuge refuge;

		public Version catalogVersion;

		private _003C_003Ec__DisplayClass0_0 _003C_003E8__1;

		private ConfiguredTaskAwaiter<IRescuedCatalogPackage> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_012c: Expected O, but got Unknown
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			PotentialPackageDirectory result;
			try
			{
				if (num != 0)
				{
					_003C_003E8__1 = new _003C_003Ec__DisplayClass0_0();
					_003C_003E8__1.entry = entry;
					if (_003C_003E8__1.entry.CatalogedVersion == null)
					{
						throw new ArgumentException("Must have cataloged version", "entry");
					}
					if (_003C_003E8__1.entry.CatalogedPath == null)
					{
						throw new ArgumentException("Must have cataloged path", "entry");
					}
					if (refuge == null)
					{
						throw new ArgumentNullException("refuge");
					}
				}
				try
				{
					ConfiguredTaskAwaiter<IRescuedCatalogPackage> awaiter;
					if (num != 0)
					{
						awaiter = refuge.RescuePackage(catalogVersion, _003C_003E8__1.entry.EntryName, _003C_003E8__1.entry.CatalogedVersion, _003C_003E8__1.entry.CatalogedPath).DetachContext().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<IRescuedCatalogPackage>, _003CCopyToRefugeAndFallbackOnCatalog_003Ed__0>(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter<IRescuedCatalogPackage>);
						num = (_003C_003E1__state = -1);
					}
					result = awaiter.GetResult().RescuedDirectory;
				}
				catch (IOException val)
				{
					IOException e = val;
					_003C_003E8__1.e = e;
					_003C_003E8__1.messagePropertyName = AppCenterPropertyName.ParseAndThrowOnErrors("ERROR_MESSAGE");
					_003C_003E8__1.messageEndingPropertyName = AppCenterPropertyName.ParseAndThrowOnErrors("ERROR_MESSAGE_ENDING");
					TrackedEvent.Log(InlineEventDataDescriptor.Create(TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("SAFE_REFUGE_FALLBACK"), LauncherAppTarget.Instance), EventProperties, EventUserDataCensor.CensorProperties(_003C_003E8__1.messagePropertyName, _003C_003E8__1.messageEndingPropertyName)));
					Logger.LogFormattedException((System.Exception)(object)_003C_003E8__1.e, "Copying catalog packages to safety.");
					Logger.Log(LogLevel.Warning, "Using unsafe catalog package path: " + PathHelpers.QuotePathIfNeeded(_003C_003E8__1.entry.CatalogedPath));
					result = PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.Cataloged, _003C_003E8__1.entry.CatalogedPath);
				}
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E8__1 = null;
			_003C_003Et__builder.SetResult(result);
			[IteratorStateMachine(typeof(_003C_003Ec__DisplayClass0_0._003C_003CCopyToRefugeAndFallbackOnCatalog_003Eg__EventProperties_007C0_003Ed))]
			System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>> EventProperties()
			{
				yield return KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseAndThrowOnErrors("GTI_NAME"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(((_003C_003Ec__DisplayClass0_0)(object)this).entry.EntryName));
				yield return KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(((_003C_003Ec__DisplayClass0_0)(object)this).messagePropertyName, AppCenterPropertyValue.ParseTruncateAndHandleErrors(((System.Exception)(object)((_003C_003Ec__DisplayClass0_0)(object)this).e).Message));
				if (((System.Exception)(object)((_003C_003Ec__DisplayClass0_0)(object)this).e).Message.Length > 125)
				{
					yield return KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(((_003C_003Ec__DisplayClass0_0)(object)this).messageEndingPropertyName, AppCenterPropertyValue.ParseTruncateFromBeginningAndHandleErrors(((System.Exception)(object)((_003C_003Ec__DisplayClass0_0)(object)this).e).Message));
				}
				yield return KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseAndThrowOnErrors("ERROR_HRESULT"), AppCenterPropertyValue.ParseTruncateAndHandleErrors("0X" + ((System.Exception)(object)((_003C_003Ec__DisplayClass0_0)(object)this).e).HResult.ToString("X8", (IFormatProvider)(object)CultureInfo.InvariantCulture)));
			}
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[AsyncStateMachine(typeof(_003CCopyToRefugeAndFallbackOnCatalog_003Ed__0))]
	public static async System.Threading.Tasks.Task<PotentialPackageDirectory> CopyToRefugeAndFallbackOnCatalog(this ICatalogPackageRefuge refuge, Version catalogVersion, GtiListingEntry entry)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		GtiListingEntry entry2 = entry;
		if (entry2.CatalogedVersion == null)
		{
			throw new ArgumentException("Must have cataloged version", "entry");
		}
		if (entry2.CatalogedPath == null)
		{
			throw new ArgumentException("Must have cataloged path", "entry");
		}
		if (refuge == null)
		{
			throw new ArgumentNullException("refuge");
		}
		IOException e;
		AppCenterPropertyName messagePropertyName;
		AppCenterPropertyName messageEndingPropertyName;
		try
		{
			return (await refuge.RescuePackage(catalogVersion, entry2.EntryName, entry2.CatalogedVersion, entry2.CatalogedPath).DetachContext()).RescuedDirectory;
		}
		catch (IOException val)
		{
			IOException val2 = val;
			e = val2;
			messagePropertyName = AppCenterPropertyName.ParseAndThrowOnErrors("ERROR_MESSAGE");
			messageEndingPropertyName = AppCenterPropertyName.ParseAndThrowOnErrors("ERROR_MESSAGE_ENDING");
			TrackedEvent.Log(InlineEventDataDescriptor.Create(TrackedEventPrototype.Create(AppCenterEventName.ParseAndThrowOnErrors("SAFE_REFUGE_FALLBACK"), LauncherAppTarget.Instance), EventProperties, EventUserDataCensor.CensorProperties(messagePropertyName, messageEndingPropertyName)));
			Logger.LogFormattedException((System.Exception)(object)e, "Copying catalog packages to safety.");
			Logger.Log(LogLevel.Warning, "Using unsafe catalog package path: " + PathHelpers.QuotePathIfNeeded(entry2.CatalogedPath));
			return PotentialPackageDirectory.FromExistingDirectory(TrainerPackageType.Cataloged, entry2.CatalogedPath);
		}
		[IteratorStateMachine(typeof(_003C_003Ec__DisplayClass0_0._003C_003CCopyToRefugeAndFallbackOnCatalog_003Eg__EventProperties_007C0_003Ed))]
		System.Collections.Generic.IEnumerable<KeyValuePair<AppCenterPropertyName, AppCenterPropertyValue>> EventProperties()
		{
			yield return KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseAndThrowOnErrors("GTI_NAME"), AppCenterPropertyValue.ParseTruncateAndHandleErrors(entry2.EntryName));
			yield return KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(messagePropertyName, AppCenterPropertyValue.ParseTruncateAndHandleErrors(((System.Exception)(object)e).Message));
			if (((System.Exception)(object)e).Message.Length > 125)
			{
				yield return KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(messageEndingPropertyName, AppCenterPropertyValue.ParseTruncateFromBeginningAndHandleErrors(((System.Exception)(object)e).Message));
			}
			yield return KeyValuePair.Create<AppCenterPropertyName, AppCenterPropertyValue>(AppCenterPropertyName.ParseAndThrowOnErrors("ERROR_HRESULT"), AppCenterPropertyValue.ParseTruncateAndHandleErrors("0X" + ((System.Exception)(object)e).HResult.ToString("X8", (IFormatProvider)(object)CultureInfo.InvariantCulture)));
		}
	}
}
