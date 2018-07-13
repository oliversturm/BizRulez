// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Collections;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace pct.BizRulez.Rules.Parameters {
  public class RuleParameterSet : List<RuleParameter> {
    /// <summary>
    /// Create a new empty parameter set.
    /// </summary>
    public RuleParameterSet() {}

    /// <summary>
    /// Create a new parameter set filling it with the given parameters.
    /// </summary>
    /// <param name="ruleParameters"></param>
    public RuleParameterSet(IEnumerable<RuleParameter> ruleParameters) : base(ruleParameters) { }

    /// <summary>
    /// Access a parameter by its name.
    /// </summary>
    public RuleParameter this[string name]  {
      get  {
        return this.FirstOrDefault(rp => rp.Name == name);
      }
    }

    private static bool ruleParameterTypesRegistered;
    /// <summary>
    /// Find out whether default rule parameter types have already been registered
    /// This property can also be set before the RuleParameterTypes
    /// property is accessed for the first time, thereby preventing
    /// default registration altogether.
    /// </summary>
    public static bool RuleParameterTypesRegistered {
      get {
        return ruleParameterTypesRegistered;
      }
      set {
        ruleParameterTypesRegistered = value;
      }
    }

    private static Dictionary<string, Type> ruleParameterTypes;
    /// <summary>
    /// Get the known rule parameter types. If default rule parameter types have
    /// not yet been registered when this property is accessed (which is 
    /// checked using the RuleParameterTypesRegistered property), they
    /// will be registered just in time.
    /// </summary>
    public static Dictionary<string, Type> RuleParameterTypes {
      get {
        if (! RuleParameterTypesRegistered)
          RegisterDefaultRuleParameterTypes();
        
        return ruleParameterTypes;
      }
    }

    /// <summary>
    /// Register all default rule parameter types and add them to the RuleParameterTypes
    /// dictionary.
    /// </summary>
    public static void RegisterDefaultRuleParameterTypes() {
      if (ruleParameterTypes == null)
        ruleParameterTypes = new Dictionary<string, Type>( );

      ruleParameterTypes.Add(RuleIntParameter.StaticParameterTypeId, typeof(RuleIntParameter));
      ruleParameterTypes.Add(RuleStringParameter.StaticParameterTypeId, typeof(RuleStringParameter));
      ruleParameterTypes.Add(RuleBoolParameter.StaticParameterTypeId, typeof(RuleBoolParameter));
      ruleParameterTypes.Add(RuleComparisonOperatorParameter.StaticParameterTypeId, typeof(RuleComparisonOperatorParameter));
      ruleParameterTypes.Add(RuleDateTimeParameter.StaticParameterTypeId, typeof(RuleDateTimeParameter));
      ruleParameterTypes.Add(RuleDecimalParameter.StaticParameterTypeId, typeof(RuleDecimalParameter));
      ruleParameterTypes.Add(RuleDoubleParameter.StaticParameterTypeId, typeof(RuleDoubleParameter));
      ruleParameterTypes.Add(RuleInvariantDateTimeParameter.StaticParameterTypeId, typeof(RuleInvariantDateTimeParameter));

      ruleParameterTypesRegistered = true;
    }

    public static RuleParameter CreateParameter(string typeId, string name, string valueStorageString) {
      Type paramType = RuleParameterTypes[typeId];
      if (paramType == null)
        throw new InvalidParameterTypeException(String.Format(CultureInfo.CurrentCulture, "The parameter type '{0}' is unknown.", typeId));
      RuleParameter param = (RuleParameter) paramType.InvokeMember(String.Empty,
        BindingFlags.CreateInstance, null, null, new object[] { name, valueStorageString }, CultureInfo.InvariantCulture);
      return param;
    }
  }
}
