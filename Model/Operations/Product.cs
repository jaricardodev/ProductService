using System;

namespace Model.Operations
{
    public class Product : OperationEntity
    {
        public string Name { get; set; }

        public string Brand { get; set; }

        public DateTime? Expiration { get; set; }

        public bool IsInStock { get; set; }

        public string ImageUrl { get; set; }    
    }
}
