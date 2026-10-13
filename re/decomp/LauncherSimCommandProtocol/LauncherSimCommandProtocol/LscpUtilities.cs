using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using LOLA.Reactive;
using LOLA.Tasks;
using LauncherCoreLib.Models.Simulated.Panel;

namespace LauncherSimCommandProtocol;

public static class LscpUtilities
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_0
	{
		public SimCommandType commandType;

		internal UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse> _003CDoesSimSupportCommand_003Eb__0(LscpPacketReceipt receipt)
		{
			if (LscpCommandResponse.TryParseForCommand(receipt.CommandPacket.OpenPayload(), SimCommandType.COMMAND_SUPPORTED_QUERY, out LscpCommandResponse response))
			{
				if (response.Error == LscpCommandError.sim_LSCP_ERR_CMD_UNSUPPORTED)
				{
					return ParsedLscpCommandResponse.CreateUnsafe<LscpCommandSupportQueryResponse>(response, null);
				}
				if (LscpCommandSupportQueryResponse.TryParse(response.OpenPayload(), out LscpCommandSupportQueryResponse commandQuerryResponse))
				{
					if (commandQuerryResponse.QueriedCommand != commandType)
					{
						return null;
					}
					return ParsedLscpCommandResponse.CreateUnsafe(response, commandQuerryResponse);
				}
			}
			return null;
		}
	}

	[StructLayout(3)]
	[CompilerGenerated]
	private struct _003CDoesSimSupportCommand_003Ed__0 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<LscpCommandSupportResponseType> _003C_003Et__builder;

		public SimCommandType commandType;

		public ILauncherSimMasterCommander masterCommander;

		public SimInstallKey installKey;

		public CancellationToken token;

		private System.Threading.Tasks.Task<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>> _003CreceiveTask_003E5__2;

		private ConfiguredTaskAwaiter _003C_003Eu__1;

		private ConfiguredTaskAwaiter<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			LscpCommandSupportResponseType result;
			try
			{
				if ((uint)num <= 1u)
				{
					goto IL_00e4;
				}
				_003C_003Ec__DisplayClass0_0 CS_0024_003C_003E8__locals0 = new _003C_003Ec__DisplayClass0_0
				{
					commandType = commandType
				};
				if (masterCommander == null)
				{
					throw new ArgumentNullException("masterCommander");
				}
				if ((object)installKey == null)
				{
					throw new ArgumentNullException("installKey");
				}
				if (!System.Enum.IsDefined(typeof(SimCommandType), (object)CS_0024_003C_003E8__locals0.commandType))
				{
					throw new ArgumentException("Invalid enum value.", "commandType");
				}
				LauncherSimCommandPacket packet = default(LauncherSimCommandPacket);
				if (masterCommander.DoesSimSupportCommands(installKey))
				{
					_003CreceiveTask_003E5__2 = masterCommander.Messages(installKey).Select<LscpPacketReceipt, UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>((Func<LscpPacketReceipt, UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>)delegate(LscpPacketReceipt receipt)
					{
						if (LscpCommandResponse.TryParseForCommand(receipt.CommandPacket.OpenPayload(), SimCommandType.COMMAND_SUPPORTED_QUERY, out LscpCommandResponse response))
						{
							if (response.Error == LscpCommandError.sim_LSCP_ERR_CMD_UNSUPPORTED)
							{
								return ParsedLscpCommandResponse.CreateUnsafe<LscpCommandSupportQueryResponse>(response, null);
							}
							if (LscpCommandSupportQueryResponse.TryParse(response.OpenPayload(), out LscpCommandSupportQueryResponse commandQuerryResponse))
							{
								if (commandQuerryResponse.QueriedCommand != CS_0024_003C_003E8__locals0.commandType)
								{
									return (UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>)null;
								}
								return ParsedLscpCommandResponse.CreateUnsafe(response, commandQuerryResponse);
							}
						}
						return (UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>)null;
					}).WhereClassNotNull<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>()
						.FirstAsync<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>()
						.ToTask<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>(token);
					LscpCommandSupportQuery lscpCommandSupportQuery = new LscpCommandSupportQuery(CS_0024_003C_003E8__locals0.commandType);
					packet = LauncherSimCommandPacket.CreatePacket(SimCommandType.COMMAND_SUPPORTED_QUERY, lscpCommandSupportQuery.ToBinary());
					goto IL_00e4;
				}
				result = LscpCommandSupportResponseType.sim_LSCP_COMMAND_NOT_SUPPORTED;
				goto end_IL_0007;
				IL_0204:
				result = LscpCommandSupportResponseType.sim_LSCP_SUPPORT_UNKNOWN;
				goto end_IL_0007;
				IL_00e4:
				try
				{
					ConfiguredTaskAwaiter<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>> awaiter;
					ConfiguredTaskAwaiter awaiter2;
					if (num != 0)
					{
						if (num == 1)
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ConfiguredTaskAwaiter<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>);
							num = (_003C_003E1__state = -1);
							goto IL_01c8;
						}
						ConfiguredTaskAwaitable val = masterCommander.SendToClient(installKey, packet, token).DetachContext();
						awaiter2 = ((ConfiguredTaskAwaitable)(ref val)).GetAwaiter();
						if (!((ConfiguredTaskAwaiter)(ref awaiter2)).IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter, _003CDoesSimSupportCommand_003Ed__0>(ref awaiter2, ref this);
							return;
						}
					}
					else
					{
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ConfiguredTaskAwaiter);
						num = (_003C_003E1__state = -1);
					}
					((ConfiguredTaskAwaiter)(ref awaiter2)).GetResult();
					awaiter = _003CreceiveTask_003E5__2.DetachContext().GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__2 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ConfiguredTaskAwaiter<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>, _003CDoesSimSupportCommand_003Ed__0>(ref awaiter, ref this);
						return;
					}
					goto IL_01c8;
					IL_01c8:
					UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse> result2 = awaiter.GetResult();
					result = (result2.GenericResponse.IsSuccess ? (result2.Payload?.SupportedResponse ?? LscpCommandSupportResponseType.sim_LSCP_SUPPORT_UNKNOWN) : LscpCommandSupportResponseType.sim_LSCP_SUPPORT_UNKNOWN);
				}
				catch (InvalidOperationException)
				{
					goto IL_0204;
				}
				catch (OperationCanceledException)
				{
					goto IL_0204;
				}
				catch (SimOfflineException)
				{
					goto IL_0204;
				}
				catch (IOException)
				{
					goto IL_0204;
				}
				end_IL_0007:;
			}
			catch (System.Exception exception)
			{
				_003C_003E1__state = -2;
				_003CreceiveTask_003E5__2 = null;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003CreceiveTask_003E5__2 = null;
			_003C_003Et__builder.SetResult(result);
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}
	}

	[AsyncStateMachine(typeof(_003CDoesSimSupportCommand_003Ed__0))]
	public static async System.Threading.Tasks.Task<LscpCommandSupportResponseType> DoesSimSupportCommand(ILauncherSimMasterCommander masterCommander, SimInstallKey installKey, SimCommandType commandType, CancellationToken token)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (masterCommander == null)
		{
			throw new ArgumentNullException("masterCommander");
		}
		if ((object)installKey == null)
		{
			throw new ArgumentNullException("installKey");
		}
		if (!System.Enum.IsDefined(typeof(SimCommandType), (object)commandType))
		{
			throw new ArgumentException("Invalid enum value.", "commandType");
		}
		if (!masterCommander.DoesSimSupportCommands(installKey))
		{
			return LscpCommandSupportResponseType.sim_LSCP_COMMAND_NOT_SUPPORTED;
		}
		System.Threading.Tasks.Task<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>> receiveTask = masterCommander.Messages(installKey).Select<LscpPacketReceipt, UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>((Func<LscpPacketReceipt, UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>)delegate(LscpPacketReceipt receipt)
		{
			if (LscpCommandResponse.TryParseForCommand(receipt.CommandPacket.OpenPayload(), SimCommandType.COMMAND_SUPPORTED_QUERY, out LscpCommandResponse response))
			{
				if (response.Error == LscpCommandError.sim_LSCP_ERR_CMD_UNSUPPORTED)
				{
					return ParsedLscpCommandResponse.CreateUnsafe<LscpCommandSupportQueryResponse>(response, null);
				}
				if (LscpCommandSupportQueryResponse.TryParse(response.OpenPayload(), out LscpCommandSupportQueryResponse commandQuerryResponse))
				{
					if (commandQuerryResponse.QueriedCommand != commandType)
					{
						return (UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>)null;
					}
					return ParsedLscpCommandResponse.CreateUnsafe(response, commandQuerryResponse);
				}
			}
			return (UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>)null;
		}).WhereClassNotNull<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>()
			.FirstAsync<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>()
			.ToTask<UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse>>(token);
		LscpCommandSupportQuery lscpCommandSupportQuery = new LscpCommandSupportQuery(commandType);
		LauncherSimCommandPacket packet = LauncherSimCommandPacket.CreatePacket(SimCommandType.COMMAND_SUPPORTED_QUERY, lscpCommandSupportQuery.ToBinary());
		try
		{
			await masterCommander.SendToClient(installKey, packet, token).DetachContext();
			UnsafeParsedLscpCommandResponse<LscpCommandSupportQueryResponse> unsafeParsedLscpCommandResponse = await receiveTask.DetachContext();
			return unsafeParsedLscpCommandResponse.GenericResponse.IsSuccess ? (unsafeParsedLscpCommandResponse.Payload?.SupportedResponse ?? LscpCommandSupportResponseType.sim_LSCP_SUPPORT_UNKNOWN) : LscpCommandSupportResponseType.sim_LSCP_SUPPORT_UNKNOWN;
		}
		catch (InvalidOperationException)
		{
		}
		catch (OperationCanceledException)
		{
		}
		catch (SimOfflineException)
		{
		}
		catch (IOException)
		{
		}
		return LscpCommandSupportResponseType.sim_LSCP_SUPPORT_UNKNOWN;
	}
}
