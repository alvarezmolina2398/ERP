Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Data


' Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente.
<System.Web.Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class wsformaspago
    Inherits System.Web.Services.WebService
    'metod utilizado para mostrar el listado de los tipos de articulo 
    <WebMethod()>
    Public Function ObtenerDatos() As List(Of Datos)

        'enviar tipo
        Dim result As List(Of [Datos]) = New List(Of Datos)()
        Dim StrEncabezado As String = "SELECT * FROM FORMASPAGO WHERE estado = 1 "
        Dim TablaEncabezado As DataTable = manipular.Login(StrEncabezado)

        For i = 0 To TablaEncabezado.Rows.Count - 1
            For ii = 0 To 1
                Dim Elemento As New Datos
                Elemento.id = TablaEncabezado.Rows(i).Item("idFormaPago")
                Elemento.nombre = TablaEncabezado.Rows(i).Item("texto").ToString
                Elemento.adicionales = TablaEncabezado.Rows(i).Item("adicionales")
                result.Add(Elemento)
                ii = ii + 1
            Next
        Next

        Return result
    End Function

    'metodo utilizado para insertar un tipo
    <WebMethod()>
    Public Function GuardarDatos(ByVal nombre As String, ByVal adicional As Integer) As Boolean
        Dim result As Boolean = False


        Dim StrEncabezado As String = "INSERT INTO FORMASPAGO (texto, adicionales, estado) VALUES ('" & nombre & "', " & adicional & ",1)"
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para actualizar un tipo
    <WebMethod()>
    Public Function ActualizarDatos(ByVal nombre As String, ByVal id As Integer, ByVal adicional As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE FORMASPAGO SET texto = '" & nombre & "', adicionales = " & adicional & " WHERE idFormaPago =" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    'metodo utilizado para deshabilitar un tipo
    <WebMethod()>
    Public Function Eliminar(ByVal id As Integer) As Boolean
        Dim result As Boolean = False
        Dim StrEncabezado As String = "UPDATE FORMASPAGO SET estado=0 WHERE idFormaPago=" & id
        result = manipular.EjecutaTransaccion1(StrEncabezado)
        Return result
    End Function

    Public Class Datos
        Public id As Integer
        Public nombre As String
        Public adicionales As Integer
    End Class

End Class