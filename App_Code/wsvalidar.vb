Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Security.Cryptography
Imports System.IO

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsvalidar
    Inherits System.Web.Services.WebService

    'metodo utilizado para validar el usuario a traves del login
    <WebMethod()> _
    Public Function verificacion(ByVal cadena As String) As String

        Dim retorno As String = Nothing

        Dim data As Byte() = Convert.FromBase64String(cadena)
        Dim info As String() = Encoding.UTF8.GetString(data).Split("|")
        Dim usuario As String = info(0)
        Dim contra As String = info(1)

        Dim sql As String = "SELECT u.id, u.USUARIO, u.Nombres, u.PASSWORD, r.idRole, r.descripcion as rol " &
            "FROM USUARIO U  " &
            "INNER JOIN ENC_ROLE r ON r.idRole = u.idrole " &
            "WHERE u.USUARIO = '" & usuario & "' AND u.password = '" & contra & "' AND u.ESTADO = 1 "
        Dim validacion As Boolean = manipular.Verificacion(sql)

        If validacion = False Then
            retorno = "e"
        ElseIf validacion = True Then
            retorno = "p|" & EncryptData(usuario & "|" & contra, "seguridadJADE")
        Else
            retorno = "e"
        End If

        Return retorno

    End Function


    Private Const initVector As String = "pemgail9uzpgzl88"
    Private Const keysize As Integer = 256

    'metodo utilizado para encriptar el usuario y el password a base 64
    Public Function EncryptData(ByVal plaintext As String, ByVal passPhrase As String) As String

        Dim initVectorBytes As Byte() = Encoding.UTF8.GetBytes(initVector)
        Dim plainTextBytes As Byte() = Encoding.UTF8.GetBytes(plaintext)
        Dim password As New PasswordDeriveBytes(passPhrase, Nothing)
        Dim keyBytes As Byte() = password.GetBytes(keysize / 8)
        Dim symmetricKey As New RijndaelManaged()
        symmetricKey.Mode = CipherMode.CBC
        Dim encryptor As ICryptoTransform = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes)
        Dim memoryStream As New MemoryStream()
        Dim cryptoStream As New CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length)
        cryptoStream.FlushFinalBlock()
        Dim cipherTextBytes As Byte() = memoryStream.ToArray()
        memoryStream.Close()
        cryptoStream.Close()
        Return Convert.ToBase64String(cipherTextBytes)

    End Function

    'metodo utilizado para desencriptar el usuario y password
    Public Function DecryptData(ByVal plaintext As String, ByVal passPhrase As String) As String

        Dim initVectorBytes As Byte() = Encoding.ASCII.GetBytes(initVector)
        Dim cipherTextBytes As Byte() = Convert.FromBase64String(plaintext)
        Dim password As New PasswordDeriveBytes(passPhrase, Nothing)
        Dim keyBytes As Byte() = password.GetBytes(keysize / 8)
        Dim symmetricKey As New RijndaelManaged()
        symmetricKey.Mode = CipherMode.CBC
        Dim decryptor As ICryptoTransform = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes)
        Dim memoryStream As New MemoryStream(cipherTextBytes)
        Dim cryptoStream As New CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)
        Dim plainTextBytes As Byte() = New Byte(cipherTextBytes.Length - 1) {}
        Dim decryptedByteCount As Integer = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length)
        memoryStream.Close()
        cryptoStream.Close()
        Return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount)

    End Function

End Class