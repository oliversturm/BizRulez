// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Runtime.Serialization;

namespace pct.BizRulez.Rules {
  /// <summary>
  /// This exception is thrown when a rule is detected to be invalid
  /// during construction.
  /// </summary>
  [Serializable]
  public class InvalidRuleException : Exception {
    public InvalidRuleException() {}
    public InvalidRuleException(String message): base(message) {}
    protected InvalidRuleException(SerializationInfo si, StreamingContext sc) : base(si,sc) {}
    public InvalidRuleException(String s, Exception e) : base(s,e) {}
  }
}
