﻿// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using PortalRecordsMover.AppCode;
//
//    var moverSettings = MoverSettings.FromJson(jsonString);

namespace PortalRecordsMover.AppCode
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public enum DateFilterOptionsEnum
    {
        CreateOnly = 1,
        ModifyOnly,
        CreateAndModify
    }

    public partial class MoverSettingsConfig
    {
        [JsonProperty("ActiveItemsOnly", Required = Required.Always)]
        public bool ActiveItemsOnly { get; set; }

        [JsonProperty("CreateFilter", Required = Required.Default)]
        public DateTime? CreateFilter { get; set; }

        [JsonProperty("ModifyFilter", Required = Required.Default)]
        public DateTime? ModifyFilter { get; set; }

        [JsonProperty("PriorDaysToRetrieve", Required = Required.Always)]
        public long? PriorDaysToRetrieve { get; set; }

        [JsonProperty("DateFilterOptions", Required = Required.Default)]
        [JsonConverter(typeof(ParseStringConverter))]
        public DateFilterOptionsEnum DateFilterOptions { get; set; }

        [JsonProperty("WebsiteFilter", Required = Required.Always)]
        public Guid WebsiteFilter { get; set; }

        [JsonProperty("WebsiteIdMapping", Required = Required.Always)]
        public List<WebsiteIdMap> WebsiteIdMapping { get; set; }

        [JsonProperty("ExportFilename", Required = Required.Default)]
        public string ExportFilename { get; set; }

        [JsonProperty("ImportFilename", Required = Required.Default)]
        public string ImportFilename { get; set; }

        [JsonProperty("SourceEnvironment", Required = Required.Default)]
        public string SourceEnvironment { get; set; }

        [JsonProperty("TargetEnvironment", Required = Required.Default)]
        public string TargetEnvironment { get; set; }


        [JsonProperty("Username", Required = Required.Always)]
        public string Username { get; set; }

        [JsonProperty("Password", Required = Required.Always)]
        public string Password { get; set; }


        [JsonProperty("BatchCount", Required = Required.Always)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long BatchCount { get; set; }

        [JsonProperty("SelectedEntities", Required = Required.Default)]
        public List<string> SelectedEntities { get; set; }
    }

    /// <summary>
    /// Helper class to capture the mapping of website IDs from one environment to the other
    /// </summary>
    public partial class WebsiteIdMap
    {
        [JsonProperty("SourceId", Required = Required.Always)]
        public Guid SourceId { get; set; }

        [JsonProperty("TargetId", Required = Required.Always)]
        public Guid TargetId { get; set; }
    }

    public partial class MoverSettingsConfig
    {
        public static MoverSettingsConfig FromJson(string json) => JsonConvert.DeserializeObject<MoverSettingsConfig>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this MoverSettingsConfig self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?) || t==typeof(DateFilterOptionsEnum);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);

            if (Int64.TryParse(value, out long l)) {
                return l;
            }

            if (Enum.TryParse<DateFilterOptionsEnum>(value, out DateFilterOptionsEnum df)) {
                return df;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null) {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
