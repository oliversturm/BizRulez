// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace pct.BizRulez.Results {
  public static class ResultsExtensions {
    public static void ThrowException(this IEnumerable<Result> results) {
      throw new ResultsException(results);
    }

    public static void ThrowException(this IEnumerable<Result> results, Exception innerException) {
      throw new ResultsException(results, innerException);
    }

    /// <summary>
    /// This method returns the first result, if any, that references a rule
    /// that has a Key property matching the giving key. Be aware that (a) this method
    /// is only meant for use in interactive scenarios and 
    /// (b) just the first result found will ever be returned, so you probably have to
    /// make sure that the key is unique in the needed context.
    /// </summary>
    public static Result ResultByRuleKey(this IEnumerable<Result> results, string key) {
      return results.FirstOrDefault(r => r.Rule.Key == key);
    }
  }
}
