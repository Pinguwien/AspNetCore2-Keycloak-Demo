using System;
/**
 * Code modified by dominik.guhr@codecentric.de according to apache v2 license
 */
namespace IdentityModel.AspNetCore
{
    public class AutomaticTokenManagementOptions
    {
        public string Scheme { get; set; }
        public TimeSpan RefreshBeforeExpiration { get; set; } = TimeSpan.FromMinutes(1);
        public bool RevokeRefreshTokenOnSignout { get; set; } = false; //keycloak doesn't suppport that yet - see https://issues.jboss.org/browse/KEYCLOAK-5325
    }
}