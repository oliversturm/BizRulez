// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Runtime.Serialization;

namespace pct.BizRulez.RuleChecking {
  /// <summary>
  /// This exception is thrown when a rule checker of unknown type is detected
  /// in a rule file.
  /// </summary>
  [Serializable]
  public class InvalidRuleCheckerException : Exception {
    public InvalidRuleCheckerException() {}
    public InvalidRuleCheckerException(String message): base(message) {}
    protected InvalidRuleCheckerException(SerializationInfo si, StreamingContext sc) : base(si,sc) {}
    public InvalidRuleCheckerException(String s, Exception e) : base(s,e) {}
  }
}
