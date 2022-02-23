using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FoosballScore.Domain;

namespace FoosballScore.Api.Models;

[DataContract]
public record GameViewModel
{
    public GameViewModel(Guid id, string name, IReadOnlyCollection<SetViewModel> sets, Team? winner, DateTimeOffset created, DateTimeOffset modified)
    {
        Id = id;
        Name = name;
        Sets = sets;
        Winner = winner;
        Created = created;
        Modified = modified;
    }

    [DataMember]
    public Guid Id { get; init; }

    [DataMember]
    public string Name { get; init; }

    [DataMember]
    public IReadOnlyCollection<SetViewModel> Sets { get; init; }

    [DataMember, JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Team? Winner { get; init; }

    [DataMember]
    public DateTimeOffset Created { get; init; }

    [DataMember]
    public DateTimeOffset Modified { get; init; }
}