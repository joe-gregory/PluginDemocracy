using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginDemocracy.DTOs
{
    public class HomeDto : BaseCitizenDto
    {
        public override string? FullName => throw new NotImplementedException();

        public override List<CommunityDto> Citizenships => throw new NotImplementedException();
    }
}
