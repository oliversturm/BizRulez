// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Runtime.Serialization;

namespace pct.BizRulez.Rules.Parameters {
  /// <summary>
  /// This exception is thrown when a parameter of unknown type is detected
  /// in a saved rule file.
  /// </summary>
  [Serializable]
  public class InvalidParameterTypeException : Exception {
    public InvalidParameterTypeException() {}
    public InvalidParameterTypeException(String message): base(message) {}
    protected InvalidParameterTypeException(SerializationInfo si, StreamingContext sc) : base(si,sc) {}
    public InvalidParameterTypeException(String s, Exception e) : base(s,e) {}
  }
}
