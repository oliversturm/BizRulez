// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using pct.BizRulez.Rules;
using pct.BizRulez.Rules.Parameters;
using pct.BizRulez.Results;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace pct.BizRulez.RuleChecking {
  public static class RuleCheckersExtensions {
    public static RuleChecker FromCheckerId(this List<RuleChecker> ruleCheckers, string checkerId) {
      return ruleCheckers.FirstOrDefault(r => r.CheckerId == checkerId);
    }
  }
}
