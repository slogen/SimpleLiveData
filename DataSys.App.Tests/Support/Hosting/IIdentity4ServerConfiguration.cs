using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DataSys.App.Tests.Support.Hosting
{
    public interface IIdentity4ServerConfiguration
    {
        string ClientSecret { get; }
        IEnumerable<Client> Id4Clients { get; }
        void ConfigureServices(IServiceCollection services);
    }
}