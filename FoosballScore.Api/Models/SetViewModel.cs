using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FoosballScore.Domain;

namespace FoosballScore.Api.Models;

[DataContract]
public readonly struct SetViewModel
{
    [DataMember]
    public int Set { get; init; }

    [DataMember]
    public int BlueGoals { get; init; }

    [DataMember]
    public int RedGoals { get; init; }

    [DataMember, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Team? Winner { get; init; }
}