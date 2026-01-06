//
//using System.Threading.Tasks;
//using Plugin.Fingerprint;
//using Plugin.Fingerprint.Abstractions;
//using Microsoft.Maui.Controls;

//namespace Proyecto_servicio.Services
//{

//  public class BiometricService
//{
//public async Task<bool> AuthenticateAsync()
//{


//var isAvailable = await CrossFingerprint.Current.IsAvailableAsync();
//if (!isAvailable)
//
//return false;
//
//
//var config = new AuthenticationRequestConfiguration(
//"Autenticación biométrica",
//"Usa tu huella o Face ID para iniciar sesión"
// )
//{
//   CancelTitle = "Cancelar",
//FallbackTitle = "Usar contraseña"
//}
//;

//var result = await CrossFingerprint.Current.AuthenticateAsync(config);
//return result.Authenticated;
//  }
//}
//}

