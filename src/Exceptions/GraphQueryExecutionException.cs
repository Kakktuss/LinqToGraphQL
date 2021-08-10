using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Client
{
    public class GraphQueryExecutionException : Exception
    {
        public GraphQueryExecutionException(IEnumerable<GraphQueryError> errors)
            : base($"One or more errors occured during query execution. Check {nameof(Errors)} property for details")
        {
            Errors = errors;
        }
        public IEnumerable<GraphQueryError> Errors { get; private set; }
    }

    public class GraphQueryParseException : Exception
    {
        public GraphQueryParseException() 
            : base("One or more errors occured while trying to parse the result.")
        {
        }
    }
    
    public class GraphQueryError
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        [JsonPropertyName("locations")]
        public ErrorLocation[] Locations { get; set; }
    }

    public class ErrorLocation
    {
        [JsonPropertyName("line")]
        public int Line { get; set; }
        
        [JsonPropertyName("column")]
        public int Column { get; set; }
    }
}