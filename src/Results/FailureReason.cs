// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using pct.BizRulez.RuleChecking;
using pct.BizRulez.Rules;

namespace pct.BizRulez.Results {
  public enum FailureReason {
    None,
    CheckFailed,
    CheckNotPerformed
  }
}
