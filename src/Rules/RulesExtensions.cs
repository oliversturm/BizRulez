// Copyright (C) 2009-2012 Oliver Sturm <oliver@oliversturm.com> All rights reserved.

using System;
using System.Globalization;
using pct.BizRulez;
using pct.BizRulez.Results;
using pct.BizRulez.RuleChecking;
using pct.BizRulez.Rules.Parameters;
using System.Collections.Generic;

using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Reflection;

namespace pct.BizRulez.Rules {
  public static class RulesExtensions {
    public static IEnumerable<Rule> LoadDefaultRules(Assembly assembly) {
      // I don't want to make any assumptions about the names of namespaces
      // in the assembly - unfortunately that's a bit harder than it should be.
      
      var rulesResources = from name in assembly.GetManifestResourceNames( )
                           where name.EndsWith("defaultrules.xml")
                           select name;
      int ruleFileCount = rulesResources.Count();
      if (ruleFileCount == 0)
        throw new InvalidOperationException("No defaultrules.xml found in resource stream.");
      if (ruleFileCount > 1)
        throw new InvalidOperationException("Found more than one defaultrules.xml in resource stream.");
      var ruleFile = rulesResources.First( );
      var stream = assembly.GetManifestResourceStream(ruleFile);
      return Load(stream);
    }

    public static IEnumerable<Rule> LoadFile(string filename) {
      return Load(File.OpenRead(filename));
    }

    public static IEnumerable<Rule> Load(Stream stream) {
      return Load(new StreamReader(stream));
    }

    public static IEnumerable<Rule> Load(TextReader reader) {
      var doc = XDocument.Load(reader);

      foreach (var ruleNode in doc.Element("rules").Elements("rule")) {
        var rule = new Rule {
          RuleChecker = BizRulezChecker.RuleCheckers.FromCheckerId(ruleNode.Element("checkerId").Value),
          ObjectTypeId = ruleNode.Element("objectTypeId").Value,
          ViewName = ruleNode.Element("viewName").Value,
          Key = ruleNode.Element("key").Value,
          TechnicalComment = ruleNode.Element("technicalComment").Value,
          ErrorInfoTemplate = ruleNode.Element("errorInfoTemplate").Value,
          Parameters = new RuleParameterSet(
            from paramNode in ruleNode.Element("parameters").Elements("parameter")
            select RuleParameterSet.CreateParameter(
              paramNode.Attribute("typeId").Value,
              paramNode.Attribute("name").Value,
              paramNode.Value))
        };
        if (rule.RuleChecker == null)
          throw new InvalidRuleCheckerException(String.Format("Unknown rule checker id {0} found", ruleNode.Element("checkerId").Value));

        var contextAttribute = ruleNode.Attribute("context");
        if (contextAttribute != null) {
          rule.Contexts = new List<string> { contextAttribute.Value };
        }
        else {
          var contexts = ruleNode.Element("contexts");
          if (contexts != null) {
            rule.Contexts = new List<string>(
              from contextNode in contexts.Elements("context")
              select contextNode.Value);
          }
        }

        var affectedFieldNames = ruleNode.Element("affectedFieldNames");
        if (affectedFieldNames != null) {
          rule.AffectedFieldNames = new List<string>(
            from fieldNameNode in affectedFieldNames.Elements("fieldName")
            select fieldNameNode.Value);
        }

        yield return rule;
      }
    }

    public static List<Rule> FillFromFile(this List<Rule> list, string filename) {
      foreach (var rule in LoadFile(filename))
        list.Add(rule);
      return list;
    }

    public static void SaveFile(this IEnumerable<Rule> rules, string filename) {
      var rulesElement = new XElement("rules");

      foreach (var rule in rules) {
        var ruleElement = new XElement("rule",
          new XElement("checkerId", rule.RuleChecker.CheckerId),
          new XElement("objectTypeId", rule.ObjectTypeId),
          new XElement("viewName", new XCData(rule.ViewName)),
          new XElement("key", rule.Key),
          new XElement("technicalComment", new XCData(rule.TechnicalComment)),
          new XElement("errorInfoTemplate", new XCData(rule.ErrorInfoTemplate)));
        if (rule.Contexts != null) {
          int contextCount = rule.Contexts.Count;
          if (contextCount == 1) {
            ruleElement.Add(new XElement("context", rule.Contexts[0]));
          }
          else {
            ruleElement.Add(new XElement("contexts",
              from c in rule.Contexts
              select new XElement("context", c)));
          }
        }
        if (rule.AffectedFieldNames != null && rule.AffectedFieldNames.Count > 0) {
          ruleElement.Add(new XElement("affectedFieldNames",
            from fn in rule.AffectedFieldNames
            select new XElement("fieldName", fn)));
        }
        ruleElement.Add(new XElement("parameters",
          from p in rule.Parameters
          select new XElement("parameter",
            new XAttribute("name", p.Name),
            new XAttribute("typeId", p.ParameterTypeId),
            new XCData(p.StorageString))));
        rulesElement.Add(ruleElement);
      }

      var document = new XDocument(
        rulesElement);
      document.Save(filename);
    }
  }
}
