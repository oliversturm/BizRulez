// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Globalization;
using pct.BizRulez.Results;
using pct.BizRulez.Rules;
using pct.BizRulez.RuleChecking;
using System.Collections.Generic;
using System.Linq;

namespace pct.BizRulez {
  /// <summary>
  /// Options that can be passed in to the BizRulezChecker.Check methods.
  /// </summary>
  [Flags]
  public enum CheckOptions {
    Default = 0x00,
    /// <summary>
    /// Abort the check when the first result with status != Pass in encountered.
    /// This can't be combined with IncludePassResults.
    /// </summary>
    StopAfterFirstNonPass = 0x01,
    /// <summary>
    /// Include the positive (Pass) results in the result set in addition
    /// to the non-Pass results. This can't be combined with StopAfterFirstNonPass
    /// because the target is to create a ResultSet that containes exactly
    /// one result per checked rule.
    /// </summary>
    IncludePassResults = 0x02,

    /// <summary>
    /// Don't pay attention to any contexts passed in to any Check functions, 
    /// or to contexts associated with rules. Basically, all rules in a given
    /// list will be checked when this flag is set.
    /// </summary>
    IgnoreContexts = 0x04,
  }
}
