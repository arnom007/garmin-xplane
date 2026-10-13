using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LOLA.StringHelpers;
using LauncherCoreLib.Models.Simulated.Flight;
using TrainerPackageLib;

namespace LauncherCoreLib.Models.Winfs;

internal class WinfsNonAsciiErrorSessionWorkflow : PreflightOnlyWorkflow
{
	public override System.Threading.Tasks.Task<ISessionEnvironment> PreFlight(ISessionEnvironment sessionEnvironment, CancellationToken token)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		List<char> val = Enumerable.ToList<char>(Enumerable.Distinct<char>(Enumerable.SelectMany<string, char>(Enumerable.Select<TrainerSessionData, string>(((IReadOnlyDictionary<TrainerIdentifier, TrainerSessionData>)(object)sessionEnvironment.Data.TrainerData).Values, (Func<TrainerSessionData, string>)((TrainerSessionData tsd) => tsd.WinfsMountPath)), (Func<string, System.Collections.Generic.IEnumerable<char>>)TextHelpers.NonAsciiCharactersInString)));
		if (val.Count > 0)
		{
			throw new InvalidOperationException(string.Format((IFormatProvider)(object)CultureInfo.InvariantCulture, "The following unsupported filename character(s) were detected: [ {0} ] Please reinstall this application and ensure the \"Databases & large resources\" folder option only contains English characters.", (object)string.Join<char>(", ", (System.Collections.Generic.IEnumerable<char>)val)));
		}
		return System.Threading.Tasks.Task.FromResult<ISessionEnvironment>(sessionEnvironment);
	}
}
