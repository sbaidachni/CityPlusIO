//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAnalyticProject
{
    using System;
    using System.Collections.Generic;
    
    public partial class Resource
    {
        public Resource()
        {
            this.ResourcesProvideds = new HashSet<ResourcesProvided>();
        }
    
        public long Id { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public Nullable<int> Food { get; set; }
        public Nullable<int> Shelter { get; set; }
        public Nullable<int> Clothes { get; set; }
        public Nullable<int> Medicine { get; set; }
    
        public virtual ICollection<ResourcesProvided> ResourcesProvideds { get; set; }
    }
}
