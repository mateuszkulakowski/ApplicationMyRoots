using ApplicationMyRoots.Models;
using ApplicationMyRoots.ViewsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationMyRoots.Common
{
    public class Converters
    {
        public static User RegistryUserToUserConverter(RegistryUser registryUser)
        {
            return new User(registryUser.Login,registryUser.Password,registryUser.Name,registryUser.Surname);
        }
        
    }
}