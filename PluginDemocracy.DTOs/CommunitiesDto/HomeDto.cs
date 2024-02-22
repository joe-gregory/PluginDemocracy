﻿using PluginDemocracy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs
{
    public class HomeDto : BaseCitizenDto
    {
        public CommunityDto? ParentCommunity { get; set; }
        public HashSet<HomeOwnershipDto> Ownerships { get; set; } = [];
        [JsonIgnore]
        public override string? FullName
        {
            get
            {
                string fullName = string.Empty;
                foreach(HomeOwnershipDto ho in Ownerships) fullName += ho.Owner?.FullName + ", ";
                return fullName;
            }
        }
        [JsonIgnore]
        public override List<CommunityDto> Citizenships
        {
            get
            {
                if (ParentCommunity != null) return [ParentCommunity];
                else return [];
            }
        }
        public string InternalAddress { get; set; } = string.Empty;
        [JsonIgnore]
        public override string Address => InternalAddress + "/n" + ParentCommunity?.Address;
        [JsonIgnore]
        public Dictionary<BaseCitizenDto, double> Owners
        {
            get => Ownerships.Where(o => o.Owner != null).ToDictionary(o => o.Owner!, o => o.OwnershipPercentage);
        }
        public List<UserDto> Residents { get; set; } = [];
        /// <summary>
        /// You are a Citizen of this home if you are either an owner or a resident of Home. home.AddOwner, AddResident, etc need to happen in the GatedCommunity so that
        /// Citizen.Citizenships can be updated for both the GatedCommunity and the Home. The Home doesn't have access to its parent GatedCommunity, so it must be done in the
        /// parent GatedCommunity in order to maintain Citizen.Citizenships.
        /// </summary>
        [JsonIgnore]
        public List<BaseCitizenDto> Citizens
        {
            get => Owners.Keys.Union(Residents).ToList();
        }
    }
}
