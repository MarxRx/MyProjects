using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActitudDental
{
    static class VariablesGlobales
    {
        private static string _UsuarioActivo = "";
        private static string _RolActivo = "";
        private static int _IdActivo = 0;

        public static string UsuarioActivo
        {
            get { return _UsuarioActivo; }
            set { _UsuarioActivo = value; }
        }
        public static string RolActivo
        {
            get { return _RolActivo; }
            set { _RolActivo = value; }
        }

        public static int IdActivo
        {
            get { return _IdActivo; }
            set { _IdActivo = value; }
        }
    }
}
