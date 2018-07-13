// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Globalization;
using pct.BizRulez.Results;
using pct.BizRulez.Rules;
using pct.BizRulez.RuleChecking;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace pct.BizRulez {
  public class BizRulezChecker {
    public BizRulezChecker() {
      Rules = new List<Rule>();
    }

    string GetObjectTypeId(object source) {
      var interfaceCheckable = source as ICheckable;
      if (interfaceCheckable != null)
        return interfaceCheckable.ObjectTypeId;
      return source.GetType( ).FullName;
    }


    public IEnumerable<Result> Check(IEnumerable<Rule> rules, CheckOptions checkOptions, IEnumerable<string> contexts, IEnumerable checkables) {
      if (rules == null)
        throw new ArgumentNullException("rules");
      foreach (object checkable in checkables) {
        var objectTypeId = GetObjectTypeId(checkable);
        // break things down to the rules for this object type
        IEnumerable<Rule> rulesToCheck =
          from r in rules where r.ObjectTypeId == objectTypeId select r;
        if ((checkOptions & CheckOptions.IgnoreContexts) == 0) {
          // contexts are relevant, so filter down the list of rules
          if (contexts == null || contexts.Count( ) == 0)
            // filter down the list to those rules that don't have contexts assigned
            rulesToCheck =
              from r in rulesToCheck
              where 
                (r.Contexts == null ||
                r.Contexts.Count( ) == 0)
              select r;
          else
            // filter down the list to those rules that have any of the contexts 
            // assigned that we're supposed to check
            rulesToCheck =
              from r in rulesToCheck
              where 
                r.Contexts != null &&
                r.Contexts.Intersect(contexts).Count( ) > 0
              select r;
        }

        foreach (var rule in rulesToCheck) {
          var results = rule.Check(checkable);

          foreach (var result in results) {
            // I'm not really happy with this check - shouldn't I *always*
            // get a result? I'm going to take it out and see what happens.
            //if (result != null) {
            if ((result.Status != ResultStatus.Pass) ||
              ((checkOptions & CheckOptions.IncludePassResults) != 0))
              yield return result;

            // Hm, is this a bug...
            // it doesn't seem to make sense to have the check for IncludePassResults
            // in here - the first two lines say "if this is not a pass result and 
            // we've been asked to stop after the first non-pass result" - that seems
            // to be sufficient reason to stop yielding now, regardless of whether 
            // we're including pass results or not.
            //if ((result.Status != ResultStatus.Pass) &&
            //  ((checkOptions & CheckOptions.StopAfterFirstNonPass) != 0) &&
            //  ((checkOptions & CheckOptions.IncludePassResults) == 0))

            if ((result.Status != ResultStatus.Pass) &&
                ((checkOptions & CheckOptions.StopAfterFirstNonPass) != 0) /*&&
              ((checkOptions & CheckOptions.IncludePassResults) == 0)*/
                                                                         )
              yield break;
            //}
          }
        }
      }
    }

    #region Check overloads
    // variations with all parameters
    public IEnumerable<Result> Check(IEnumerable<Rule> rules, CheckOptions checkOptions, string context, IEnumerable checkables) {
      return Check(rules, checkOptions, new[] { context }, (IEnumerable) checkables);
    }

    public IEnumerable<Result> Check(IEnumerable<Rule> rules, CheckOptions checkOptions, IEnumerable<string> contexts, params object[] checkables) {
      return Check(rules, checkOptions, contexts, (IEnumerable) checkables);
    }

    public IEnumerable<Result> Check(IEnumerable<Rule> rules, CheckOptions checkOptions, string context, params object[] checkables) {
      return Check(rules, checkOptions, new[] { context }, (IEnumerable) checkables);
    }

    // variations without explicit rules
    public IEnumerable<Result> Check(CheckOptions checkOptions, IEnumerable<string> contexts, IEnumerable checkables) {
      return Check(Rules, checkOptions, contexts, (IEnumerable) checkables);
    }

    public IEnumerable<Result> Check(CheckOptions checkOptions, string context, IEnumerable checkables) {
      return Check(Rules, checkOptions, new[] { context }, (IEnumerable) checkables);
    }

    public IEnumerable<Result> Check(CheckOptions checkOptions, IEnumerable<string> contexts, params object[] checkables) {
      return Check(Rules, checkOptions, contexts, (IEnumerable) checkables);
    }

    public IEnumerable<Result> Check(CheckOptions checkOptions, string context, params object[] checkables) {
      return Check(Rules, checkOptions, new[] { context }, (IEnumerable) checkables);
    }

    // variations without explicit rules and without checkoptions
    public IEnumerable<Result> Check(IEnumerable<string> contexts, IEnumerable checkables) {
      return Check(Rules, CheckOptions.Default, contexts, (IEnumerable) checkables);
    }

    public IEnumerable<Result> Check(string context, IEnumerable checkables) {
      return Check(Rules, CheckOptions.Default, new[] { context }, (IEnumerable) checkables);
    }

    public IEnumerable<Result> Check(IEnumerable<string> contexts, params object[] checkables) {
      return Check(Rules, CheckOptions.Default, contexts, (IEnumerable) checkables);
    }

    public IEnumerable<Result> Check(string context, params object[] checkables) {
      return Check(Rules, CheckOptions.Default, new[] { context }, (IEnumerable) checkables);
    }

    // variations with only checkables
    public IEnumerable<Result> Check(IEnumerable checkables) {
      return Check(Rules, CheckOptions.Default, (IEnumerable<string>) null, (IEnumerable) checkables);
    }

    public IEnumerable<Result> Check(params object[] checkables) {
      return Check(Rules, CheckOptions.Default, (IEnumerable<string>) null, (IEnumerable) checkables);
    }

    // variations with only checkoptions and checkables
    public IEnumerable<Result> Check(CheckOptions checkOptions, IEnumerable checkables) {
      return Check(Rules, checkOptions, (IEnumerable<string>) null, (IEnumerable) checkables);
    }

    public IEnumerable<Result> Check(CheckOptions checkOptions, params object[] checkables) {
      return Check(Rules, checkOptions, (IEnumerable<string>) null, (IEnumerable) checkables);
    }

    #endregion
    
    /// <summary>
    /// The main rule set that is used by the Check method when no
    /// other rule set is passed in.
    /// </summary>
    public List<Rule> Rules { get; set; }

    #region Rule checker registry
    /// <summary>
    /// Find out whether default rule checkers have already been
    /// initialized. This property can also be set before the RuleCheckers
    /// property is accessed for the first time, thereby preventing
    /// default registration altogether.
    /// </summary>
    public static bool RuleCheckersInitialized { get; set; }

    private static List<RuleChecker> ruleCheckers = new List<RuleChecker>();
    /// <summary>
    /// Get the known rule checkers. If default rule checkers have
    /// not yet been initialized when this property is accessed (which is 
    /// checked using the RuleCheckersInitialized property), they
    /// will be registered just in time.
    /// </summary>
    public static List<RuleChecker> RuleCheckers {
      get {
        if (! RuleCheckersInitialized)
          InitializeDefaultRuleCheckers();
        
        return ruleCheckers;
      }
    }

    /// <summary>
    /// Initialize all default rule checkers and add them to the RuleCheckers
    /// dictionary.
    /// </summary>
    public static void InitializeDefaultRuleCheckers() {
      ruleCheckers.Add(new FormatRuleChecker());
      ruleCheckers.Add(new ComparisonRuleChecker());
      ruleCheckers.Add(new NotNullRuleChecker());
      ruleCheckers.Add(new NonEmptyStringRuleChecker( ));
      ruleCheckers.Add(new CollectionContainsRuleChecker( ));
      ruleCheckers.Add(new UniquenessRuleChecker( ));

      RuleCheckersInitialized = true;
    }
    #endregion 
  }
}
