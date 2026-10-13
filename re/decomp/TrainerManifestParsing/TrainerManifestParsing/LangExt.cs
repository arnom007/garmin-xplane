using System;
using System.Collections.Generic;
using LanguageExt;

namespace TrainerManifestParsing;

public static class LangExt
{
	public static System.Collections.Generic.IEnumerable<Either<TLeft, TRight>> LeftToRightEnumerable<TLeft, TRight>(this Either<TLeft, System.Collections.Generic.IEnumerable<Either<TLeft, TRight>>> either)
	{
		return either.IfLeft((Func<TLeft, System.Collections.Generic.IEnumerable<Either<TLeft, TRight>>>)((TLeft e) => Prelude.Seq1(Prelude.Left(e).Bind<TRight>())));
	}
}
