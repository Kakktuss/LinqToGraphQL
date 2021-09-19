using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LinqToGraphQL.Exceptions
{
    public class GraphQueryExecutionException : Exception
    {
        public GraphQueryExecutionException(string query, IEnumerable<GraphQueryError> errors)
            : base($"One or more errors occured during query execution. Check {nameof(Errors)} property for details")
        {
            Query = query;
            
            Errors = errors;
        }
        
        public string Query { get; set; }
        
        public IEnumerable<GraphQueryError> Errors { get; private set; }
    }

    public class GraphQueryParseException : Exception
    {
        public GraphQueryParseException(string query) 
            : base("One or more errors occured while trying to parse the result.")
        {
            Query = query;
        }
        
        public string Query { get; set; }
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