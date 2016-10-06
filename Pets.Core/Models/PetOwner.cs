using System.Collections.Generic;

namespace Pets.Core.Models
{
    public class PetOwner
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public IList<Pet> Pets { get; set; }
    }
}
