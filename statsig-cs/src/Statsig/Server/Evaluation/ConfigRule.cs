﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Statsig.src.Statsig.Server
{
    class ConfigRule
    {
        internal string Name { get; }
        internal double PassPercentage { get; }
        internal string ID { get; }
        internal List<ConfigCondition> Conditions { get; }
        internal DynamicConfig DynamicConfigValue { get; }
        internal FeatureGate FeatureGateValue { get; }

        internal ConfigRule(string name, double passPercentage, JToken returnValue, string id, List<ConfigCondition> conditions)
        {
            Name = name;
            PassPercentage = passPercentage;
            Conditions = conditions;
            ID = id;

            FeatureGateValue = new FeatureGate(name, true, id);
            try
            {
                DynamicConfigValue =
                    new DynamicConfig(name, returnValue.ToObject<Dictionary<string, JToken>>(), id);
            }
            catch {}
        }

        internal static ConfigRule FromJObject(JObject jobj)
        {
            JToken name, passPercentage, salt, returnValue, conditions, id;

            if (jobj == null ||
                !jobj.TryGetValue("name", out name) ||
                !jobj.TryGetValue("passPercentage", out passPercentage) ||
                !jobj.TryGetValue("returnValue", out returnValue) ||
                !jobj.TryGetValue("conditions", out conditions) ||
                !jobj.TryGetValue("id", out id))
            {
                return null;
            }

            var conditionsList = new List<ConfigCondition>();
            foreach (JObject cond in conditions.ToObject<JObject[]>())
            {
                conditionsList.Add(ConfigCondition.FromJObject(cond));
            }

            return new ConfigRule(
                name.Value<string>(),
                passPercentage.Value<double>(),
                returnValue,
                id.Value<string>(),
                conditionsList);
        }
    }
}
