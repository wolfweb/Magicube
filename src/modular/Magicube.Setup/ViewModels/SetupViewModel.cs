using Magicube.Data.Abstractions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Magicube.Setup.ViewModels {
    public class SetupViewModel {
        public string                         SiteName                { get; set; }
                                                                      
        public string                         Description             { get; set; }
                                                                      
        public string                         DatabaseProvider        { get; set; }
                                                                      
        public string                         ConnectionString        { get; set; }
                                                                      
        public string                         TablePrefix             { get; set; }
        [Required]                                                    
        public string                         UserName                { get; set; }
                                                                      
        [Required]                                                    
        public string                         Email                   { get; set; }
                                                                
        [DataType(DataType.Password)]                           
        public string                         Password                { get; set; }
                                                                
        [DataType(DataType.Password)]                           
        public string                         PasswordConfirmation    { get; set; }

        public IEnumerable<DatabaseProvider> DatabaseProviders       { get; set; } = Enumerable.Empty<DatabaseProvider>();
    }
}
