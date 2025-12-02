using AccessControl.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Infraestructure.Common
{
    public class ViewModelLocator
    {
        public static LoginViewModel LoginViewModel => App.Services!.GetService<LoginViewModel>()!;
    }
}
