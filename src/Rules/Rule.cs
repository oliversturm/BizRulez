// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Globalization;
using pct.BizRulez;
using pct.BizRulez.Results;
using pct.BizRulez.RuleChecking;
using pct.BizRulez.Rules.Parameters;
using System.Collections.Generic;

namespace pct.BizRulez.Rules {
  public class Rule {
    /// <summary>
    /// Find out if the given rule is valid, i.e. it contains all 
    /// the information needed to actually use the rule.
    /// </summary>
    /// <returns>true if the rule is valid</returns>
    public bool Validate() {
      return (RuleChecker != null &&
        ObjectTypeId.Length > 0 &&
        ViewName.Length > 0 &&
        RuleChecker.IsCompatibleParameterSet(Parameters));
    }

    /// <summary>
    /// The RuleChecker object that is associated with this rule.
    /// </summary>
    public RuleChecker RuleChecker { get; set; }

    /// <summary>
    /// The object type that this rule is supposed to work on.
    /// </summary>
    public string ObjectTypeId { get; set; }

    /// <summary>
    /// A (short) view name for the rule that may be shown in error dialogs, e.g.
    /// </summary>
    public string ViewName { get; set; }

    /// <summary>
    /// A list of contexts for which this rule is going to be evaluated.
    /// </summary>
    public List<string> Contexts { get; set; }

    /// <summary>
    /// A key for this rule. This can be used to locate a specific result in a ResultSet easily.
    /// To be able to find the right result using this method, you should make sure that
    /// the key is unique in the context where it's needed.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The technical comment about the rule.
    /// </summary>
    public string TechnicalComment { get; set; }

    /// <summary>
    /// The info text that is generated for the result if the rule
    /// fails. Depending on the rule checker, this may include placeholders
    /// in the form of {0}, {1}, ... where detail on the failed check
    /// will be inserted into the string by the rule checker.
    /// </summary>
    public string ErrorInfoTemplate { get; set; }

    /// <summary>
    /// The set of parameters relevant to this rule.
    /// </summary>
    public RuleParameterSet Parameters { get; set; }

    /// <summary>
    /// The field names that are affected by this rule. These may be used
    /// to show error information in a UI, for instance.
    /// </summary>
    public List<string> AffectedFieldNames { get; set; }

    public IEnumerable<Result> Check(object checkable) {
      IEnumerable<Result> results = RuleChecker.Check(checkable, this);
      
      foreach(var r in results)
        r.SetValid();
      return results;
    }

    public override string ToString() {
      return ViewName;
    }

  }
}