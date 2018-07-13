// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Collections.Generic;

using pct.BizRulez.RuleChecking;
using pct.BizRulez.Rules;

namespace pct.BizRulez.Results {
  public class Result {
    public Result(RuleChecker ruleChecker, Rule rule, ResultStatus status, FailureReason failureReason, string errorInfo) {
      this.RuleChecker = ruleChecker;
      this.Rule = rule;
      this.Status = status;
      this.FailureReason = failureReason;
      this.ErrorInfo = errorInfo;

      this.Details = new List<ResultDetail>();
    }

    /// <summary>
    /// The rule checker that created this result.
    /// </summary>
    public RuleChecker RuleChecker { get; private set; }

    /// <summary>
    /// The rule that this result pertains to.
    /// </summary>
    public Rule Rule { get; private set; }

    /// <summary>
    /// The status of the check.
    /// </summary>
    public ResultStatus Status { get; private set; }

    /// <summary>
    /// If the Status is not Pass, this property tells if the corresponding 
    /// check has been performed and failed or whether the check hasn't
    /// even been performed (for technical reasons, like a field value
    /// couldn't be queried or didn't return a valid data type or similar).
    /// </summary>
    public FailureReason FailureReason { get; private set; }

    /// <summary>
    /// For Status == ResultStatus.Fail, this property contains extended information
    /// about the problem in a form that's suitable to be shown to a user.
    /// </summary>
    public string ErrorInfo { get; private set; }

    /// <summary>
    /// The Details collection may be used by rule checkers to record arbitrary
    /// additional information about the result.
    /// </summary>
    public List<ResultDetail> Details { get; private set; }

    /// <summary>
    /// Returns whether this result is valid. During construction, the result may
    /// not yet be valid up to some specific point.
    /// </summary>
    public bool Valid { get; private set; }

    internal void SetValid() {
      if (Valid)
        throw new InvalidOperationException("SetValid may only be called if the state is not already valid.");
      Valid = true;
    }
  }
}
